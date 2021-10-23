using System;

//using r for row, c for column
namespace BlockPuzzle
{
    public partial class MainClass
    {
        public static void sout(String str)
        {
            Console.Write(str);
        }

        public void RestartPiece()
        {
            piecePosition.y = 0;
            piecePosition.x = 4;
            choosePiece();
            if (isPieceCollidingWithBoard(piecePosition))
            {
                gameOver = true;
            }
        }

        public void GetUserInput()
        {
            if (!Console.KeyAvailable)
            {
                key = new ConsoleKeyInfo();
                System.Threading.Thread.Sleep(50);
                return;
            }
            

            key = Console.ReadKey();
            //Console.WriteLine(key.Key);
        }

        public void imprintPiece(char replace)
        {
            currentPiece.changeChars(pieceCharacter, replace);
            for (int r = 0; r < currentPiece.Height; ++r)
            {
                for (int c = 0; c < currentPiece.Width; c++)
                {
                    int x = piecePosition.x + c, y = piecePosition.y + r;
                    bool oob = x < 0 || y < 0 || x >= width || y >= height;
                    if (!oob && currentPiece[r, c] != ' ')
                    {
                        board[y][x] = currentPiece[r, c];
                    }
                }
            }
            currentPiece.changeChars(replace, pieceCharacter);
        }

        public bool isPieceOOB()
        {
            return piecePosition.x < 0 || piecePosition.y < 0 || piecePosition.x +
                currentPiece.Width > width || piecePosition.y + currentPiece.Height > height;
        }

        public bool isPieceCollidingWithBoard(Coord pos)
        {
            for (int r = 0; r < currentPiece.Height; r++)
            {
                for (int c = 0; c < currentPiece.Width; c++)
                {
                    if (board[pos.y + r][pos.x + c] != boardCharacter && currentPiece.map[0 + r, 0 + c] == pieceCharacter)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    

        private char background = '.';
        private int height = 20;
        private int width = 10;
        private char[][] board;
        private long fallCounter = 0;
        private bool gameOver = false;

        char[][] holdArea;
        Coord holdCoordinate = new Coord(12, 5);

        char[][] qArea;

        Coord initialQCoordinate = new Coord(20, 1);

        Coord piecePosition = Coord.ZERO;
        private Piece shadow;
        Coord shadowPos;

        ConsoleKeyInfo key = new ConsoleKeyInfo();


        public static void Main(string[] args)
        {
            MainClass m = new MainClass();
            m.Init();
            m.Run();

        }

        AABB test = new AABB
        {
            position = new Coord(4, 3),
            size = new Coord(2, 2)
        };

        //universal time in miliseconds
        public static long UTCMS()
        {
            return System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        }

        public void Run()
        {
            //int then = System.Environment.TickCount;
            long then = UTCMS();

            while (key.Key != ConsoleKey.Escape && gameOver != true)
            {
                Draw();
                long now = UTCMS();
                //time passed
                long passed = now - then;
                then = now;
                fallCounter += passed;

                if(fallCounter >= 1000)
                {
                    piecePosition.y++;
                    fallCounter -= 1000;
                }
                int bottomOfPiece = piecePosition.y + currentPiece.Height;
                if (bottomOfPiece > height || isPieceCollidingWithBoard(piecePosition))
                {
                    //if (isPieceCollidingWithBoard(piecePosition)) piecePosition.y--;
                    piecePosition.y--;
                    imprintPiece(placedCharacter);
                    RestartPiece();
                }

                // Console.SetCursorPosition(0, height + 1);
                // Console.Write(fallCounter);
                GetUserInput();
                Update();
            }
            Console.Clear();
            Console.Write("GAME OVER");
            Console.SetCursorPosition(0, 1);
            Console.Write("lol u suck");
            Console.SetCursorPosition(0, 2);
            Console.Write("PRESS ESCAPE TO LEAVE");
            do
            {
                Console.SetCursorPosition(0, 3);
                key = Console.ReadKey();
            }
            while (key.Key != ConsoleKey.Escape);
        }
    }
}
