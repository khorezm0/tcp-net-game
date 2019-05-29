using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace NetBase
{
    public class AsyncTcpClient
    {
        public delegate void OnPeerConnected(Peer peer);
        public delegate void OnTcpError(string text);

        public IPEndPoint EndPoint { get; private set; }
        public Peer Client { get; private set; }
        public event OnPeerConnected onPeerConnected;
        public event OnTcpError onTcpError;
        TcpClient tcp;

        public AsyncTcpClient(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            tcp = new TcpClient();
        }

        public async void Connect()
        {
            try
            {
                await tcp.ConnectAsync(EndPoint.Address, EndPoint.Port);
                Client = new Peer(tcp,false);
                Client.ListenData();
                Client.onSocketClose += ()=>{onTcpError("Disconnected!");};
                onPeerConnected.Invoke(Client);
            }
            catch (Exception ex)
            {
                onTcpError.Invoke(ex.ToString());
                Console.WriteLine(ex);
            }
        }

        public void Disconnect()
        {
            if(tcp != null && tcp.Connected)
            {
                tcp.Close();
            }
        }

    }
}
