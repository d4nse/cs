using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace rps_server;

class Program {
    static void Main(string[] args) {
        TcpListener server = new TcpListener(IPAddress.Any, 8888);
        server.Start();
        Console.WriteLine("Server started. Waiting for connections...");

        while (true) {
            TcpClient client = server.AcceptTcpClient();
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj) {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        Random rand = new Random();

        try {
            // Receive game mode and match settings
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string[] settings = Encoding.ASCII.GetString(buffer, 0, bytesRead).Split(',');
            int gameMode = int.Parse(settings[0]);
            int gamesInMatch = int.Parse(settings[1]);

            List<MatchResult> matchResults = new List<MatchResult>();
            Stopwatch matchTimer = new Stopwatch();
            matchTimer.Start();

            for (int gameNumber = 1; gameNumber <= gamesInMatch; gameNumber++) {
                GameResult gameResult = PlayGame(stream, gameMode, rand, gameNumber);
                matchResults.Add(new MatchResult(gameResult));
                SendGameResult(stream, gameResult);
            }

            matchTimer.Stop();
            SendMatchResult(stream, matchResults, matchTimer.Elapsed);
        } catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); } finally {
            client.Close();
        }
    }

    static GameResult PlayGame(NetworkStream stream, int gameMode, Random rand, int gameNumber) {
        GameResult result = new GameResult(gameNumber);
        Stopwatch gameTimer = new Stopwatch();
        gameTimer.Start();

        for (int round = 1; round <= 5; round++) {
            string player1Choice = GetChoice(stream, gameMode, 1, rand);
            if (HandleSpecialChoice(player1Choice, result, 1)) break;

            string player2Choice = GetChoice(stream, gameMode, 2, rand);
            if (HandleSpecialChoice(player2Choice, result, 2)) break;

            string roundResult = DetermineRoundResult(player1Choice, player2Choice);
            result.AddRoundResult(round, player1Choice, player2Choice, roundResult);
            SendRoundResult(stream, round, player1Choice, player2Choice, roundResult);

            if (roundResult != "Draw" && result.Player1Wins + result.Player2Wins >= 3) break;
        }

        gameTimer.Stop();
        result.Duration = gameTimer.Elapsed;
        return result;
    }

    static bool HandleSpecialChoice(string choice, GameResult result, int player) {
        if (choice == "D") {
            result.IsDraw = true;
            return true;
        }
        if (choice == "C") {
            result.Winner = player == 1 ? 2 : 1;
            return true;
        }
        return false;
    }

    static string GetChoice(NetworkStream stream, int gameMode, int player, Random rand) {
        if (gameMode == 2 || (gameMode == 1 && player == 2)) return GenerateComputerChoice(rand);

        byte[] prompt = Encoding.ASCII.GetBytes($"ROUND: Choose (R)ock, (P)aper, (S)cissors, (D)raw, or (C)oncede: ");
        stream.Write(prompt, 0, prompt.Length);

        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        return Encoding.ASCII.GetString(buffer, 0, bytesRead).ToUpper();
    }

    static string GenerateComputerChoice(Random rand) {
        string[] choices = { "R", "P", "S" };
        return choices[rand.Next(choices.Length)];
    }

    static string DetermineRoundResult(string p1, string p2) {
        if (p1 == p2) return "Draw";
        if ((p1 == "R" && p2 == "S") || (p1 == "S" && p2 == "P") || (p1 == "P" && p2 == "R")) return "Player1 Wins";
        return "Player2 Wins";
    }

    static void SendRoundResult(NetworkStream stream, int round, string p1, string p2, string result) {
        string message = $"Round {round}: P1:{p1} vs P2:{p2} => {result}\n";
        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    static void SendGameResult(NetworkStream stream, GameResult result) {
        string message =
            $"Game {result.GameNumber} Result: " + $"{(result.IsDraw ? "Draw" : $"Player {result.Winner} Wins")} " +
            $"Duration: {result.Duration:mm\\:ss}\n";
        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    static void SendMatchResult(NetworkStream stream, List<MatchResult> results, TimeSpan duration) {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("\nMatch Results:");
        foreach (var result in results) {
            sb.AppendLine($"Game {result.GameNumber}: " +
                          $"{(result.IsDraw ? "Draw" : $"Player {result.Winner} Wins")}");
        }

        byte[] data = Encoding.ASCII.GetBytes(sb.ToString());
        stream.Write(data, 0, data.Length);
    }
}

class GameResult {
    public int GameNumber { get; }
    public int Winner { get; set; }
    public bool IsDraw { get; set; }
    public TimeSpan Duration { get; set; }
    public int Player1Wins { get; set; }
    public int Player2Wins { get; set; }
    public List<RoundResult> Rounds { get; } = new List<RoundResult>();

    public GameResult(int gameNumber) => GameNumber = gameNumber;

    public void AddRoundResult(int round, string p1, string p2, string result) {
        Rounds.Add(new RoundResult(round, p1, p2, result));
        if (result == "Player1 Wins")
            Player1Wins++;
        else if (result == "Player2 Wins")
            Player2Wins++;
    }
}

class RoundResult {
    public int RoundNumber { get; }
    public string Player1Choice { get; }
    public string Player2Choice { get; }
    public string Result { get; }

    public RoundResult(int round, string p1, string p2, string result) {
        RoundNumber = round;
        Player1Choice = p1;
        Player2Choice = p2;
        Result = result;
    }
}

class MatchResult {
    public int GameNumber { get; }
    public int Winner { get; }
    public bool IsDraw { get; }

    public MatchResult(GameResult gameResult) {
        GameNumber = gameResult.GameNumber;
        Winner = gameResult.Winner;
        IsDraw = gameResult.IsDraw;
    }
}
