namespace Xeno.Contract.Dcil;

/// <summary>DCIL v1 opcode set. Frozen. JSON names are the dotted mnemonics in <see cref="DcilOpNames"/>.</summary>
public enum DcilOp
{
    // Stack
    PushConst, Pop, Dup, LoadArg, StoreArg, LoadLocal, StoreLocal, LoadField, StoreField,
    // Arithmetic (flags: Unsigned for div/rem, Overflow for add/sub/mul)
    Add, Sub, Mul, Div, Rem, Neg,
    // Bitwise (flags: Unsigned for shr)
    And, Or, Xor, Shl, Shr, Not,
    // Compare: pop b, pop a, push (a OP b) as int32 0/1 (flags: Unsigned)
    CmpEq, CmpNe, CmpLt, CmpLe, CmpGt, CmpGe,
    // Conversion: Type = target numeric type (flags: Unsigned source, Overflow)
    Conv,
    // Control
    Br, BrTrue, BrFalse, Switch, Ret, Throw, Rethrow, Leave, EndFinally,
    // Objects
    NewObj, NewArr, LoadElem, StoreElem, LoadLen, Cast, Box, Unbox,
    // Calls (Token = real method token in the module being lifted)
    Call, CallVirt,
    // Escape hatch for an unclassified handler
    Unknown,
}

public static class DcilOpNames
{
    private static readonly Dictionary<DcilOp, string> ToName = new()
    {
        [DcilOp.PushConst] = "push.const", [DcilOp.Pop] = "pop", [DcilOp.Dup] = "dup",
        [DcilOp.LoadArg] = "load.arg", [DcilOp.StoreArg] = "store.arg",
        [DcilOp.LoadLocal] = "load.local", [DcilOp.StoreLocal] = "store.local",
        [DcilOp.LoadField] = "load.field", [DcilOp.StoreField] = "store.field",
        [DcilOp.Add] = "add", [DcilOp.Sub] = "sub", [DcilOp.Mul] = "mul", [DcilOp.Div] = "div",
        [DcilOp.Rem] = "rem", [DcilOp.Neg] = "neg",
        [DcilOp.And] = "and", [DcilOp.Or] = "or", [DcilOp.Xor] = "xor",
        [DcilOp.Shl] = "shl", [DcilOp.Shr] = "shr", [DcilOp.Not] = "not",
        [DcilOp.CmpEq] = "cmp.eq", [DcilOp.CmpNe] = "cmp.ne", [DcilOp.CmpLt] = "cmp.lt",
        [DcilOp.CmpLe] = "cmp.le", [DcilOp.CmpGt] = "cmp.gt", [DcilOp.CmpGe] = "cmp.ge",
        [DcilOp.Conv] = "conv",
        [DcilOp.Br] = "br", [DcilOp.BrTrue] = "br.true", [DcilOp.BrFalse] = "br.false",
        [DcilOp.Switch] = "switch", [DcilOp.Ret] = "ret", [DcilOp.Throw] = "throw",
        [DcilOp.Rethrow] = "rethrow", [DcilOp.Leave] = "leave", [DcilOp.EndFinally] = "endfinally",
        [DcilOp.NewObj] = "newobj", [DcilOp.NewArr] = "newarr", [DcilOp.LoadElem] = "load.elem",
        [DcilOp.StoreElem] = "store.elem", [DcilOp.LoadLen] = "load.len", [DcilOp.Cast] = "cast",
        [DcilOp.Box] = "box", [DcilOp.Unbox] = "unbox",
        [DcilOp.Call] = "call", [DcilOp.CallVirt] = "callvirt",
        [DcilOp.Unknown] = "unknown",
    };

    private static readonly Dictionary<string, DcilOp> FromName = ToName.ToDictionary(kv => kv.Value, kv => kv.Key);

    public static string Name(this DcilOp op) => ToName[op];

    public static DcilOp Parse(string name) =>
        FromName.TryGetValue(name, out var op) ? op : throw new FormatException($"Unknown DCIL opcode '{name}'.");
}
