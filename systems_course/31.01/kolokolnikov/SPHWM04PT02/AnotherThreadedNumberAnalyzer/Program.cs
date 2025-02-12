using System.Text;

namespace AnotherThreadedNumberGenerator {

public class Program {
    private const string rawDataFile = "dataRaw.bin";
    private const string sumDataFile = "dataSums.txt";
    private const string prodDataFile = "dataProducts.txt";
    private static List<Tuple<int, int>>? data = null;
    private static event Action? GenerationCompleted;
    private static Mutex mutex = new();

    public static void Main() {
        GenerationCompleted += () => {
            Thread sumThread = new(() => CalculateSums());
            Thread prodThread = new(() => CalculateProds());
            sumThread.Start();
            prodThread.Start();
        };
        Thread genThread = new(() => GenerateRawData(1_000_000, 1, 101));
        genThread.Start();
        genThread.Join();
    }

    public static void GenerateRawData(int pairsNum, int min, int max) {
        Console.WriteLine("Gen thread started.");
        Random rng = new();
        int first, second;
        using (FileStream fs = new(rawDataFile, FileMode.Create)) {
            using (BinaryWriter writer = new(fs)) {
                Console.WriteLine("Gen thread begun generating.");
                for (int i = 0; i < pairsNum; ++i) {
                    first = rng.Next(min, max);
                    second = rng.Next(min, max);
                    writer.Write(first);
                    writer.Write(second);
                }
            }
        }
        Console.WriteLine("Gen thread finished generating.");
        GenerationCompleted?.Invoke();
    }

    public static void CalculateSums() {
        Console.WriteLine("Sum thread started.");
        Console.WriteLine("Sum thread begun calculating.");
        var data = ReadRawData();
        StringBuilder str = new();
        int sum;
        foreach (var pair in data) {
            sum = pair.Item1 + pair.Item2;
            str.Append($"{pair.Item1}\t+\t{pair.Item2}\t= {sum}\n");
        }
        File.WriteAllText(sumDataFile, str.ToString(), Encoding.UTF8);
        Console.WriteLine("Sum thread finished calculating.");
    }

    public static void CalculateProds() {
        Console.WriteLine("Prod thread started.");
        Console.WriteLine("Prod thread begun calculating.");
        var data = ReadRawData();
        StringBuilder str = new();
        int Prod;
        foreach (var pair in data) {
            Prod = pair.Item1 * pair.Item2;
            str.Append($"{pair.Item1}\t*\t{pair.Item2}\t= {Prod}\n");
        }
        File.WriteAllText(prodDataFile, str.ToString(), Encoding.UTF8);
        Console.WriteLine("Prod thread finished calculating.");
    }

    public static List<Tuple<int, int>> ReadRawData() {
        mutex.WaitOne();
        if (data == null) {
            data = new();
            int first, second;
            using (FileStream fs = new(rawDataFile, FileMode.Open)) {
                using (BinaryReader reader = new(fs)) {
                    while (reader.BaseStream.Position != reader.BaseStream.Length) {
                        first = reader.ReadInt32();
                        second = reader.ReadInt32();
                        data.Add(Tuple.Create(first, second));
                    }
                }
            }
        }
        mutex.ReleaseMutex();
        return data;
    }
}

} //! namespace AnotherThreadedNumberGenerator
