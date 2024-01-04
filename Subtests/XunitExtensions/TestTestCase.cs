using Xunit.Abstractions;
using Xunit.Sdk;

namespace Subtests.XunitExtensions;

public class TestTestCase : XunitTestCase
{
    private readonly ICaseHandler _caseHandler;
    
    public TestTestCase(IMessageSink diagnosticMessageSink,
        TestMethodDisplay defaultMethodDisplay,
        TestMethodDisplayOptions defaultMethodDisplayOptions,
        ITestMethod testMethod,
        ICaseHandler caseHandler)
        : base(diagnosticMessageSink,
            defaultMethodDisplay,
            defaultMethodDisplayOptions,
            testMethod,
            testMethodArguments: new object?[] { caseHandler.DisplayName })
    {
        _caseHandler = caseHandler;
    }
    
    protected override string? GetDisplayName(IAttributeInfo factAttribute, string displayName)
    {
        return _caseHandler.DisplayName;
    }

    public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        object[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        return new TestTestCaseRunner(this,
            DisplayName,
            SkipReason,
            constructorArguments,
            TestMethodArguments,
            messageBus,
            aggregator,
            cancellationTokenSource,
            this).RunAsync();
    }

    public bool SubtestIsInvoked(string callerMemberName, int callerLineNumber)
    {
        if (WithAllSubtests == false && WithAllSubtestsIsRunning)
            return false;
        
        return _caseHandler.SubtestIsInvoked(callerMemberName, callerLineNumber);
    }

    public bool WithAllSubtests => _caseHandler is WithAllSubtestsCaseHandler;
    
    public bool WithAllSubtestsIsRunning { get; set; }
}