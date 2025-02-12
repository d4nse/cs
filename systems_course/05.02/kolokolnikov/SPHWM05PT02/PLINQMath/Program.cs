using static System.Console;

namespace PLINQMath {

public class LisResult {
    public bool isValid { get; set; }
    public int Length { get; set; }
    public int PrevIndex { get; set; }
}

public class Program {
    private const string dataFile = "data.txt";

    public static void Main() {
        // GenerateData(100, -100, 101);
        List<int> numbers = File.ReadAllLines(dataFile)
                                .AsParallel()
                                .Select(line => int.Parse(line))
                                .ToList();
        WriteLine($"Unique values: {CountUniqueValues(numbers)}");
        WriteLine(
            $"Longest increasing subsequence: {String.Join(' ', LongestIncreasingSubsequence(numbers))}");
        WriteLine(
            $"Longest contiguous positive subsequence: {String.Join(' ', LongestContiguousPositiveSubsequence(numbers))}");
    }

    public static int CountUniqueValues(List<int> numbers) {
        int count = numbers.AsParallel().Distinct().Count();
        return count;
    }

    public static List<int> LongestIncreasingSubsequence(List<int> numbers) {
        if (numbers.Count == 0)
            return new List<int>();
        List<int> lis = new();
        foreach (var num in numbers) {
            int idx = BinarySearch(lis, num);
            if (idx == lis.Count)
                lis.Add(num);
            else
                lis[idx] = num;
        }
        return lis;
    }

    public static int BinarySearch(List<int> data, int target) {
        int left = 0, right = data.Count;
        while (left < right) {
            int mid = left + (right - left) / 2;
            if (data[mid] < target)
                left = mid + 1;
            else
                right = mid;
        }
        return left;
    }

    public static List<int>
    LongestContiguousPositiveSubsequence(List<int> numbers) {
        if (numbers.Count == 0)
            return new List<int>();
        List<List<int>> store = new();
        List<int> run = new();
        foreach (var num in numbers) {
            if (num > 0) {
                run.Add(num);
            } else {
                store.Add(run);
                run = new();
            }
        }
        store.Add(run);
        var lcps = store.AsParallel()
                       .OrderByDescending(run => run.Count)
                       .FirstOrDefault();
        return lcps ?? new();
    }

    public static void GenerateData(int numOfInt, int min, int max) {
        Random rng = new();
        using (var fstream = new FileStream(dataFile, FileMode.Create)) {
            using (var writer = new StreamWriter(fstream)) {
                for (int i = 0; i < numOfInt; ++i) {
                    int rand = rng.Next(min, max);
                    writer.WriteLine(rand);
                }
            }
        }
    }
}
}
