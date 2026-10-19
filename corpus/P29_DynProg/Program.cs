using System.Text;

namespace Corpus;

public static class Program
{
    static int Lcs(string a, string b, out string seq)
    {
        var t = new int[a.Length + 1][];
        for (int i = 0; i <= a.Length; i++) t[i] = new int[b.Length + 1];
        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
                t[i][j] = a[i - 1] == b[j - 1] ? t[i - 1][j - 1] + 1 : (t[i - 1][j] >= t[i][j - 1] ? t[i - 1][j] : t[i][j - 1]);
        var sb = new StringBuilder();
        int x = a.Length, y = b.Length;
        while (x > 0 && y > 0)
        {
            if (a[x - 1] == b[y - 1]) { sb.Insert(0, a[x - 1]); x--; y--; }
            else if (t[x - 1][y] >= t[x][y - 1]) x--;
            else y--;
        }
        seq = sb.ToString();
        return t[a.Length][b.Length];
    }

    static int EditDistance(string a, string b)
    {
        var d = new int[a.Length + 1][];
        for (int i = 0; i <= a.Length; i++) { d[i] = new int[b.Length + 1]; d[i][0] = i; }
        for (int j = 0; j <= b.Length; j++) d[0][j] = j;
        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                int best = d[i - 1][j] + 1;
                if (d[i][j - 1] + 1 < best) best = d[i][j - 1] + 1;
                if (d[i - 1][j - 1] + cost < best) best = d[i - 1][j - 1] + cost;
                d[i][j] = best;
            }
        }
        return d[a.Length][b.Length];
    }

    static int Knapsack(int[] w, int[] v, int cap)
    {
        var best = new int[cap + 1];
        for (int i = 0; i < w.Length; i++)
            for (int c = cap; c >= w[i]; c--)
                if (best[c - w[i]] + v[i] > best[c]) best[c] = best[c - w[i]] + v[i];
        return best[cap];
    }

    public static string Run(string input)
    {
        string[] p = input.Split(',');
        string seq;
        int lcs = Lcs(p[0], p[1], out seq);
        int ed = EditDistance(p[0], p[1]);
        int n = Math.Min(p[0].Length, p[1].Length);
        var w = new int[n];
        var v = new int[n];
        for (int i = 0; i < n; i++) { w[i] = p[0][i] % 7 + 1; v[i] = p[1][i] % 11 + 1; }
        return "lcs=" + lcs + "(" + seq + ") edit=" + ed + " knap=" + Knapsack(w, v, 15);
    }
}
