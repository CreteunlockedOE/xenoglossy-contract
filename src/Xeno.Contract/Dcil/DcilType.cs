using System.Globalization;
using System.Text.RegularExpressions;

namespace Xeno.Contract.Dcil;

/// <summary>
/// DCIL type descriptor grammar (a plain string so it survives JSON):
/// <code>
///   type := prim | "T:" hex-token | type "[]"
///   prim := bool | char | i1 | u1 | i2 | u2 | i4 | u4 | i8 | u8 | r4 | r8 | i | u | string | object
/// </code>
/// <c>T:0x02000005</c> is a TypeDef/TypeRef/TypeSpec token in the module being lifted.
/// Conv targets are restricted to the numeric primitives.
/// </summary>
public static partial class DcilType
{
    public static readonly IReadOnlySet<string> Primitives = new HashSet<string>
    {
        "bool", "char", "i1", "u1", "i2", "u2", "i4", "u4", "i8", "u8", "r4", "r8", "i", "u", "string", "object",
    };

    public static readonly IReadOnlySet<string> NumericPrimitives = new HashSet<string>
    {
        "i1", "u1", "i2", "u2", "i4", "u4", "i8", "u8", "r4", "r8", "i", "u",
    };

    public static string Token(uint token) => "T:0x" + token.ToString("X8", CultureInfo.InvariantCulture);

    public static string ArrayOf(string element) => element + "[]";

    public static bool IsValid(string? type)
    {
        if (string.IsNullOrEmpty(type)) return false;
        while (type.EndsWith("[]", StringComparison.Ordinal)) type = type[..^2];
        return Primitives.Contains(type) || TokenPattern().IsMatch(type);
    }

    public static bool TryGetToken(string type, out uint token)
    {
        token = 0;
        var m = TokenPattern().Match(type);
        return m.Success && uint.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out token);
    }

    [GeneratedRegex("^T:0x([0-9A-Fa-f]{8})$")]
    private static partial Regex TokenPattern();
}
