using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using NetBase;

namespace RemoteDemo
{
    public partial class ConnectionForm : Form
    {

        AsyncTcpClient tcpClient;
        AsyncTcpServer tcpServer;

        public ConnectionForm()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs args)
        {
            Peer peer;
            string ip = textBox1.Text;

            button1.Enabled = false;
            button2.Hide();

            try
            {
                IPEndPoint e = new IPEndPoint(IPAddress.Parse(ip), 7778);
                tcpClient = new AsyncTcpClient(e);
                Rectangle size = Screen.PrimaryScreen.Bounds;

                tcpClient.onPeerConnected += (p) =>
                {
                    peer = p;
                    ViewForm form = new ViewForm();
                    form.Size = size.Size;
                    form.server = p;
                    form.Show();
                };
                tcpClient.onTcpError += (err) =>
                {
                    MessageBox.Show(err);
                };
                tcpClient.Connect();
            }
            catch
            {
                button1.Enabled = true;
                button2.Show();
            }

        }


        private void button2_Click(object sender, EventArgs args)
        {
            button2.Enabled = false;
            button1.Hide();

            try
            {

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7778);
                tcpServer = new AsyncTcpServer(endPoint);


                textBox1.Text = endPoint.Address.ToString();

                Rectangle size = Screen.PrimaryScreen.Bounds;


                tcpServer.onPeerConnected += (p) =>
                {
                    Invoke(new Action(() =>
                    {
                        Timer t = new Timer();
                        t.Interval = 100;
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
                    }));
                };

                tcpServer.Listen();
            }
            catch
            {
                button2.Enabled = true;
                button1.Show();
            }
        }

        private void FormClose(object sender, FormClosedEventArgs e)
        {
            if (tcpClient != null &&  tcpClient.) tcpClient.Client.Stop();
            if (tcpServer != null) tcpServer.Stop();
        }
    }
}
