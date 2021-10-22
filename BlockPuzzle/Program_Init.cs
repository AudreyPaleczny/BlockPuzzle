using System;

namespace BlockPuzzle
{
    public partial class MainClass
    {
        public void Init()
        {
            choosePiece();
            board = new char[height][];

            for (int i = 0; i < height; i++)
            {
                board[i] = new char[width];
                for (int k = 0; k < width; k++)
                {
                    board[i][k] = ',';
                }
            }
        }
    }
}