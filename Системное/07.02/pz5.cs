using System;
using System.Threading.Tasks;
class Program
{
    static void Main(string[] args)
    {
        string filePath = "numbers.txt";

        List<int> numbers = ReadNumbersFromFile(filePath);

        List<long> factorials = new List<long>();
        object lockObj = new object();

        Parallel.ForEach(numbers, number =>
        {
            long factorial = CalculateFactorial(number);

            lock (lockObj)
            {
                factorials.Add(factorial);
            }
        });

        for (int i = 0; i < numbers.Count; i++)
        {
            Console.WriteLine($"факт ч {numbers[i]} : {factorials[i]}");
        }
    }

    static long CalculateFactorial(long n)
    {
        long result = 1;
        for (int i = 1; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }

    static List<int> ReadNumbersFromFile(string filePath)
    {
        List<int> numbers = new List<int>();
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (int.TryParse(line, out int number))
                {
                    numbers.Add(number);
                }
                else
                {
                    Console.WriteLine($"не удалось {line}");
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"нет {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ошибка при чтенни {ex.Message}");
        }

        return numbers;
    }
}