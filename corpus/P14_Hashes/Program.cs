using System.Text;

namespace Corpus;

public static class Program
{
    static uint Fnv1a(string s)
    {
        uint h = 2166136261u;
        foreach (char c in s)
        {
            h ^= (byte)c;
            h *= 16777619u;
        }
        return h;
    }

    static ulong Fnv64(string s)
    {
        ulong h = 14695981039346656037UL;
        for (int i = 0; i < s.Length; i++)
        {
            h ^= s[i];
            h *= 1099511628211UL;
        }
        return h;
    }

    static uint Djb2(string s)
    {
        uint h = 5381;
        foreach (char c in s) h = (h << 5) + h + c;
        return h;
    }

    static int JavaHash(string s)
    {
        int h = 0;
        for (int i = 0; i < s.Length; i++) h = unchecked(31 * h + s[i]);
        return h;
    }

    static uint XorShift(uint x, int rounds)
    {
        if (x == 0) x = 1;
        for (int i = 0; i < rounds; i++)
        {
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
        }
        return x;
    }

    static uint Murmur3Mix(uint k)
    {
        k *= 0xcc9e2d51;
        k = (k << 15) | (k >> 17);
        k *= 0x1b873593;
        return k;
    }

    public static string Run(string input)
    {
        uint a = Fnv1a(input);
        var sb = new StringBuilder();
        sb.Append("fnv32=").Append(a);
        sb.Append(" fnv64=").Append(Fnv64(input));
        sb.Append(" djb2=").Append(Djb2(input));
        sb.Append(" java=").Append(JavaHash(input));
        sb.Append(" xs=").Append(XorShift(a, input.Length % 7 + 1));
        sb.Append(" mm=").Append(Murmur3Mix(a));
        sb.Append(" bucket=").Append(a % 16);
        return sb.ToString();
    }
}
