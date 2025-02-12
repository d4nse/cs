using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;

namespace MailSender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string server="100.30.0.30";// sets the server address
        private void button2_Click(object sender, EventArgs e)
        {
           // create a message object
            MailMessage message = new MailMessage(fromBox.Text, toBox.Text, themeBox.Text,bodyBox.Text);
            // create a send object
            SmtpClient client = new SmtpClient(server);
            client.Port = 25;//sets the server port
            // settings for sending mail
            client.Credentials = new NetworkCredential("test","test");
            // call asynchronous message sending
            client.SendAsync(message,"That's all");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // here you need to add a file selection dialog box and add the selected file to the MailMessage
            // attachment collection and to the list on the form
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // here you need to clear all fields of the form and the content of the MailMessage object
        }
    }
}
