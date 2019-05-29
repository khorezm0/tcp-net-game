using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

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

        Thread listenThread;
        Thread sendThread;
        bool listening;

        public void ListenData()
        {
            listening = true;
            listenThread = new Thread(ListenDataTh);
            sendThread = new Thread(SendThread);
            listenThread.Start();
            sendThread.Start();
        }

        public void Stop()
        {
            listening = false;
            if (listenThread != null && listenThread.IsAlive)
                listenThread.Abort();
            if (sendThread != null && sendThread.IsAlive)
                sendThread.Abort();
        }

        void ListenDataTh()
        {
            var stream = client.GetStream();
            while (listening)
            {
                try
                {
                    Datas.ReadPart(stream, separator, ProcessData);
                    if ((DateTime.Now - oldPing).TotalSeconds > 0.1)
                    {
                        //Datas.WriteText(client.GetStream(), "p" + separator);
                        sendQueue.AddLast("ping" + separator);
                        oldPing = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                if (!client.Client.Connected)
                {
                    onSocketClose();
                    break;
                }
                Thread.Sleep(100);
            }

            Console.WriteLine("End listen!");
        }

        void ProcessData(string data)
        {
            if (data.StartsWith("m"))
            {
                onReceiveMessage.Invoke(data.Substring(1));//is not null
            }
            else if (data.StartsWith("s"))
            {
                var bmp = Datas.Base64ToBitmap(data.Substring(1));
                onReceiveBitmap(bmp);
            }
        }

        LinkedList<string> sendQueue = new LinkedList<string>();
        void SendThread()
        {
            while (listening)
            {
                try
                {
                    if (sendQueue.First != null)
                    {
                        Datas.WriteText(client.GetStream(), sendQueue.First.Value);
                        sendQueue.RemoveFirst();
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void SendChatMessage(string message)
        {
            message = message.Replace(separator, ' ');
            //Datas.WriteText(client.GetStream(), "m" + message + separator);
            sendQueue.AddLast("m" + message + separator);
        }

        public void SendScreenData(System.Drawing.Bitmap base64)
        {
            //Datas.WriteText(client.GetStream(), "s" + Datas.BitmapToBase64(base64) + separator);
            sendQueue.AddLast("s" + Datas.BitmapToBase64(base64) + separator);
        }

        int parseInt(string str)
        {
            int i = 0;
            int.TryParse(str, out i);
            return i;
        }
    }
}
