using System.Text;

namespace Corpus;

public static class Program
{
    static uint[] BuildTable()
    {
        var table = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            uint c = i;
            for (int k = 0; k < 8; k++)
            {
                if ((c & 1) != 0) c = 0xEDB88320u ^ (c >> 1);
                else c >>= 1;
            }
            table[i] = c;
        }
        return table;
    }

    static uint Crc32(byte[] data, uint[] table)
    {
        uint crc = 0xFFFFFFFFu;
        for (int i = 0; i < data.Length; i++)
            crc = table[(crc ^ data[i]) & 0xFF] ^ (crc >> 8);
        return ~crc;
    }

    static uint Adler32(byte[] data)
    {
        uint a = 1, b = 0;
        foreach (byte x in data)
        {
            a = (a + x) % 65521;
            b = (b + a) % 65521;
        }
        return (b << 16) | a;
    }

    static string Hex(uint v)
    {
        const string digits = "0123456789abcdef";
        var sb = new StringBuilder();
        for (int shift = 28; shift >= 0; shift -= 4)
            sb.Append(digits[(int)((v >> shift) & 0xF)]);
        return sb.ToString();
    }

    static byte[] ToBytes(string s)
    {
        var bytes = new byte[s.Length];
        for (int i = 0; i < s.Length; i++) bytes[i] = (byte)(s[i] & 0xFF);
        return bytes;
    }

    public static string Run(string input)
    {
        var data = ToBytes(input);
        var table = BuildTable();
        uint crc = Crc32(data, table);
        uint adler = Adler32(data);
        int signedView = unchecked((int)crc);
        return "crc=" + Hex(crc) + " adler=" + Hex(adler) + " len=" + data.Length + " signed=" + signedView;
    }
}
