using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web;

namespace HTTP_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(URL.Text);
            req.Method = "GET";
            if (IfProxy.Checked)
            {
                WebProxy proxy = new WebProxy(proxyAddr.Text);
                
                proxy.Credentials = new NetworkCredential(proxyUser.Text, proxyPassword.Text);
                req.Proxy = proxy;
            }
 
            HttpWebResponse rez = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(rez.GetResponseStream(), Encoding.Default);
            response.Text = sr.ReadToEnd();

        }
    }
}