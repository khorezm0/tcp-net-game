using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace NetBase
{
    public class AsyncTcpServer
    {
        public delegate void OnPeerConnected(Peer peer);
        public delegate void OnTcpError(string text);

        public IPEndPoint EndPoint { get; private set; }
        public List<Peer> Clients { get; private set; }
        public event OnPeerConnected onPeerConnected;
        public event OnTcpError onTcpError;

        public bool Listening { get; private set; }

        TcpListener listener;

        public AsyncTcpServer(IPEndPoint endPoint) {
            EndPoint = endPoint;
            listener = new TcpListener(endPoint);
            Clients = new List<Peer>();
        }

        public async void Listen()
        {
            Listening = true;
            listener.Start();
            while (Listening)
            {
                try
                {
                    var Client = new Peer(await listener.AcceptTcpClientAsync(), true);
                    Clients.Add(Client);
                    Client.ListenData();
                    Client.onSocketClose += ()=>{
                        Clients.Remove(Client);
                        if (onTcpError != null) onTcpError("Disconnected!");
                    };
                    onPeerConnected.Invoke(Client);
                }
                catch(Exception ex)
                {
                    if(onTcpError != null) onTcpError.Invoke(ex.ToString());
                }
            }

        }

        public void ForEachClient(Action<Peer> action)
        {
            foreach (var p in Clients) action(p);
        }

    }
}
