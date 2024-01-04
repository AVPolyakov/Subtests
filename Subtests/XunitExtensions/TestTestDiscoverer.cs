using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using Subtests.Usages;
using Xunit.Abstractions;
using Xunit.Sdk;
using static Subtests.Usages.UsageResolver;

namespace Subtests.XunitExtensions;

public class TestTestDiscoverer : IXunitTestCaseDiscoverer
{
    public const string TypeName = "Subtests.XunitExtensions." + nameof(TestTestDiscoverer);
    public const string AssemblyName = "Subtests";

    private readonly IMessageSink _diagnosticMessageSink;

    public TestTestDiscoverer(IMessageSink diagnosticMessageSink)
    {
        _diagnosticMessageSink = diagnosticMessageSink;
    }

    public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
    {
        yield return new TestTestCase(_diagnosticMessageSink,
            discoveryOptions.MethodDisplayOrDefault(),
            discoveryOptions.MethodDisplayOptionsOrDefault(),
            testMethod,
            new WithAllSubtestsCaseHandler(testMethod));
        
        yield return new TestTestCase(_diagnosticMessageSink,
            discoveryOptions.MethodDisplayOrDefault(),
            discoveryOptions.MethodDisplayOptionsOrDefault(),
            testMethod,
            new WithoutSubtestsCaseHandler(testMethod));
        
        if (testMethod.Method is IReflectionMethodInfo reflectionMethodInfo)
        {
            var subtestCaseHandlers = GetSubtestUsageInfos(reflectionMethodInfo.MethodInfo)
                .GroupBy(x => x.DisplayName)
                .SelectMany(g => g.Skip(1).Any() 
                    ? g.OrderBy(x => x.Usage.CallerLineNumber).Select((subtestUsageInfo, index) => new SubtestCaseHandler(subtestUsageInfo, index)) 
                    : g.Select(subtestUsageInfo => new SubtestCaseHandler(subtestUsageInfo, index: null)));
            foreach (var subtestCaseHandler in subtestCaseHandlers)
            {
                yield return new TestTestCase(_diagnosticMessageSink,
                    discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(),
                    testMethod,
                    subtestCaseHandler);
            }
        }
    }

    private static IEnumerable<SubtestUsageInfo> GetSubtestUsageInfos(MethodInfo testMethodInfo)
    {
        var declaringType = testMethodInfo.GetRequiredDeclaringType();
        return GetSubtestUsageInfos(declaringType)[testMethodInfo];
    }

    private static readonly ConcurrentDictionary<Type, ILookup<MethodBase, SubtestUsageInfo>> _subtestUsageInfosByRootTest = new();

    private static ILookup<MethodBase, SubtestUsageInfo> GetSubtestUsageInfos(Type declaringType)
    {
        return _subtestUsageInfosByRootTest.GetOrAdd(declaringType, t => GetSubtestUsageInfosEnumerable(t).ToLookup(x => x.RootTest));
    }

    private static readonly ConcurrentDictionary<Type, ILookup<MemberInfo?, SubtestUsageInfo>> _subtestCallerInfosByType = new();

    internal static ILookup<MemberInfo?, SubtestUsageInfo> GetSubtestCallerInfos(Type declaringType)
    {
        return _subtestCallerInfosByType.GetOrAdd(declaringType, t => GetSubtestUsageInfosEnumerable(t).ToLookup(x => x.Usage.ArgumentMethod));
    }

    private static IEnumerable<SubtestUsageInfo> GetSubtestUsageInfosEnumerable(Type declaringType)
    {
        foreach (var subtestMethod in GetSubtestMethods(declaringType))
        {
            foreach (var subtestUsage in GetUsages(declaringType, subtestMethod))
            {
                var path = new List<Usage> { subtestUsage };
                GetFullPaths(declaringType, subtestUsage, path);
                yield return new SubtestUsageInfo(subtestUsage, path, subtestMethod);
            }
        }
    }

    private static void GetFullPaths(Type rootType, Usage targetUsage, List<Usage> path)
    {
        foreach (var callingMethod in GetUsages(rootType, targetUsage.CurrentMethod))
        {
            path.Add(callingMethod);
            GetFullPaths(rootType, callingMethod, path);
        }
    }
    
    private static IEnumerable<MethodInfo> GetSubtestMethods(Type declaringType)
    {
        foreach (var methodInfo in declaringType.GetMethods(AllBindingFlags))
            if (methodInfo.Name == "Subtest")
                yield return methodInfo;

        if (declaringType.BaseType != null)
            foreach (var methodInfo in GetSubtestMethods(declaringType.BaseType))
                yield return methodInfo;
    }

    private static IEnumerable<Usage> GetUsages(Type rootType, MethodBase targetMethod)
    {
        var resolveUsages = ResolveUsages(rootType);
        var usages = resolveUsages[targetMethod];
        foreach (var usage in usages)
        {
            var declaringType = usage.CurrentMethod.DeclaringType;
            if (declaringType?.GetCustomAttribute<CompilerGeneratedAttribute>() != null && typeof(IAsyncStateMachine).IsAssignableFrom(declaringType))
            {
                if (declaringType.IsValueType)
                {
                    foreach (var localVariableTuple in GetLocalVariableInfos(rootType)[declaringType])
                    {
                        yield return new Usage(localVariableTuple.Method, usage.ResolvedMember, usage.Instructions);
                    }
                }
                else
                {
                    foreach (var constructorInfo in declaringType.GetConstructors(AllBindingFlags))
                    {
                        var constructorUsages = resolveUsages[constructorInfo];
                        foreach (var constructorUsage in constructorUsages)
                        {
                            if (constructorUsage.CurrentMethod.DeclaringType == rootType)
                            {
                                yield return new Usage(constructorUsage.CurrentMethod, usage.ResolvedMember, usage.Instructions);
                            }
                        }
                    }
                }
            }
            else
            {
                yield return usage;
            }
        }
    }

    private static readonly ConcurrentDictionary<Type, ILookup<MemberInfo?, Usage>> _usagesDictionary = new();
    
    private static ILookup<MemberInfo?, Usage> ResolveUsages(Type rootType)
    {
        return _usagesDictionary.GetOrAdd(rootType, t => GetTypes(t).ResolveUsages().ToLookup(x => x.ResolvedMember));
    }

    private static readonly ConcurrentDictionary<Type, ILookup<Type, LocalVariableTuple>> _localVariablesDictionary = new();
    
    private static ILookup<Type, LocalVariableTuple> GetLocalVariableInfos(Type rootType)
    {
        return _localVariablesDictionary.GetOrAdd(rootType, t => GetLocalVariableInfoEnumerable(t).ToLookup(x => x.LocalVariableInfo.LocalType));
    }

    private static IEnumerable<LocalVariableTuple> GetLocalVariableInfoEnumerable(Type type)
    {
        var methodBases = type.GetMethods(AllBindingFlags).AsMethodBases().Concat(type.GetConstructors(AllBindingFlags));
        foreach (var methodBase in methodBases)
        {
            var methodBody = methodBase.GetMethodBody();
            if (methodBody != null)
                foreach (var localVariableInfo in methodBody.LocalVariables)
                    yield return new LocalVariableTuple(methodBase, localVariableInfo);
        }
    }

    private record LocalVariableTuple(MethodBase Method, LocalVariableInfo LocalVariableInfo);

    private static IEnumerable<Type> GetTypes(Type rootType)
    {
        yield return rootType;

        var nestedTypes = rootType.GetNestedTypes(AllBindingFlags);
        foreach (var nestedType in nestedTypes)
        foreach (var type in GetTypes(nestedType))
            yield return type;
    }
}