using Xeno.Contract.Dcil;
using Xeno.Contract.Trace;

namespace Xeno.Contract.Tests;

public class ContractTests
{
    private static DcilMethod Sample() => new(
        0x06000002,
        ["i4", "string", "T:0x02000003[]"],
        [
            new(DcilOp.PushConst) { Const = DcilConst.Int32(0) },
            new(DcilOp.StoreLocal) { Index = 0 },
            new(DcilOp.LoadArg) { Index = 0 },
            new(DcilOp.PushConst) { Const = DcilConst.String("x\n\"y\"") },
            new(DcilOp.CmpLt) { Unsigned = true },
            new(DcilOp.BrFalse) { Target = 7 },
            new(DcilOp.Switch) { Targets = [0, 7] },
            new(DcilOp.Conv) { Type = "i8", Overflow = true },
            new(DcilOp.Call) { Token = 0x0A000001 },
            new(DcilOp.Unknown) { Tag = "h17" },
            new(DcilOp.Ret),
        ])
    {
        Regions = [new DcilExceptionRegion(DcilRegionKind.Catch, 0, 2, 2, 4, "T:0x01000005")],
    };

    [Fact]
    public void DcilRoundTrips()
    {
        var program = new DcilProgram([Sample()]);
        var json = DcilJson.Write(program);
        var back = DcilJson.Read(json);

        Assert.Equal(json, DcilJson.Write(back));
        Assert.Contains("\"op\":\"push.const\"", json);
        Assert.Contains("\"kind\":\"i4\"", json);
        Assert.Contains("\"op\":\"cmp.lt\",\"unsigned\":true", json);
        Assert.Equal(DcilOp.PushConst, back.Methods[0].Body[0].Op);
        Assert.Equal(1, back.Version);
    }

    [Fact]
    public void EveryOpcodeHasAUniqueName()
    {
        var names = Enum.GetValues<DcilOp>().Select(o => o.Name()).ToList();
        Assert.Equal(names.Count, names.Distinct().Count());
        foreach (var op in Enum.GetValues<DcilOp>()) Assert.Equal(op, DcilOpNames.Parse(op.Name()));
    }

    [Fact]
    public void ValidatorAcceptsSampleAndRejectsBrokenOperands()
    {
        Assert.Empty(DcilValidator.Validate(Sample()));

        var broken = new DcilMethod(1, ["i4"],
        [
            new(DcilOp.Br) { Target = 9 },
            new(DcilOp.LoadLocal) { Index = 3 },
            new(DcilOp.Conv) { Type = "string" },
            new(DcilOp.Call),
            new(DcilOp.Unknown),
        ]);
        Assert.Equal(5, DcilValidator.Validate(broken).Count);
    }

    [Theory]
    [InlineData("i4", true)]
    [InlineData("string[][]", true)]
    [InlineData("T:0x02000001", true)]
    [InlineData("T:0x2000001", false)]
    [InlineData("int", false)]
    [InlineData("", false)]
    public void TypeGrammar(string type, bool valid) => Assert.Equal(valid, DcilType.IsValid(type));

    [Fact]
    public void TraceLinesRoundTrip()
    {
        var e = new TraceEvent(42, 0x06000001, 0x1A, "ldelem.u1")
        {
            Pop = [new("arr", "System.Byte[]", 7), new("i4", "3")],
            Push = [new("i4", "200") { Taint = ["blob"] }],
            Touches = [new("elem", "r", "3", new("i4", "200")) { Owner = 7 }],
            Next = 0x1B,
        };
        var line = TraceJson.WriteLine(e);
        Assert.DoesNotContain('\n', line);
        var back = TraceJson.ReadLine(line);
        Assert.Equal(line, TraceJson.WriteLine(back));
        Assert.Equal(7, back.Touches![0].Owner);
    }

    [Fact]
    public void RunResultEquality()
    {
        Assert.True(new RunResult("1", "a", null).SameAs(new RunResult("1", "a", null)));
        Assert.False(new RunResult("1", "a", null).SameAs(new RunResult("1", "a\n", null)));
    }
}
