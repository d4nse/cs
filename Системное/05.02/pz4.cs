using System;
using System.Linq;
using System.Threading.Tasks;
class Program
{
    static async Task Main(string[] args)
    {
        int[] numbers = { 5, 3, 8, 1, 4, 9, 2, 7, 6, 2, 8};

        Task<int> minTask = Task.Run(() => FindMinimun(numbers));
        Task<int> maxTask = Task.Run(() => FindMaximun(numbers));
        Task<double> averageTask = Task.Run(() => CalculateAverage(numbers));
        Task<int> sumTask = Task.Run(() => CalculateSum(numbers));

        await Task.WhenAll(minTask, maxTask, averageTask, sumTask);

        int min = await minTask;
        int max = await maxTask;
        double average = await averageTask;
        int sum = await sumTask;

        Console.WriteLine(min);
        Console.WriteLine(max);
        Console.WriteLine(average);
        Console.WriteLine(sum);
    }
    static int FindMinimun(int[] array)
    {
        return array.Min();
    }

    static int FindMaximun(int[] array)
    {
        return array.Max();
    }

    static double CalculateAverage(int[] array)
    {
        return array.Average();
    }

    static int CalculateSum(int[] array)
    {
        return array.Sum();
    }
}