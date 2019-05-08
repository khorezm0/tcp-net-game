﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CrossZero
{
    public class AsyncTcpServer
    {
        public delegate void OnPeerConnected(Peer peer);
        public delegate void OnTcpError(string text);

        public IPEndPoint EndPoint { get; private set; }
        public Peer Client { get; private set; }
        public OnPeerConnected onPeerConnected { get; set; }
        public OnTcpError onTcpError { get; set; }
        TcpListener listener;

        public AsyncTcpServer(IPEndPoint endPoint) {
            EndPoint = endPoint;
            listener = new TcpListener(endPoint);
        }

        public async void Listen()
        {
            try
            {
                listener.Start();
                Client = new Peer(await listener.AcceptTcpClientAsync());
                Client.ListenData();
                onPeerConnected?.Invoke(Client);
            }
            catch(Exception ex)
            {
                onTcpError?.Invoke(ex.ToString());
            }


        }

    }
}
