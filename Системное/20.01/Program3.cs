using System;
using System.IO;
using System.Reflection;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Модуль в действии";
            Console.WindowWidth = 35;
            Console.BufferWidth = 35;

            try
            {
                // Проверяем текущую директорию
                Console.WriteLine("Текущая директория: " + Environment.CurrentDirectory);

                // Указываем путь к сборке
                string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClassLibrary1.dll");
                Console.WriteLine("Путь к сборке: " + assemblyPath);

                // Загружаем сборку
                Assembly asm = Assembly.LoadFrom(assemblyPath);

                // Получаем необходимый модуль этой сборки
                Module mod = asm.GetModule("ClassLibrary1.dll");

                // Выводим список типов данных, объявленный в текущем модуле
                Console.WriteLine("Объявленные типы данных:");
                foreach (Type t in mod.GetTypes())
                    Console.WriteLine(t.FullName);

                // Получаем тип данных из сборки
                Type Person = mod.GetType("ClassLibrary1.Person") as Type;
                if (Person == null)
                {
                    Console.WriteLine("Тип ClassLibrary1.Person не найден.");
                    return;
                }

                // Создаём объект полученного типа данных
                object person = Activator.CreateInstance(Person, new object[] { "Иван", "Иванов", 30 });
                Console.WriteLine();

                // Вызываем метод Print от созданного объекта
                MethodInfo printMethod = Person.GetMethod("Print");
                if (printMethod == null)
                {
                    Console.WriteLine("Метод Print не найден.");
                    return;
                }
                printMethod.Invoke(person, null);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Ошибка: Сборка не найдена. " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
    }
}