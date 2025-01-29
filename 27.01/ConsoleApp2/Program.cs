using System;
using System.IO;
using System.Linq;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("количество потоков");
        int threadCount = int.Parse(Console.ReadLine());

        Console.WriteLine("начало диапазона");
        int startRange = int.Parse(Console.ReadLine());

        Console.WriteLine("конец диапазона");
        int endRange = int.Parse(Console.ReadLine());

        int rangePerThread = (endRange - startRange + 1) / threadCount;

        // задание 1-3
        for (int i = 0; i < threadCount; i++)
        {
            int threadStart = startRange + i * rangePerThread; 
            int threadEnd = (i == threadCount - 1) ? endRange : threadStart + rangePerThread - 1; 

            Thread thread = new Thread(() => DisplayNumbers(threadStart, threadEnd));
            thread.Start(); 
        }

        // задание 4
        Thread processingThread = new Thread(GenerateAndProcessNumbers);
        processingThread.Start(); 

        Console.WriteLine("основной поток завершен");
    }

    // задание 1-3
    static void DisplayNumbers(int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            Console.WriteLine($"поток {Thread.CurrentThread.ManagedThreadId}: {i}");
            Thread.Sleep(10); 
        }
    }

    // задание 4
    static void GenerateAndProcessNumbers()
    {
        int[] numbers = new int[10000]; 
        Random random = new Random(); 

        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = random.Next(1, 101);
        }

        int sum = numbers.Sum();
        double average = numbers.Average();
        int max = numbers.Max();
        int min = numbers.Min();

        Console.WriteLine("результаты вычислений:");
        Console.WriteLine($"сумма: {sum}");
        Console.WriteLine($"среднее: {average:F2}");
        Console.WriteLine($"максимум: {max}");
        Console.WriteLine($"минимум: {min}");

        // задание 5
        Thread fileWriterThread = new Thread(() => WriteResultsToFile(numbers, sum, average, max, min));
        fileWriterThread.Start(); 
    }

    // задание 5
    static void WriteResultsToFile(int[] numbers, int sum, double average, int max, int min)
    {
        string filePath = "results.txt"; 

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("сгенерированные числа:");
                foreach (var number in numbers)
                {
                    writer.Write(number + " ");
                }

                writer.WriteLine(); 

                writer.WriteLine("результаты вычислений:");
                writer.WriteLine($"сумма: {sum}");
                writer.WriteLine($"среднее: {average:F2}");
                writer.WriteLine($"максимум: {max}");
                writer.WriteLine($"минимум: {min}");
            }

            Console.WriteLine("результаты записаны в файл");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ошибка записи в файл: {ex.Message}");
        }
    }
}
