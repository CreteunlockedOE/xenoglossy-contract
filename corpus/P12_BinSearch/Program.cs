using System.Text;

namespace Corpus;

public static class Program
{
    static int[] ParseList(string s)
    {
        var parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var arr = new int[parts.Length];
        for (int i = 0; i < parts.Length; i++) arr[i] = int.Parse(parts[i].Trim(), System.Globalization.CultureInfo.InvariantCulture);
        return arr;
    }

    static void InsertionSort(int[] a)
    {
        for (int i = 1; i < a.Length; i++)
        {
            int key = a[i];
            int j = i - 1;
            while (j >= 0 && a[j] > key) { a[j + 1] = a[j]; j--; }
            a[j + 1] = key;
        }
    }

    static int Find(int[] a, int target, out int probes)
    {
        int lo = 0, hi = a.Length - 1;
        probes = 0;
        while (lo <= hi)
        {
            probes++;
            int mid = lo + (hi - lo) / 2;
            if (a[mid] == target) return mid;
            if (a[mid] < target) lo = mid + 1; else hi = mid - 1;
        }
        return ~lo;
    }

    static int LowerBound(int[] a, int target)
    {
        int lo = 0, hi = a.Length;
        while (lo < hi)
        {
            int mid = (lo + hi) >> 1;
            if (a[mid] < target) lo = mid + 1; else hi = mid;
        }
        return lo;
    }

    public static string Run(string input)
    {
        int bar = input.IndexOf('|');
        int target = int.Parse(input.Substring(0, bar), System.Globalization.CultureInfo.InvariantCulture);
        int[] a = ParseList(input.Substring(bar + 1));
        InsertionSort(a);
        int probes;
        int idx = Find(a, target, out probes);
        var sb = new StringBuilder();
        foreach (int x in a) sb.Append(x).Append(' ');
        return sb.ToString().TrimEnd() + " => idx=" + idx + " probes=" + probes + " lb=" + LowerBound(a, target);
    }
}
