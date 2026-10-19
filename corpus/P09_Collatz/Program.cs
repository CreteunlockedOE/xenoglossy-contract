namespace Corpus;

public static class Program
{
    static int Steps(long n, out long peak)
    {
        int steps = 0;
        peak = n;
        while (n != 1)
        {
            if ((n & 1) == 0) n >>= 1;
            else n = 3 * n + 1;
            if (n > peak) peak = n;
            steps++;
        }
        return steps;
    }

    static int StepsRec(long n)
    {
        if (n == 1) return 0;
        return 1 + StepsRec(n % 2 == 0 ? n / 2 : 3 * n + 1);
    }

    static long BestStart(int limit, out int bestSteps)
    {
        long best = 1;
        bestSteps = 0;
        for (int i = 1; i <= limit; i++)
        {
            long peak;
            int s = Steps(i, out peak);
            if (s > bestSteps) { bestSteps = s; best = i; }
        }
        return best;
    }

    public static string Run(string input)
    {
        long n = long.Parse(input, System.Globalization.CultureInfo.InvariantCulture);
        if (n < 1) return "invalid";
        long peak;
        int s = Steps(n, out peak);
        Console.WriteLine("steps=" + s);
        int bestSteps;
        long best = BestStart((int)(n % 500) + 1, out bestSteps);
        return "steps=" + s + " peak=" + peak + " rec=" + StepsRec(n) + " best=" + best + "/" + bestSteps;
    }
}
