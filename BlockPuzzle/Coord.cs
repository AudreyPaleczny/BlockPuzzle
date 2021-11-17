using System;
namespace BlockPuzzle
{
    public struct Coord
    {
        public int x, y;
        public static int numOfDimensions = 2;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Coord(Coord copy)
        {
            this.x = copy.x;
            this.y = copy.y;
        }

        public static readonly Coord ZERO = new Coord(0, 0);
    }
}
