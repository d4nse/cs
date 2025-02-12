using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Console;

namespace server;

public class TcpServer {
    private TcpListener listener;
    private bool is_running;

    public TcpServer(string address, int port) {
        listener = new(IPAddress.Parse(address), port);
        Console.CancelKeyPress +=
            new ConsoleCancelEventHandler(on_server_shutdown);
    }

    public async Task start() {
        listener.Start();
        is_running = true;
        var server_ep = listener.Server.LocalEndPoint as IPEndPoint;
        log($"Server started on {server_ep?.Address}:{server_ep?.Port}, listening...");
        await accept_clients_async();
    }

    private async Task accept_clients_async() {
        while (is_running) {
            var client = await listener.AcceptTcpClientAsync();
            var client_ep = client.Client.RemoteEndPoint as IPEndPoint;
            log($"Client connected: {client_ep?.Address}:{client_ep?.Port}");
            await handle_client_async(client);
        }
    }

    private async Task handle_client_async(TcpClient client) {
        using var stream = client.GetStream();
        var buf = new byte[1024];
        int bytes_read;
        try {
            while ((bytes_read = await stream.ReadAsync(buf, 0, buf.Length)) !=
                   0) {
                var request = Encoding.ASCII.GetString(buf, 0, bytes_read);
                log($"Received request: '{request}'");
                string response = process_request(request);
                await write_async(response, stream);
                log($"Sent response: '{response}'");
                if (response.Equals("bye"))
                    break;
            }
        } catch (Exception ex) {
            log($"Client error: {ex.Message}");
        } finally {
            client.Close();
            log($"Client disconnected.");
        }
    }

    private string process_request(string request) {
        return request switch {
            "echo" => request,
            "bye" => request,
            "ping" => "pong",
            "hello" => "hi",
            "date" => DateTime.Now.ToString("yyyy-MM-dd"),
            "time" => DateTime.Now.ToString("HH:mm:ss"),
            _ => $"Error: Unknown request",
        };
    }

    public void stop() {
        is_running = false;
        listener.Stop();
        WriteLine("Server stopped.");
    }

    private async Task write_async(string message, NetworkStream stream) {
        var response = Encoding.ASCII.GetBytes(message);
        await stream.WriteAsync(response, 0, response.Length);
    }

    private void on_server_shutdown(object? sender, ConsoleCancelEventArgs e) {
        stop();
    }

    private void log(object message) {
        WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} {message}");
    }
}

public static class Program {
    public static async Task Main(string[] args) {
        var server = new TcpServer("127.0.0.1", 8888);
        await server.start();
    }
}
