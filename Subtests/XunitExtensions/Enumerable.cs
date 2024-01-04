using System.Reflection;

namespace Subtests.XunitExtensions;

public static class Enumerable
{
    public static IEnumerable<MethodBase> AsMethodBases(this IEnumerable<MethodBase> methods) => methods;
}