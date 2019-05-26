using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Drawing;

namespace NetBase
{
    public class Datas
    {
        public static string ReadText(NetworkStream stream)
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

        public static void WriteText(NetworkStream stream, string text)
        {
            var txt = Encoding.UTF8.GetBytes(text);
            stream.Write(txt, 0, txt.Length);
        }

        public static string BitmapToBase64(    Bitmap bmp)
        {
            byte[] byteImage = null;
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byteImage = ms.ToArray();
            }
            return Convert.ToBase64String(byteImage);
        }

        public static Bitmap Base64ToBitmap(string str)
        {
            byte[] bytes = Convert.FromBase64String(str);
            Bitmap bmp = null;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }

    }
}
