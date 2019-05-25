using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace CrossZero
{
    public class Peer
    {
        public delegate void ReceiveMessage(string message);
        public delegate void SocketClose();

        public event ReceiveMessage onReceiveMessage;
        public event SocketClose onSocketClose;

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
                    foreach(var c in comms)
                    {
                        if (c.StartsWith("m"))
                            onReceiveMessage.Invoke(c.Substring(1));//is not null
                        
                        
                    }
                    if((DateTime.Now - oldPing).TotalSeconds > 0.1)
                    {
                        Datas.WriteText(client.GetStream(),"p"+ separator);
                        oldPing = DateTime.Now;
                    }
                }
                catch {
                    
                }
                if(!client.Client.Connected)
                {
                  onSocketClose();
                  break;
                }
            }
        }

        public void SendChatMessage(string message)
        {
            message = message.Replace(separator, ' ');
            Datas.WriteText(client.GetStream(),"m"+ message + separator);
        }

        int parseInt(string str)
        {
            int i = 0;
            int.TryParse(str, out i);
            return i;
        }
    }
}
