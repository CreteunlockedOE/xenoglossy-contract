using System.Text;

namespace Corpus;

public static class Program
{
    static char Closer(char open)
    {
        switch (open)
        {
            case '(': return ')';
            case '[': return ']';
            case '{': return '}';
            case '<': return '>';
            default: return '\0';
        }
    }

    static int Check(string s, out int maxDepth)
    {
        var stack = new char[s.Length + 1];
        int sp = 0;
        maxDepth = 0;
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            char cl = Closer(c);
            if (cl != '\0')
            {
                stack[sp++] = cl;
                if (sp > maxDepth) maxDepth = sp;
            }
            else if (c == ')' || c == ']' || c == '}' || c == '>')
            {
                if (sp == 0 || stack[--sp] != c) return i;
            }
        }
        return sp == 0 ? -1 : s.Length;
    }

    static string Describe(int rc)
    {
        try
        {
            if (rc == -1) return "balanced";
            Console.WriteLine("error at " + rc);
            return "broken@" + rc;
        }
        finally
        {
            Console.WriteLine("described " + rc);
        }
    }

    static int CountPairs(string s, int i, int depth)
    {
        if (i >= s.Length) return 0;
        int here = Closer(s[i]) != '\0' ? depth + 1 : 0;
        return here + CountPairs(s, i + 1, Closer(s[i]) != '\0' ? depth + 1 : (s[i] == ')' || s[i] == ']' || s[i] == '}' || s[i] == '>') ? Math.Max(depth - 1, 0) : depth);
    }

    public static string Run(string input)
    {
        int depth;
        int rc = Check(input, out depth);
        string d = Describe(rc);
        return d + " depth=" + depth + " weight=" + CountPairs(input, 0, 0);
    }
}
