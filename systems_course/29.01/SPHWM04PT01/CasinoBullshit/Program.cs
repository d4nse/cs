
namespace CasinoBullshit
{
    public class Program
    {
        public static Random rng = new();

        public static void Main()
        {
            int totalPlayers = rng.Next(20, 101);
            var casinoTable = new CasinoTable(5);
            for (int i = 0; i < totalPlayers; i++)
            {
                casinoTable.AddPlayer($"Player_{i + 1}", rng.Next(10, 101));
            }
            casinoTable.StartGame();
        }
    }

    public class CasinoTable
    {
        int maxPlayers;
        Queue<Player> playerQueue = new();
        object tableLock = new();
        List<string> results = new();

        public CasinoTable(int maxPlayers)
        {
            this.maxPlayers = maxPlayers;
        }

        public void AddPlayer(string name, int initialMoney)
        {
            playerQueue.Enqueue(new Player(name, initialMoney));
        }

        public void StartGame()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < maxPlayers; i++)
            {
                if (playerQueue.Count > 0)
                {
                    var player = playerQueue.Dequeue();
                    var task = Task.Run(() => player.Play(this));
                    tasks.Add(task);
                }
            }
            Task.WaitAll(tasks.ToArray());
            GenerateReport();
        }

        public void PlayerFinished(Player player)
        {
            lock (tableLock)
            {
                if (playerQueue.Count > 0)
                {
                    var newPlayer = playerQueue.Dequeue();
                    Task.Run(() => newPlayer.Play(this));
                }
            }
        }

        public void RecordResult(string result)
        {
            lock (tableLock)
            {
                results.Add(result);
            }
        }

        private void GenerateReport()
        {
            using (var writer = new StreamWriter("casino_report.txt"))
            {
                foreach (var result in results)
                {
                    writer.WriteLine(result);
                }
            }
            Console.WriteLine("Report generated: casino_report.txt");
        }
    }

    public class Player
    {
        public string name { get; }
        private double initialMoney;
        private double currentMoney;

        public Player(string name, double initialMoney)
        {
            this.name = name;
            this.initialMoney = initialMoney;
            currentMoney = initialMoney;
        }

        public void Play(CasinoTable casinoTable)
        {
            Console.WriteLine($"{name} started with ${initialMoney}.");

            while (currentMoney > 0)
            {
                double betAmount = Program.rng.Next(1, (int)currentMoney + 1);
                int betNumber = Program.rng.Next(0, 37);
                Console.WriteLine($"{name} bets ${betAmount} on number {betNumber}.");
                int winningNumber = Program.rng.Next(0, 37);
                Console.WriteLine($"Roulette spins... Result: {winningNumber}");
                if (betNumber == winningNumber)
                {
                    currentMoney += betAmount;
                    Console.WriteLine($"{name} wins! New balance: ${currentMoney}.");
                }
                else
                {
                    currentMoney -= betAmount;
                    Console.WriteLine($"{name} loses! New balance: ${currentMoney}.");
                }
                Thread.Sleep(100);
            }
            casinoTable.RecordResult($"{name} [{initialMoney}] [{currentMoney}]");
            Console.WriteLine($"{name} leaves the table.");
            casinoTable.PlayerFinished(this);
        }
    }
}