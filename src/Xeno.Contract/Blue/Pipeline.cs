using AsmResolver.DotNet;
using Xeno.Contract.Dcil;
using Xeno.Contract.Trace;

namespace Xeno.Contract.Blue;

// Blue-team internal seams. Red never references this namespace.

/// <summary>Stage 1. Runs a method under instrumentation and records every executed IL instruction.</summary>
public interface ITracer
{
    /// <summary>Invoke <paramref name="method"/> (static, or instance on a default-constructed object) with
    /// <paramref name="args"/> inside <paramref name="assembly"/> and return the full trace.</summary>
    ExecutionTrace Run(byte[] assembly, uint method, object?[] args, TraceOptions options);

    /// <summary>Plain mode: no instrumentation, just the observable result.</summary>
    RunResult Execute(byte[] assembly, uint method, object?[] args);
}

public sealed record TraceOptions
{
    /// <summary>Stop recording after this many events (the run still completes).</summary>
    public long MaxEvents { get; init; } = 5_000_000;

    /// <summary>Only record instructions in this module (calls into the BCL are recorded as single call events).</summary>
    public bool OnlyTargetModule { get; init; } = true;
}

/// <summary>Where the VM's bytecode came from, as found by the taint engine.</summary>
public sealed record TaintSource(string Kind, string Description)
{
    /// <summary>Object id of the array holding (decrypted) bytecode, if one exists.</summary>
    public long? ArrayRef { get; init; }
}

/// <summary>A control transfer decided by tainted (bytecode-derived) data.</summary>
public sealed record DispatchPoint(long Seq, uint Method, int Offset, string Kind, TraceValue Selector);

/// <summary>A contiguous slice of the trace that implements one virtual instruction.</summary>
public sealed record HandlerExecution(long StartSeq, long EndSeq, string HandlerId, TraceValue Opcode);

public sealed record DispatchAnalysis(
    IReadOnlyList<TaintSource> Sources,
    IReadOnlyList<DispatchPoint> DispatchPoints,
    IReadOnlyList<HandlerExecution> Executions);

/// <summary>Stage 2. Finds dispatch points and carves the trace into handler executions.</summary>
public interface ITaintEngine
{
    DispatchAnalysis Find(ExecutionTrace trace);
}

/// <summary>
/// One handler: identified by where its code lives (a method, or a region inside the dispatcher method),
/// with every observed execution. Handlers are NOT assumed to be separate methods.
/// </summary>
public sealed record HandlerRegion(string HandlerId, uint Method, int StartOffset, IReadOnlyList<HandlerExecution> Executions);

/// <summary>
/// Semantics of one handler. <see cref="Variants"/> has one entry for a plain handler and several for a
/// flag-driven super-handler; <see cref="VariantSelector"/> says which bytecode-derived value picks the variant.
/// </summary>
public sealed record HandlerEffect(string HandlerId, IReadOnlyList<HandlerVariant> Variants, double Confidence)
{
    public string? VariantSelector { get; init; }
}

/// <summary>
/// One concrete operation. <see cref="Template"/> is the DCIL it lowers to; operand fields that come from the
/// bytecode stream are described by <see cref="Operands"/> rather than filled in.
/// </summary>
public sealed record HandlerVariant(long? SelectorValue, IReadOnlyList<DcilInstr> Template, IReadOnlyList<OperandSlot> Operands);

/// <summary>An operand read from the bytecode: which template instruction/field it fills and how it was decoded.</summary>
public sealed record OperandSlot(int TemplateIndex, string Field, string Decoding);

/// <summary>Stage 3. Classifies a handler by its effect on VM state, never by its IL shape.</summary>
public interface IHandlerModel
{
    HandlerEffect Classify(HandlerRegion handler, ExecutionTrace trace);
}

/// <summary>Stage 4. Writes a DCIL method back as a real IL body into <paramref name="module"/>.</summary>
public interface ILifter
{
    /// <summary>Replace the body of <see cref="DcilMethod.MethodToken"/>. <c>unknown(tag)</c> becomes a stub that throws
    /// <c>NotSupportedException("xeno:unknown:&lt;tag&gt;")</c>.</summary>
    void Lift(ModuleDefinition module, DcilMethod method);
}

/// <summary>Stage 5. Differential check of a candidate against the protected original.</summary>
public interface IDivergenceChecker
{
    DivergenceResult Compare(byte[] original, byte[] candidate, uint method, IReadOnlyList<object?[]> inputs);

    /// <summary>Generate extra inputs that reach new branches in <paramref name="method"/>.</summary>
    IReadOnlyList<object?[]> GenerateInputs(byte[] assembly, uint method, IReadOnlyList<object?[]> seeds, int budget);
}

public sealed record DivergenceResult(bool Match, int InputsRun, object?[]? FirstDivergingInput, string? Detail);
