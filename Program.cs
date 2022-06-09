
Console.WriteLine("PiHexFind, v1.0\n");
if (args.Length == 0)
{
    Console.WriteLine("Usage: PiHexFind <target-string> [<start-digit>]");
    return;
}

int gotten = 0;

if (args.Length == 2) // starting digit number
{
    if (int.TryParse(args[1], out var value))
    {
        gotten = value;
    }
}

Queue<char> queue = new Queue<char>();
while(true)
{
    while (queue.Count < args[0].Length * 2)
    {
        char[] next = TwelveHexDigitsOfPiFrom(gotten + queue.Count).ToCharArray();
        foreach(char c in next)
            queue.Enqueue(c);
    }

    int n = Contains(queue, args[0]);
    switch(n)
    {
        case int x when x >= 0 && x < 6:
            break;
        case int x when x == args[0].Length:
            Console.WriteLine("\nFound '{0}' at digit {1} of Pi.", args[0], gotten);
            return;
        case int x when x < 0:
            throw new ApplicationException(String.Format("Logic error, '{0}', {1}, '{2}'", queue.ToArray().ToString(), gotten, args[0]));
        default:
            Console.WriteLine("[{0}-{1}]", gotten, n);
            break;
    }

    queue.Dequeue();
    gotten++;
    if (gotten % 100000 == 0)
        Console.Write(" {0}\r\n", gotten);
    else if (gotten % 1000 == 0)
        Console.Write(".");
}

int Contains(Queue<char> source, string target)
{
    if (source == null || target == null)
        return -1;
    int i = 0;
    while (i < source.Count && i < target.Length && queue.ElementAt(i) == args[0][i])
        i++;
    return i;
}

string TwelveHexDigitsOfPiFrom(int id)
{
    double pid, s1, s2, s3, s4;
    const int NHX = 16;
    var chx = new char[NHX];

    /*  id is the digit position.  Digits generated follow immediately after id. */

    s1 = series(1, id);
    s2 = series(4, id);
    s3 = series(5, id);
    s4 = series(6, id);
    pid = 4.0f * s1 - 2.0f * s2 - s3 - s4;
    pid = pid - (int)pid + 1.0f;
    ihex(pid, NHX, chx);
    //Console.WriteLine(" position = {0}\n fraction = {1} \n hex digits =  {2}\n", id, pid, new string(chx).Substring(0, 12));
    return new string(chx).Substring(0, 12);
}

void ihex(double x, int nhx, char[] chx)
/*  This returns, in chx, the first nhx hex digits of the fraction of x. */
{
    int i;
    double y;
    var hx = "0123456789ABCDEF";

    y = Math.Abs(x);

    for (i = 0; i < nhx; i++)
    {
        y = 16.0f * (y - Math.Floor(y));
        chx[i] = hx[(int)y];
    }
}

double series(int m, int id)
/*  This routine evaluates the series  sum_k 16^(id-k)/(8*k+m) 
    using the modular exponentiation technique. */
{
    int k;
    double ak, eps, p, s, t;
    //double expm(double x, double y);
    eps = 1e-17;

    s = 0.0f;

    /*  Sum the series up to id. */

    for (k = 0; k < id; k++)
    {
        ak = 8 * k + m;
        p = id - k;
        t = expm(p, ak);
        s = s + t / ak;
        s = s - (int)s;
    }

    /*  Compute a few terms where k >= id. */

    for (k = id; k <= id + 100; k++)
    {
        ak = 8 * k + m;
        t = Math.Pow(16.0f, (double)(id - k)) / ak;
        if (t < eps) break;
        s = s + t;
        s = s - (int)s;
    }
    return s;
}

double expm(double p, double ak)

/*  expm = 16^p mod ak.  This routine uses the left-to-right binary 
    exponentiation scheme. */

{
    int i, j;
    double p1, pt, r;

    /*  If this is the first call to expm, fill the power of two table tp. */

    if (Globals.tp1 == 0)
    {
        Globals.tp1 = 1;
        Globals.tp[0] = 1.0f;

        for (i = 1; i < Globals.ntp; i++) Globals.tp[i] = 2.0f * Globals.tp[i - 1];
    }

    if (ak == 1.0f) return 0.0f;

    /*  Find the greatest power of two less than or equal to p. */

    for (i = 0; i < Globals.ntp; i++) if (Globals.tp[i] > p) break;

    pt = Globals.tp[i - 1];
    p1 = p;
    r = 1.0f;

    /*  Perform binary exponentiation algorithm modulo ak. */

    for (j = 1; j <= i; j++)
    {
        if (p1 >= pt)
        {
            r = 16.0f * r;
            r = r - (int)(r / ak) * ak;
            p1 = p1 - pt;
        }
        pt = 0.5 * pt;
        if (pt >= 1.0f)
        {
            r = r * r;
            r = r - (int)(r / ak) * ak;
        }
    }

    return r;
}

public static class Globals
{
    public const int ntp = 25;
    public static double[] tp = new double[ntp];
    public static int tp1 = 0;
}

class DigitComparer : IEqualityComparer<char>
{
    public bool Equals(char x, char y)
    {
        return x == y;
    }

    public int GetHashCode([DisallowNull] char obj)
    {
        return obj.GetHashCode();
    }
}

