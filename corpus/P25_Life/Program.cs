using System.Text;

namespace Corpus;

public static class Program
{
    static byte[][] Parse(string s)
    {
        string[] rows = s.Split('/');
        var g = new byte[rows.Length][];
        for (int r = 0; r < rows.Length; r++)
        {
            g[r] = new byte[rows[r].Length];
            for (int c = 0; c < rows[r].Length; c++) g[r][c] = (byte)(rows[r][c] == '#' ? 1 : 0);
        }
        return g;
    }

    static int Neighbors(byte[][] g, int r, int c)
    {
        int n = 0;
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int rr = r + dr, cc = c + dc;
                if (rr < 0 || rr >= g.Length || cc < 0 || cc >= g[rr].Length) continue;
                n += g[rr][cc];
            }
        }
        return n;
    }

    static byte[][] Step(byte[][] g)
    {
        var next = new byte[g.Length][];
        for (int r = 0; r < g.Length; r++)
        {
            next[r] = new byte[g[r].Length];
            for (int c = 0; c < g[r].Length; c++)
            {
                int n = Neighbors(g, r, c);
                if (g[r][c] == 1) next[r][c] = (byte)(n == 2 || n == 3 ? 1 : 0);
                else next[r][c] = (byte)(n == 3 ? 1 : 0);
            }
        }
        return next;
    }

    static string Render(byte[][] g)
    {
        var sb = new StringBuilder();
        for (int r = 0; r < g.Length; r++)
        {
            if (r > 0) sb.Append('/');
            foreach (byte b in g[r]) sb.Append(b == 1 ? '#' : '.');
        }
        return sb.ToString();
    }

    static int Alive(byte[][] g)
    {
        int n = 0;
        foreach (byte[] row in g) foreach (byte b in row) n += b;
        return n;
    }

    public static string Run(string input)
    {
        int colon = input.IndexOf(':');
        int gens = int.Parse(input.Substring(0, colon));
        byte[][] g = Parse(input.Substring(colon + 1));
        for (int i = 0; i < gens; i++) g = Step(g);
        return Render(g) + " alive=" + Alive(g);
    }
}
