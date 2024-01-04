using System.Runtime.CompilerServices;
using Subtests.XunitExtensions;

namespace Subtests.Examples;

[TestCaseOrderer(SubtestOrderer.TypeName, SubtestOrderer.AssemblyName)]
public class TestBase
{
    protected Task Subtest(Func<Task> func, string? name = null, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0) 
        => TestHelper.Subtest(func, name, callerMemberName, callerLineNumber);

    protected void Subtest(Action action, string? name = null,  [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0) 
        => TestHelper.Subtest(action, name, callerMemberName, callerLineNumber);
}