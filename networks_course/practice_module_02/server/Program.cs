using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Console;

namespace server;

public class Server {
    private IPAddress? addr;
    private int port;
    private TcpListener? listener;

    public Server(string address, int port) {
        this.addr = IPAddress.Parse(address);
        this.port = port;
    }

    public async Task run() {
        if (addr == null)
            throw new Exception("Address is null");
        Console.CancelKeyPress +=
            new ConsoleCancelEventHandler(on_server_shutdown);
        listener = new(addr, port);
        listener.Start();
        log($"Server started on {addr}:{port}. Listening...");
        try {
            while (true) {
                using var client = await listener.AcceptTcpClientAsync();
                var client_ep = client.Client.RemoteEndPoint as IPEndPoint;
                log($"Client connected: {client_ep?.Address}:{client_ep?.Port}");

                using var stream = client.GetStream();
                var request = await get_request(stream);
                await respond(request, stream);
            }
        } catch (Exception ex) {
            log($"{ex.Message}");
        }
        // finally {
        //     listener.Stop();
        //     log("Server shutdown success.");
        // }
    }

    private async Task<string> get_request(NetworkStream stream) {
        var buf = new byte[1024];
        int bytes_read = await stream.ReadAsync(buf, 0, buf.Length);
        var request = Encoding.ASCII.GetString(buf, 0, bytes_read);
        log($"Received request: '{request}'");
        return request;
    }

    private async Task respond(string request, NetworkStream stream) {
        bool has_responded = true;
        string message = string.Empty;
        switch (request) {
        case "hello":
            message = "Hello from server, client!";
            break;
        case "time":
            message = DateTime.Now.ToString("HH:mm:ss");
            break;
        case "date":
            message = DateTime.Now.ToString("yyyy-MM-dd");
            break;
        default:
            log($"Unknown request received: '{request}'. Ignoring.");
            has_responded = false;
            break;
        }
        if (has_responded) {
            await write_async(message, stream);
            log($"Sent '{request}' response: '{message}'");
        }
    }
    private async Task write_async(string message, NetworkStream stream) {
        var response = to_bytes(message);
        await stream.WriteAsync(response, 0, response.Length);
    }

    private byte[] to_bytes(string message) {
        return Encoding.ASCII.GetBytes(message);
    }

    private void on_server_shutdown(object? sender, ConsoleCancelEventArgs e) {
        if (listener == null)
            log("Listener is null, shutting down...");
        else {
            listener?.Stop();
            log("Server shutdown success.");
        }
    }

    private void log(object message) {
        WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} {message}");
    }
}

public static class Program {
    public static async Task Main(string[] args) {
        var server = new Server("127.0.0.1", 8888);
        await server.run();
    }
}
