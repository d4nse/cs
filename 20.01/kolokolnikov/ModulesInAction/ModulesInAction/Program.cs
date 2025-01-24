using System.Reflection;
using static System.Console;
using SampleLibrary;

class Program {
    static void Main(string[] args) {
        Person person = new Person("Ivan", "Ivanov", 30);
        person.Print();
    }

    static void reflection_bullshit() {
        string cwd = Directory.GetCurrentDirectory();
        string libPath =
            Path.Combine(cwd, "..", "SampleLibrary", "bin", "Debug", "net8.0", "SampleLibrary.dll");

        Assembly asm = Assembly.LoadFrom(libPath);
        Module? mod = asm.GetModule("SampleLibrary.dll");

        if (mod == null) {
            WriteLine("Module not found: SampleLibrary.dll");
            Environment.Exit(1);
        }

        WriteLine("Data types:");
        foreach (Type t in mod.GetTypes()) {
            Console.WriteLine(t.FullName);
        }

        Type? personType = mod.GetType("SampleLibrary.Person");
        object? person =
            Activator.CreateInstance(personType!, new object[] { "Иван", "Иванов", 30 });

        WriteLine();
        personType?.GetMethod("Print")?.Invoke(person, null);
    }
}
