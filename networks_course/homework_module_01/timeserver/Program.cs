using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Console;

namespace timeserver;

public static class Program {
    public static void Main(string[] args) {
        int port = 12345;
        using var server = new UdpClient(port);
        WriteLine($"UDP Time Server is running on {port}...");
        IPEndPoint client_ep = new(IPAddress.Any, 0);
        while (true) {
            // wait for time request
            var request_bytes = server.Receive(ref client_ep);
            var request = Encoding.ASCII.GetString(request_bytes);
            WriteLine($"Received request from client: {request}");

            if (request == "TIME_REQUEST") {
                // send time
                var time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                var response_bytes = Encoding.ASCII.GetBytes(time);
                server.Send(response_bytes, response_bytes.Length, client_ep);
                WriteLine(
                    $"Sent: {time} to {client_ep.Address}:{client_ep.Port}");
            } else {
                WriteLine(
                    $"Invalid request received from {client_ep.Address}:{client_ep.Port}: {request}");
            }
        }
    }
}
