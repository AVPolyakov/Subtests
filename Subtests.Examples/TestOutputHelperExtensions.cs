using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace Subtests.Examples;

public static class TestOutputHelperExtensions
{
    public static void WriteCallerInfo(this ITestOutputHelper testOutputHelper, string postfix = "", [CallerMemberName] string memberName = "")
    {
        testOutputHelper.WriteLine($"Вызван метод {memberName} {postfix}");
    }
}