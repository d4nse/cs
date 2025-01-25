using static System.Console;

namespace SPPM02 {
public class Program {
    public static int Main() {
        ThreadedCounter();
        ThreadedSearch();
        return 0;
    }

    public static void ThreadedSearch() {
        WriteLine("Allocating memory for 536m870k912 elements.");
        Random rng = new Random();
        Int64[] array = new Int64[1_000];
        for (Int64 i = 0; i < array.Length; ++i) {
            array[i] = rng.NextInt64(-1000, 1000);
        }
        WriteLine("Dispatching threads.");
        var searchMaxThread = new Thread(LinearSearchMax);
        var searchMinThread = new Thread(LinearSearchMin);
        var searchAvgThread = new Thread(SearchAvg);
        var writeThread = new Thread(WriteFile);

        searchMaxThread.Start((object)array);
        searchMinThread.Start((object)array);
        searchAvgThread.Start((object)array);

        WriteLine("Waiting for threads to finish.");
        searchMaxThread.Join();
        searchMinThread.Join();
        searchAvgThread.Join();
        writeThread.Start((object)array);

        if (searchMaxResult != null) {
            WriteLine($"Max result found: {searchMaxResult}");
        } else {
            WriteLine("SearchMax thread failed.");
        }
        if (searchMinResult != null) {
            WriteLine($"Min result found: {searchMinResult}");
        } else {
            WriteLine("SearchMin thread failed.");
        }
        if (searchAvgResult != null) {
            WriteLine($"Avg result found: {searchAvgResult}");
        } else {
            WriteLine("SearchAvg thread failed.");
        }
        writeThread.Join();
    }

    public static Int64? searchMaxResult;
    public static Int64? searchMinResult;
    public static double? searchAvgResult;

    public static void WriteFile(object? array_obj) {
        if (array_obj == null)
            return;
        Int64[] array = (Int64[])array_obj;
        string path = "results.txt";
        WriteLine("Writing results...");
        try {
            using (StreamWriter writer = new(path)) {
                writer.WriteLine($"Max: {searchMaxResult}");
                writer.WriteLine($"Min: {searchMinResult}");
                writer.WriteLine($"Avg: {searchAvgResult}");
                writer.WriteLine("Data:");
                foreach (var integer in array) {
                    writer.Write($"{integer} ");
                }
            }
            WriteLine($"Results saved to file: {path}");
        } catch (Exception ex) {
            WriteLine(ex.Message);
        }
    }
    public static void LinearSearchMax(object? array_obj) {
        if (array_obj == null)
            return;
        Int64[] array = (Int64[])array_obj;
        searchMaxResult = array[0];
        for (Int64 i = 1; i < array.Length; ++i) {
            if (array[i] > searchMaxResult) {
                searchMaxResult = array[i];
            }
        }
    }
    public static void LinearSearchMin(object? array_obj) {
        if (array_obj == null)
            return;
        Int64[] array = (Int64[])array_obj;
        searchMinResult = array[0];
        for (Int64 i = 1; i < array.Length; ++i) {
            if (array[i] < searchMinResult) {
                searchMinResult = array[i];
            }
        }
    }
    public static void SearchAvg(object? array_obj) {
        if (array_obj == null)
            return;
        Int64[] array = (Int64[])array_obj;
        double sum = 0;
        for (Int64 i = 0; i < array.Length; ++i) {
            sum += array[i];
        }
        searchAvgResult = sum / array.Length;
    }

    public static void ThreadedCounter() {

        Write("Enter range 'a b': ");
        string input = ReadLine() ?? "";
        var parts = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        int a, b;
        if (parts.Length < 2) {
            WriteLine("Invalid range entered");
            return;
        }
        if (!int.TryParse(parts[0], out a) || !int.TryParse(parts[1], out b)) {
            WriteLine("Invalid input");
            return;
        }

        Write("Enter number of threads: ");
        input = ReadLine() ?? "";
        int threads_count = 0;
        if (!int.TryParse(input, out threads_count) || threads_count <= 0) {
            WriteLine("Invalid number of threads.");
            return;
        }

        int range_size = Math.Abs(b - a);
        int chunk_size = range_size / threads_count;
        Thread[] threads = new Thread[threads_count];
        for (int i = 0; i < threads_count; ++i) {
            int start = a + i * chunk_size;
            int end = (i == threads_count - 1) ? b : start + chunk_size;
            threads[i] = new Thread(new ParameterizedThreadStart(Counter));
            threads[i].Start(new Range(i, start, end));
        }

        foreach (var thread in threads) {
            thread.Join();
        }
    }

    public static void Counter(object? range) {
        if (range == null)
            return;
        Range r = (Range)range;
        for (int i = r.start; i < r.end; ++i) {
            WriteLine($"{r.id}\t{i}");
        }
    }
}

struct Range {
    public int id;
    public int start;
    public int end;
    public Range(int id, int start, int end) {
        this.id = id;
        this.start = start;
        this.end = end;
    }
}
}
