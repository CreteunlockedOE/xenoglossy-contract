namespace Xeno.Contract;

/// <summary>
/// Red team. Takes a clean assembly image, returns a protected one.
/// <para>Rules: the output is a single self-contained assembly (VM runtime embedded, no extra files);
/// the corpus entry point <see cref="CorpusConventions.EntryType"/>.<see cref="CorpusConventions.EntryMethod"/>
/// keeps its name and signature; the same <paramref name="seed"/> gives byte-identical output, different seeds give
/// different opcode numbering.</para>
/// <para>Exposed to the referee as: <c>Xeno.Red.Cli protect &lt;vm&gt; &lt;in.dll&gt; &lt;out.dll&gt; --seed &lt;n&gt;</c>.</para>
/// </summary>
public interface IVirtualizer
{
    /// <summary>Short id, e.g. "R1".</summary>
    string Name { get; }

    byte[] Protect(byte[] cleanAssembly, int seed);
}

/// <summary>
/// Blue team top level. Takes a protected assembly image, returns a recovered one.
/// <para>Rules: generic, no per-protector code paths keyed on names or signatures of a specific VM.
/// The recovered methods must not reference the VM runtime (the referee checks this structurally; see
/// <c>REFEREE.md</c>), otherwise the program counts as not recovered even if its outputs match.</para>
/// <para>Exposed to the referee as: <c>Xeno.Blue.Cli recover &lt;in.dll&gt; &lt;out.dll&gt; [--report report.json]</c>.</para>
/// </summary>
public interface IDevirtualizer
{
    byte[] Recover(byte[] protectedAssembly, RecoveryReport report);
}

/// <summary>What the devirtualizer says it did. Informational only; the referee decides pass/fail.</summary>
public sealed class RecoveryReport
{
    public List<MethodRecovery> Methods { get; } = [];
}

public enum RecoveryStatus { NotVirtualized, Lifted, Partial, FallbackOnly, Failed }

public sealed record MethodRecovery(string Method, RecoveryStatus Status, string? Note = null);

/// <summary>Shape every corpus program follows.</summary>
public static class CorpusConventions
{
    /// <summary>Full name of the entry type in every corpus assembly.</summary>
    public const string EntryType = "Corpus.Program";

    /// <summary><c>public static string Run(string input)</c>. Stdout written during Run is part of the output.</summary>
    public const string EntryMethod = "Run";
}
