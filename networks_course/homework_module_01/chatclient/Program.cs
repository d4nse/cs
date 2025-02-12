using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Console;

namespace chatclient;

public static class Program {

    public static string get_input() {
        string ? input;
        Write("> ");
        while (string.IsNullOrEmpty(input = ReadLine())) {
            WriteLine("Invalid input.");
            Write("> ");
        }
        return input;
    }

    public static void Main(string[] args) {
        var server_addr = "127.0.0.1";
        var server_port = 12345;

        using var server = new TcpClient(server_addr, server_port);
        var stream = server.GetStream();
        WriteLine("Connected to chat server.");

        while (true) {
            WriteLine("Enter command like `send <recipient> <message>` or " +
                      "`receive <recipient>` or `exit`");
            string input = get_input();
            if (input == "exit") {
                break;
            }
            var data = Encoding.UTF8.GetBytes(input);
            stream.Write(data, 0, data.Length);
            if (input.StartsWith("receive")) {
                var response_bytes = new byte[256];
                int bytes =
                    stream.Read(response_bytes, 0, response_bytes.Length);
                string response =
                    Encoding.UTF8.GetString(response_bytes, 0, bytes);
                WriteLine($"Server responded: {response}");
            }
        }
    }
}
