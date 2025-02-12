namespace SPPM04PT02 {

public class Program {

    private static List<int> data = new();
    private static Random rng = new();
    private static ManualResetEvent resetEvent = new(false);

    public delegate void GenFinishedEventHandler(object? sender, EventArgs e);
    public static event GenFinishedEventHandler? GenFinishedEvent;
    public static void OnGenFinished() {
        if (GenFinishedEvent != null) {
            GenFinishedEvent.Invoke(null, EventArgs.Empty);
        }
    }

    public static void Main() {
        GenFinishedEvent += HandleEvent;
        Thread genThread = new Thread(GenerateNumbers);
        Thread fmaxThread = new Thread(FindMaxNumber);
        Thread fminThread = new Thread(FindMinNumber);
        Thread favgThread = new Thread(FindAvgNumber);

        genThread.Start();
        fmaxThread.Start();
        fminThread.Start();
        favgThread.Start();

        genThread.Join();
        fmaxThread.Join();
        fminThread.Join();
        favgThread.Join();
    }

    private static void HandleEvent(object? sender, EventArgs e) {
        Console.WriteLine("Generation event triggered! Resuming threads...");
        resetEvent.Set();
    }

    private static void GenerateNumbers() {
        Console.WriteLine("gen: Thread started.");
        for (int i = 0; i < 1_000_000; ++i) {
            data.Add(rng.Next(0, 5001));
        }
        Console.WriteLine("gen: Finished.");
        OnGenFinished();
    }

    private static void FindMaxNumber() {
        Console.WriteLine("fmax: Thread started.");
        resetEvent.WaitOne();
        int a = data[0];
        for (int i = 1; i < data.Count; ++i) {
            if (data[i] > a)
                a = data[i];
        }
        Console.WriteLine($"fmax: Number found: {a}");
    }

    private static void FindMinNumber() {
        Console.WriteLine("fmin: Thread started.");
        resetEvent.WaitOne();
        int a = data[0];
        for (int i = 1; i < data.Count; ++i) {
            if (data[i] < a)
                a = data[i];
        }
        Console.WriteLine($"fmin: Number found: {a}");
    }

    private static void FindAvgNumber() {
        Console.WriteLine("favg: Thread started.");
        resetEvent.WaitOne();
        double a = 0;
        foreach (var n in data) {
            a += n;
        }
        a /= data.Count;
        Console.WriteLine($"favg: number found: {a}");
    }
}

}
