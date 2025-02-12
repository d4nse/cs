using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
class Program
{
    static async Task Main(string[] args)
    {
        int number = 10;

        long factorial = 0;
        int digitCount = 0;
        int digitSum = 0;

        Task factorialTask = Task.Run(() => factorial = CalculateFactorial(number));
        Task digitCountTask = Task.Run(() => digitCount = CountDigits(number));
        Task digitSumTask = Task.Run(() => digitSum = SumDigits(number));

        await Task.WhenAll(factorialTask, digitCountTask, digitSumTask);

        Console.WriteLine($"факториал числа {number} : {factorial}");
        Console.WriteLine($"количество цифр в числе {number} : {digitCount}");
        Console.WriteLine($"сумма цифр в числе {number} : {digitSum}");
    }

    static long CalculateFactorial(int n)
    {
        long result = 1;
        for (int i = 1; i <= n; i++)
        {
            result += i;
        }
        return result;
    }

    static int CountDigits(int n)
    {
        return n.ToString().Length;
    }

    static int SumDigits(int n)
    {
        int sum = 0;
        string numberStr = n.ToString();
        foreach (char c in numberStr)
        {
            sum += int.Parse(c.ToString());
        }
        return sum;
    }
}