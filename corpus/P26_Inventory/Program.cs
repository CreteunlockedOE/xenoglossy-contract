using System.Text;

namespace Corpus;

public class StockException : Exception
{
    public StockException(string sku, int have, int want) : base(sku + ": have " + have + ", want " + want) { }
}

public class Item
{
    public string Sku;
    public int Qty;
    public int PriceCents;
    public Item(string sku, int qty, int price) { Sku = sku; Qty = qty; PriceCents = price; }
}

public class Warehouse
{
    public static int Operations;
    List<Item> items = new List<Item>();
    Dictionary<string, int> index = new Dictionary<string, int>();

    public void Receive(string sku, int qty, int price)
    {
        Operations++;
        int i;
        if (index.TryGetValue(sku, out i)) { items[i].Qty += qty; items[i].PriceCents = price; }
        else { index[sku] = items.Count; items.Add(new Item(sku, qty, price)); }
    }

    public int Ship(string sku, int qty)
    {
        Operations++;
        int i;
        if (!index.TryGetValue(sku, out i)) throw new KeyNotFoundException("no sku " + sku);
        Item it = items[i];
        if (it.Qty < qty) throw new StockException(sku, it.Qty, qty);
        it.Qty -= qty;
        return qty * it.PriceCents;
    }

    public int Value()
    {
        int v = 0;
        foreach (Item it in items) v += it.Qty * it.PriceCents;
        return v;
    }

    public int Count { get { return items.Count; } }
}

public static class Program
{
    static string Cents(int c) { return (c / 100) + "." + (c % 100 < 10 ? "0" : "") + (c % 100); }

    public static string Run(string input)
    {
        Warehouse.Operations = 0;
        var w = new Warehouse();
        int revenue = 0;
        int failures = 0;
        foreach (string cmd in input.Split(';'))
        {
            string[] p = cmd.Split(' ');
            try
            {
                if (p[0] == "in") w.Receive(p[1], int.Parse(p[2]), int.Parse(p[3]));
                else if (p[0] == "out") revenue += w.Ship(p[1], int.Parse(p[2]));
                else throw new InvalidOperationException("bad cmd " + p[0]);
            }
            catch (StockException e)
            {
                failures++;
                Console.WriteLine("stock: " + e.Message);
            }
            finally
            {
                Console.WriteLine("op " + Warehouse.Operations);
            }
        }
        return "revenue=" + Cents(revenue) + " value=" + Cents(w.Value()) + " skus=" + w.Count + " fail=" + failures;
    }
}
