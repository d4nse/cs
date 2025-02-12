using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Console;

namespace chatserver;

public static class Program {

    private static Dictionary<string, List<string>> messages =
        new Dictionary<string, List<string>>();

    public static void Main(string[] args) {
        int port = 12345;
        TcpListener? server = null;
        try {
            server = new(IPAddress.Any, port);
            server.Start();
            WriteLine($"Chat server is up on port {port}...");
            while (true) {
                var client = server.AcceptTcpClient();
                Task.Run(() => handle_client(client));
            }
        } catch (Exception ex) {
            WriteLine($"Error: {ex.Message}");
        } finally {
            server?.Stop();
        }
    }

    private static void handle_client(TcpClient client) {
        WriteLine($"Client connected.");
        using var stream = client.GetStream();
        var buf = new byte[256];
        int bytes_read;
        while ((bytes_read = stream.Read(buf, 0, buf.Length)) != 0) {
            string request = Encoding.UTF8.GetString(buf, 0, bytes_read);
            WriteLine($"Received message: `{request}`");
            var parts = request.Split(new[] { ' ' }, 2);
            var cmd = parts[0].ToLower();
            if (cmd == "send" && parts.Length == 2) {
                var msg_parts = parts[1].Split(new[] { ' ' }, 2);
                var recipient = msg_parts[0];
                var message = msg_parts.Length > 1 ? msg_parts[1] : "";

                if (!messages.ContainsKey(recipient)) {
                    messages[recipient] = new List<string>();
                }
                messages[recipient].Add(message);
                WriteLine($"Message from `{recipient}` saved.");
            } else if (cmd == "receive") {
                var recipient = parts[1];
                if (messages.TryGetValue(recipient, out var user_messages)) {
                    var response = new StringBuilder();
                    foreach (var msg in user_messages) {
                        response.AppendLine(msg);
                    }
                    messages.Remove(recipient);
                    var response_bytes =
                        Encoding.UTF8.GetBytes(response.ToString());
                    stream.Write(response_bytes, 0, response_bytes.Length);
                    WriteLine($"Sent messages for `{recipient}`");
                } else {
                    var response_bytes = Encoding.UTF8.GetBytes("No messages.");
                    stream.Write(response_bytes, 0, response_bytes.Length);
                }
            }
        }
    }
}
