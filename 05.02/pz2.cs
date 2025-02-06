using System;
using System.Threading.Tasks;
class Program
{
    static void Main(string[] args)
    {
        Task task = Task.Run(() => DisplayPrimeNumbers(0, 1000));
        task.Wait();
        Console.WriteLine("отображено");
    }

    static void DisplayPrimeNumbers(int start, int end)
    {
        Console.WriteLine("простые числа от 0 до 1000");

        for (int number = start; number <= end; number++)
        {
            if (IsPrime(number))
            {
                Console.WriteLine(number);
            }
        }
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