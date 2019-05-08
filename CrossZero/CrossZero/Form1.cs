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
        Bitmap circle, cross;
        public Form1()
        {
            InitializeComponent();
            button3.Enabled = false;
            //panel2.Visible = false;
            circle = new Bitmap("circle.png");
            cross = new Bitmap("cross.png");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Text = "Server";
            panel1.Enabled = false;
            AsyncTcpServer serv = new AsyncTcpServer(new IPEndPoint(IPAddress.Any, 7777));
            serv.onTcpError = OnServerFail;
            serv.Listen();
            serv.onPeerConnected = OnConnected;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Text = "Client";
            panel1.Enabled = false;
            AsyncTcpClient client = new AsyncTcpClient(new IPEndPoint(IPAddress.Parse(textBox1.Text), 7777));
            client.onTcpError = OnDisconnected;
            client.onPeerConnected = OnConnected;
            client.Connect();
        }
        void OnServerFail(string fail)
        {
            panel1.Visible = true;
            panel1.Enabled = true;
            button3.Enabled = false;
            listBox1.Items.Add(fail);
        }
        void OnDisconnected(string text)
        {
            panel1.Visible = true;
            panel1.Enabled = true;
            button3.Enabled = false;
            listBox1.Items.Add(text);
        }
        void OnConnected(Peer p)
        {
            panel1.Visible = false;
            listBox1.Items.Add("Connected to server!");
            peer = p;
            peer.onReceiveMessage = (x) => { listBox1.Items.Add("peer: " + x); };
            peer.onReceiveStep = (x) =>
            {
                var pb = (PictureBox) (panel2.Controls.Find("pictureBox" + x, false)[0]);
                pb.Image = cross;
            };
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

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            var pb = (PictureBox)sender;
            var i = pb.Name.Replace("pictureBox","");
            pb.Image = (Image)cross;
            peer.SendGameStep(i);
            //MessageBox.Show(i);
            //panel2.Controls.Find();
        }
    }
}
