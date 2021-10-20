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

            //board[3][4] = 'X';
            //board[3][5] = 'X';
            //board[4][4] = 'X';
            //board[4][5] = 'X';
        }
    }
}