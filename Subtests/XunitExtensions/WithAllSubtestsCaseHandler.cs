using Xunit.Abstractions;

namespace Subtests.XunitExtensions;

public class WithAllSubtestsCaseHandler : ICaseHandler
{
    private readonly ITestMethod _testMethod;

    public WithAllSubtestsCaseHandler(ITestMethod testMethod)
    {
        _testMethod = testMethod;
    }

    public string? DisplayName => $"* {_testMethod.Method.Name} with all subtests";

    public bool SubtestIsInvoked(string callerMemberName, int callerLineNumber) => true;
}