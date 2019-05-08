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
        public delegate void ReceiveWinner(bool win);

        public event ReceiveMessage onReceiveMessage;
        public event ReceiveMessage onReceiveStep;
        public event ReceiveWinner onReceiveWinner;

        public bool IsServer { get; private set; }
        public bool MyTurn { get; private set; }

        GameLogic logic = new GameLogic();
        TcpClient client;

        const char separator = '\0';

        public Peer(TcpClient client, bool isServer)
        {
            this.client = client;
            IsServer = isServer;
            MyTurn = isServer;
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
                    string[] comms = str.Split(separator);
                    foreach(var c in comms)
                    {
                        if (c.StartsWith("m"))
                            onReceiveMessage.Invoke(c.Substring(1));//is not null
                        else if (c.StartsWith("s"))
                        {
                            MyTurn = !MyTurn;
                            onReceiveStep.Invoke(c.Substring(1));
                        }
                        else if (c.StartsWith("w"))
                        {
                            onReceiveWinner.Invoke(false);
                            MyTurn = false;
                        }
                    }
                }
            }
            catch { }
        }

        public void SendChatMessage(string message)
        {
            message = message.Replace(separator, ' ');
            Datas.WriteText(client.GetStream(),"m"+ message + separator);
        }

        public void SendGameStatus() { }

        public void SendGameStep(string step)
        {
            int me = IsServer ? 1 : 2;
            int pos = parseInt(step) - 1;
            int win = logic.Step(pos % 3, pos / 3, me);

            MyTurn = !MyTurn;
            Datas.WriteText(client.GetStream(), "s" + step + separator);
            if (win == me)
            {
                client.GetStream().Flush();
                Datas.WriteText(client.GetStream(), "w" + separator);
                onReceiveWinner.Invoke(true);
                MyTurn = false;
            }
        }

        int parseInt(string str)
        {
            int i = 0;
            int.TryParse(str, out i);
            return i;
        }
    }
}
