using System;
namespace BlockPuzzle
{
    public partial class MainClass
    {
        public void startGame()
        {
            Console.Write("B O L K   P U Z E L e");
            Console.SetCursorPosition(0, 1);
            Console.Write("---------------------");
            Console.SetCursorPosition(0, 3);
            Console.Write("press [any key] to start the game");
            Console.SetCursorPosition(0, 5);
            Console.Write("press [Q] to exit");
            key = Console.ReadKey();

            if(key.Key == ConsoleKey.Q)
            {
                Environment.Exit(0);
            }

            Console.Clear();
        }
    }

}
