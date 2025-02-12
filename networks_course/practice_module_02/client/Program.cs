using System.Net.Sockets;
using System.Text;
using static System.Console;

namespace client;

public static class Program {

    public static async Task send_request(string request) {
        using var client = new TcpClient();
        try {
            await client.ConnectAsync("127.0.0.1", 8888);
        } catch (Exception ex) {
            WriteLine($"Error: {ex.Message}. Is server running?");
            return;
        }
        using var stream = client.GetStream();
        // send request
        string message = string.Empty;
        await write_async(request, stream);
        WriteLine($"Sent request: '{request}'");
        // receive response
        var buf = new byte[1024];
        int bytes_read = await stream.ReadAsync(buf, 0, buf.Length);
        var response = Encoding.ASCII.GetString(buf, 0, bytes_read);
        if (string.IsNullOrEmpty(response)) {
            WriteLine(
                "Server has ignored response or returned an empty string");
        } else {
            WriteLine($"Received response: '{response}'");
        }
    }

    public static async Task write_async(string message, NetworkStream stream) {
        var data = Encoding.ASCII.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);
    }

    public static string argtoreq(string arg) {
        return arg.Remove(0, 2);
    }

    public static async Task Main(string[] args) {
        if (args.Length == 0) {
            WriteLine("Usage: client --requestname");
            return;
        }
        foreach (var arg in args) {
            await send_request(argtoreq(arg));
        }
    }
}
