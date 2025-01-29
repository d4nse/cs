using System;
using System.Threading;

namespace SuspendAndResumeThread
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadStart ts = new ThreadStart(Method);
            Thread t = new Thread(ts);
            t.Start(); // Запуск потока.

            ConsoleKey usersAns = ConsoleKey.G;

            while (usersAns != ConsoleKey.Escape)
            {
                Console.WriteLine("Нажмите любую клавишу для остановки");
                usersAns = Console.ReadKey().Key;
                t.Suspend(); // Приостановка потока.
                Console.WriteLine("Поток остановлен!");
                Console.WriteLine("Нажмите любую клавишу для возобновления");
                usersAns = Console.ReadKey().Key;
                t.Resume(); // Возобновление работы.
            }

            t.Abort();

        }

  
        static void Method()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(100);
            }
        }
    }
}
