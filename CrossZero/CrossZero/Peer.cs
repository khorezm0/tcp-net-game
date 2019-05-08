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

        public ReceiveMessage onReceiveMessage { get; set; }
        public ReceiveMessage onReceiveStep { get; set; }
        TcpClient client;

        public Peer(TcpClient client)
        {
            this.client = client;
        }

        public async void ListenData()
        {
            try
            {
                var stream = client.GetStream();
                var buffer = new byte[1024];
                while (true)
                {
                    string str = "";
                    while (true)
                    {
                        int len = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (len == 0) break;
                        str += Encoding.UTF8.GetString(buffer, 0, len);
                        if (!stream.DataAvailable) break;
                    }
                    if (str.StartsWith("m"))
                        onReceiveMessage?.Invoke(str.Substring(1));//is not null
                    else if (str.StartsWith("s")) onReceiveStep?.Invoke(str.Substring(1));
                }
            }
            catch { }
        }

        public void SendChatMessage(string message)
        {
            Datas.WriteText(client.GetStream(),"m"+ message);
        }
        public void SendGameStatus() { }
        public void SendGameStep(string step)
        {
            Datas.WriteText(client.GetStream(), "s" + step);
        }
    }
}
