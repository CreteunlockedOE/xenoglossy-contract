using System.Text;

namespace Corpus;

public class EmptyContainerException : Exception
{
    public EmptyContainerException(string what) : base(what + " is empty") { }
}

public class IntStack
{
    int[] items = new int[2];
    int count;
    public int Count { get { return count; } }
    public void Push(int v)
    {
        if (count == items.Length)
        {
            var bigger = new int[items.Length * 2];
            for (int i = 0; i < count; i++) bigger[i] = items[i];
            items = bigger;
        }
        items[count++] = v;
    }
    public int Pop()
    {
        if (count == 0) throw new EmptyContainerException("stack");
        return items[--count];
    }
    public int Peek()
    {
        if (count == 0) throw new EmptyContainerException("stack");
        return items[count - 1];
    }
}

public class IntQueue
{
    int[] buf = new int[4];
    int head, tail, size;
    public int Size { get { return size; } }
    public void Enqueue(int v)
    {
        if (size == buf.Length)
        {
            var nb = new int[buf.Length * 2];
            for (int i = 0; i < size; i++) nb[i] = buf[(head + i) % buf.Length];
            buf = nb; head = 0; tail = size;
        }
        buf[tail] = v;
        tail = (tail + 1) % buf.Length;
        size++;
    }
    public int Dequeue()
    {
        if (size == 0) throw new EmptyContainerException("queue");
        int v = buf[head];
        head = (head + 1) % buf.Length;
        size--;
        return v;
    }
}

public static class Program
{
    public static string Run(string input)
    {
        var stack = new IntStack();
        var queue = new IntQueue();
        var sb = new StringBuilder();
        foreach (char c in input)
        {
            switch (c)
            {
                case 'p': stack.Push(sb.Length); break;
                case 'q': queue.Enqueue(sb.Length * 3); break;
                case 'P': sb.Append(stack.Pop()).Append(','); break;
                case 'Q': sb.Append(queue.Dequeue()).Append(','); break;
                case 'k': sb.Append('[').Append(stack.Peek()).Append(']'); break;
                default: sb.Append(c); break;
            }
        }
        return sb + "|" + stack.Count + "/" + queue.Size;
    }
}
