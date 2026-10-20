namespace Xeno.Contract.Trace;

/// <summary>
/// A runtime value as seen on the eval stack or in a touched slot.
/// <para><see cref="K"/> kinds: <c>i4 i8 r4 r8 i</c> (numbers, <see cref="V"/> = invariant-culture text),
/// <c>null</c>, <c>str</c> (V = the string), <c>obj</c>/<c>arr</c> (V = runtime type full name, <see cref="Ref"/> = object id),
/// <c>ptr</c> (managed/unmanaged pointer, V opaque).</para>
/// <para><see cref="Ref"/> is stable for the lifetime of one trace: same object, same id.</para>
/// <para><see cref="Taint"/> is empty from the tracer; the taint engine fills it in.</para>
/// </summary>
public sealed record TraceValue(string K, string? V = null, long? Ref = null)
{
    public string[]? Taint { get; init; }
}

/// <summary>
/// A slot read or written by an instruction.
/// <para><see cref="Kind"/>: <c>arg</c>, <c>local</c> (Target = index), <c>field</c>, <c>sfield</c> (Target = field token hex),
/// <c>elem</c> (Target = index, <see cref="Owner"/> = array Ref).</para>
/// <para><see cref="Access"/>: <c>r</c> or <c>w</c>.</para>
/// </summary>
public sealed record TraceTouch(string Kind, string Access, string Target, TraceValue Value)
{
    /// <summary>Owning object id for <c>field</c> (instance) and <c>elem</c> touches.</summary>
    public long? Owner { get; init; }
}

/// <summary>
/// One executed IL instruction. One JSON line per event.
/// <para><see cref="Pop"/>/<see cref="Push"/> are the values consumed and produced, bottom-to-top
/// (the last element of <see cref="Pop"/> was the top of stack).</para>
/// <para><see cref="Next"/> is the IL offset executed next in this frame (tells you which branch was taken).
/// For an instruction that throws (<see cref="Threw"/> set), Next is the handler start offset when the exception is
/// caught in the same frame, otherwise null. Null for ret. <see cref="Callee"/> is the actually-invoked method token for call/callvirt/calli/newobj
/// and delegate Invoke (after virtual resolution), if it is in a traced module.</para>
/// </summary>
public sealed record TraceEvent(long Seq, uint Method, int Offset, string Op)
{
    public string? MethodName { get; init; }
    public string? Operand { get; init; }
    public int Depth { get; init; }
    public TraceValue[] Pop { get; init; } = [];
    public TraceValue[] Push { get; init; } = [];
    public TraceTouch[]? Touches { get; init; }
    public int? Next { get; init; }
    public uint? Callee { get; init; }
    /// <summary>Set when this instruction raised an exception; holds the exception type full name.</summary>
    public string? Threw { get; init; }
}

/// <summary>Observable outcome of one entry-point invocation (the referee compares these).</summary>
public sealed record RunResult(string? Return, string Stdout, string? Exception)
{
    public bool SameAs(RunResult other) => Return == other.Return && Stdout == other.Stdout && Exception == other.Exception;
}

/// <summary>A full trace of one run.</summary>
public sealed record ExecutionTrace(IReadOnlyList<TraceEvent> Events, RunResult Result);
