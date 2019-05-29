using System;
using System.Threading;
using System.Diagnostics;
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
                    Thread tr = new Thread(new ThreadStart(
                        () =>
                        {
                            Stopwatch t = new Stopwatch();
                            Stopwatch t1 = new Stopwatch();
                            t.Start();
                            t1.Start();
                            SendFullDataToPeer(p);

                            while (Visible)
                            {
                                if (t.ElapsedMilliseconds > 1000)
                                {
                                    SendFullDataToPeer(p);
                                    t.Restart();
                                }
                                if (t1.ElapsedMilliseconds > 50)
                                {
                                    SendPartialDataToPeer(p);
                                    t1.Restart();
                                }
                            }
                    }));

                    tr.Start();
                };

                tcpServer.Listen();
            }
            catch
            {
                button2.Enabled = true;
                button1.Show();
            }
        }

        Bitmap oldbmp;

        void SendFullDataToPeer(Peer p)
        {
            Rectangle size = Screen.PrimaryScreen.Bounds;

            Bitmap img = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(img))
            {
                g.CopyFromScreen(new Point(), new Point(), size.Size);
            }
            img = Datas.ResizeImage(img, 256, 256);
            oldbmp = img;

            p.SendScreenData(img);
        }

        void SendPartialDataToPeer(Peer p)
        {
            if(oldbmp != null)
            {

                Rectangle size = Screen.PrimaryScreen.Bounds;

                Bitmap img = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(img))
                {
                    g.CopyFromScreen(new Point(), new Point(), size.Size);
                }
                img = Datas.ResizeImage(img, 256, 256);
                oldbmp = img;

                var changes = BitmapChangesData.GetChanges(oldbmp, img);
                p.SendScreenPartialData(changes);
            }
        }

        private void FormClose(object sender, FormClosedEventArgs e)
        {
            if (tcpClient != null) tcpClient.Client.Stop();
            if (tcpServer != null) tcpServer.Stop();
        }
    }
}
