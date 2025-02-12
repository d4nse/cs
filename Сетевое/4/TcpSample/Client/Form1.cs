
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        TcpClient client;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //creation of the IPEndPoint class instance
                IPEndPoint endPoint = new IPEndPoint(
                    IPAddress.Parse(textBox1.Text),
                    Convert.ToInt32(textBox3.Text)
                );
                client = new TcpClient();
                //connection setup using
                //IP data and port number
                client.Connect(endPoint);

                //getting network stream
                NetworkStream nstream = client.GetStream();
                //converting a message string to byte array
                byte[] barray = Encoding.Unicode.GetBytes(textBox2.Text);
                //writing the whole array to network stream
                nstream.Write(barray, 0, barray.Length);
                //closing the client
                client.Close();
            }
            catch (SocketException sockEx)
            {
                MessageBox.Show("Socket Exception:" + sockEx.Message);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Exception :" + Ex.Message);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (client != null)
                client.Close();
        }
    }
}
