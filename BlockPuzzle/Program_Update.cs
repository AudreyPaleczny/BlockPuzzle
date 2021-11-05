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
                score += dropCalculation(piecePosition).y - piecePosition.y;
                piecePosition = new Coord(dropCalculation(piecePosition));
                imprintPiece(placedCharacter, 1);
                RestartPiece(1);
            } else
            {
                score += dropCalculation(piecePosition2).y - piecePosition2.y;
                piecePosition2 = new Coord(dropCalculation(piecePosition2));
                imprintPiece(placedCharacter, 2);
                RestartPiece(2);
            }
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

        Dictionary<ConsoleKey,Action> SinglePlayerControls()
        {
            Dictionary<ConsoleKey, Action> c = new Dictionary<ConsoleKey, Action>(){
                [ConsoleKey.DownArrow] = () =>
                {
                    softDrop(1);
                },
                [ConsoleKey.LeftArrow] = () =>
                {
                    piecePosition.x--;
                },
                [ConsoleKey.RightArrow] = () =>
                {
                    piecePosition.x++;
                },
                [ConsoleKey.Spacebar] = () =>
                {
                    hardDrop(1);
                },
                [ConsoleKey.Z] = () =>
                {
                    currentPiece.RotateCCW();
                },
                [ConsoleKey.UpArrow] = () =>
                {
                    currentPiece.RotateCW();
                },
                [ConsoleKey.A] = () =>
                {
                    currentPiece.RotateCCW();
                    currentPiece.RotateCCW();
                },
                [ConsoleKey.C] = () =>
                {
                    swapHold(1);
                }
            };
            return c;
        }

        Dictionary<ConsoleKey, Action> MultiPlayerControls()
        {
            Dictionary<ConsoleKey, Action> c = new Dictionary<ConsoleKey, Action>()
            {
                [ConsoleKey.S] = () =>
                {
                    softDrop(1);
                },
                [ConsoleKey.A] = () =>
                {
                    piecePosition.x--;
                },
                [ConsoleKey.D] = () =>
                {
                    piecePosition.x++;
                },
                [ConsoleKey.Spacebar] = () =>
                {
                    hardDrop(1);
                },
                [ConsoleKey.Q] = () =>
                {
                    currentPiece.RotateCCW();
                },
                [ConsoleKey.E] = () =>
                {
                    currentPiece.RotateCW();
                },
                [ConsoleKey.F] = () =>
                {
                    currentPiece.RotateCCW();
                    currentPiece.RotateCCW();
                },
                [ConsoleKey.C] = () =>
                {
                    swapHold(1);
                }, // first player above, second player below
                [ConsoleKey.DownArrow] = () =>
                {
                    softDrop(2);
                },
                [ConsoleKey.LeftArrow] = () =>
                {
                    piecePosition2.x--;
                },
                [ConsoleKey.RightArrow] = () =>
                {
                    piecePosition2.x++;
                },
                [ConsoleKey.OemPeriod] = () =>
                {
                    hardDrop(2);
                },
                [ConsoleKey.K] = () =>
                {
                    currentPiece2.RotateCCW();
                },
                [ConsoleKey.L] = () =>
                {
                    currentPiece2.RotateCW();
                },
                [ConsoleKey.O] = () =>
                {
                    currentPiece2.RotateCCW();
                    currentPiece2.RotateCCW();
                },
                [ConsoleKey.OemComma] = () =>
                {
                    swapHold(2);
                }
            };
            return c;
        }


        public void Update()
        {
            Coord oldPiecePos = new Coord(piecePosition.x, piecePosition.y);
            Coord oldPiecePos2 = new Coord(piecePosition2.x, piecePosition2.y);
            Piece oldPiece = currentPiece.clone();
            Piece oldPiece2 = currentPiece2 != null ? currentPiece2.clone() : null;

            if (players == 1)
            {
                Dictionary<ConsoleKey, Action> Controls = SinglePlayerControls();
                if (Controls.TryGetValue(key.Key, out Action thingToDo))
                {
                    thingToDo();
                }
            } else
            {
                Dictionary<ConsoleKey, Action> Controls = MultiPlayerControls();
                if (Controls.TryGetValue(key.Key, out Action thingToDo))
                {
                    thingToDo();
                }
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
            shadowPos = new Coord(dropCalculation(piecePosition));
            if (currentPiece2 != null)
            {
                shadow2 = currentPiece2.clone().changeChars(pieceCharacter, shadowCharacter);
                shadowPos2 = new Coord(dropCalculation(piecePosition2));
            }

        }
    }
}