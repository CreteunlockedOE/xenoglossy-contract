using System.Text;

namespace Corpus;

public class InsufficientFundsException : Exception
{
    public int Shortfall;
    public InsufficientFundsException(string name, int shortfall) : base(name + " short by " + shortfall) { Shortfall = shortfall; }
}

public abstract class Account
{
    public static int Created;
    public string Name;
    protected int Balance;
    protected Account(string name, int opening) { Name = name; Balance = opening; Created++; }
    public int GetBalance() { return Balance; }
    public void Deposit(int amount)
    {
        if (amount <= 0) throw new ArgumentException("bad amount " + amount);
        Balance += amount;
    }
    public virtual void Withdraw(int amount)
    {
        if (amount > Balance) throw new InsufficientFundsException(Name, amount - Balance);
        Balance -= amount;
    }
    public abstract void EndOfMonth();
}

public class Checking : Account
{
    public Checking(string name, int opening) : base(name, opening) { }
    public override void Withdraw(int amount) { base.Withdraw(amount + 1); }
    public override void EndOfMonth() { Balance -= 5; }
}

public class Savings : Account
{
    int rateBp;
    public Savings(string name, int opening, int rateBp) : base(name, opening) { this.rateBp = rateBp; }
    public override void EndOfMonth() { Balance += Balance * rateBp / 10000; }
}

public static class Program
{
    static string Execute(Account[] accts, string cmd)
    {
        string[] p = cmd.Split(' ');
        int idx = p[1][0] - 'a';
        Account a = accts[idx];
        try
        {
            switch (p[0])
            {
                case "dep": a.Deposit(int.Parse(p[2])); break;
                case "wd": a.Withdraw(int.Parse(p[2])); break;
                case "eom": a.EndOfMonth(); break;
                default: throw new InvalidOperationException("unknown " + p[0]);
            }
            return "ok";
        }
        catch (InsufficientFundsException e)
        {
            return "NSF(" + e.Shortfall + ")";
        }
        finally
        {
            Console.WriteLine(a.Name + ":" + a.GetBalance());
        }
    }

    public static string Run(string input)
    {
        Account.Created = 0;
        var accts = new Account[] { new Checking("chk", 100), new Savings("sav", 1000, 250) };
        var log = new StringBuilder();
        foreach (string cmd in input.Split(';'))
            log.Append(Execute(accts, cmd.Trim())).Append(',');
        return log + "total=" + (accts[0].GetBalance() + accts[1].GetBalance()) + " created=" + Account.Created;
    }
}
