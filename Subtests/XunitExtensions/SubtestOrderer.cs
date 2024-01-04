using Xunit.Abstractions;
using Xunit.Sdk;

namespace Subtests.XunitExtensions;

public class SubtestOrderer : ITestCaseOrderer
{
    public const string TypeName = "Subtests.XunitExtensions." + nameof(SubtestOrderer);
    public const string AssemblyName = "Subtests";

    private readonly DefaultTestCaseOrderer _defaultTestCaseOrderer;

    public SubtestOrderer(IMessageSink diagnosticMessageSink)
    {
        _defaultTestCaseOrderer = new DefaultTestCaseOrderer(diagnosticMessageSink);
    }

    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        var testCasesList = testCases.ToList();
        
        var withAllSubtestsIsRunning = testCasesList.Any(testCase => testCase is TestTestCase { WithAllSubtests: true });
        foreach (var testCase in testCasesList)
            if (testCase is TestTestCase testTestCase)
                testTestCase.WithAllSubtestsIsRunning = withAllSubtestsIsRunning;

        return _defaultTestCaseOrderer.OrderTestCases(testCasesList);
    }
}