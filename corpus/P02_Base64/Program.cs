using System.Text;

namespace Corpus;

public class Base64Exception : Exception
{
    public int Position;
    public Base64Exception(string message, int position) : base(message + " at " + position) { Position = position; }
}

public static class Program
{
    const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    static string Encode(string text)
    {
        var sb = new StringBuilder();
        int i = 0;
        while (i < text.Length)
        {
            int b0 = text[i] & 0xFF;
            int b1 = i + 1 < text.Length ? text[i + 1] & 0xFF : 0;
            int b2 = i + 2 < text.Length ? text[i + 2] & 0xFF : 0;
            int n = (b0 << 16) | (b1 << 8) | b2;
            sb.Append(Alphabet[(n >> 18) & 63]);
            sb.Append(Alphabet[(n >> 12) & 63]);
            sb.Append(i + 1 < text.Length ? Alphabet[(n >> 6) & 63] : '=');
            sb.Append(i + 2 < text.Length ? Alphabet[n & 63] : '=');
            i += 3;
        }
        return sb.ToString();
    }

    static int Value(char c, int pos)
    {
        int v = Alphabet.IndexOf(c);
        if (v < 0) throw new Base64Exception("bad char '" + c + "'", pos);
        return v;
    }

    static string Decode(string s)
    {
        if (s.Length % 4 != 0) throw new Base64Exception("bad length " + s.Length, s.Length);
        var sb = new StringBuilder();
        for (int i = 0; i < s.Length; i += 4)
        {
            int pad = 0;
            int n = 0;
            for (int k = 0; k < 4; k++)
            {
                char c = s[i + k];
                if (c == '=')
                {
                    if (i + 4 != s.Length || k < 2) throw new Base64Exception("misplaced pad", i + k);
                    pad++;
                    n <<= 6;
                }
                else
                {
                    if (pad > 0) throw new Base64Exception("data after pad", i + k);
                    n = (n << 6) | Value(c, i + k);
                }
            }
            sb.Append((char)((n >> 16) & 0xFF));
            if (pad < 2) sb.Append((char)((n >> 8) & 0xFF));
            if (pad < 1) sb.Append((char)(n & 0xFF));
        }
        return sb.ToString();
    }

    public static string Run(string input)
    {
        if (input.StartsWith("enc:")) return Encode(input.Substring(4));
        if (input.StartsWith("dec:")) return Decode(input.Substring(4));
        string e = Encode(input);
        return e + "|" + Decode(e).Length;
    }
}
