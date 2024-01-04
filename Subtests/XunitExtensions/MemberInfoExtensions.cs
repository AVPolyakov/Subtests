using System.Reflection;

namespace Subtests.XunitExtensions;

public static class MemberInfoExtensions
{
    public static Type GetRequiredDeclaringType(this MemberInfo memberInfo) => memberInfo.DeclaringType!;
}