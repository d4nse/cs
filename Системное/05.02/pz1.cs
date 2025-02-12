using System;
using System.Threading;
using System.Threading.Tasks;
class Program
{
    static void Main(string[] args)
    {
        RunTasks();
        Console.WriteLine(" все закончили");
    }

    static void RunTasks()
    {
        // Start
        Task task1 = new Task(DisplayCurrentDateTime);
        task1.Start();
        task1.Wait();

        // StartNew
        Task task2 = Task.Factory.StartNew(DisplayCurrentDateTime);
        task2.Wait();

        // Run
        Task task3 = Task.Run(DisplayCurrentDateTime);
        task3.Wait();
    }

    static void DisplayCurrentDateTime()
    {
        Console.WriteLine($"Текущая {DateTime.Now}");
    }
}