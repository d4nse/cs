using System.Net.Sockets;
using System.Text;
using static System.Console;

namespace QuoteClient;

public static class Program {

    public static string get_input() {
        string ? input;
        Write("> ");
        while (string.IsNullOrEmpty(input = ReadLine())) {
            Write("Invalid input.\n> ");
        }
        return input;
    }

    public static async Task Main() {
        const string server = "127.0.0.1";
        const int port = 8888;
        string? response = string.Empty;
        string? request = string.Empty;

        using var client = new TcpClient();
        try {
            await client.ConnectAsync(server, port);
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.AutoFlush = true;

            // handle initial message
            response = await reader.ReadLineAsync();
            switch (response) {
            case "SERVER_FULL":
                WriteLine("Server is full. Try again later.");
                return;
            case "ACCESS_GRANTED":
                WriteLine("Access granted by server.");
                break;
            default:
                WriteLine($"Unknown server initial response: {response}");
                return;
            }

            // make an auth request
            Write("Username: ");
            var username = ReadLine();
            Write("Password: ");
            var password = ReadLine();
            await writer.WriteLineAsync($"{username}:{password}");
            // handle auth response
            response = await reader.ReadLineAsync();
            switch (response) {
            case "AUTH_SUCCESS":
                WriteLine("Authentication successful.");
                break;
            case "AUTH_FAILURE":
                WriteLine("Authentication failed.");
                return;
            default:
                WriteLine($"Unknown server auth response: {response}");
                return;
            }

            while (true) {
                string input = get_input();
                try {
                    await writer.WriteLineAsync(input);
                    if (input == "exit")
                        break;
                    response = await reader.ReadLineAsync();
                    switch (response) {
                    case "LIMIT_REACHED":
                        WriteLine("Reached limits for today.");
                        return;
                    case "INVALID_REQUEST":
                        WriteLine("Invalid command.");
                        break;
                    case null:
                        WriteLine("Connection closed by server");
                        return;
                    default:
                        WriteLine($"Quote: {response}");
                        break;
                    }
                } catch (IOException) {
                    WriteLine("Connection lost");
                    return;
                }
            }
        } catch (Exception ex) {
            WriteLine($"Error: {ex.Message}");
        } finally {
            WriteLine("Disconnected");
        }
    }
}
