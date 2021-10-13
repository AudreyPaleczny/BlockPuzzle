using System;

//using r for row, c for column
namespace BlockPuzzle
{

    public class Piece
    {
        public Coord size = Coord.ZERO;
        //public string data;
        public char[,] map;

        public Piece(Coord s, string d)
        {
            size = s;
            map = new char[Height, Width];
            //data = d;

            for(int r = 0; r < Height; ++r)
            {
                for(int c = 0; c < Width; ++c)
                {
                    map[r, c] = d[r * Width + c];
                }
            }
        }

        public void FlipHorizontal()
        {
            for (int r = 0; r < size.y; r++)
            {
                for (int i = 0; i < size.x / 2; i++)
                {
                    char temp = map[r, i];
                    map[r, i] = map[r, size.x - i - 1];
                    map[r, size.x - i - 1] = temp;

                }
            }

        }

        public void FlipVertical()
        {
            for (int r = 0; r < size.y / 2; r++)
            {
                for (int c = 0; c < size.x; c++)
                {
                    char temp = map[size.y - r - 1, c];
                    map[size.y - r - 1, c] = map[r, c];
                    map[r, c] = temp;

                }
            }
        }

        public void FlipDiagonal()
        {
            //over y = -x
            Coord newSize = new Coord(size.y, size.x);
            char[,] newMap = new char[newSize.y, newSize.x];

            for(int r = 0; r < newSize.y; r++)
            {
                //newSize.y is for the number of rows
                for(int c = 0; c < newSize.x; c++)
                {
                    //newSize.x is for the number of cols
                    newMap[r, c] = map[c, r];
                }
            }

            size = newSize;
            map = newMap;

        }

        public void RotateCW()
        {
            FlipDiagonal();
            FlipHorizontal();
        }

        public void RotateCCW()
        {
            FlipHorizontal();
            FlipDiagonal();
        }

        public int Width
        {
            get
            {
                return size.x;
            }
            set
            {
                size.x = value;
            }
        }

        public int Height
        {
            get
            {
                return size.y;
            }
            set
            {
                size.y = value;
            }
        }

        public char this[int r, int c]
        {
            get
            {
                //return data[r * Width + c];
                return map[r, c];
            }
        }
    }

    class MainClass
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

        public void PrintPiece(Coord p, Piece tPiece)
        {
            for(int r = 0; r < tPiece.Height; r++)
            {
                for(int c = 0; c < tPiece.Width; c++)
                {
                    int x = p.x + c, y = p.y + r;

                    //oob stands for out of bounds
                    bool oob = x < 0 || y < 0 || x >= width || y >= height;
                    if (!oob && tPiece[r, c] != ' ')
                    {
                        //out of bounds or space, do not print
                        Console.SetCursorPosition(x, y);
                        Console.Write(tPiece[r, c]);
                    }
                }
            }
        }

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

        public void Update()
        {
            Coord oldPiecePos = new Coord(piecePosition.x, piecePosition.y);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:        piecePosition.y--;      break;

                case ConsoleKey.DownArrow:      piecePosition.y++;      break;

                case ConsoleKey.LeftArrow:      piecePosition.x--;      break;

                case ConsoleKey.RightArrow:     piecePosition.x++;      break;

                case ConsoleKey.Spacebar:       imprintPiece();         break;

                case ConsoleKey.H:              tPiece.FlipHorizontal();break;

                case ConsoleKey.V:              tPiece.FlipVertical();  break;

                case ConsoleKey.B:              tPiece.FlipDiagonal();  break;

                case ConsoleKey.O:              tPiece.RotateCCW();     break;

                case ConsoleKey.P:              tPiece.RotateCW();      break;

                default:                                                break;
            }

            if (isPieceOOB() || isPieceCollidingWithBoard())
            {
                //prevent rotating out of bounds (use isPieceOOB as reference)
                piecePosition.x = oldPiecePos.x;
                piecePosition.y = oldPiecePos.y;
            }

            AABB p = new AABB
            {
                position = piecePosition,
                size = tPiece.size
            };

            Console.SetCursorPosition(0, height);

            //if (p.overlap(test))
            if(!isPieceOOB() && isPieceCollidingWithBoard())
            {
                Console.Write("Overlap");
            }
            else
            {
                Console.Write("...........");
            }

        }

        public bool isPieceCollidingWithBoard()
        {
            for (int r = 0; r < tPiece.Height; r++)
            {
                for (int c = 0; c < tPiece.Width; c++)
                {
                    if (board[piecePosition.y + r][piecePosition.x + c] != ',' && tPiece.map[0 + r, 0 + c] == '#')
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

        public void Init()
        {
            board = new char[height][];

            for (int i = 0; i < height; i++)
            {
                board[i] = new char[width];
                for (int k = 0; k < width; k++)
                {
                    board[i][k] = ',';
                }
            }

            board[3][4] = 'X';
            board[3][5] = 'X';
            board[4][4] = 'X';
            board[4][5] = 'X';
        }

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
                    //piecePosition.y++;
                    fallCounter -= 1000;
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
