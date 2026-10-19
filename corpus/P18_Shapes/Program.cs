using System.Text;

namespace Corpus;

public interface IShape
{
    int Area();
    int Perimeter();
    string Name { get; }
}

public abstract class ShapeBase : IShape
{
    public static int Count;
    protected int A, B;
    protected ShapeBase(int a, int b) { A = a; B = b; Count++; }
    public abstract int Area();
    public virtual int Perimeter() { return 2 * (A + B); }
    public virtual string Name { get { return "shape"; } }
    public override string ToString() { return Name + "[" + Area() + "/" + Perimeter() + "]"; }
}

public class Rect : ShapeBase
{
    public Rect(int w, int h) : base(w, h) { }
    public override int Area() { return A * B; }
    public override string Name { get { return "rect"; } }
}

public class Square : Rect
{
    public Square(int s) : base(s, s) { }
    public override string Name { get { return "square"; } }
}

public class Triangle : ShapeBase
{
    int c;
    public Triangle(int a, int b, int c) : base(a, b) { this.c = c; }
    public override int Area()
    {
        int s2 = A + B + c;
        // 16*area^2 via Heron, integer sqrt
        long v = (long)s2 * (s2 - 2 * A) * (s2 - 2 * B) * (s2 - 2 * c);
        if (v < 0) return -1;
        long r = 0;
        while ((r + 1) * (r + 1) * 4 <= v) r++;
        return (int)(r / 2);
    }
    public override int Perimeter() { return A + B + c; }
    public override string Name { get { return "tri"; } }
}

public static class Program
{
    static IShape Make(string spec)
    {
        string[] p = spec.Split(':');
        switch (p[0])
        {
            case "r": return new Rect(int.Parse(p[1]), int.Parse(p[2]));
            case "s": return new Square(int.Parse(p[1]));
            case "t": return new Triangle(int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]));
            default: throw new ArgumentException("shape? " + p[0]);
        }
    }

    public static string Run(string input)
    {
        ShapeBase.Count = 0;
        var shapes = new List<IShape>();
        foreach (string spec in input.Split(' ')) shapes.Add(Make(spec));
        int total = 0;
        var sb = new StringBuilder();
        foreach (IShape s in shapes)
        {
            total += s.Area();
            sb.Append(s.ToString()).Append(' ');
            if (s is Square) sb.Append("(sq) ");
        }
        return sb + "total=" + total + " n=" + ShapeBase.Count;
    }
}
