using System;

namespace BlockPuzzle
{
    public partial class MainClass
    {
        public void PrintPiece(Coord p, Piece tPiece)
        {
            for (int r = 0; r < tPiece.Height; r++)
            {
                for (int c = 0; c < tPiece.Width; c++)
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

        public void printPieceOutside(Coord p, Piece str)
        {
            //int i = 0;

            for (int r = 0; r < str.Height; r++)
            {
                for (int c = 0; c < str.Width; c++)
                {
                    int x = p.x + c, y = p.y + r;

                    if (str[r, c] != ' ')
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(str[r, c]);
                    }
                    // i++;
                }
            }
        }

        public void printHoldArea()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    Console.Write(holdArea[row][col]);
                }
                Console.SetCursorPosition(12, row + 4);
            }

            if (!(holdPiece == null))
            {
                // revert orientation -----------------------------------------------------------------------------------------------------------------------------------------------------
                printPieceOutside(holdCoordinate, holdPiece);
            }
        }

    public void Draw()
        {
            //before drawing something specific, change color
            //ConsoleColor original = Console.ForegroundColor;
            Console.SetCursorPosition(0, 0);
            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    char ch = board[r][c];
                    //^^ is for a specific character on the board
                    //where we left off: if statement to change colors :)
                    Console.Write(ch);
                }
                sout("\n");
            }

            if (shadow != null)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                PrintPiece(shadowPos, shadow);
            }

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            PrintPiece(piecePosition, currentPiece);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, height + 2);
            Console.Write(score);

            //for(int i = 0; i < 16; i++)
            //{
            //    Console.ForegroundColor = (ConsoleColor)i;
            //    Console.Write("#####");
            //    Console.ForegroundColor = ConsoleColor.Black;
            //    Console.WriteLine(((ConsoleColor)i).ToString());
            //}
        }
        
    }
}