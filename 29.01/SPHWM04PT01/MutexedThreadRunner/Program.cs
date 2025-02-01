
namespace MutexedThreadRunner
{
    public class Program
    {
        const string generatedDataFile = "dataGenerated.txt";
        const string primeDataFile = "dataPrimes.txt";
        const string specialDataFile = "dataSpecial.txt"; // ending with 7
        const string reportFile = "report.txt";
        static Mutex mutex = new Mutex();

        public static int Main()
        {
            Thread generatorThread = new Thread(GenerateData);
            Thread primeFinderThread = new Thread(FindPrimes);
            Thread specialFinderThread = new Thread(FindSpecials);
            Thread reportThread = new Thread(GenerateReport);

            generatorThread.Start();
            Thread.Sleep(100);
            primeFinderThread.Start();
            Thread.Sleep(100);
            specialFinderThread.Start();
            Thread.Sleep(100);
            reportThread.Start();

            generatorThread.Join();
            primeFinderThread.Join();
            specialFinderThread.Join();
            reportThread.Join();

            return 0;
        }

        static void GenerateData()
        {
            Console.WriteLine("Generator thread started.");
            mutex.WaitOne();
            using (var writer = new StreamWriter(generatedDataFile))
            {
                Random rng = new();
                for (int i = 0; i < 1_000_000; i++)
                {
                    int n = rng.Next(1, 1_000_000);
                    writer.WriteLine(n);
                }
            }
            mutex.ReleaseMutex();
            Console.WriteLine("Data generated.");
        }

        static void FindPrimes()
        {
            Console.WriteLine("Prime finder thread started.");
            mutex.WaitOne();
            var nums = File.ReadAllLines(generatedDataFile).Select(int.Parse).ToList();
            var primes = nums.Where(IsPrime).ToList();
            using (var writer = new StreamWriter(primeDataFile))
            {
                foreach (var pnum in primes)
                {
                    writer.WriteLine(pnum);
                }
            }
            mutex.ReleaseMutex();
            Console.WriteLine("Primes found.");
        }
        static bool IsPrime(int number)
        {
            if (number < 2) return false;
            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                    return false;
            }
            return true;
        }

        static void FindSpecials()
        {
            Console.WriteLine("Special finder thread started.");
            mutex.WaitOne();
            var primes = File.ReadAllLines(primeDataFile).Select(int.Parse).ToList();
            var specials = primes.Where(p => p % 10 == 7).ToList();
            using (var writer = new StreamWriter(specialDataFile))
            {
                foreach (var pnum in specials)
                {
                    writer.WriteLine(pnum);
                }
            }
            mutex.ReleaseMutex();
            Console.WriteLine("Specials found.");
        }

        static void GenerateReport()
        {
            Console.WriteLine("Report generator thread started.");
            mutex.WaitOne();
            using (var writer = new StreamWriter(reportFile))
            {
                WriteFileReport(writer, generatedDataFile);
                WriteFileReport(writer, primeDataFile);
                WriteFileReport(writer, specialDataFile);
            }
            mutex.ReleaseMutex();
            Console.WriteLine("Report generated.");
        }

        static void WriteFileReport(StreamWriter writer, string filePath)
        {
            if (File.Exists(filePath))
            {
                var lineCount = File.ReadLines(filePath).Count();
                var fileSize = new FileInfo(filePath).Length;
                writer.WriteLine($"File: {filePath}");
                writer.WriteLine($"Number of entries: {lineCount}");
                writer.WriteLine($"File size: {fileSize} bytes");
                writer.WriteLine("Contents:");
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
                writer.WriteLine();
            }
            else
            {
                writer.WriteLine($"File: {filePath} does not exist.");
                writer.WriteLine();
            }
        }
    }
}