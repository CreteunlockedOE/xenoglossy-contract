using System.Text;

namespace Corpus;

public static class Program
{
    static int[] Parse(string s)
    {
        if (s.Length == 0) return new int[0];
        string[] parts = s.Split(' ');
        var a = new int[parts.Length];
        for (int i = 0; i < a.Length; i++) a[i] = int.Parse(parts[i]);
        return a;
    }

    static int BubbleSort(int[] a)
    {
        int swaps = 0;
        bool swapped = true;
        int n = a.Length;
        while (swapped)
        {
            swapped = false;
            for (int i = 1; i < n; i++)
            {
                if (a[i - 1] > a[i])
                {
                    int t = a[i - 1]; a[i - 1] = a[i]; a[i] = t;
                    swaps++;
                    swapped = true;
                }
            }
            n--;
        }
        return swaps;
    }

    static void QuickSort(int[] a, int lo, int hi)
    {
        if (lo >= hi) return;
        int pivot = a[(lo + hi) / 2];
        int i = lo, j = hi;
        while (i <= j)
        {
            while (a[i] < pivot) i++;
            while (a[j] > pivot) j--;
            if (i <= j)
            {
                int t = a[i]; a[i] = a[j]; a[j] = t;
                i++; j--;
            }
        }
        QuickSort(a, lo, j);
        QuickSort(a, i, hi);
    }

    static bool IsSorted(int[] a)
    {
        for (int i = 1; i < a.Length; i++) if (a[i - 1] > a[i]) return false;
        return true;
    }

    static string Join(int[] a)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < a.Length; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append(a[i]);
        }
        return sb.ToString();
    }

    public static string Run(string input)
    {
        int[] a = Parse(input);
        int[] b = (int[])a.Clone();
        int swaps = BubbleSort(a);
        QuickSort(b, 0, b.Length - 1);
        Console.WriteLine("bubble: " + Join(a));
        Console.WriteLine("quick: " + Join(b));
        bool same = Join(a) == Join(b);
        return Join(b) + " swaps=" + swaps + " sorted=" + IsSorted(a) + " same=" + same;
    }
}
