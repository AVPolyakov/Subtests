namespace Subtests.XunitExtensions;

public interface ICaseHandler
{
    string? DisplayName { get; }
    bool SubtestIsInvoked(string callerMemberName, int callerLineNumber);
}