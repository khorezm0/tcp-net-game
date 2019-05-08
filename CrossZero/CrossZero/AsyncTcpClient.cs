using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CrossZero
{
    public class AsyncTcpClient
    {
        public delegate void OnPeerConnected(Peer peer);

        public IPEndPoint EndPoint { get; private set; }
        public Peer Client { get; private set; }
        public OnPeerConnected onPeerConnected { get; set; }
        TcpClient tcp;

        public AsyncTcpClient(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            tcp = new TcpClient();
        }

        public async void Connect()
        {
            await tcp.ConnectAsync(EndPoint.Address, EndPoint.Port);
            Client = new Peer(tcp);
            Client.ListenData();
            onPeerConnected?.Invoke(Client);
        }
    }
}
