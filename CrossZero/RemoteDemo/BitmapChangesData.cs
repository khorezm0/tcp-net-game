using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDemo
{
    [System.Serializable]
    public class BitmapChangesData
    {
        [System.Serializable]
        public class Chunk
        {
            public int X;
            public int Y;
            public int color;

            public Chunk(int x, int y, int colr)
            {
                X = x;
                Y = y;
                color = colr;
            }

        }

        public Chunk[] Chunks {get;set;}
        public int Length {
            get {
                if(Chunks != null)
                    return Chunks.Length;
                return 0;
            }
        }

        public BitmapChangesData() { }
        public BitmapChangesData(Chunk[] chunks)
        {
            Chunks = chunks;
        }

        public void ApplyChanges(Bitmap bmp)
        {
            foreach(Chunk c in Chunks)
            {
                if (bmp.Width > c.X && bmp.Height > c.Y) bmp.SetPixel(c.X, c.Y, Color.FromArgb(c.color));
            }
        }

        public static BitmapChangesData Parse(string base64)
        {
            if (base64 != null)
            {
                BinaryFormatter binaryReader = new BinaryFormatter();
                byte[] form = Convert.FromBase64String(base64);
                using (MemoryStream mem = new MemoryStream(form))
                {
                    return (BitmapChangesData)binaryReader.Deserialize(mem);
                }
            }

            return new BitmapChangesData();
        }

        public string GetBase64()
        {
            string str = "";
            if (Chunks != null && Length > 0)
            {
                byte[] bytes;
                BinaryFormatter binaryReader = new BinaryFormatter();
                using (MemoryStream mem = new MemoryStream())
                {
                    binaryReader.Serialize(mem, this);
                    bytes = mem.ToArray();
                }
                return Convert.ToBase64String(bytes);
            }
            return str;
        }

        public static BitmapChangesData GetChanges(Bitmap bmp1, Bitmap bmp2)
        {
            LinkedList<Chunk> chunks = new LinkedList<Chunk>();
            if(bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height)
            { 
                return new BitmapChangesData();
            }

            for (int i =0;i< bmp1.Width; i++)
            {
                for (int j = 0; j < bmp1.Height; j++)
                {
                    var newcol = bmp2.GetPixel(i, j);
                    if (bmp1.GetPixel(i, j) != newcol)
                        chunks.AddLast(new Chunk(i, j, newcol.ToArgb()));
                }
            }

            return new BitmapChangesData(chunks.ToArray());
        }

    }
}
