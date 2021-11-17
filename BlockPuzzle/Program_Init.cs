using System;

namespace BlockPuzzle
{
    public partial class MainClass
    {
        public char boardCharacter = ',';
        public char pieceCharacter = '#';
        public char placedCharacter = 'X';
        public char shadowCharacter = '/';

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

            // init the q
            p1.initQ(this, 1);
            p1.choosePiece(this);
            Controls = SinglePlayerControls();

            if (players == 2)
            {
                p2.initQ(this, 2);
                p2.choosePiece(this);
                Controls = MultiPlayerControls();
            }
        }
    }
}