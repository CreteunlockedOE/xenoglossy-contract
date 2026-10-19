using System.Text.Json.Serialization;

namespace Xeno.Contract.Dcil;

public enum DcilConstKind { I4, I8, R4, R8, Str, Null }

/// <summary>Constant pushed by <c>push.const</c>. Integers live in <see cref="I"/>, floats in <see cref="R"/>.</summary>
public sealed record DcilConst([property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] DcilConstKind Kind, long I = 0, double R = 0, string? S = null)
{
    public static DcilConst Int32(int v) => new(DcilConstKind.I4, v);
    public static DcilConst Int64(long v) => new(DcilConstKind.I8, v);
    public static DcilConst Float32(float v) => new(DcilConstKind.R4, R: v);
    public static DcilConst Float64(double v) => new(DcilConstKind.R8, R: v);
    public static DcilConst String(string v) => new(DcilConstKind.Str, S: v);
    public static readonly DcilConst NullRef = new(DcilConstKind.Null);
}

/// <summary>
/// One DCIL instruction. Which operand fields are required depends on <see cref="Op"/>; see
/// <see cref="DcilValidator"/> for the exact rules.
/// <list type="bullet">
/// <item><see cref="Index"/>: arg/local index (load.arg, store.arg, load.local, store.local).</item>
/// <item><see cref="Token"/>: metadata token in the module being lifted (fields, methods, types).</item>
/// <item><see cref="Target"/>/<see cref="Targets"/>: instruction indices within the same method.</item>
/// <item><see cref="Type"/>: a <see cref="DcilType"/> string (conv target, newarr/load.elem/store.elem element type,
///   cast/box/unbox type when not given by <see cref="Token"/>).</item>
/// </list>
/// </summary>
public sealed record DcilInstr([property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] DcilOp Op)
{
    public DcilConst? Const { get; init; }
    public int? Index { get; init; }
    public uint? Token { get; init; }
    public int? Target { get; init; }
    public int[]? Targets { get; init; }
    public string? Type { get; init; }
    public string? Tag { get; init; }
    public bool Unsigned { get; init; }
    public bool Overflow { get; init; }
    /// <summary>For <c>cast</c>: true = isinst (null on failure), false = castclass (throws).</summary>
    public bool IsInst { get; init; }
}

public enum DcilRegionKind { Catch, Finally, Fault }

/// <summary>Exception region. Start indices inclusive, end indices exclusive, all instruction indices.</summary>
public sealed record DcilExceptionRegion(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] DcilRegionKind Kind,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int TryStart,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int TryEnd,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int HandlerStart,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int HandlerEnd,
    string? CatchType = null);

/// <summary>A recovered method body. <see cref="MethodToken"/> is the MethodDef token in the protected module.</summary>
public sealed record DcilMethod(uint MethodToken, IReadOnlyList<string> Locals, IReadOnlyList<DcilInstr> Body)
{
    public IReadOnlyList<DcilExceptionRegion> Regions { get; init; } = [];
}

public sealed record DcilProgram(IReadOnlyList<DcilMethod> Methods)
{
    public int Version { get; init; } = 1;
}
