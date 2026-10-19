namespace Corpus;

public static class Program
{
    static long Gcd(long a, long b)
    {
        while (b != 0) { long t = a % b; a = b; b = t; }
        return a < 0 ? -a : a;
    }

    static long ModPow(long b, long e, long m)
    {
        long result = 1;
        b %= m;
        while (e > 0)
        {
            if ((e & 1) == 1) result = result * b % m;
            b = b * b % m;
            e >>= 1;
        }
        return result;
    }

    static long ExtGcd(long a, long b, out long x, out long y)
    {
        if (b == 0) { x = 1; y = 0; return a; }
        long x1, y1;
        long g = ExtGcd(b, a % b, out x1, out y1);
        x = y1;
        y = x1 - (a / b) * y1;
        return g;
    }

    static int IntPow(int b, int e)
    {
        int r = 1;
        for (int i = 0; i < e; i++) r *= b;
        return r;
    }

    static bool IsProbablePrime(long n)
    {
        if (n < 2) return false;
        if (n % 2 == 0) return n == 2;
        for (long d = 3; d * d <= n; d += 2) if (n % d == 0) return false;
        return true;
    }

    public static string Run(string input)
    {
        string[] p = input.Split(' ');
        long a = long.Parse(p[0]);
        long b = long.Parse(p[1]);
        long m = long.Parse(p[2]);
        long g = Gcd(a, b);
        long lcm = a / g * b;
        long x, y;
        ExtGcd(a, b, out x, out y);
        string inv = "none";
        if (Gcd(a, m) == 1)
        {
            long ix, iy;
            ExtGcd(((a % m) + m) % m, m, out ix, out iy);
            inv = (((ix % m) + m) % m).ToString();
        }
        return "gcd=" + g + " lcm=" + lcm + " bezout=" + x + "," + y + " pow=" + ModPow(a, b, m) +
               " wrap=" + IntPow((int)a, (int)(b % 40)) + " inv=" + inv + " prime=" + IsProbablePrime(m);
    }
}
