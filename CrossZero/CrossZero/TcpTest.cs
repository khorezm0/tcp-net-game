using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CrossZero
{
    public class TcpTest
    {
        public static void Main(string[] str)
        {
            TcpClient client;
            try
            {
                TcpListener serv = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777));
                serv.Start();
                client = serv.AcceptTcpClient();

                Console.WriteLine("Client connected");

                var stream = client.GetStream();
                

                var txt = Encoding.UTF8.GetBytes("Hello client");
                stream.Write(txt, 0, txt.Length);
            }
            catch(Exception ex)
            {
                client = new TcpClient("127.0.0.1", 7777);
                Console.WriteLine("Connected to server");
                var stream = client.GetStream();

                var txt = Encoding.UTF8.GetBytes("Hello server");
                //stream.Write(txt, 0, txt.Length);

                var haveMessage = false;
                while (!haveMessage)
                {
                    if (stream.DataAvailable)
                    {
                        haveMessage = true;
                        Console.WriteLine(ReadText(stream));
                    }
                }
            }
            Console.ReadLine();
        }

        static string ReadText(NetworkStream stream)
        {
            var str = "";
            var buffer = new byte[64];
            while (stream.DataAvailable)
            {
                int len = stream.Read(buffer, 0, buffer.Length);
                if (len == 0) break;
                str += Encoding.UTF8.GetString(buffer, 0, len);
            }
            return str;
        }
    }
}
