using System.Text;

namespace Corpus;

public static class Program
{
    static List<string> Words(string s)
    {
        var list = new List<string>();
        var cur = new StringBuilder();
        foreach (char c in s)
        {
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '\'')
                cur.Append(char.ToLowerInvariant(c));
            else if (cur.Length > 0)
            {
                list.Add(cur.ToString());
                cur.Clear();
            }
        }
        if (cur.Length > 0) list.Add(cur.ToString());
        return list;
    }

    static Dictionary<string, int> Count(List<string> words)
    {
        var d = new Dictionary<string, int>();
        foreach (string w in words)
        {
            int n;
            if (d.TryGetValue(w, out n)) d[w] = n + 1;
            else d.Add(w, 1);
        }
        return d;
    }

    static List<string> SortedKeys(Dictionary<string, int> d)
    {
        var keys = new List<string>(d.Keys);
        for (int i = 1; i < keys.Count; i++)
        {
            string k = keys[i];
            int j = i - 1;
            while (j >= 0 && Before(d, k, keys[j]))
            {
                keys[j + 1] = keys[j];
                j--;
            }
            keys[j + 1] = k;
        }
        return keys;
    }

    static bool Before(Dictionary<string, int> d, string a, string b)
    {
        if (d[a] != d[b]) return d[a] > d[b];
        return string.CompareOrdinal(a, b) < 0;
    }

    public static string Run(string input)
    {
        List<string> words = Words(input);
        Dictionary<string, int> freq = Count(words);
        List<string> keys = SortedKeys(freq);
        var sb = new StringBuilder();
        int shown = 0;
        foreach (string k in keys)
        {
            if (shown++ == 4) break;
            sb.Append(k).Append('=').Append(freq[k]).Append(' ');
        }
        int longest = 0;
        foreach (string w in words) if (w.Length > longest) longest = w.Length;
        return sb + "total=" + words.Count + " distinct=" + keys.Count + " longest=" + longest;
    }
}
