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
                imprintPiece((char)currentPiece.color, 1);
                RestartPiece(1);
            } else
            {
                score += dropCalculation(piecePosition2, 2).y - piecePosition2.y;
                piecePosition2 = new Coord(dropCalculation(piecePosition2, 2));
                imprintPiece((char)currentPiece2.color, 2);
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
            while (!isPieceCollidingWithBoard(endPos,p))
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

        Coord oldPiecePos;
        Coord oldPiecePos2;
        Piece oldPiece;
        Piece oldPiece2;

        public void Update()
        {
            oldPiecePos = new Coord(piecePosition.x, piecePosition.y);
            oldPiecePos2 = new Coord(piecePosition2.x, piecePosition2.y);
            oldPiece = currentPiece.clone();
            oldPiece2 = currentPiece2 != null ? currentPiece2.clone() : null;

            // this is the controls dictionary 
            if (Controls.TryGetValue(key.Key, out Action thingToDo))
            {
                thingToDo();
            }

            if (isPieceOOB(1) || isPieceCollidingWithBoard(piecePosition,1))
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

            if (currentPiece2 != null && (isPieceOOB(2) || isPieceCollidingWithBoard(piecePosition2,2)))
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