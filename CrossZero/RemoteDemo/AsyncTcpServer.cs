using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
        Thread listenThread;
        bool threadsWorking;

        public AsyncTcpServer(IPEndPoint endPoint) {
            EndPoint = endPoint;
            listener = new TcpListener(endPoint);
            Clients = new List<Peer>();
        }

        public void Listen()
        {
            Listening = true;
            listenThread = new Thread(ListenTh);
            listenThread.Start();
        }

        public void Stop()
        {
            Listening = false;
            foreach (var p in Clients)
            {
                try
                {
                    p.Stop();
                }
                catch { }
            }
            listener.Stop();

            if (listenThread != null && listenThread.IsAlive)
                listenThread.Abort();
        }

        void ListenTh()
        {
            Listening = true;
            listener.Start();
            while (Listening)
            {
                Peer Client = null;
                try
                {
                    var tcp = listener.AcceptTcpClient();
                    Client = new Peer(tcp, true);
                    Clients.Add(Client);
                    Client.ListenData();
                    Client.onSocketClose += () => {
                        Clients.Remove(Client);
                        if (onTcpError != null) onTcpError("Disconnected!");
                    };
                    onPeerConnected.Invoke(Client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    if (onTcpError != null)
                    {
                        onTcpError.Invoke(ex.ToString());
                    }
                    if (Client != null) Clients.Remove(Client);
                }
            }
        }

        public void ForEachClient(Action<Peer> action)
        {
            foreach (var p in Clients) action(p);
        }

    }
}
