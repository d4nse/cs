using System;
using System.Threading.Tasks;
class Program
{
    static void Main(string[] args)
    {
        int number = 10;

        long[] particalFactorials = new long[number];

        for (int i = 0; i < particalFactorials.Length; i++)
        {
            particalFactorials[i] = 1;
        }

        Parallel.For(1, number + 1, i =>
        {
            long factorial = 1;
            for (int j = 1; j <= i; j++)
            {
                factorial += j;
            }

            particalFactorials[i - 1] = factorial;
        });

        long result = 1;
        for (int i = 0; i< particalFactorials.Length; i++)
        {
            result += particalFactorials[i];
        }

        Console.WriteLine($"факториал числа {number} равен {result}");
    }
}