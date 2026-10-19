using System.Text;

namespace Corpus;

public static class Program
{
    static bool Luhn(string digits)
    {
        int sum = 0;
        bool dbl = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i] - '0';
            if (dbl)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            dbl = !dbl;
        }
        return sum % 10 == 0;
    }

    static string Clean(string s)
    {
        var sb = new StringBuilder();
        foreach (char c in s)
        {
            if (c >= '0' && c <= '9') sb.Append(c);
            else if (c != ' ' && c != '-') return "";
        }
        return sb.ToString();
    }

    static int CheckDigit(string partial)
    {
        for (int d = 0; d < 10; d++)
            if (Luhn(partial + (char)('0' + d))) return d;
        return -1;
    }

    static string Brand(string n)
    {
        if (n.Length == 15 && n[0] == '3' && (n[1] == '4' || n[1] == '7')) return "amex";
        if (n.Length == 16 && n[0] == '4') return "visa";
        if (n.Length == 16 && n[0] == '5' && n[1] >= '1' && n[1] <= '5') return "master";
        if (n.Length >= 13 && n[0] == '6') return "disc";
        return "unknown";
    }

    public static string Run(string input)
    {
        string n = Clean(input);
        if (n.Length < 2) return "invalid";
        bool ok = Luhn(n);
        int expect = CheckDigit(n.Substring(0, n.Length - 1));
        return (ok ? "valid" : "bad") + " brand=" + Brand(n) + " check=" + expect + " len=" + n.Length;
    }
}
