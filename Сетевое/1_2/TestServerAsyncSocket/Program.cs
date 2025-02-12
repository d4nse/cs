using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TestServerAsyncSocket
{
    class AsyncServer
    {
        
        IPEndPoint endP;
        Socket socket;        

        public AsyncServer(string strAddr, int port)
        {
            endP = new IPEndPoint(IPAddress.Parse(strAddr), port);
        }
        void MyAcceptCallbakFunction(IAsyncResult ia)
        {
            //get a link to the listening socket
            Socket socket=(Socket)ia.AsyncState;
           
            //get a socket to exchange data with the client
            Socket ns = socket.EndAccept(ia);
            
            //output the connection information to the console
            Console.WriteLine(ns.RemoteEndPoint.ToString());
            
            //send the client the current time asynchronously, 
            //by the end of the sending operation, the  
            //MySendCallbackFunction method will be called.
            byte[] sendBufer = System.Text.Encoding.ASCII.GetBytes(DateTime.Now.ToString());
            ns.BeginSend(sendBufer, 0, sendBufer.Length, SocketFlags.None, new AsyncCallback(MySendCallbackFunction), ns);
            
            //resume asynchronous Accept
            socket.BeginAccept(new AsyncCallback(MyAcceptCallbakFunction), socket);
        }
        void MySendCallbackFunction(IAsyncResult ia)
        { 
            //after sending data to the client,
            //close the socket (if we would need to continue
            //data exchange, we could have arranged it here)
            Socket ns=(Socket)ia.AsyncState;
            int n = ((Socket)ia.AsyncState).EndSend(ia);
            ns.Shutdown(SocketShutdown.Send);
            ns.Close();
        }

        public void StartServer()
        {
            if (socket != null)
                return;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            socket.Bind(endP);
            socket.Listen(10);
            //start asynchronous Accept, when the client 
            //is connected, the MyAcceptCallbakFunction handler is called.
            socket.BeginAccept(new AsyncCallback(MyAcceptCallbakFunction), socket);
        }
    }


    class Program
    {     
        static void Main(string[] args)
        {
            AsyncServer server = new AsyncServer("127.0.0.1", 1024);
            server.StartServer();
            Console.Read();         
        }
    }
}
