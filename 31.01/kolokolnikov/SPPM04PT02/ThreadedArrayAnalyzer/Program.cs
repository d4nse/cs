
namespace ThreadedArrayAnalyzer {
public class Program {

    private const int max = 1_000_000;
    private static int[] data = new int[max];
    private static Random rng = new();

    public static void Main() {
        GenData(data, out data);
        int intendedNumber = rng.Next(0, max);
        Thread sortThread = new Thread(() => SortData(data));
        Thread checkerThread = new Thread(() => CheckData(intendedNumber));
        sortThread.Start();
        // Thread.Sleep(100);
        checkerThread.Start();
        sortThread.Join();
        checkerThread.Join();
    }

    public static void GenData(int[] data, out int[] odata) {
        for (int i = 0; i < data.Length; ++i) {
            data[i] = rng.Next(0, data.Length + 1);
        }
        odata = data;
    }

    public static void SortData(int[] data) {
        Console.WriteLine("SortThread: Thread started.");
        lock (data) {
            Console.WriteLine("SortThread: Data locked.");
            int n = data.Length;
            for (int gap = n / 2; gap > 0; gap /= 2) {
                for (int i = gap; i < n; i++) {
                    int temp = data[i];
                    int j;
                    for (j = i; j >= gap && data[j - gap] > temp; j -= gap) {
                        data[j] = data[j - gap];
                    }
                    data[j] = temp;
                }
            }
            Console.WriteLine("SortThread: Data sorted and unlocked.");
        }
    }

    public static void CheckData(int intendedNumber) {
        int count = 0;
        Console.WriteLine("CheckThread: Thread started.");
        lock (data) {
            Console.WriteLine("CheckThread: Data locked.");
            foreach (var n in data) {
                if (n == intendedNumber)
                    count++;
            }
            Console.WriteLine("CheckThread: Data unlocked.");
        }
        Console.WriteLine(
            $"Found exactly {count} instances of the intended number {intendedNumber}.");
    }
}

}
