using System.Text;

namespace Corpus;

public static class Program
{
    const int Start = 0, InWord = 1, InNumber = 2, InString = 3, Escape = 4, LineComment = 5, MaybeComment = 6, Done = 7;

    static int Classify(char c)
    {
        if (c >= '0' && c <= '9') return 0;
        if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) return 1;
        switch (c)
        {
            case ' ': return 2;
            case '"': return 3;
            case '\\': return 4;
            case '/': return 5;
            case '\n': return 6;
            case '#': return 7;
            default: return 8;
        }
    }

    static int Step(int state, int cls)
    {
        switch (state)
        {
            case Start:
                switch (cls) { case 0: return InNumber; case 1: return InWord; case 3: return InString; case 5: return MaybeComment; case 7: return Done; default: return Start; }
            case InWord:
                return cls == 0 || cls == 1 ? InWord : (cls == 3 ? InString : Start);
            case InNumber:
                return cls == 0 ? InNumber : (cls == 1 ? InWord : Start);
            case InString:
                return cls == 4 ? Escape : (cls == 3 ? Start : InString);
            case Escape:
                return InString;
            case MaybeComment:
                return cls == 5 ? LineComment : Start;
            case LineComment:
                return cls == 6 ? Start : LineComment;
            default:
                return Done;
        }
    }

    public static string Run(string input)
    {
        int state = Start;
        var counts = new int[8];
        var path = new StringBuilder();
        int last = -1;
        for (int i = 0; i < input.Length; i++)
        {
            int next = Step(state, Classify(input[i]));
            if (next != state && next != last)
            {
                counts[next]++;
                last = next;
            }
            if (next != state) path.Append(next);
            state = next;
            if (state == Done)
            {
                Console.WriteLine("halt at " + i);
                break;
            }
        }
        string cs = "";
        for (int k = 0; k < counts.Length; k++) cs += counts[k] + (k < counts.Length - 1 ? "." : "");
        return "final=" + state + " path=" + path + " counts=" + cs;
    }
}
