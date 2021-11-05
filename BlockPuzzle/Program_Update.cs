using System;
using System.Collections.Generic;

namespace BlockPuzzle
{
    public partial class MainClass
    {

        public void hardDrop(int p)
        {
            if (p == 1)
            {
                score += dropCalculation(piecePosition, 1).y - piecePosition.y;
                piecePosition = new Coord(dropCalculation(piecePosition, 1));
                imprintPiece(placedCharacter, 1);
                RestartPiece(1);
            } else
            {
                score += dropCalculation(piecePosition2, 2).y - piecePosition2.y;
                piecePosition2 = new Coord(dropCalculation(piecePosition2, 2));
                imprintPiece(placedCharacter, 2);
                RestartPiece(2);
            }
        }

        public Coord dropCalculation(Coord oldPos, int p)
        {
            Coord endPos = new Coord(oldPos);
            Piece cp = currentPiece;
            if (p == 2)
            {
                cp = currentPiece2;
            }
            while (!isPieceCollidingWithBoard(endPos))
            {
                ++endPos.y;
                if (endPos.y + cp.Height > height)
                {
                    break;
                }
            }
            endPos.y--;
            return endPos;
        }

        public void softDrop(int p)
        {
            if (p == 1)
            {
                piecePosition.y++;
                ++score;
            } else
            {
                piecePosition2.y++;
                ++score;
            }
        }
        public void Update()
        {
            Coord oldPiecePos = new Coord(piecePosition.x, piecePosition.y);
            Coord oldPiecePos2 = new Coord(piecePosition2.x, piecePosition2.y);
            Piece oldPiece = currentPiece.clone();
            Piece oldPiece2 = currentPiece2 != null ? currentPiece2.clone() : null;

            if (Controls.TryGetValue(key.Key, out Action thingToDo))
            {
                thingToDo();
            }
            

            // pretty switch statement :)
            /* 
             * switch (key.Key)
            {
                //case ConsoleKey.UpArrow:    piecePosition.y--;          break;

                //case ConsoleKey.DownArrow:  softDrop(1);                   break;

                //case ConsoleKey.LeftArrow:  piecePosition.x--;            break;

                //case ConsoleKey.RightArrow: piecePosition.x++;            break;

                // case ConsoleKey.Backspace:   hardDrop(oldPiecePos, 1);        break;


                //case ConsoleKey.H:        currentPiece.FlipHorizontal();break;

                //case ConsoleKey.V:        currentPiece.FlipVertical();  break;

                //case ConsoleKey.B:        currentPiece.FlipDiagonal();  break;

                // case ConsoleKey.Z:          currentPiece.RotateCCW();     break;

                // case ConsoleKey.X:          currentPiece.RotateCW();      break;
                    
                case ConsoleKey.C:          swapHold(2);                   break;


                case ConsoleKey.I:          currentPiece.RotateCCW();      break;

                case ConsoleKey.O:          currentPiece.RotateCW();       break;

                // case ConsoleKey.OemComma:   swapHold(1); break;
            }
            */

            if (isPieceOOB(1) || isPieceCollidingWithBoard(piecePosition))
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

            if (currentPiece2 != null && (isPieceOOB(2) || isPieceCollidingWithBoard(piecePosition2)))
            {
                // move back to old position
                piecePosition2.x = oldPiecePos2.x;
                piecePosition2.y = oldPiecePos2.y;
                // rotate back to old rotation i think
                currentPiece2 = oldPiece2;
                if (key.Key == ConsoleKey.S)
                {
                    --score;

                }
            }

            /*
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
                Console.Write("Not Overlap");
            }
            */

            clearLines();

            shadow = currentPiece.clone().changeChars(pieceCharacter, shadowCharacter);
            shadowPos = new Coord(dropCalculation(piecePosition,1));
            if (currentPiece2 != null)
            {
                shadow2 = currentPiece2.clone().changeChars(pieceCharacter, shadowCharacter);
                shadowPos2 = new Coord(dropCalculation(piecePosition2,2));
            }

        }
    }
}