using System.Net
using System.Net.Socet
using System
using System.Text

namespace TestVSCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string iPAddress = "127.0.0.1";
            int port = 1024;

            Socket serverSocket = new Socket(AddreSSFamily.InterNetwork, SocketType.Stream, ProtocalType.Tcp);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(iPAddress), port);
            serverSocket.Bind(ipEndPoint);

            serverSocket.Listen(1);
            Console.WriteLine("Ожидание подключение клиента");

            Socket clientSocket = serverSocket.Accept();
            Console.WriteLine("Клиент подключен" + clientSocket.RemoteEndPoint.ToString());

            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            string receivedMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

            Console.WriteLine($"Сервер:\nD {DateTime.Now:HH:mm} от");
        }
    }
}