using System.Text;

namespace Corpus;

public static class Program
{
    static int PopCount(uint x)
    {
        int c = 0;
        while (x != 0) { x &= x - 1; c++; }
        return c;
    }

    static uint ReverseBits(uint x)
    {
        uint r = 0;
        for (int i = 0; i < 32; i++)
        {
            r = (r << 1) | (x & 1);
            x >>= 1;
        }
        return r;
    }

    static uint RotL(uint x, int k) { k &= 31; return (x << k) | (x >> (32 - k)); }

    static int LeadingZeros(uint x)
    {
        if (x == 0) return 32;
        int n = 0;
        if ((x & 0xFFFF0000) == 0) { n += 16; x <<= 16; }
        if ((x & 0xFF000000) == 0) { n += 8; x <<= 8; }
        if ((x & 0xF0000000) == 0) { n += 4; x <<= 4; }
        if ((x & 0xC0000000) == 0) { n += 2; x <<= 2; }
        if ((x & 0x80000000) == 0) n += 1;
        return n;
    }

    static bool IsPow2(long x) { return x > 0 && (x & (x - 1)) == 0; }

    static string Bin(byte b)
    {
        var sb = new StringBuilder();
        for (int i = 7; i >= 0; i--) sb.Append((b >> i) & 1);
        return sb.ToString();
    }

    public static string Run(string input)
    {
        long v = long.Parse(input);
        uint u = (uint)v;
        int s = (int)v;
        var sb = new StringBuilder();
        sb.Append("pop=").Append(PopCount(u));
        sb.Append(" rev=").Append(ReverseBits(u));
        sb.Append(" rotl=").Append(RotL(u, (int)(v & 7) + 1));
        sb.Append(" lz=").Append(LeadingZeros(u));
        sb.Append(" pow2=").Append(IsPow2(v));
        sb.Append(" sar=").Append(s >> 3).Append(" shr=").Append(u >> 3);
        sb.Append(" lsh=").Append(v << 33);
        sb.Append(" b=").Append(Bin((byte)v));
        sb.Append(" sb=").Append((sbyte)v).Append(" us=").Append((ushort)v);
        sb.Append(" neg=").Append(-s).Append(" not=").Append(~u);
        return sb.ToString();
    }
}
