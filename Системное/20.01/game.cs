using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start");
            Console.WriteLine("Нажмите сейчас!");

            Random random = new Random();
            int delay = random.Next(1000, 5000);
            Thread.Sleep(delay);

            Console.WriteLine("Нажмите сейчас!");
            DateTime startTime = DateTime.Now;

            Console.ReadKey(true);

            DateTime endTime = DateTime.Now;
            TimeSpan reactionTime = endTime - startTime;

            Console.WriteLine($"Ваше время реакции: {reactionTime.TotalMilliseconds} мс");
        }
    }
}
