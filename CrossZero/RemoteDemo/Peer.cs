using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace NetBase
{
    public class Peer
    {
        public delegate void ReceiveMessage(string message);
        public delegate void ReceiveBitmap(System.Drawing.Bitmap message);
        public delegate void SocketClose();

        public event ReceiveMessage onReceiveMessage;
        public event SocketClose onSocketClose;
        public event ReceiveBitmap onReceiveBitmap;

        public bool IsServer { get; private set; }

        TcpClient client;
        DateTime oldPing;

        const char separator = '\0';

        public Peer(TcpClient client, bool isServer)
        {
            this.client = client;
            IsServer = isServer;
            oldPing = DateTime.Now;
        }

        public async void ListenData()
        {
            var stream = client.GetStream();
            var buffer = new byte[1024];
            while (true)
            {
                try
                {
                    string str = "";
                    while (true)
                    {
                        int len = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (len == 0) break;
                        str += Encoding.UTF8.GetString(buffer, 0, len);
                        if (!stream.DataAvailable) break;
                    }
                    string[] comms = str.Split(separator);
                    foreach (var c in comms)
                    {
                        if (c.StartsWith("m"))
                            onReceiveMessage.Invoke(c.Substring(1));//is not null
                        else if (c.StartsWith("s"))
                        {
                            var bmp = Datas.Base64ToBitmap(c.Substring(1));
                            onReceiveBitmap(bmp);
                        }
                    }
                    if ((DateTime.Now - oldPing).TotalSeconds > 0.1)
                    {
                        Datas.WriteText(client.GetStream(), "p" + separator);
                        oldPing = DateTime.Now;
                    }
                }
                catch
                {

                }
                if (!client.Client.Connected)
                {
                    onSocketClose();
                    break;
                }
            }
        }

        public void SendChatMessage(string message)
        {
            message = message.Replace(separator, ' ');
            Datas.WriteText(client.GetStream(), "m" + message + separator);
        }

        public void SendScreenData(System.Drawing.Bitmap base64)
        {
            Datas.WriteText(client.GetStream(), "s" + Datas.BitmapToBase64(base64) + separator);
        }

        int parseInt(string str)
        {
            int i = 0;
            int.TryParse(str, out i);
            return i;
        }
    }
}
