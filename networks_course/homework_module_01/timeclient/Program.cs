using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Console;

namespace timeclient;

public static class Program {
    public static void Main(string[] args) {
        string server_addr = "127.0.0.1";
        int server_port = 12345;
        using UdpClient client = new UdpClient();

        // request time
        var request_bytes = Encoding.UTF8.GetBytes("TIME_REQUEST");
        client.Send(request_bytes, request_bytes.Length, server_addr,
                    server_port);
        WriteLine("Request sent to the server.");

        // receive time
        IPEndPoint response_ep = new IPEndPoint(IPAddress.Any, 0);
        byte[] response_bytes = client.Receive(ref response_ep);
        string response = Encoding.ASCII.GetString(response_bytes);
        WriteLine($"Timeserver response: {response}");
    }
}
