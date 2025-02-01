
namespace CriticalThreadSyncer {

public class Program {
    private static readonly object threadLock = new();

    public static void Main(string[] args) {
        if (args.Length < 1) {
            Console.WriteLine("Path to file must be provided.");
            return;
        }

        string filepath = args[0];
        Thread counterThread = new Thread(() => CountSentences(filepath));
        Thread modifierThread = new Thread(() => ModifyFile(filepath));

        counterThread.Start();
        modifierThread.Start();

        counterThread.Join();
        modifierThread.Join();
    }

    public static void CountSentences(string filepath) {
        Console.WriteLine("CountSentences thread started.");
        try {
            lock (threadLock) {
                Console.WriteLine("CountSentences has locked the file.");
                var content = File.ReadAllText(filepath);
                var sentences = content.Split(new char[] { '.', '!', '?' },
                                              StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("CountSentences has unlocked the file.");
                Console.WriteLine($"Found {sentences.Length} sentences.");
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error reading file `{filepath}`: {ex.Message}");
        }
    }

    public static void ModifyFile(string filepath) {
        Console.WriteLine("ModifyFile thread started.");
        Thread.Sleep(100);
        try {
            lock (threadLock) {
                Console.WriteLine("ModifyFile has locked the file.");
                var content = File.ReadAllText(filepath);
                var mod = content.Replace('!', '#');
                File.WriteAllText(filepath, mod);
                Console.WriteLine("ModifyFile has unlocked the file.");
                Console.WriteLine("File has been modified.");
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error modifying file `{filepath}`: {ex.Message}");
        }
    }
}

}
