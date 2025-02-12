using System;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TcpListener list;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //creating the TcpListener class instance
                //data on host and port are read
                //from text boxes
                list = new TcpListener(
                    IPAddress.Parse(textBox1.Text),
                    Convert.ToInt32(textBox2.Text));
                //start listening to clients
                list.Start();
                //creating a separate thread to read messages
                Thread thread = new Thread(
                        new ThreadStart(ThreadFun)
                        );
                thread.IsBackground = true;
                //start a thread
                thread.Start();
            }
            catch (SocketException sockEx)
            {
                MessageBox.Show("Socket exception: " + sockEx.Message);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Exception : " + Ex.Message);
            }
        }

        void ThreadFun()
        {
            while (true)
            {
                //server informs a client that it is ready 
                //for connection
                TcpClient cl = list.AcceptTcpClient();
                //reading data from network in the Unicode format
                StreamReader sr = new StreamReader(
                    cl.GetStream(), Encoding.Unicode
                );
                string s = sr.ReadLine();
                //adding the obtained message to the list
                messageList.Items.Add(s);
                cl.Close();
                //when getting the EXIT message, exit the application
                if (s.ToUpper() == "EXIT")
                {
                    list.Stop();
                    this.Close();
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (list != null)
                list.Stop();
        }
    }
}
