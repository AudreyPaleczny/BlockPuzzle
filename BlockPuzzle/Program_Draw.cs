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
            if (str == null) {
                return;
            }
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

        public void printBoardArea()
        {
            Console.SetCursorPosition(0, 0);
            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    char ch = board[r][c];
                    if (ch == boardCharacter)
                    {
                        Console.ForegroundColor = backgroundColor;
                    }
                    else
                    {
                        Console.ForegroundColor = (ConsoleColor)ch;
                    }
                    ch = placedCharacter;
                    Console.Write(ch);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                sout("\n");
            }
        }

        // stop it from rotating or smth
       
        public void Draw()
        {
            //before drawing something specific, change color
            //ConsoleColor original = Console.ForegroundColor;
            Console.SetCursorPosition(0, 0);

            printBoardArea();
            p1.printHoldArea(this);
            p1.printQArea(this);
            p1.printQPieces(this);

            if (p1.shadow != null)
            {
                Console.ForegroundColor = shadowColor;
                PrintPiece(p1.shadowPos, p1.shadow);
            }

            if (players ==2)
            {
                p2.printHoldArea(this);
                p2.printQArea(this);
                p2.printQPieces(this);

                if (p2.shadow != null)
                {
                    Console.ForegroundColor = shadowColor;
                    PrintPiece(p2.shadowPos, p2.shadow);
                }
            }
            
            Console.ForegroundColor = p1.queue[0].color;
            PrintPiece(p1.piecePosition, p1.currentPiece);
            if(p2.queue.Count > 0)
            {
                Console.ForegroundColor = p2.queue[0].color;
                PrintPiece(p2.piecePosition, p2.currentPiece);
            }
            
            Console.BackgroundColor = backgroundColor;
            Console.SetCursorPosition(0, height + 1);
            Console.ForegroundColor = placedColor;
            Console.Write("score: " + score);

            //for (int i = 0; i < 16; i++)
            //{
            //    Console.ForegroundColor = (ConsoleColor)i;
            //    Console.Write("#####");
            //    Console.ForegroundColor = ConsoleColor.Black;
            //    Console.WriteLine(((ConsoleColor)i).ToString());
            //}

            //int i = 10;
            //Console.ForegroundColor = (ConsoleColor)i;
            //Console.Write("#####");
            //Console.ForegroundColor = (ConsoleColor)i - 8;
            //Console.Write("#####");
            //Console.ForegroundColor = ConsoleColor.Black;


        }
        
    }
}