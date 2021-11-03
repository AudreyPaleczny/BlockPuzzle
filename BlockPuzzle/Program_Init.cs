using System;

namespace BlockPuzzle
{
    public partial class MainClass
    {
        char boardCharacter = ',';
        char pieceCharacter = '#';
        char placedCharacter = 'X';
        char shadowCharacter = '/';

        ConsoleColor backgroundColor = ConsoleColor.Gray;
        ConsoleColor shadowColor = ConsoleColor.DarkGray;
        ConsoleColor placedColor = ConsoleColor.Black;

        public void Init()
        {

            startGame();

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
            holdArea = new char[5][];
            for (int i = 0; i < 5; i++)
            {
                holdArea[i] = new char[5];
                for (int j = 0; j < 5; j++)
                {
                    holdArea[i][j] = background;
                }
            }

            // init queue area
            qArea = new char[21][];
            for (int i = 0; i < 21; i++)
            {
                qArea[i] = new char[5];
                for (int j = 0; j < 5; j++)
                {
                    qArea[i][j] = background;
                }
            }

            // init the q
            initQ();
            canhold = true;
            choosePiece(1);

            if (players == 2)
            {
                // init second player board
                board2 = new char[height][];

                for (int i = 0; i < height; i++)
                {
                    board2[i] = new char[width];
                    for (int k = 0; k < width; k++)
                    {
                        board2[i][k] = boardCharacter;
                    }
                }

                // init second player hold area
                holdArea2 = new char[5][];
                for (int i = 0; i < 5; i++)
                {
                    holdArea2[i] = new char[5];
                    for (int j = 0; j < 5; j++)
                    {
                        holdArea2[i][j] = background;
                    }
                }

                // init second player queue area
                qArea2 = new char[21][];
                for (int i = 0; i < 21; i++)
                {
                    qArea2[i] = new char[5];
                    for (int j = 0; j < 5; j++)
                    {
                        qArea2[i][j] = background;
                    }
                }

                initQ2();
                canhold2 = true;
                choosePiece(2);
            }
        }
    }
}