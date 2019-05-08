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
            serv.onTcpError += OnConnectionRefuse;
            serv.onPeerConnected += OnConnected;
            serv.Listen();
            richTextBox1.Text += "Waiting for opponent....\n";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Text = "Client";
            panel1.Enabled = false;
            AsyncTcpClient client = new AsyncTcpClient(new IPEndPoint(IPAddress.Parse(textBox1.Text), 7777));
            client.onTcpError += OnConnectionRefuse;
            client.onPeerConnected += OnConnected;
            client.Connect();
            richTextBox1.Text += "Connecting...\n";
        }
        void OnConnectionRefuse(string fail)
        {
            panel1.Visible = true;
            panel1.Enabled = true;
            panel2.Visible = false;
            button3.Enabled = false;
            richTextBox1.Text += "Connection refused! \n"+fail;
        }
        void OnConnected(Peer p)
        {
            panel1.Visible = false;
            panel2.Visible = true;
            richTextBox1.Text += "Connected to server!\n";
            peer = p;
            peer.onReceiveMessage += (x) => { richTextBox1.Text += "peer: " + x + "\n"; };
            peer.onReceiveStep += (x) =>
            {
                var pb = (PictureBox) (panel2.Controls.Find("pictureBox" + x, false)[0]);
                if (!peer.IsServer) pb.Image = cross;
                else pb.Image = circle;
            };
            peer.onReceiveWinner += (win) =>
            {
                if (win == 1)
                {
                    BackColor = Color.FromArgb(147,255,155);
                    richTextBox1.Text += "You win!\n";
                }
                else if(win == 0)
                {
                    BackColor = Color.FromArgb(255,147,147);
                    richTextBox1.Text += "You lose!\n";
                }
                else if (win == -1)
                {
                    BackColor = Color.FromArgb(255, 147, 147);
                    richTextBox1.Text += "Draft!\n";
                }
            };
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (peer != null)
            {
                peer.SendChatMessage(textBox2.Text);
                richTextBox1.Text += "me: " + textBox2.Text + "\n";
                textBox2.Text = "";
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if(peer != null && peer.MyTurn)
            { 
                var pb = (PictureBox)sender;
                if(pb.Image == null)
                {
                    var i = pb.Name.Replace("pictureBox", "");
                    peer.SendGameStep(i);
                    if (peer.IsServer) pb.Image = cross;
                    else pb.Image = circle;
                }
            }
            //MessageBox.Show(i);
            //panel2.Controls.Find();
        }
    }
}
