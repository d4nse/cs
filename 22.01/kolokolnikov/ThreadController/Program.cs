using static System.Console;

namespace ThreadController {
public class Program {
    public static volatile bool shouldTerminate = false;
    public static ManualResetEvent pauseEvent = new(true);
    public static readonly object outputLock = new();

    public static int Main(string[] args) {
        Thread countThread = new Thread(CountToIntMax);
        countThread.Start();
        WriteLine("'p' - pause\n'r' - resume\n'q' - quit");
        while (!shouldTerminate) {
            var key = ReadKey(true).Key;
            switch (key) {
            case ConsoleKey.P:
                WriteLine("Counting paused.");
                pauseEvent.Reset();
                break;
            case ConsoleKey.R:
                WriteLine("Counting resumed.");
                pauseEvent.Set();
                break;
            case ConsoleKey.Q:
                WriteLine("Stopping counting...");
                shouldTerminate = true;
                pauseEvent.Set();
                break;
            default:
                break;
            }
        }
        countThread.Join();
        WriteLine("Done. Exiting...");
        return 0;
    }

    public static void CountToIntMax() {
        for (int i = 0; i <= int.MaxValue; ++i) {
            if (shouldTerminate) {
                break;
            }
            pauseEvent.WaitOne();
            lock (outputLock) {
                if (!shouldTerminate)
                    WriteLine(i);
            }
            Thread.Sleep(100);
        }
    }
}

}
