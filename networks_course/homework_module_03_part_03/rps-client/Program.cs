using static System.Console;
using System.Net.Sockets;
using System.Text;

namespace rps_client;

class Program {
    static void Main(string[] args) {
        try {
            using TcpClient client = new TcpClient("localhost", 8888);
            NetworkStream stream = client.GetStream();

            WriteLine("Select game mode:\n1. Human vs Computer\n2. Computer vs Computer");
            int gameMode = int.Parse(ReadLine() ?? "1");
            WriteLine("Enter number of games in match:");
            int gamesInMatch = int.Parse(ReadLine() ?? "5");

            string settings = $"{gameMode},{gamesInMatch}";
            byte[] data = Encoding.ASCII.GetBytes(settings);
            stream.Write(data, 0, data.Length);

            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0) {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Write(message);

                if (message.Contains("Choose (R)")) {
                    string choice = ReadLine()?.ToUpper()!;
                    data = Encoding.ASCII.GetBytes(choice);
                    stream.Write(data, 0, data.Length);
                }
            }
        } catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }
}
