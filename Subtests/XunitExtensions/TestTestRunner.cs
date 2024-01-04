using System.Reflection;
using Xunit.Sdk;

namespace Subtests.XunitExtensions;

public class TestTestRunner : XunitTestRunner
{
    private readonly TestTestCase _testTestCase;

    public TestTestRunner(XunitTest test,
        IMessageBus messageBus,
        Type testClass,
        object[] constructorArguments,
        MethodInfo testMethod,
        object[] testMethodArguments,
        string skipReason,
        IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
        ExceptionAggregator exceptionAggregator,
        CancellationTokenSource cancellationTokenSource,
        TestTestCase testTestCase)
        : base(test,
            messageBus,
            testClass,
            constructorArguments,
            testMethod,
            testMethodArguments,
            skipReason,
            beforeAfterAttributes,
            exceptionAggregator,
            cancellationTokenSource)
    {
        _testTestCase = testTestCase;
    }

    protected override Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
    {
        return new TestTestInvoker(Test,
            MessageBus,
            TestClass,
            ConstructorArguments,
            TestMethod,
            BeforeAfterAttributes,
            aggregator,
            CancellationTokenSource,
            _testTestCase).RunAsync();
    }
}