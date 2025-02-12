using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Объявляем.объект.класса.Process
            Process proc = new Process();
            //устанавливаем.имя.файла,.который.будет.запущен.в.
            //рамках.процесса
            proc.StartInfo.FileName = "notepad.exe";
            //Запущен.процесс:.notepad
            //запускаем.процесс
            proc.Start();
            //выводим.имя.процесса
            Console.WriteLine("Запущен.процесс:." + proc.
             ProcessName);
            //ожидаем.закрытия.процесса
            proc.WaitForExit();
            //Процесс.завершился.с.кодом:.0
            //Текущий.процесс.имеет.имя:.ConsoleApp1
            //выводим.код,.с.которым.завершился.процесс
            Console.WriteLine("Процесс.завершился.с.кодом:." +
             proc.ExitCode);
            //выводим.имя.текущего.процесса
            Console.WriteLine("Текущий.процесс.имеет.имя:." +

            Process.GetCurrentProcess().ProcessName);

        }
    }
}
