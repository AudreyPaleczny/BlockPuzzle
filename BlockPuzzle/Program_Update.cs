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

            currentPiece.changeChars('#', 'X');
            imprintPiece();
            currentPiece.changeChars('X', '#');
            RestartPiece();
        }

        public Coord dropCalculation(Coord oldPos)
        {
            Coord endPos = new Coord(oldPos);

            while (!isPieceCollidingWithBoard(endPos))
            {
                ++endPos.y;
                if (endPos.y + currentPiece.Height > height)
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
            Piece oldPiece = currentPiece.clone();

            // pretty switch statement :)
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:    piecePosition.y--;      break;

                case ConsoleKey.DownArrow:  piecePosition.y++;      break;

                case ConsoleKey.LeftArrow:  piecePosition.x--;      break;

                case ConsoleKey.RightArrow: piecePosition.x++;      break;

                case ConsoleKey.Spacebar:   hardDrop(oldPiecePos);  break;

                case ConsoleKey.H:          currentPiece.FlipHorizontal();break;

                case ConsoleKey.V:          currentPiece.FlipVertical();  break;

                case ConsoleKey.B:          currentPiece.FlipDiagonal();  break;

                case ConsoleKey.O:          currentPiece.RotateCCW();     break;

                case ConsoleKey.P:          currentPiece.RotateCW();      break;

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

                currentPiece = oldPiece;
            }

            AABB p = new AABB
            {
                position = piecePosition,
                size = currentPiece.size
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

            shadow = currentPiece.clone().changeChars('#', '/');
            shadowPos = new Coord(dropCalculation(piecePosition));
        }
    }
}