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

        public char background = '.';
        public int height = 20;
        public int width = 10;
        public long fallCounter = 0;
        public char[][] board;
        public bool gameOver = false;
        public ConsoleKeyInfo key = new ConsoleKeyInfo();
        Player p1 = new Player(new Coord(12,5), new Coord(20,1), new Coord(2, 0), new Coord (1,0));
        Player p2 = new Player(new Coord(27,5), new Coord(35,1), new Coord(6, 0), new Coord (5,0));

        public static void Main(string[] args)
        {
            MainClass m = new MainClass();
            m.Init();
            m.Run();
        }

        //universal time in miliseconds
        public static long UTCMS()
        {
            return System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        }

        public void fallCounterUpdate()
        {
            double iterationDelay = ((11 - level) * 0.1)*1000;  // [seconds] used to be 0.05

            if (fallCounter >= iterationDelay)
            {
                p1.piecePosition.y++;
                if (players == 2)
                {
                    p2.piecePosition.y++;
                }
                fallCounter -= (long)iterationDelay;
            }
            Console.SetCursorPosition(0, height + 2);
            Console.Write("level: " + level);

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
                Console.SetCursorPosition(0, height + 3);

                ChangeLevel();
                fallCounterUpdate();

                // this is fixing the soft drop bug i was stupid and i deleted it earlier - Nina
                p1.idkhowthisisdifferentbutitsoksothisissoftdropplacedownthinginprogramdotcs(this);
                p2.idkhowthisisdifferentbutitsoksothisissoftdropplacedownthinginprogramdotcs(this);

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
