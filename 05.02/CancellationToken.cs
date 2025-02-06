class Program
{
    static async Task Main(string[] args)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;


        Console.WriteLine("запускаем несколько задач");

        Task task1 = Task.Run(() => PerformTask("Задача 1", 3000, token), token);
        Task task2 = Task.Run(() => PerformTask("Задача 2", 2000, token), token);
        Task task3 = Task.Run(() => PerformTask("Задача 3", 1000, token), token);

        Task firstComletedTask = await Task.WhenAny(task1, task2, task3);
        Console.WriteLine($"{firstComletedTask.Id} завершена первой");

        cts.Cancel();

        await Task.WhenAll(task1, task2, task3);
        Console.WriteLine(" все закончили");
    }

    static void PerformTask(string taskName, int delay, CancellationToken ct)
    {
        Console.WriteLine($"{taskName} начата");
        for (int i = 0; i < delay / 1000; i++)
        {
            if (ct.IsCancellationRequested)
            {
                Console.WriteLine("отмена");
                return;
            }
            Thread.Sleep(1000);
            Console.WriteLine($"{taskName} выполняется");
        }
        Console.WriteLine($"{taskName} заверешна");
    }
}