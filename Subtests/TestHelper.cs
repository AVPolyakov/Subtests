using Subtests.XunitExtensions;

namespace Subtests;

public static class TestHelper
{
    public static Task Subtest(Func<Task> func, string? name, string callerMemberName, int callerLineNumber)
    {
        if (SubtestIsInvoked(callerMemberName, callerLineNumber))
            return func();
        
        return Task.CompletedTask;
    }
    
    public static void Subtest(Action action, string? name, string callerMemberName, int callerLineNumber)
    {
        if (SubtestIsInvoked(callerMemberName, callerLineNumber))
            action();
    }
    
    private static bool SubtestIsInvoked(string callerMemberName, int callerLineNumber)
    {
        var invocationContext = InvocationContext.Current;
        
        if (invocationContext == null)
            return true;
        
        return invocationContext.TestTestCase.SubtestIsInvoked(callerMemberName, callerLineNumber);
    }
}