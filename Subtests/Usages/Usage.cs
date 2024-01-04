using System.Reflection;
using System.Reflection.Emit;

namespace Subtests.Usages;

public class Usage
{
    public MethodBase CurrentMethod { get; }
    public MemberInfo? ResolvedMember { get; }
    public List<Instruction> Instructions { get; }

    public Usage(MethodBase currentMethod, MemberInfo? resolvedMember, List<Instruction> instructions)
    {
        CurrentMethod = currentMethod;
        ResolvedMember = resolvedMember;
        Instructions = instructions;
    }
    
    public string? Name => Instructions[2].InlineStringOperand;
    
    public string? CallerMemberName => Instructions[3].InlineStringOperand;
    
    public int? CallerLineNumber => GetLineNumber(Instructions[4]);
    
    public MemberInfo? ArgumentMethod => Instructions[0].InlineMethodOperand;

    private static int? GetLineNumber(Instruction instruction)
    {
        var opCode = instruction.OpCode;

        if (opCode == OpCodes.Ldc_I4_S)
            return instruction.ShortInlineIOperand;

        if (opCode == OpCodes.Ldc_I4)
            return instruction.InlineIOperand;

        if (_ldc_I4_08_OpCodes.TryGetValue(opCode, out var info))
            return info.LineNumber;
        
        if (opCode == OpCodes.Ldc_I4_M1)
            return -1;

        return null;
    }

    private static readonly Dictionary<OpCode, Ldc_I4_08_OpCodesInfo> _ldc_I4_08_OpCodes = GetLdc_I4_08_OpCodesInfos().ToDictionary(x => x.OpCode);
    
    private static IEnumerable<Ldc_I4_08_OpCodesInfo> GetLdc_I4_08_OpCodesInfos()
    {
        foreach (var fieldInfo in typeof(OpCodes).GetFields())
        {
            if (fieldInfo.FieldType == typeof(OpCode))
            {
                const string postfix = "Ldc_I4_";
                var name = fieldInfo.Name;
                if (name.StartsWith(postfix) && name.Length > postfix.Length)
                {
                    var lineNumberString = name[postfix.Length];
                    if (char.IsDigit(lineNumberString))
                    {
                        var opCode = (OpCode)fieldInfo.GetValue(null)!;
                        yield return new Ldc_I4_08_OpCodesInfo(opCode, int.Parse(lineNumberString.ToString()));
                    }
                }
            }
        }
    }
    
    private class Ldc_I4_08_OpCodesInfo
    {
        public OpCode OpCode { get; }
        public int LineNumber { get; }

        public Ldc_I4_08_OpCodesInfo(OpCode opCode, int lineNumber)
        {
            OpCode = opCode;
            LineNumber = lineNumber;
        }
    }
}