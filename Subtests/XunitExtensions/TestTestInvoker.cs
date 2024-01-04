using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Subtests.XunitExtensions;

public class TestTestInvoker : XunitTestInvoker
{
    private readonly TestTestCase _testTestCase;

    public TestTestInvoker(ITest test,
        IMessageBus messageBus,
        Type testClass,
        object[] constructorArguments,
        MethodInfo testMethod,
        IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        TestTestCase testTestCase)
        : base(test,
            messageBus,
            testClass,
            constructorArguments,
            testMethod,
            testMethodArguments: null,
            beforeAfterAttributes,
            aggregator,
            cancellationTokenSource)
    {
        _testTestCase = testTestCase;
    }
    
    protected override object? CallTestMethod(object testClassInstance)
    {
        if (_testTestCase is { WithAllSubtests: false, WithAllSubtestsIsRunning: true })
            return typeof(Task).IsAssignableFrom(TestMethod.ReturnType)
                ? Task.CompletedTask
                : null;

        InvocationContext.Current = new InvocationContext(_testTestCase);
        try
        {
            return base.CallTestMethod(testClassInstance);
        }
        finally
        {
            InvocationContext.Current = default;
        }
    }
}