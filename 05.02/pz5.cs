using System;
using System.Linq;
using System.Threading.Tasks;
class Program
{
    static async Task Main(string[] args)
    {
        int[] numbers = { 5, 3, 8, 1, 4, 9, 2, 7, 6, 2, 8 };

        Task<int[]> removeDuplicates = Task.Run(() => RemoveDuplicates(numbers));

        Task<int[]> sortTask = removeDuplicates.ContinueWith(antecedent =>
        {
            return SortArray(antecedent.Result);
        });

        Task<int> searchTask = sortTask.ContinueWith(antecedent =>
        {
            int valueToFind = 7;
            return BinarySearch(antecedent.Result, valueToFind);
        });

        int index = await searchTask;

        if (index != -1)
        {
            Console.WriteLine($"найдено по {index}");
        }
        else
        {
            Console.WriteLine("не найдено");
        }
    }

    static int[] RemoveDuplicates(int[] array)
    {
        Console.WriteLine("удаление");
        return array.Distinct().ToArray();
    }

    static int[] SortArray(int[] array)
    {
        Console.WriteLine("сортировка");
        Array.Sort(array);
        return array;
    }

    static int BinarySearch(int[] array, int value)
    {
        Console.WriteLine("поиск");

        int left = 0;
        int right = array.Length - 1;

        while (left <= right) 
        {
            int mid = left + (right - left) / 2;

            if (array[mid] == value)
                return mid;

            if (array[mid] < value)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return -1;
    }
}