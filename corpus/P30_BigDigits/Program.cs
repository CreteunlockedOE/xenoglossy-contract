using System.Text;

namespace Corpus;

public static class Program
{
    static byte[] FromString(string s)
    {
        var d = new byte[s.Length];
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[s.Length - 1 - i];
            if (c < '0' || c > '9') throw new FormatException("not a digit: " + c);
            d[i] = (byte)(c - '0');
        }
        return d;
    }

    static string ToStr(byte[] d)
    {
        int top = d.Length - 1;
        while (top > 0 && d[top] == 0) top--;
        var sb = new StringBuilder();
        for (int i = top; i >= 0; i--) sb.Append((char)('0' + d[i]));
        return sb.Length == 0 ? "0" : sb.ToString();
    }

    static byte[] Add(byte[] a, byte[] b)
    {
        int n = Math.Max(a.Length, b.Length) + 1;
        var r = new byte[n];
        int carry = 0;
        for (int i = 0; i < n; i++)
        {
            int s = carry + (i < a.Length ? a[i] : 0) + (i < b.Length ? b[i] : 0);
            r[i] = (byte)(s % 10);
            carry = s / 10;
        }
        return r;
    }

    static byte[] Multiply(byte[] a, byte[] b)
    {
        var acc = new int[a.Length + b.Length + 1];
        for (int i = 0; i < a.Length; i++)
            for (int j = 0; j < b.Length; j++)
                acc[i + j] += a[i] * b[j];
        var r = new byte[acc.Length];
        int carry = 0;
        for (int i = 0; i < acc.Length; i++)
        {
            int s = acc[i] + carry;
            r[i] = (byte)(s % 10);
            carry = s / 10;
        }
        return r;
    }

    static int DigitSum(byte[] d)
    {
        int s = 0;
        foreach (byte x in d) s += x;
        return s;
    }

    static string Factorial(int n)
    {
        byte[] acc = FromString("1");
        for (int i = 2; i <= n; i++) acc = Multiply(acc, FromString(i.ToString()));
        return ToStr(acc);
    }

    public static string Run(string input)
    {
        string[] p = input.Split('*');
        if (p.Length == 1)
        {
            string f = Factorial(int.Parse(p[0]));
            return f + " ds=" + DigitSum(FromString(f));
        }
        byte[] a = FromString(p[0]);
        byte[] b = FromString(p[1]);
        return "sum=" + ToStr(Add(a, b)) + " prod=" + ToStr(Multiply(a, b));
    }
}
