using System.Text;

namespace Corpus;

public static class Program
{
    static string Encode(string s)
    {
        var sb = new StringBuilder();
        int i = 0;
        while (i < s.Length)
        {
            int j = i;
            while (j < s.Length && s[j] == s[i] && j - i < 99) j++;
            int run = j - i;
            if (run > 1) sb.Append(run);
            sb.Append(s[i]);
            i = j;
        }
        return sb.ToString();
    }

    static string Decode(string s)
    {
        var sb = new StringBuilder();
        int count = 0;
        foreach (char c in s)
        {
            if (c >= '0' && c <= '9') count = count * 10 + (c - '0');
            else
            {
                sb.Append(c, count == 0 ? 1 : count);
                count = 0;
            }
        }
        return sb.ToString();
    }

    static int LongestRun(string s)
    {
        int best = 0, cur = 0;
        char prev = '\0';
        for (int i = 0; i < s.Length; i++)
        {
            if (i > 0 && s[i] == prev) cur++; else cur = 1;
            prev = s[i];
            if (cur > best) best = cur;
        }
        return best;
    }

    public static string Run(string input)
    {
        string enc = Encode(input);
        string dec = Decode(enc);
        bool ok = dec == input;
        return enc + " ok=" + ok + " longest=" + LongestRun(input) + " ratio=" + (enc.Length * 100 / (input.Length == 0 ? 1 : input.Length));
    }
}
