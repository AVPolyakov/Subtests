using Xunit.Sdk;

namespace Subtests.XunitExtensions;

public class TestTestCaseRunner : XunitTestCaseRunner
{
    private readonly TestTestCase _testTestCase;

    public TestTestCaseRunner(IXunitTestCase testCase,
        string displayName,
        string skipReason,
        object[] constructorArguments,
        object[] testMethodArguments,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        TestTestCase testTestCase)
        : base(testCase,
            displayName,
            skipReason,
            constructorArguments,
            testMethodArguments,
            messageBus,
            aggregator,
            cancellationTokenSource)
    {
        _testTestCase = testTestCase;
    }

    protected override Task<RunSummary> RunTestAsync()
    {
        return new TestTestRunner(new XunitTest(TestCase, DisplayName),
            MessageBus,
            TestClass,
            ConstructorArguments,
            TestMethod,
            TestMethodArguments,
            SkipReason,
            BeforeAfterAttributes,
            new ExceptionAggregator(Aggregator),
            CancellationTokenSource,
            _testTestCase).RunAsync();
    }
}