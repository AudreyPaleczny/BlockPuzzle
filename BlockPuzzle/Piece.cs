using System;

namespace BlockPuzzle
{
    public class Piece
    {
        public Coord size = Coord.ZERO;
        //public string data;
        public char[,] map;
        public ConsoleColor color;

        public Piece(Coord s, string d, ConsoleColor color)
        {
            size = s;
            map = new char[Height, Width];
            //data = d;
            this.color = color;
            if (d == null) return;

            for (int r = 0; r < Height; ++r)
            {
                for (int c = 0; c < Width; ++c)
                {
                    map[r, c] = d[r * Width + c];
                }
            }

            
        }

        public Piece clone()
        {
            Piece p = new Piece(this.size, null, this.color);
            p.map = new char[Height, Width];

            for (int r = 0; r < size.y; r++)
            {
                for(int c = 0; c < size.x; c++)
                {
                    p.map[r, c] = this.map[r, c];
                }
            }

            return p;
        }

        public Piece changeChars(char target, char replace)
        {
            //change targets to replaces
            for(int r = 0; r < Height; r++)
            {
                for(int c = 0; c < Width; c++)
                {
                    if(map[r, c] == target)
                    {
                        map[r, c] = replace;
                    }
                }
            }
            return this;
        }

        public void FlipHorizontal()
        {
            for (int r = 0; r < size.y; r++)
            {
                for (int i = 0; i < size.x / 2; i++)
                {
                    char temp = map[r, i];
                    map[r, i] = map[r, size.x - i - 1];
                    map[r, size.x - i - 1] = temp;

                }
            }

        }

        public void FlipVertical()
        {
            for (int r = 0; r < size.y / 2; r++)
            {
                for (int c = 0; c < size.x; c++)
                {
                    char temp = map[size.y - r - 1, c];
                    map[size.y - r - 1, c] = map[r, c];
                    map[r, c] = temp;

                }
            }
        }

        public void FlipDiagonal()
        {
            //over y = -x
            Coord newSize = new Coord(size.y, size.x);
            char[,] newMap = new char[newSize.y, newSize.x];

            for (int r = 0; r < newSize.y; r++)
            {
                //newSize.y is for the number of rows
                for (int c = 0; c < newSize.x; c++)
                {
                    //newSize.x is for the number of cols
                    newMap[r, c] = map[c, r];
                }
            }

            size = newSize;
            map = newMap;

        }

        public void RotateCW()
        {
            FlipDiagonal();
            FlipHorizontal();
        }

        public void RotateCCW()
        {
            FlipHorizontal();
            FlipDiagonal();
        }

        public int Width
        {
            get
            {
                return size.x;
            }
            set
            {
                size.x = value;
            }
        }

        public int Height
        {
            get
            {
                return size.y;
            }
            set
            {
                size.y = value;
            }
        }

        public char this[int r, int c]
        {
            get
            {
                //return data[r * Width + c];
                return map[r, c];
            }
        }
    }
}