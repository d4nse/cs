
using static System.Console;
using System.Diagnostics;

namespace Project {

class Program {
    public static void Main(string[] args) {
        Random random = new Random();
        const string alphabet = "abcdefghijklmnopqrstuvwxyz";
        bool shouldTerminate = false;
        WriteLine("Press Escape to exit.");
        while (!shouldTerminate) {
            char randomLetter = alphabet[random.Next(alphabet.Length)];
            WriteLine($"Press: {randomLetter}");
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true) {
                if (KeyAvailable) {
                    var key = ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Escape) {
                        shouldTerminate = true;
                        break;
                    }
                    if (key.KeyChar == randomLetter) {
                        stopwatch.Stop();
                        WriteLine($"\nTook {stopwatch.ElapsedMilliseconds} ms");
                        break;
                    }
                }
            }
        }
    }
}

}
