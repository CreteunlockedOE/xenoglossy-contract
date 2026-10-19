using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xeno.Contract.Trace;

/// <summary>JSON Lines reader/writer for traces. One <see cref="TraceEvent"/> per line.</summary>
public static class TraceJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
    };

    public static string WriteLine(TraceEvent e) => JsonSerializer.Serialize(e, Options);

    public static TraceEvent ReadLine(string line) =>
        JsonSerializer.Deserialize<TraceEvent>(line, Options) ?? throw new FormatException("Empty trace line.");

    public static void WriteEvents(TextWriter w, IEnumerable<TraceEvent> events)
    {
        foreach (var e in events) w.WriteLine(WriteLine(e));
    }

    public static IEnumerable<TraceEvent> ReadEvents(TextReader r)
    {
        while (r.ReadLine() is { } line)
            if (line.Length > 0) yield return ReadLine(line);
    }

    public static string WriteResult(RunResult r) => JsonSerializer.Serialize(r, Options);

    public static RunResult ReadResult(string json) =>
        JsonSerializer.Deserialize<RunResult>(json, Options) ?? throw new FormatException("Empty run result.");
}
