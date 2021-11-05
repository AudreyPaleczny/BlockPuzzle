using System;
using System.Collections.Generic;

namespace BlockPuzzle
{
    public partial class MainClass
    {
        Dictionary<ConsoleKey, Action> SinglePlayerControls()
        {
            Dictionary<ConsoleKey, Action> c = new Dictionary<ConsoleKey, Action>()
            {
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
        Dictionary<ConsoleKey, Action> Controls;
    }
}