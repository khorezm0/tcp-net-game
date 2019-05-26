using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using NetBase;

namespace RemoteDemo
{
    public partial class ConnectionForm : Form
    {
        public ConnectionForm()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs args)
        {
            Peer peer;
            string ip = textBox1.Text;
            IPEndPoint e = new IPEndPoint(IPAddress.Parse(ip), 7778);
            AsyncTcpClient c = new AsyncTcpClient(e);
            Rectangle size = Screen.PrimaryScreen.Bounds;

            c.onPeerConnected += (p) => 
            {
                peer = p;
                ViewForm form = new ViewForm();
                form.Size = size.Size;
                form.server = p;
                form.Show();
            };
            c.onTcpError += (err) => 
            {
                MessageBox.Show(err);
            };
            c.Connect();
        }

        private void button2_Click(object sender, EventArgs args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7778);
            AsyncTcpServer tcpServer = new AsyncTcpServer(endPoint);


            textBox1.Text = endPoint.Address.ToString();

            Rectangle size = Screen.PrimaryScreen.Bounds;


            tcpServer.onPeerConnected += (p) =>
            {
                Timer t = new Timer();
                t.Interval = 200;
                t.Tick += (o, e) => 
                {
                    Bitmap img = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (var g = Graphics.FromImage(img))
                    {
                        g.CopyFromScreen(new Point(), new Point(), size.Size);
                    }
                    p.SendScreenData(img);
                };
                t.Start();
            };

            tcpServer.Listen();


        }
    }
}
