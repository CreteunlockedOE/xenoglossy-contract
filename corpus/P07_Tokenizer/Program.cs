using System.Text;

namespace Corpus;

public enum TokKind { Number, Ident, Op, Str, Space }

public class Token
{
    public TokKind Kind;
    public string Text;
    public int Pos;
    public Token(TokKind kind, string text, int pos) { Kind = kind; Text = text; Pos = pos; }
}

public class LexException : Exception
{
    public LexException(string msg, int pos) : base(msg + " @" + pos) { }
}

public static class Program
{
    static bool IsLetter(char c) { return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_'; }
    static bool IsDigit(char c) { return c >= '0' && c <= '9'; }

    static List<Token> Tokenize(string s)
    {
        var list = new List<Token>();
        int i = 0;
        while (i < s.Length)
        {
            char c = s[i];
            int start = i;
            if (c == ' ' || c == '\t') { i++; continue; }
            if (IsDigit(c))
            {
                while (i < s.Length && IsDigit(s[i])) i++;
                list.Add(new Token(TokKind.Number, s.Substring(start, i - start), start));
            }
            else if (IsLetter(c))
            {
                while (i < s.Length && (IsLetter(s[i]) || IsDigit(s[i]))) i++;
                list.Add(new Token(TokKind.Ident, s.Substring(start, i - start), start));
            }
            else if (c == '"')
            {
                i++;
                var sb = new StringBuilder();
                while (i < s.Length && s[i] != '"')
                {
                    if (s[i] == '\\' && i + 1 < s.Length) i++;
                    sb.Append(s[i]);
                    i++;
                }
                if (i >= s.Length) throw new LexException("unterminated string", start);
                i++;
                list.Add(new Token(TokKind.Str, sb.ToString(), start));
            }
            else if ("+-*/=<>!&|(){};,".IndexOf(c) >= 0)
            {
                i++;
                if (i < s.Length && s[i] == '=' && "=<>!".IndexOf(c) >= 0) i++;
                list.Add(new Token(TokKind.Op, s.Substring(start, i - start), start));
            }
            else throw new LexException("unexpected '" + c + "'", i);
        }
        return list;
    }

    static string Describe(List<Token> toks)
    {
        var sb = new StringBuilder();
        foreach (Token t in toks)
        {
            switch (t.Kind)
            {
                case TokKind.Number: sb.Append("N(").Append(t.Text).Append(')'); break;
                case TokKind.Ident: sb.Append("I(").Append(t.Text).Append(')'); break;
                case TokKind.Op: sb.Append("O(").Append(t.Text).Append(')'); break;
                case TokKind.Str: sb.Append("S(").Append(t.Text.Length).Append(')'); break;
            }
        }
        return sb.ToString();
    }

    public static string Run(string input)
    {
        List<Token> toks = Tokenize(input);
        int numSum = 0;
        foreach (Token t in toks) if (t.Kind == TokKind.Number) numSum += t.Text.Length * 7 + (t.Text[0] - '0');
        return toks.Count + ":" + Describe(toks) + ":" + numSum;
    }
}
