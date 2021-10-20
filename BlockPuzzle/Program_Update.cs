using System;

namespace BlockPuzzle
{
    public partial class MainClass
    {

        public void hardDrop(Coord oldPosition)
        {
            //while (!isPieceCollidingWithBoard(piecePosition))
            //{
            //    oldPosition.y = piecePosition.y;
            //    oldPosition.x = piecePosition.x;
            //    ++piecePosition.y;
            //    if (piecePosition.y + tPiece.Height > height)
            //    {
            //        break;
            //    }

            //}
            //piecePosition.y = oldPosition.y;
            //piecePosition.x = oldPosition.x;

            piecePosition = new Coord(dropCalculation(piecePosition));

            tPiece.changeChars('#', 'X');
            imprintPiece();
            tPiece.changeChars('X', '#');
            RestartPiece();
        }

        public Coord dropCalculation(Coord oldPos)
        {
            Coord endPos = new Coord(oldPos);

            while (!isPieceCollidingWithBoard(endPos))
            {
                ++endPos.y;
                if (endPos.y + tPiece.Height > height)
                {
                    break;
                }

            }

            endPos.y--;
            return endPos;
        }

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

                case ConsoleKey.Spacebar:   hardDrop(oldPiecePos);  break;

                case ConsoleKey.H:          tPiece.FlipHorizontal();break;

                case ConsoleKey.V:          tPiece.FlipVertical();  break;

                case ConsoleKey.B:          tPiece.FlipDiagonal();  break;

                case ConsoleKey.O:          tPiece.RotateCCW();     break;

                case ConsoleKey.P:          tPiece.RotateCW();      break;

                default: break;
            }

            if (isPieceOOB() || isPieceCollidingWithBoard(piecePosition))
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
            if (!isPieceOOB() && isPieceCollidingWithBoard(piecePosition))
            {
                Console.Write("Overlap");
            }
            else
            {
                Console.Write("...........");
            }
            clearLines();

            shadow = tPiece.clone().changeChars('#', '/');
            shadowPos = new Coord(dropCalculation(piecePosition));
        }
    }
}