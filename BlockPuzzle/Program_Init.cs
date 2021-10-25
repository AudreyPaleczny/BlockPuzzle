using System;

namespace BlockPuzzle
{
    public partial class MainClass
    {
        char boardCharacter = ',';
        char pieceCharacter = '#';
        char placedCharacter = 'X';
        char shadowCharacter = '/';

        public void Init()
        {
            board = new char[height][];

            for (int i = 0; i < height; i++)
            {
                board[i] = new char[width];
                for (int k = 0; k < width; k++)
                {
                    board[i][k] = boardCharacter;
                }
            }

            // init hold area
            holdArea = new char[4][];
            for (int i = 0; i < 4; i++)
            {
                holdArea[i] = new char[4];
                for (int j = 0; j < 4; j++)
                {
                    holdArea[i][j] = background;
                }
            }

            // init queue area
            qArea = new char[16][];
            for (int i = 0; i < 16; i++)
            {
                qArea[i] = new char[4];
                for (int j = 0; j < 4; j++)
                {
                    qArea[i][j] = background;
                }
            }

            // init the q
            initQ();

            canhold = true;

            choosePiece();
        }
    }
}