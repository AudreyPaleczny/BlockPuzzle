using System;
namespace BlockPuzzle
{
    public partial class MainClass
    {
        int players = 0;
        public void startGame()
        {
            Console.Write("B O L K   P U Z E L e");
            Console.SetCursorPosition(0, 1);
            Console.Write("---------------------");
            Console.SetCursorPosition(0, 3);
            Console.Write("press [1] to play single-player");
            Console.SetCursorPosition(0, 5);
            Console.Write("press [2] to play multi-player");
            Console.SetCursorPosition(0, 7);
            Console.Write("press [Q] to exit");

            

            while (players == 0)
            {
                Console.SetCursorPosition(0, 8);
                key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.D1: players = 1;        break;

                    case ConsoleKey.D2: players = 2;        break;

                    case ConsoleKey.Q: Environment.Exit(0); break;
                }
            }

            Console.Clear();
        }
    }

}
