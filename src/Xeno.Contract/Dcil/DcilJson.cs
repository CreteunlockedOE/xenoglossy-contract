using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xeno.Contract.Dcil;

/// <summary>The one DCIL reader/writer. Both teams use this; never hand-roll DCIL JSON.</summary>
public static class DcilJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = false,
        Converters = { new DcilOpConverter(), new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    public static string Write(DcilProgram program) => JsonSerializer.Serialize(program, Options);

    public static DcilProgram Read(string json) =>
        JsonSerializer.Deserialize<DcilProgram>(json, Options) ?? throw new FormatException("Empty DCIL document.");

    public static void WriteFile(string path, DcilProgram program) => File.WriteAllText(path, Write(program));

    public static DcilProgram ReadFile(string path) => Read(File.ReadAllText(path));

    private sealed class DcilOpConverter : JsonConverter<DcilOp>
    {
        public override DcilOp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            DcilOpNames.Parse(reader.GetString() ?? throw new FormatException("Null DCIL opcode."));

        public override void Write(Utf8JsonWriter writer, DcilOp value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.Name());
    }
}
