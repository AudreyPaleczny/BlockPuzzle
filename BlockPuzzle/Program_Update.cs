using System;

namespace BlockPuzzle
{
    public partial class MainClass
    {

        public void hardDrop(Coord oldPosition)
        {
            score += dropCalculation(piecePosition).y - piecePosition.y;
            piecePosition = new Coord(dropCalculation(piecePosition));
            imprintPiece(placedCharacter);
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

        public void softDrop()
        {
            piecePosition.y++;
            ++score;
        }

        public void Update()
        {
            Coord oldPiecePos = new Coord(piecePosition.x, piecePosition.y);
            Piece oldPiece = currentPiece.clone();

            // pretty switch statement :)
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:    piecePosition.y--;            break;

                case ConsoleKey.DownArrow:  softDrop();                   break;

                case ConsoleKey.LeftArrow:  piecePosition.x--;            break;

                case ConsoleKey.RightArrow: piecePosition.x++;            break;

                case ConsoleKey.Spacebar:   hardDrop(oldPiecePos);        break;

                case ConsoleKey.H:          currentPiece.FlipHorizontal();break;

                case ConsoleKey.V:          currentPiece.FlipVertical();  break;

                case ConsoleKey.B:          currentPiece.FlipDiagonal();  break;

                case ConsoleKey.O:          currentPiece.RotateCCW();     break;

                case ConsoleKey.P:          currentPiece.RotateCW();      break;
                    
                case ConsoleKey.C:          swapHold();                   break;
            }

            if (isPieceOOB() || isPieceCollidingWithBoard(piecePosition))
            {
                // move back to old position
                piecePosition.x = oldPiecePos.x;
                piecePosition.y = oldPiecePos.y;
                // rotate back to old rotation i think
                currentPiece = oldPiece;
                if(key.Key == ConsoleKey.DownArrow)
                {
                --score;

                }
            }

            AABB p = new AABB
            {
                position = piecePosition,
                size = currentPiece.size
            };

            /* Console.SetCursorPosition(0, height);

            //if (p.overlap(test))
            if (!isPieceOOB() && isPieceCollidingWithBoard(piecePosition))
            {
                Console.Write("Overlap");
            }
            else
            {
                Console.Write("Not Overlap");
            }
            */

            clearLines();

            shadow = currentPiece.clone().changeChars(pieceCharacter, shadowCharacter);
            shadowPos = new Coord(dropCalculation(piecePosition));
            
        }
    }
}