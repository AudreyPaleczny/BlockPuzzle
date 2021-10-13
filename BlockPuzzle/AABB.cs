using System;
namespace BlockPuzzle
{
    public class AABB
    {
        public Coord position, size;
        public int Top => position.y;
        public int Bottom => position.y + size.y;
        public int Left => position.x;
        public int Right => position.x + size.x;


        // Mr. V explain struct vs class
        public bool overlap(AABB other)
        {
            // check overlap of both x and y values
            bool tooHigh = this.Bottom <= other.Top;
            bool tooLow = this.Top >= other.Bottom;
            bool tooLeft = this.Right <= other.Left;
            bool tooRight = this.Left >= other.Right;

            bool overlapping = (!(tooHigh || tooLow || tooLeft || tooRight));
            if (!overlapping)
            {
                return false;
            }
            return true;
        }

        public bool contains(AABB other)
        {
            bool topContained = (other.Top >= this.Top);
            bool bottomContained = (other.Bottom <= this.Bottom);
            bool leftContained = (other.Left >= this.Left);
            bool rightContained = (other.Right <= this.Right);


            bool contained = topContained && bottomContained && leftContained && rightContained;
            if (contained)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// axis aligned bounding box
        /// pos and size
        /// </summary>
        public AABB()
        {
        }
    }
}
