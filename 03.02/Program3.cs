using System;

namespace ControlMemory
{
    public class MemoryDemo
    {
        private string[] _array;
        private int _count;

        public MemoryDemo(int size)
        {
            _array = new string[size];
            _count = 0;
        }

        public string this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                    throw new IndexOutOfRangeException();
                return _array[index];
            }
            set
            {
                if (index < 0 || index >= _array.Length)
                    throw new IndexOutOfRangeException();
                _array[index] = value;

                if (index >= _count)
                {
                    _count = index + 1;
                }
            }
        }

        public void Add(string item)
        {
            if (_count >= _array.Length)
                throw new InvalidOperationException("массив заполнен");
            _array[_count++] = item;
        }

        public void Remove(int index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();

            for (int i = index; i < _count; i++)
            {
                _array[i] = _array[i + 1];
            }

            _array[--_count] = null;
        }

        public int Count => _count;
    }

    unsafe class UnsafeDemo
    {
        private int[] _numbers;

        public UnsafeDemo(int size)
        {
            _numbers = new int[size];
        }

        public void SetValue(int index, int value)
        {
            fixed (int* p = &_numbers[0])
            {
                p[index] = value;
            }
        }

        public int GetValue(int index)
        {
            fixed (int* p = &_numbers[0])
            {
                return p[index];
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TestMemoryDemo();
            AnalyzeGarbageCollector();
            TestUnsafeDemo();
        }

        static void TestMemoryDemo()
        {
            Console.WriteLine("тестирование MemoryDemo");

            MemoryDemo demo = new MemoryDemo(5);

            demo.Add("первый эл");
            demo.Add("второй эл");
            demo.Add("третий эл");

            Console.WriteLine("содержание");

            for (int i = 0; i < demo.Count; i++)
            {
                Console.WriteLine(demo[i]);
            }

            demo.Remove(1);

            Console.WriteLine("содержание после удаления");

            for (int i = 0; i < demo.Count; i++)
            {
                Console.WriteLine(demo[i]);
            }

            Console.WriteLine();
        }

        static void AnalyzeGarbageCollector()
        {
            Console.WriteLine("анализ сборщика");

            for (int i = 0; i < 10000; i++)
            {
                MemoryDemo demo = new MemoryDemo(100);

                for (int j = 0; j < 100; j++)
                {
                    demo.Add($"Элемент {j}");
                }

                demo = null;
            }

            Console.WriteLine("Запуск сборщика");

            GC.Collect();
            GC.WaitForPendingFinalizers();

            long memoryUsed = GC.GetTotalMemory(true);

            Console.WriteLine($"объем {memoryUsed}");

            Console.WriteLine();
        }

#if DEBUG
        static void TestUnsafeDemo()
        {
            unsafe
            {
                UnsafeDemo unsafeDemo = new UnsafeDemo(5);
                unsafeDemo.SetValue(0, 10);
                unsafeDemo.SetValue(1, 20);
                unsafeDemo.SetValue(2, 30);

                Console.WriteLine("тестирование");

                for (int i = 0; i < 3; i++)
                {
                    Console.WriteLine($"значение по индексу {i}:{unsafeDemo.GetValue(i)}");
                }
            }
        }

#else
    static void TestUnsafeDemo()
        {
            Console.WriteLine("небезопасный в отладке")
        }
#endif
        
    }
} 
