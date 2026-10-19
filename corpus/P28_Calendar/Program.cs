using System.Text;

namespace Corpus;

public static class Program
{
    static bool IsLeap(int y) { return (y % 4 == 0 && y % 100 != 0) || y % 400 == 0; }

    static int DaysInMonth(int y, int m)
    {
        switch (m)
        {
            case 2: return IsLeap(y) ? 29 : 28;
            case 4: case 6: case 9: case 11: return 30;
            case 1: case 3: case 5: case 7: case 8: case 10: case 12: return 31;
            default: throw new ArgumentOutOfRangeException("m", "month " + m);
        }
    }

    static int DayOfWeek(int y, int m, int d)
    {
        // Zeller-style (Sakamoto); 0 = Sunday
        int[] t = { 0, 3, 2, 5, 0, 3, 5, 1, 4, 6, 2, 4 };
        if (m < 3) y -= 1;
        return (y + y / 4 - y / 100 + y / 400 + t[m - 1] + d) % 7;
    }

    static int DayOfYear(int y, int m, int d)
    {
        int n = d;
        for (int i = 1; i < m; i++) n += DaysInMonth(y, i);
        return n;
    }

    static int DaysSinceEpoch(int y, int m, int d)
    {
        int days = 0;
        for (int i = 1970; i < y; i++) days += IsLeap(i) ? 366 : 365;
        for (int i = 1970; i > y; i--) days -= IsLeap(i - 1) ? 366 : 365;
        return days + DayOfYear(y, m, d) - 1;
    }

    static string DayName(int w)
    {
        string[] names = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        return names[w];
    }

    public static string Run(string input)
    {
        string[] p = input.Split('-');
        int y = int.Parse(p[0]), m = int.Parse(p[1]), d = int.Parse(p[2]);
        if (d < 1 || d > DaysInMonth(y, m)) return "invalid day";
        return DayName(DayOfWeek(y, m, d)) + " doy=" + DayOfYear(y, m, d) + " epoch=" + DaysSinceEpoch(y, m, d) + " leap=" + IsLeap(y);
    }
}
