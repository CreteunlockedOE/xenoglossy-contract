using System.Text;

namespace Corpus;

public class RpnException : Exception
{
    public RpnException(string message) : base(message) { }
}

public static class Program
{
    static bool IsNumber(string tok)
    {
        if (tok.Length == 0) return false;
        int start = tok[0] == '-' ? 1 : 0;
        if (start == tok.Length) return false;
        for (int i = start; i < tok.Length; i++)
            if (tok[i] < '0' || tok[i] > '9') return false;
        return true;
    }

    static int ParseInt(string tok)
    {
        int sign = 1, i = 0;
        if (tok[0] == '-') { sign = -1; i = 1; }
        int v = 0;
        for (; i < tok.Length; i++) v = unchecked(v * 10 + (tok[i] - '0'));
        return unchecked(v * sign);
    }

    static int Apply(char op, int a, int b)
    {
        switch (op)
        {
            case '+': return unchecked(a + b);
            case '-': return unchecked(a - b);
            case '*': return unchecked(a * b);
            case '/': return a / b;
            case '%': return a % b;
            case '^':
                int r = 1;
                for (int i = 0; i < b; i++) r = unchecked(r * a);
                return r;
            default: throw new RpnException("unknown operator " + op);
        }
    }

    static int Eval(string expr, StringBuilder trace)
    {
        var stack = new int[16];
        int sp = 0;
        foreach (string tok in expr.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (IsNumber(tok))
            {
                if (sp == stack.Length) throw new RpnException("stack overflow");
                stack[sp++] = ParseInt(tok);
            }
            else
            {
                if (sp < 2) throw new RpnException("stack underflow at '" + tok + "'");
                int b = stack[--sp];
                int a = stack[--sp];
                int r = Apply(tok[0], a, b);
                trace.Append(a).Append(tok).Append(b).Append('=').Append(r).Append(';');
                stack[sp++] = r;
            }
        }
        if (sp != 1) throw new RpnException("leftover " + sp);
        return stack[0];
    }

    public static string Run(string input)
    {
        var trace = new StringBuilder();
        int result = Eval(input, trace);
        Console.WriteLine("steps: " + trace);
        return result.ToString();
    }
}
