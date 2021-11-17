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
                    p1.softDrop(this);
                },
                [ConsoleKey.LeftArrow] = () =>
                {
                    p1.piecePosition.x--;
                },
                [ConsoleKey.RightArrow] = () =>
                {
                    p1.piecePosition.x++;
                },
                [ConsoleKey.Spacebar] = () =>
                {
                    p1.hardDrop(this);
                },
                [ConsoleKey.Z] = () =>
                {
                    p1.currentPiece.RotateCCW();
                },
                [ConsoleKey.UpArrow] = () =>
                {
                    p1.currentPiece.RotateCW();
                },
                [ConsoleKey.A] = () =>
                {
                    p1.currentPiece.RotateCCW();
                    p1.currentPiece.RotateCCW();
                },
                [ConsoleKey.C] = () =>
                {
                    p1.swapHold(this);
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
                    p1.softDrop(this);
                },
                [ConsoleKey.A] = () =>
                {
                    p1.piecePosition.x--;
                },
                [ConsoleKey.D] = () =>
                {
                    p1.piecePosition.x++;
                },
                [ConsoleKey.Spacebar] = () =>
                {
                    p1.hardDrop(this);
                },
                [ConsoleKey.Q] = () =>
                {
                    p1.currentPiece.RotateCCW();
                },
                [ConsoleKey.E] = () =>
                {
                    p1.currentPiece.RotateCW();
                },
                [ConsoleKey.F] = () =>
                {
                    p1.currentPiece.RotateCCW();
                    p1.currentPiece.RotateCCW();
                },
                [ConsoleKey.C] = () =>
                {
                    p1.swapHold(this);
                }, // first player above, second player below
                [ConsoleKey.DownArrow] = () =>
                {
                    p2.softDrop(this);
                },
                [ConsoleKey.LeftArrow] = () =>
                {
                    p2.piecePosition.x--;
                },
                [ConsoleKey.RightArrow] = () =>
                {
                    p2.piecePosition.x++;
                },
                [ConsoleKey.OemPeriod] = () =>
                {
                    p2.hardDrop(this);
                },
                [ConsoleKey.K] = () =>
                {
                    p2.currentPiece.RotateCCW();
                },
                [ConsoleKey.L] = () =>
                {
                    p2.currentPiece.RotateCW();
                },
                [ConsoleKey.O] = () =>
                {
                    p2.currentPiece.RotateCCW();
                    p2.currentPiece.RotateCCW();
                },
                [ConsoleKey.OemComma] = () =>
                {
                    p2.swapHold(this);
                }
            };
            return c;
        }
        Dictionary<ConsoleKey, Action> Controls;
    }
}