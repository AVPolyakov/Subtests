namespace Subtests.XunitExtensions;

public class SubtestCaseHandler : ICaseHandler
{
    private readonly SubtestUsageInfo _subtestUsageInfo;

    public SubtestCaseHandler(SubtestUsageInfo subtestUsageInfo, int? index)
    {
        _subtestUsageInfo = subtestUsageInfo;

        DisplayName = index == null 
            ? _subtestUsageInfo.DisplayName 
            :  $"{_subtestUsageInfo.DisplayName} [{index + 1}]";
    }

    public string? DisplayName { get; }
    
    public bool SubtestIsInvoked(string callerMemberName, int callerLineNumber)
    {
        return _subtestUsageInfo.SubtestIsInvoked(callerMemberName, callerLineNumber);
    }
}