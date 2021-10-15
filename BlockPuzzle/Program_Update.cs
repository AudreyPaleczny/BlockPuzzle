using System;

namespace BlockPuzzle
{
    public partial class MainClass
    {
        public void Update()
        {
            Coord oldPiecePos = new Coord(piecePosition.x, piecePosition.y);
            Piece oldPiece = tPiece.clone();

            // pretty switch statement :)
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:    piecePosition.y--;      break;

                case ConsoleKey.DownArrow:  piecePosition.y++;      break;

                case ConsoleKey.LeftArrow:  piecePosition.x--;      break;

                case ConsoleKey.RightArrow: piecePosition.x++;      break;

                case ConsoleKey.Spacebar:   imprintPiece();         break;

                case ConsoleKey.H:          tPiece.FlipHorizontal();break;

                case ConsoleKey.V:          tPiece.FlipVertical();  break;

                case ConsoleKey.B:          tPiece.FlipDiagonal();  break;

                case ConsoleKey.O:          tPiece.RotateCCW();     break;

                case ConsoleKey.P:          tPiece.RotateCW();      break;

                default: break;
            }

            if (isPieceOOB() || isPieceCollidingWithBoard())
            {
                //prevent rotating out of bounds (use isPieceOOB as reference)
                //do the opposite rotation
                piecePosition.x = oldPiecePos.x;
                piecePosition.y = oldPiecePos.y;

                //switch (key.Key)
                //{
                //    case ConsoleKey.O: tPiece.RotateCW();   break;
                //    case ConsoleKey.P: tPiece.RotateCCW();  break;
                //}

                tPiece = oldPiece;
            }

            AABB p = new AABB
            {
                position = piecePosition,
                size = tPiece.size
            };

            Console.SetCursorPosition(0, height);

            //if (p.overlap(test))
            if (!isPieceOOB() && isPieceCollidingWithBoard())
            {
                Console.Write("Overlap");
            }
            else
            {
                Console.Write("...........");
            }

        }
    }
}