using System.Text;

namespace Corpus;

public static class Program
{
    static readonly int[] Values = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
    static readonly string[] Symbols = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

    static string ToRoman(int n)
    {
        if (n <= 0 || n >= 4000) throw new ArgumentOutOfRangeException("n", "out of range: " + n);
        var sb = new StringBuilder();
        for (int i = 0; i < Values.Length; i++)
        {
            while (n >= Values[i])
            {
                sb.Append(Symbols[i]);
                n -= Values[i];
            }
        }
        return sb.ToString();
    }

    static int Digit(char c)
    {
        switch (c)
        {
            case 'I': return 1;
            case 'V': return 5;
            case 'X': return 10;
            case 'L': return 50;
            case 'C': return 100;
            case 'D': return 500;
            case 'M': return 1000;
            default: return 0;
        }
    }

    static int FromRoman(string s)
    {
        int total = 0;
        for (int i = 0; i < s.Length; i++)
        {
            int v = Digit(s[i]);
            if (v == 0) return -1;
            int next = i + 1 < s.Length ? Digit(s[i + 1]) : 0;
            if (v < next) total -= v; else total += v;
        }
        return total;
    }

    public static string Run(string input)
    {
        if (input.Length > 0 && input[0] >= '0' && input[0] <= '9')
        {
            int n = int.Parse(input);
            string r = ToRoman(n);
            return r + " back=" + FromRoman(r);
        }
        int v = FromRoman(input.ToUpperInvariant());
        if (v < 0) return "not roman";
        bool canonical = v > 0 && v < 4000 && ToRoman(v) == input.ToUpperInvariant();
        return v + (canonical ? " canonical" : " odd");
    }
}
