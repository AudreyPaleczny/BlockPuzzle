using System;

namespace BlockPuzzle
{
    public partial class MainClass
    {
        public void Draw()
        {
            Console.SetCursorPosition(0, 0);
            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    Console.Write(board[r][c]);
                }
                sout("\n");
            }

            PrintPiece(piecePosition, tPiece);
        }
    }
}