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

        public void RestartPiece(int p)
        {
            if (p == 1)
            {
                piecePosition.y = 0;
                piecePosition.x = 2;
                choosePiece(1);
                if (isPieceCollidingWithBoard(piecePosition,1))
                {
                    gameOver = true;
                }
                canhold = true;
            } else
            {
                piecePosition2.y = 0;
                piecePosition2.x = 6;
                choosePiece(2);
                if (isPieceCollidingWithBoard(piecePosition2,2))
                {
                    gameOver = true;
                }
                canhold = true;
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

        public void imprintPiece(char replace, int p)
        {
            Piece cp = currentPiece;
            Coord pp = piecePosition;
            if (p == 2) { 
                cp = currentPiece2;
                pp = piecePosition2;
            }
            cp.changeChars(pieceCharacter, replace);
            for (int r = 0; r < cp.Height; ++r)
            {
                for (int c = 0; c < cp.Width; c++)
                {
                    int x = pp.x + c, y = pp.y + r;
                    bool oob = x < 0 || y < 0 || x >= width || y >= height;
                    if (!oob && cp[r, c] != ' ')
                    {
                        board[y][x] = cp[r, c];
                    }
                }
            }
            cp.changeChars(replace, pieceCharacter);
        }

        public bool isPieceOOB(int p)
        {
            Coord pp = piecePosition;
            Piece cp = currentPiece;
            if (p == 2)
            {
                pp = piecePosition2;
                cp = currentPiece2;
            }
            return pp.x < 0 || pp.y < 0 || pp.x +
                cp.Width > width || pp.y + cp.Height > height;
        }

        public bool isPieceCollidingWithBoard(Coord pos, int p)
        {
            Piece cp = currentPiece;
            if (p == 2) {
                cp = currentPiece2;
            }
            for (int r = 0; r < cp.Height; r++)
            {
                for (int c = 0; c < cp.Width; c++)
                {
                    if (board[pos.y + r][pos.x + c] != boardCharacter && cp.map[0 + r, 0 + c] == pieceCharacter)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    

        private char background = ':';
        private int height = 20;
        private int width = 10;
        private long fallCounter = 0;
        private bool gameOver = false;

        private char[][] board;
        private char[][] board2;

        char[][] holdArea;
        char[][] holdArea2;
        Coord holdCoordinate = new Coord(12, 5);
        Coord holdCoordinate2 = new Coord(25, 5);
        char[][] qArea;
        char[][] qArea2;

        Coord initialQCoordinate = new Coord(20, 1);
        Coord initialQCoordinate2 = new Coord(35, 1);

        Coord piecePosition = Coord.ZERO;
        Coord piecePosition2 = Coord.ZERO;
        private Piece shadow;
        private Piece shadow2;
        Coord shadowPos;
        Coord shadowPos2;

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

        public void fallCounterUpdate()
        {
            if (fallCounter >= 1000)
            {
                piecePosition.y++;
                if (players == 2)
                {
                    piecePosition2.y++;
                }
                fallCounter -= 1000;
            }
        }

        public void Run()
        {
            //int then = System.Environment.TickCount;

            long then = UTCMS();
            int bottomOfPiece;
            int bottomOfPiece2;

            while (key.Key != ConsoleKey.Escape && gameOver != true)
            {
                Draw();
                long now = UTCMS();
                //time passed
                long passed = now - then;
                then = now;
                fallCounter += passed;

                fallCounterUpdate();
                
                bottomOfPiece = piecePosition.y + currentPiece.Height;
                if (bottomOfPiece > height || isPieceCollidingWithBoard(piecePosition,1))
                {
                    piecePosition.y--;
                    imprintPiece((char)currentPiece.color, 1); 
                    RestartPiece(1);
                }

                if (players ==2)
                {
                    bottomOfPiece2 = piecePosition2.y + currentPiece2.Height;
                    if (bottomOfPiece2 > height || isPieceCollidingWithBoard(piecePosition2,2))
                    {
                        piecePosition2.y--;
                        imprintPiece((char)currentPiece2.color, 2);
                        RestartPiece(2);
                    }
                }
                
                GetUserInput();
                Update();
            }
            Console.Clear();
            Console.Write("GAME OVER");
            Console.SetCursorPosition(0, 1);
            Console.Write("---------");
            Console.SetCursorPosition(0, 3);
            Console.Write("your score was: " + score);
            Console.SetCursorPosition(0, 5);
            Console.Write("PRESS ESCAPE TO LEAVE");
            do
            {
                Console.SetCursorPosition(0, 6);
                key = Console.ReadKey();
            }
            while (key.Key != ConsoleKey.Escape);
        }
    }
}
