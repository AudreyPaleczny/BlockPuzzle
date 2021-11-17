using System;
using System.Collections.Generic;

namespace BlockPuzzle
{
    public partial class MainClass
    {
        public void Update()
        {
            p1.oldPiecePos = p1.piecePosition;
            p1.oldPiece = p1.currentPiece.clone();
            if (players == 2) {
                p2.oldPiecePos = p2.piecePosition;
                p2.oldPiece = p2.currentPiece.clone();
            }

            // this is the controls dictionary 
            if (Controls.TryGetValue(key.Key, out Action thingToDo))
            {
                thingToDo();
            }
            

            // pretty switch statement :)
            // you will live on in our hearts and memories :( -evan
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

            if(p1.justPlacedPiece || p2.justPlacedPiece)
            {
                ClearLines();
                p1.justPlacedPiece = false;
                p2.justPlacedPiece = false;
            }
            p1.updateShadow(this);
            p1.placePieceDownWhatever(this);
            if (players == 2) {
                p2.updateShadow(this);
                p2.placePieceDownWhatever(this);
            }

        }
    }
}