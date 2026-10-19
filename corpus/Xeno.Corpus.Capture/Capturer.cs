using System.Globalization;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xeno.Corpus.Capture;

public sealed record CaptureResult(
    [property: JsonPropertyName("input")] string Input,
    [property: JsonPropertyName("return")] string? Return,
    [property: JsonPropertyName("stdout")] string Stdout,
    [property: JsonPropertyName("exception")] string? Exception);

public static class Capturer
{
    public static readonly JsonSerializerOptions Json = new() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public static List<string> ReadInputs(string path) =>
        JsonSerializer.Deserialize<List<string>>(File.ReadAllText(path)) ?? new List<string>();

    public static List<CaptureResult> Capture(string dllPath, IEnumerable<string> inputs)
    {
        var results = new List<CaptureResult>();
        foreach (var input in inputs) results.Add(CaptureOne(Path.GetFullPath(dllPath), input));
        return results;
    }

    // Fresh collectible load context per input; Console.Out redirected for the duration of the call.
    static CaptureResult CaptureOne(string dllPath, string input)
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        var alc = new AssemblyLoadContext("corpus-capture", isCollectible: true);
        var originalOut = Console.Out;
        var sw = new StringWriter(CultureInfo.InvariantCulture);
        string? ret = null, exc = null;
        try
        {
            var asm = alc.LoadFromAssemblyPath(dllPath);
            var run = asm.GetType("Corpus.Program", throwOnError: true)!
                .GetMethod("Run", BindingFlags.Public | BindingFlags.Static)!;
            Console.SetOut(sw);
            try { ret = (string?)run.Invoke(null, new object?[] { input }); }
            catch (TargetInvocationException tie)
            {
                var e = tie.InnerException ?? tie;
                exc = e.GetType().FullName + ": " + e.Message;
            }
            finally { Console.SetOut(originalOut); }
        }
        finally { alc.Unload(); }
        return new CaptureResult(input, ret, sw.ToString(), exc);
    }
}
