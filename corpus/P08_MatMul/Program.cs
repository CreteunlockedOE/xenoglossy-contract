using System.Text;

namespace Corpus;

public static class Program
{
    static int[][] Make(int n, int seed)
    {
        var m = new int[n][];
        int x = seed;
        for (int i = 0; i < n; i++)
        {
            m[i] = new int[n];
            for (int j = 0; j < n; j++)
            {
                x = unchecked(x * 1103515245 + 12345);
                m[i][j] = (x >> 16) % 10;
            }
        }
        return m;
    }

    static int[][] Multiply(int[][] a, int[][] b)
    {
        int n = a.Length;
        var c = new int[n][];
        for (int i = 0; i < n; i++)
        {
            c[i] = new int[n];
            for (int j = 0; j < n; j++)
            {
                int sum = 0;
                for (int k = 0; k < n; k++) sum += a[i][k] * b[k][j];
                c[i][j] = sum;
            }
        }
        return c;
    }

    static int[][] Transpose(int[][] a)
    {
        var t = new int[a.Length][];
        for (int i = 0; i < a.Length; i++) t[i] = new int[a.Length];
        for (int i = 0; i < a.Length; i++)
            for (int j = 0; j < a.Length; j++) t[j][i] = a[i][j];
        return t;
    }

    static int Trace(int[][] m)
    {
        int t = 0;
        for (int i = 0; i < m.Length; i++) t += m[i][i];
        return t;
    }

    static string Format(int[][] m)
    {
        var sb = new StringBuilder();
        foreach (int[] row in m)
        {
            for (int j = 0; j < row.Length; j++)
            {
                if (j > 0) sb.Append(' ');
                sb.Append(row[j]);
            }
            sb.Append('/');
        }
        return sb.ToString();
    }

    public static string Run(string input)
    {
        string[] parts = input.Split(',');
        int n = int.Parse(parts[0]);
        int seed = parts.Length > 1 ? int.Parse(parts[1]) : 1;
        var a = Make(n, seed);
        var p = Multiply(a, Transpose(a));
        return Format(p) + " trace=" + Trace(p);
    }
}
