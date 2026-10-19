# Xenoglossy contract v1 (frozen)

Owned by the coordinator. If something here is wrong, flag it to the coordinator. Do not edit it yourself.

## Target

- C# on .NET 8 (`net8.0`). Assemblies are read and written with AsmResolver 6.0.1.
- Every program is deterministic: no clock, randomness, threads, IO, network, reflection, unsafe code or P/Invoke.

## Corpus program shape

Each corpus program is one class library at `corpus/Pnn_Name/` that builds `Corpus.Pnn.dll`:

```csharp
namespace Corpus;
public static class Program { public static string Run(string input) { ... } }
```

- **Observable output** = the return value, plus everything written to `Console.Out` during `Run`, plus the
  exception that escapes `Run`, if any (type full name and message). Nothing else counts (iron rule 5).
- `corpus/Pnn_Name/inputs.json` is a JSON array of input strings. These are the *public* inputs.
- The referee also scores on **held-out inputs** that neither team ever sees.
- `Run` parses its input and then calls ordinary helper methods (ints, bools, strings, arrays, small classes,
  exceptions, switches, recursion, loops). Those helpers are what the VMs virtualize.

## DCIL v1

Defined in code: `src/Xeno.Contract/Dcil`. Serialize it only with `DcilJson`, and check it with `DcilValidator`.

What v1 adds to the original opcode list, to cover the corpus: `store.arg`, `conv`, `load.len`, `rethrow`, `leave`,
`endfinally`, exception regions, and the instruction flags `unsigned`, `overflow` and `isInst`.

- Fused IL branches (`beq`, `blt`, ...) are written as `cmp.*` followed by `br.true`.
- Static and instance fields both use `load.field` / `store.field`. The token says which.
- Branch targets are **instruction indices**, not byte offsets.
- Metadata tokens always refer to the module being lifted, which is the protected module.

## Trace format

Defined in code: `src/Xeno.Contract/Trace`. JSON Lines, one `TraceEvent` per executed IL instruction.
Each event has: seq, method token, IL offset, opcode, the values popped and pushed (bottom to top), the slots touched
(args, locals, fields, array elements, with object ids), the next offset, the resolved callee, and any exception
raised. Taint tags start empty; the taint engine fills them in.

## Seams

| Seam | Form |
|---|---|
| Red → referee | `Xeno.Red.Cli protect <vm> <in.dll> <out.dll> --seed <n>` (implements `IVirtualizer`) |
| Blue → referee | `Xeno.Blue.Cli recover <in.dll> <out.dll> [--report r.json]` (implements `IDevirtualizer`) |
| Blue internal | `ITracer` → `ITaintEngine` → `IHandlerModel` → `ILifter`, plus `IDivergenceChecker` (`src/Xeno.Contract/Blue`) |

Only `.dll` files cross between red and blue. The two sides talk to each other only through processes, never in-process.

## Referee rules (how you are scored)

For each VM × program, a match **passes** only if all of these hold:

1. The protected program matches the original on every input. If not, the VM is disqualified for that program.
2. The recovered program matches the original on every public **and held-out** input.
3. **VM-free check.** No method of the original `Corpus` types in the recovered assembly may reference a member
   (method, field or type) that is defined in the recovered module but absent from the original. It also may not use
   reflection, resource-stream, `calli` or `ldftn`/delegate-invoke APIs that the original method did not use.
   Returning the protected assembly unchanged therefore fails.

A VM is **broken** when at least 80% of corpus programs pass. Every gauntlet run records three counts per VM: programs
passed, methods lifted cleanly, and programs that passed via fallback only.

## Isolation

- Red works only in `xenoglossy-red`, blue only in `xenoglossy-blue`. Each gets this contract as a read-only submodule.
- A red agent never reads blue code, and a blue agent never reads red code. Blue never receives red's VM design list.
- Blue develops against its **own** training VM. Blue sees red VMs only as protected `.dll`s at gauntlet time.
