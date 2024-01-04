using Xunit.Abstractions;

namespace Subtests.XunitExtensions;

public class WithoutSubtestsCaseHandler : ICaseHandler
{
    private readonly ITestMethod _testMethod;

    public WithoutSubtestsCaseHandler(ITestMethod testMethod)
    {
        _testMethod = testMethod;
    }

    public string? DisplayName => $"- {_testMethod.Method.Name} without subtests";

    public bool SubtestIsInvoked(string callerMemberName, int callerLineNumber) => false;
}