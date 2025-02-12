using System.Net.Sockets;
using System.Text;
using static System.Console;

namespace client;

public class TcpClientManager : IDisposable {
    private readonly TcpClient client;
    private readonly NetworkStream stream;
    private bool is_connected;

    public TcpClientManager(string hostname, int port) {
        client = new(hostname, port);
        stream = client.GetStream();
        is_connected = true;
    }

    public async Task<string> send_request_async(string request) {
        if (!is_connected) {
            throw new Exception("Client is not connected.");
        }
        await write_async(request);
        WriteLine($"Sent request: '{request}'");
        return await read_response_async();
    }

    private async Task write_async(string message) {
        var buf = Encoding.ASCII.GetBytes(message);
        await stream.WriteAsync(buf, 0, buf.Length);
    }

    private async Task<string> read_response_async() {
        var buf = new byte[1024];
        int bytes_read = await stream.ReadAsync(buf, 0, buf.Length);
        var response = Encoding.ASCII.GetString(buf, 0, bytes_read);
        if (string.IsNullOrEmpty(response))
            WriteLine("Received no response from server, it might be down.");
        else
            WriteLine($"Received response: '{response}'");
        return response;
    }

    public void Dispose() {
        if (is_connected) {
            stream.Close();
            client.Close();
            is_connected = false;
        }
    }
}

public static class Program {

    public static (string host, int port) split_hoststring(string hoststring) {
        var parts = hoststring.Split(':', 2);
        if (parts.Length != 2)
            throw new Exception("Hoststring must have format host:port.");
        string host = parts[0];
        if (!int.TryParse(parts[1], out int port)) {
            throw new Exception("Port must be an integer.");
        }
        return (host, port);
    }

    public static string get_input() {
        string ? input;
        while (true) {
            Write(">>> ");
            if (string.IsNullOrEmpty(input = ReadLine())) {
                WriteLine("Invalid input.");
                continue;
            } else {
                return input.Trim();
            }
        }
    }

    public static async Task Main(string[] args) {
        if (args.Length == 0) {
            WriteLine("Usage: client host:port");
            return;
        }
        try {
            (string host, int port) = split_hoststring(args[0]);
            using var client = new TcpClientManager(host, port);

            while (true) {
                string input = get_input();
                string response = await client.send_request_async(input);
                if (input == "bye") {
                    WriteLine("Said goodbyes, terminating.");
                    break;
                }
            }

        } catch (Exception ex) {
            WriteLine($"Error: {ex.Message}");
            throw;
        }
    }
}
