using System.Text;

namespace Corpus;

public static class Program
{
    static bool[] Sieve(int n)
    {
        var composite = new bool[n + 1];
        for (int i = 2; (long)i * i <= n; i++)
        {
            if (composite[i]) continue;
            for (int j = i * i; j <= n; j += i) composite[j] = true;
        }
        return composite;
    }

    static int CountPrimes(bool[] composite)
    {
        int c = 0;
        for (int i = 2; i < composite.Length; i++) if (!composite[i]) c++;
        return c;
    }

    static string Goldbach(bool[] composite, int even)
    {
        for (int p = 2; p <= even / 2; p++)
            if (!composite[p] && !composite[even - p]) return p + "+" + (even - p);
        return "none";
    }

    static string Factor(int n)
    {
        var sb = new StringBuilder();
        for (int d = 2; d * d <= n; d++)
        {
            int e = 0;
            while (n % d == 0) { n /= d; e++; }
            if (e > 0)
            {
                if (sb.Length > 0) sb.Append('*');
                sb.Append(d);
                if (e > 1) sb.Append('^').Append(e);
            }
        }
        if (n > 1)
        {
            if (sb.Length > 0) sb.Append('*');
            sb.Append(n);
        }
        return sb.ToString();
    }

    public static string Run(string input)
    {
        int n = int.Parse(input, System.Globalization.CultureInfo.InvariantCulture);
        if (n < 2) return "small";
        var sieve = Sieve(n);
        int largest = n;
        while (sieve[largest]) largest--;
        string g = n % 2 == 0 && n > 2 ? Goldbach(sieve, n) : "-";
        return "pi=" + CountPrimes(sieve) + " largest=" + largest + " goldbach=" + g + " factor=" + Factor(n);
    }
}
