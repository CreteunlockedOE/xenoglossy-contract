using System.Text;

namespace Corpus;

public static class Program
{
    static Dictionary<int, long> memo = new Dictionary<int, long>();

    static long FibMemo(int n)
    {
        if (n < 2) return n;
        long v;
        if (memo.TryGetValue(n, out v)) return v;
        v = FibMemo(n - 1) + FibMemo(n - 2);
        memo[n] = v;
        return v;
    }

    static int FibInt(int n)
    {
        if (n < 2) return n;
        return unchecked(FibInt(n - 1) + FibInt(n - 2));
    }

    static void Hanoi(int n, char from, char to, char via, StringBuilder moves, ref int count)
    {
        if (n <= 0) return;
        Hanoi(n - 1, from, via, to, moves, ref count);
        count++;
        if (count <= 8) moves.Append(from).Append(to).Append(' ');
        Hanoi(n - 1, via, to, from, moves, ref count);
    }

    static int Ackermann(int m, int n)
    {
        if (m == 0) return n + 1;
        if (n == 0) return Ackermann(m - 1, 1);
        return Ackermann(m - 1, Ackermann(m, n - 1));
    }

    static long Factorial(int n)
    {
        return n <= 1 ? 1 : n * Factorial(n - 1);
    }

    static int Depth(int n)
    {
        if (n <= 0) throw new InvalidOperationException("bottom reached");
        return 1 + Depth(n - 1);
    }

    public static string Run(string input)
    {
        memo.Clear();
        int n = int.Parse(input);
        var moves = new StringBuilder();
        int count = 0;
        Hanoi(n % 12, 'A', 'C', 'B', moves, ref count);
        string d;
        try { d = Depth(n).ToString(); }
        catch (InvalidOperationException e) { d = "err:" + e.Message; }
        return "fibm=" + FibMemo(n) + " fibi=" + FibInt(n % 40) + " fact=" + Factorial(n) +
               " ack=" + Ackermann(2, (n % 6 + 6) % 6) + " hanoi=" + count + "[" + moves.ToString().TrimEnd() + "] depth=" + d;
    }
}
