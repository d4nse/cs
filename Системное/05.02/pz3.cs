using System;
using System.Collections.Generic;
using System.Threading.Tasks;
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("нижняя гр.");
        int lowerBound = int.Parse(Console.ReadLine()); 
        Console.WriteLine("верхняя гр.");
        int upperBound = int.Parse(Console.ReadLine());

        Task<List<int>> primeNumbersTask = Task.Run(() =>
        GeneratePrimeNumbers(lowerBound, upperBound));

        List<int> primeNumbers = await primeNumbersTask;

        Console.WriteLine($"простые от {lowerBound} до {upperBound}: {primeNumbers.Count}");
    }
        
    static List<int> GeneratePrimeNumbers(int start, int end)
    {
        List<int> primes = new List<int>();

        for (int number = start; number <= end; number++)
        {
            if (IsPrime(number))
            {
                primes.Add(number);
            }
        }

        return primes;
    }

    static bool IsPrime(int number)
    {
        if (number < 2) return false;

        for (int i = 2; i * i <= number; i++)
        {
            if (number % i == 0)
            {
                return false;
            }
        }

        return true;
    }
}