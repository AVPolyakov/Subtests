using System.Reflection;
using System.Reflection.Emit;

namespace Subtests.Usages;

public class Instruction
{
    public OpCode OpCode { get; }
    public int? InlineIOperand { get; set; }
    public sbyte? ShortInlineIOperand { get; set; }
    public string? InlineStringOperand { get; set; }
    public MemberInfo? InlineMethodOperand { get; set; }

    public Instruction(OpCode opCode)
    {
        OpCode = opCode;
    }
}