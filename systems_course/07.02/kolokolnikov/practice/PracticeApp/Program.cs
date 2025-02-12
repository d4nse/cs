using static System.Console;
using System.Collections.Concurrent;

namespace PracticeApp {

public static class Program {
    public static void Main() {
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        long[] facts = new long[numbers.Length];
        int[] digitCounts = new int[numbers.Length];
        int[] digitSums = new int[numbers.Length];

        Parallel.For(0, numbers.Length, i => {
            facts[i] = Fact(numbers[i]); //
            digitCounts[i] = CountDigits(numbers[i]);
            digitSums[i] = SumDigits(numbers[i]);
        });

        for (int i = 0; i < numbers.Length; ++i) {
            WriteLine(digitSums[i]);
        }

        MultTable(5, 8);

        FactFromFile("data.txt");
        MathFromFile("data.txt");
    }

    public static int CountDigits(int n) {
        return n.ToString().Length;
    }

    public static int SumDigits(int n) {
        int sum = 0;
        while (n > 0) {
            sum += n % 10;
            n /= 10;
        }
        return sum;
    }

    public static long Fact(int n) {
        long res = 1;
        for (int i = 1; i <= n; ++i) {
            res *= i;
        }
        return res;
    }

    public static void MultTable(int start, int end) {
        ConcurrentBag<(int a, int b, int res)> results = new();
        Parallel.For(start, end + 1, i => {
            for (int j = 1; j <= 10; ++j) {
                results.Add((i, j, i * j));
            }
        });
        var ordered = results.AsParallel().OrderBy(r => r.a).ThenBy(r => r.b);
        foreach (var res in ordered) {
            WriteLine($"{res.a}*{res.b}={res.res}");
        }
    }

    public static void FactFromFile(string file) {
        var lines =
            File.ReadAllLines(file).Select(val => int.Parse(val)).ToList();
        ConcurrentBag<long> res = new();
        lines.AsParallel().ForAll(val => { res.Add(Fact(val)); });
        var ordered = res.AsParallel().OrderBy(val => val).ToList();
        foreach (var val in ordered) {
            WriteLine(val);
        }
    }

    public static void MathFromFile(string file) {
        var lines =
            File.ReadAllLines(file).Select(val => int.Parse(val)).ToList();
        var ordered = lines.AsParallel().OrderBy(val => val).ToList();
        int min = ordered[0];
        int max = ordered[^1];
        int sum = ordered.AsParallel().Sum();
        WriteLine($"Min: {min}; Max: {max}; Sum: {sum}");
    }
}

}
