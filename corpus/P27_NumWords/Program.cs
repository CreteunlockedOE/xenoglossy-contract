using System.Text;

namespace Corpus;

public static class Program
{
    static string Small(int n)
    {
        switch (n)
        {
            case 0: return "zero";
            case 1: return "one";
            case 2: return "two";
            case 3: return "three";
            case 4: return "four";
            case 5: return "five";
            case 6: return "six";
            case 7: return "seven";
            case 8: return "eight";
            case 9: return "nine";
            case 10: return "ten";
            case 11: return "eleven";
            case 12: return "twelve";
            case 13: return "thirteen";
            case 14: return "fourteen";
            case 15: return "fifteen";
            case 16: return "sixteen";
            case 17: return "seventeen";
            case 18: return "eighteen";
            default: return "nineteen";
        }
    }

    static string Tens(int t)
    {
        switch (t)
        {
            case 2: return "twenty";
            case 3: return "thirty";
            case 4: return "forty";
            case 5: return "fifty";
            case 6: return "sixty";
            case 7: return "seventy";
            case 8: return "eighty";
            default: return "ninety";
        }
    }

    static string Below1000(int n)
    {
        var sb = new StringBuilder();
        if (n >= 100)
        {
            sb.Append(Small(n / 100)).Append(" hundred");
            n %= 100;
            if (n > 0) sb.Append(" and ");
        }
        if (n >= 20)
        {
            sb.Append(Tens(n / 10));
            if (n % 10 > 0) sb.Append('-').Append(Small(n % 10));
        }
        else if (n > 0 || sb.Length == 0) sb.Append(Small(n));
        return sb.ToString();
    }

    static string Words(long n)
    {
        if (n == 0) return "zero";
        string[] scale = { "", " thousand", " million", " billion" };
        var parts = new List<string>();
        bool neg = n < 0;
        if (neg) n = -n;
        for (int i = 0; n > 0; i++)
        {
            int chunk = (int)(n % 1000);
            if (chunk > 0) parts.Insert(0, Below1000(chunk) + scale[i]);
            n /= 1000;
        }
        return (neg ? "minus " : "") + string.Join(" ", parts);
    }

    public static string Run(string input)
    {
        long n = long.Parse(input);
        if (n > 999999999999L) throw new ArgumentOutOfRangeException("input", "too big");
        return Words(n);
    }
}
