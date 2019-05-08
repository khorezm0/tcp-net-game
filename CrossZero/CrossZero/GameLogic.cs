using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossZero
{
    public class GameLogic
    {
        public int[,] field = new int[3, 3];

        public int Step(int x, int y, int peer)
        {
            field[x, y] = peer;
            return CheckWinner();
        }

        public int CheckWinner()
        {
            int win;
            for(int i = 0;i<3;i++)
            {
                win = CheckHor(i);
                if (win > 0) return win;

                win = CheckVert(i);
                if (win > 0) return win;
            }

            win = CheckDiag(0,1);
            if (win > 0) return win;

            win = CheckDiag(2,-1);
            if (win > 0) return win;

            return Draft();
        }

        public int CheckHor(int y)
        {
            int z = field[0, y];
            for(int x = 1; x<3; x++)
            {
                if (z != field[x, y]) return 0;

            }
            return z;
        }
        public int CheckVert(int x)
        {
            int z = field[x, 0];
            for (int y = 1; y < 3; y++)
            {
                if (z != field[x, y]) return 0;
            }
            return z;
        }
        public int CheckDiag(int y, int sig)
        {
            int z = field[0, y];
            for (int x = 0; x < 3 && y < 3 && y >= 0; y += sig,x++)
            {
                if (z != field[x, y]) return 0;
            }
            return z;
        }
        public int Draft()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (field[i, j] == 0) return 0;
            return -1;
        }
    }
}
