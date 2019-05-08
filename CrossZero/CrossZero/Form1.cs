using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace CrossZero
{
    public partial class Form1 : Form
    {
        Peer peer;

        public Form1()
        {
            InitializeComponent();
            button3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Text = "Server";
            button1.Enabled = false;
            button2.Enabled = false;
            AsyncTcpServer serv = new AsyncTcpServer(new IPEndPoint(IPAddress.Any, 7777));
            serv.Listen();
            serv.onPeerConnected = OnConnected;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Text = "Client";
            button1.Enabled = false;
            button2.Enabled = false;
            AsyncTcpClient client = new AsyncTcpClient(new IPEndPoint(IPAddress.Parse(textBox1.Text), 7777));
            client.Connect();
            client.onPeerConnected = OnConnected;
        }

        void OnConnected(Peer p)
        {
            listBox1.Items.Add("Connected to server!");
            peer = p;
            peer.onReceiveMessage = (x) => { listBox1.Items.Add("peer: " + x); };
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (peer != null)
            {
                peer.SendChatMessage(textBox2.Text);
                listBox1.Items.Add("me: "+textBox2.Text);
                textBox2.Text = "";
            }
        }
    }
}
