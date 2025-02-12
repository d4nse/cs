class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("запускаем несколько задач");

        Task task1 = Task.Run(() => PerformTask("Задача 1", 3000));
        Task task2 = Task.Run(() => PerformTask("Задача 2", 2000));
        Task task3 = Task.Run(() => PerformTask("Задача 3", 1000));

        await Task.WhenAll(task1, task2, task3);

        Console.WriteLine(" все закончили");
    }

    static void PerformTask(string taskName, int delay)
    {
        Console.WriteLine($"{taskName} начата");
        Thread.Sleep(delay);
        Console.WriteLine($"{taskName} заверешна");
    }
}