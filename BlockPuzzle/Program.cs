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

        public static void FlipHorizontal(int[] nums)
        {
            int length = nums.Length;
            
            for(int i = 0; i < length/2; i++)
            {
                int temp = nums[i];
                nums[i] = nums[length - i - 1];
                nums[length - i - 1] = temp;

            }

        }

        public void RestartPiece()
        {
            piecePosition.y = 0;
            piecePosition.x = 4;
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

        public void imprintPiece()
        {
            for (int r = 0; r < tPiece.Height; ++r)
            {
                for (int c = 0; c < tPiece.Width; c++)
                {
                    int x = piecePosition.x + c, y = piecePosition.y + r;
                    bool oob = x < 0 || y < 0 || x >= width || y >= height;
                    if (!oob && tPiece[r, c] != ' ')
                    {
                        board[y][x] = tPiece[r, c];
                    }
                }
            }
        }

        public bool isPieceOOB()
        {
            return piecePosition.x < 0 || piecePosition.y < 0 || piecePosition.x +
                tPiece.Width > width || piecePosition.y + tPiece.Height > height;
        }

        public bool isPieceCollidingWithBoard(Coord pos)
        {
            for (int r = 0; r < tPiece.Height; r++)
            {
                for (int c = 0; c < tPiece.Width; c++)
                {
                    if (board[pos.y + r][pos.x + c] != ',' && tPiece.map[0 + r, 0 + c] == '#')
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

        //these are auto-private vars
        //String tPiece =
        //        " # " +
        //        "###";
        //Coord pieceSize = new Coord(3, 2);
        Coord piecePosition = Coord.ZERO;
        //Piece tPiece = new Piece(new Coord(3, 2), " # ###");
        //Piece tPiece = new Piece(new Coord(3, 2), " #### ");
        Piece tPiece = new Piece(new Coord(3, 2), "###  #");
        private Piece shadow;
        Coord shadowPos;

        ConsoleKeyInfo key = new ConsoleKeyInfo();


        public static void Main(string[] args)
        {
            //int[] nums = new int[] { 1, 2, 3, 4 };
            //FlipHorizontal(nums);
            //for(int i = 0; i < nums.Length; i++)
            //{
            //    Console.Write(nums[i] + " ");
            //}
            //Console.ReadKey();
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

            while (key.Key != ConsoleKey.Escape)
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
                int bottomOfPiece = piecePosition.y + tPiece.Height;
                if (bottomOfPiece >= 20 || isPieceCollidingWithBoard(piecePosition))
                {
                    imprintPiece();
                    RestartPiece();
                }

                Console.SetCursorPosition(0, height + 1);
                Console.Write(fallCounter);
                GetUserInput();
                Update();
                //background++;

            }

        }
    }
}
