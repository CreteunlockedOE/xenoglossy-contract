using System.Text;

namespace Corpus;

public static class Program
{
    static string Reverse(string s)
    {
        var chars = s.ToCharArray();
        for (int i = 0, j = chars.Length - 1; i < j; i++, j--)
        {
            char t = chars[i]; chars[i] = chars[j]; chars[j] = t;
        }
        return new string(chars);
    }

    static bool IsPalindrome(string s)
    {
        int i = 0, j = s.Length - 1;
        while (i < j)
        {
            while (i < j && !char.IsLetterOrDigit(s[i])) i++;
            while (i < j && !char.IsLetterOrDigit(s[j])) j--;
            if (char.ToLowerInvariant(s[i]) != char.ToLowerInvariant(s[j])) return false;
            i++; j--;
        }
        return true;
    }

    static string Caesar(string s, int shift)
    {
        var sb = new StringBuilder(s.Length);
        foreach (char c in s)
        {
            if (c >= 'a' && c <= 'z') sb.Append((char)('a' + ((c - 'a' + shift) % 26 + 26) % 26));
            else if (c >= 'A' && c <= 'Z') sb.Append((char)('A' + ((c - 'A' + shift) % 26 + 26) % 26));
            else sb.Append(c);
        }
        return sb.ToString();
    }

    static int Vowels(string s)
    {
        int n = 0;
        foreach (char c in s)
        {
            switch (char.ToLowerInvariant(c))
            {
                case 'a': case 'e': case 'i': case 'o': case 'u': n++; break;
            }
        }
        return n;
    }

    static string TitleCase(string s)
    {
        var sb = new StringBuilder();
        bool start = true;
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            sb.Append(start ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c));
            start = c == ' ';
        }
        return sb.ToString();
    }

    public static string Run(string input)
    {
        string head = input.Length > 3 ? input.Substring(0, 3) : input;
        string tail = input.Length > 3 ? input.Substring(input.Length - 3) : "";
        return Reverse(input) + "|" + IsPalindrome(input) + "|" + Caesar(input, 3) + "|" + Vowels(input) + "|" +
               TitleCase(input) + "|" + head + "~" + tail + "|" + string.Concat(input, input.Length.ToString());
    }
}
