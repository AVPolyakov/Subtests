using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Subtests.Usages;
using static Subtests.XunitExtensions.TestTestDiscoverer;

namespace Subtests.XunitExtensions;

public class SubtestUsageInfo
{
    public Usage Usage { get; }
    public MethodInfo SubtestMethod { get; }
    public string? DisplayName { get; }
    private readonly List<Usage> _path;

    public SubtestUsageInfo(Usage usage, List<Usage> path, MethodInfo subtestMethod)
    {
        Usage = usage;
        SubtestMethod = subtestMethod;
        _path = path;

        DisplayName = GetDisplayName();
    }

    private string? GetDisplayName()
    {
        var names = new List<string?>();
        
        if (Usage.Name != null)
            names.Add(Usage.Name);
        
        if (TryGetLocalFunctionName(Usage.CurrentMethod, Usage.CallerMemberName, out var localFunctionName)) 
            names.Add(localFunctionName);
        
        names.Add(Usage.CallerMemberName);

        return string.Join(", ", names);
    }

    public MethodBase RootTest => _path[^1].CurrentMethod;
    
    /// <summary>
    /// https://stackoverflow.com/a/72370663
    /// </summary>
    private static bool TryGetLocalFunctionName(MethodBase localMethod, string? surroundingMethodName, [MaybeNullWhen(false)] out string localFunctionName)
    {
        if (localMethod.GetCustomAttribute<CompilerGeneratedAttribute>() != null ||
            localMethod.DeclaringType?.GetCustomAttribute<CompilerGeneratedAttribute>() != null)
        {        
            var match = Regex.Match(localMethod.Name, $@"^<{surroundingMethodName}>g__(\w+)\|\d+(_\d+)?");
            if (match.Success)
            {
                localFunctionName = match.Groups[1].Value;
                return true;
            }
        }

        localFunctionName = null;
        return false;
    }
    
    public bool SubtestIsInvoked(string callerMemberName, int callerLineNumber)
    {
        return _path.Where((usage, index) => IsMatch(index, usage)).Any();

        bool IsMatch(int index, Usage usage)
        {
            if (index == 0)
            {
                return usage.CallerMemberName == callerMemberName && usage.CallerLineNumber == callerLineNumber;
            }
            else
            {
                var declaringType = Usage.CurrentMethod.GetRequiredDeclaringType();
                var subtestCallerInfos = GetSubtestCallerInfos(declaringType)[usage.ResolvedMember];
                return subtestCallerInfos.Any(x => x.Usage.CallerMemberName == callerMemberName && x.Usage.CallerLineNumber == callerLineNumber);
            }
        }
    }
}