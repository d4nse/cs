using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace TestUDP
{
    public partial class Form1 : Form
    {
        // Делегат для безопасного обновления текстового поля из другого потока
        delegate void AddTextDelegate(String text);

        // Поток для приема сообщений
        System.Threading.Thread thread;

        // Сокет для UDP-соединения
        Socket socket;

        public Form1()
        {
            InitializeComponent();
        }      

        // Метод для добавления текста в textBox1
        void AddText(String text)
        {
            textBox1.Text += text;
        }

        // Функция приема сообщений, выполняемая в отдельном потоке
        void RecivFunction(object obj)
        {
            Socket rs = (Socket)obj;

            byte[] buffer = new byte[1024];
            do
            {
                EndPoint ep = new IPEndPoint(0x7F000000, 100);
                int l = rs.ReceiveFrom(buffer, ref ep);
                String strClientIP = ((IPEndPoint)ep).Address.ToString();
                String str = String.Format("\nReceived from {0}\r\n{1}\r\n", 
                    strClientIP, System.Text.Encoding.Unicode.GetString(buffer, 0, l));
                textBox1.BeginInvoke(new AddTextDelegate(AddText), str);
            } while (true);
        }

        // Обработчик нажатия кнопки "Start"
        private void button1_Click(object sender, EventArgs e)
        {
            if (socket != null && thread != null)
                return;

            // Создание и настройка сокета
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
            socket.Bind(new IPEndPoint(IPAddress.Parse("10.2.21.129"), 100));

            // Запуск потока для приема сообщений
            thread = new System.Threading.Thread(RecivFunction);
            thread.Start(socket);
        }

        // Обработчик нажатия кнопки "Stop"
        private void button2_Click(object sender, EventArgs e)
        {
            if (socket != null)
            {               
                // Остановка потока и закрытие сокета
                thread.Abort();
                thread = null;
                socket.Shutdown(SocketShutdown.Receive);
                socket.Close();
                socket = null;
                textBox1.Text = "";
            }
        }

        // Обработчик нажатия кнопки "Clear"
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        // Обработчик нажатия кнопки "Send"
        private void button4_Click(object sender, EventArgs e)
        {
            // Отправка сообщения через UDP
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
            socket.SendTo(System.Text.Encoding.Unicode.GetBytes(textBox2.Text), new IPEndPoint(IPAddress.Parse("10.2.21.255"), 100));
            socket.Shutdown(SocketShutdown.Send);
            socket.Close();
        }
    }
}

