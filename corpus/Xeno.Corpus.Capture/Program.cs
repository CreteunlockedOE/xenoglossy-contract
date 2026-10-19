using System.Text.Json;
using Xeno.Corpus.Capture;

if (args.Length < 2)
{
    Console.Error.WriteLine("usage: Xeno.Corpus.Capture <corpus.dll> <inputs.json> [out.json]");
    return 2;
}
var results = Capturer.Capture(args[0], Capturer.ReadInputs(args[1]));
var json = JsonSerializer.Serialize(results, Capturer.Json) + "\n";
if (args.Length > 2) File.WriteAllText(args[2], json); else Console.Out.Write(json);
return 0;
