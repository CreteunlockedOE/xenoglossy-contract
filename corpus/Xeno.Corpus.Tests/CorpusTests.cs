using System.Text.Json;
using Xeno.Corpus.Capture;

namespace Xeno.Corpus.Tests;

public class CorpusTests
{
    static string CorpusRoot()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d != null && !File.Exists(Path.Combine(d.FullName, "Xeno.Contract.slnx"))) d = d.Parent;
        return Path.Combine(d!.FullName, "corpus");
    }

    public static IEnumerable<object[]> Programs() =>
        Directory.GetDirectories(CorpusRoot(), "P??_*").OrderBy(x => x, StringComparer.Ordinal)
            .Select(d => new object[] { Path.GetFileName(d) });

    static string DllPath(string dir)
    {
        var id = dir.Substring(0, 3);
        var p = Path.Combine(CorpusRoot(), dir, "bin", "Debug", "net8.0", $"Corpus.{id}.dll");
        if (!File.Exists(p))
        {
            var cfg = AppContext.BaseDirectory.Contains($"{Path.DirectorySeparatorChar}Release{Path.DirectorySeparatorChar}") ? "Release" : "Debug";
            p = Path.Combine(CorpusRoot(), dir, "bin", cfg, "net8.0", $"Corpus.{id}.dll");
        }
        Assert.True(File.Exists(p), "missing " + p);
        return p;
    }

    [Fact]
    public void ThirtyPrograms() => Assert.Equal(30, Programs().Count());

    [Theory, MemberData(nameof(Programs))]
    public void ExpectedMatchesFreshCapture(string dir)
    {
        var root = Path.Combine(CorpusRoot(), dir);
        var inputs = Capturer.ReadInputs(Path.Combine(root, "inputs.json"));
        Assert.Equal(8, inputs.Count);
        var expected = JsonSerializer.Deserialize<List<CaptureResult>>(File.ReadAllText(Path.Combine(root, "expected.json")))!;
        var actual = Capturer.Capture(DllPath(dir), inputs);
        Assert.Equal(expected, actual);
        Assert.Equal(inputs, expected.Select(e => e.Input).ToList());
    }

    [Theory, MemberData(nameof(Programs))]
    public void Deterministic(string dir)
    {
        var inputs = Capturer.ReadInputs(Path.Combine(CorpusRoot(), dir, "inputs.json"));
        var a = Capturer.Capture(DllPath(dir), inputs);
        var b = Capturer.Capture(DllPath(dir), inputs);
        Assert.Equal(a, b);
    }
}
