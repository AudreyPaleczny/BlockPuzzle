using System;
namespace BlockPuzzle
{
    public class Coord
    {
        public int x, y;
        public static int numOfDimensions = 2;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static readonly Coord ZERO = new Coord(0, 0);
    }
}
