
using System.Security.Cryptography.X509Certificates;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            int totalPlayers = random.Next(20, 101);
            List<Player> players = new List<Player>();

            for (int i = 0; i < totalPlayers; i++)
            {
                players.Add(new Player($"Игрок {i + 1}", random.Next(100, 1001)));
            }

            RouletteTable table = new RouletteTable();
            List<Thread> threads = new List<Thread>();
            foreach (var player in players)
            {
                Thread playerThread = new Thread(() => table.PlayRound(player));
                threads.Add(playerThread);
                playerThread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            using (StreamWriter writer = new StreamWriter("report.txt"))
            {
                foreach (var player in players)
                {
                    writer.WriteLine($"{player.Name} {player.InitialAmount} {player.CurrentAmount}");  
                }
            }

            Console.WriteLine("игра окончена");

        }
        class Player
        {
            public string Name { get; private set; }
            public decimal InitialAmount { get; private set; }
            public decimal CurrentAmount { get; set; }
            public bool IsActive { get; set; }

            private static object LoackObj = new object();

            public Player(string name, decimal initialAmount)
            {
                Name = name;
                InitialAmount = initialAmount;
                CurrentAmount = initialAmount;
                IsActive = true;
            }

            public (decimal betAmount, int number) PlaceBet(Random random)
            {
                if (CurrentAmount <= 0)
                    return (0, 0);

                decimal betAmount = Math.Min(random.Next(1, (int)CurrentAmount + 1), CurrentAmount);
                int number = random.Next(0, 37);

                return (betAmount, number);
            }

            public void UpdateAmount(bool isWin, decimal betAmount)
            {
                lock (LoackObj)
                {
                    if (isWin)
                    {
                        CurrentAmount += betAmount;
                    }
                    else
                    {
                        CurrentAmount -= betAmount;
                    }

                    if (CurrentAmount <= 0)
                    {
                        IsActive = false;
                        Console.WriteLine($"{Name} покинул стол");
                    }
                }
            }
        }
        class RouletteTable
        {
            private Random random = new Random();

            public void PlayRound(Player player)
            {
                while (player.IsActive)
                {
                    var (betAmount, number) = player.PlaceBet(random);

                    if (betAmount > 0)
                    {
                        int winningNumber = GenerateWinningNumber();
                        Console.WriteLine($"{player.Name} ставит {betAmount} на число {number} выигрышное: {winningNumber}");

                        if (winningNumber == number)
                        {
                            player.UpdateAmount(true, betAmount * 2);
                            Console.WriteLine($"{player.Name} выиграл. Текущая сумма: {player.CurrentAmount}");
                        }
                        else
                        {
                            player.UpdateAmount(false, betAmount);
                            Console.WriteLine($"{player.Name} проиграл. Текущая сумма: {player.CurrentAmount}");
                        }
                    }
                    Thread.Sleep(170);
                }
            }

            private int GenerateWinningNumber()
            {
                return random.Next(0, 37);
            }

        }
    }
}