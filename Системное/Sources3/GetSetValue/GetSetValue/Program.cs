using System;
using System.Globalization;
using Microsoft.Win32;

public class GetSetValue
{
    public static void Main()
    {
        string user = Registry.CurrentUser.Name;
        string skey = "MyRegistryTest";
        string skeyName = $"HKEY_CURRENT_USER\\Software\\{skey}";

        RegistryKey myKey = Registry.CurrentUser.CreateSubKey($"Software\\{skey}");

        myKey.SetValue("", "Это тестовый ключ");
        myKey.SetValue("TestString", "Hello, World!");
        myKey.SetValue("TestInt", 42);

        Console.WriteLine("читаем из реестра");

        int testIntValue = (int)(Registry.GetValue(skeyName, "TestInt", -1) ?? -1);
        string testStringValue = (string)Registry.GetValue(skeyName, "TestString", "значение отсу");

        Console.WriteLine($"TestString: {testStringValue}");
        Console.WriteLine($"TestInt: {testIntValue}");

        Console.WriteLine("изменяем");

        myKey.SetValue("TestInt", 100);
        myKey.SetValue("TestString", "измененное");

        int testIntValue2 = (int)(Registry.GetValue(skeyName, "TestInt", -1) ?? -1);
        string testStringValue2 = (string)Registry.GetValue(skeyName, "TestString", "значение отсу");

        Console.WriteLine($"измененный TestString: {testStringValue2}");
        Console.WriteLine($"измененный TestInt: {testIntValue2}");

        Console.WriteLine("удаляем");

        myKey.DeleteValue("TestString");
        string testStringValue3 = (string)Registry.GetValue(skeyName, "TestString", "значение отсу");
        Console.WriteLine($"после удаления TestString: {testStringValue3}");

        Console.WriteLine("удаляем");

        Registry.CurrentUser.DeleteSubKey($"Software\\{skey}");

    }
}
