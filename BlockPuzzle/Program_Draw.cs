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

        public void printHoldArea(int p)
        {
            if (p == 1)
            {
                for (int row = 0; row < 5; row++)
                {
                    for (int col = 0; col < 5; col++)
                    {
                        Console.Write(holdArea[row][col]);
                    }
                    Console.SetCursorPosition(12, row + 4);
                }
            } else
            {
                for (int row = 0; row < 5; row++)
                {
                    for (int col = 0; col < 5; col++)
                    {
                        Console.Write(holdArea2[row][col]);
                    }
                    Console.SetCursorPosition(32, row + 4);
                }
            }
            

            if (!(holdPiece == null))
            {
                // revert orientation -----------------------------------------------------------------------------------------------------------------------------------------------------
                printPieceOutside(holdCoordinate, holdPiece);
            }
        }

        public void printQArea(int p)
        {
            if (p == 1)
            {
                for (int row = 0; row < 21; row++)
                {
                    Console.SetCursorPosition(20, row);
                    for (int col = 0; col < 5; col++)
                    {
                        Console.Write(qArea[row][col]);
                    }
                }
            } else
            {
                for (int row = 0; row < 21; row++)
                {
                    Console.SetCursorPosition(40, row);
                    for (int col = 0; col < 5; col++)
                    {
                        Console.Write(qArea2[row][col]);
                    }
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
                    //ch = (char)('A' + (int)ch);
                    Console.Write(ch);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                sout("\n");
            }
        }

        // stop it from rotating or smth
        public void printQPieces(int p)
        {
            if (p == 1)
            {
                for (int i = 0; i < numberInQ; i++)
                {
                    initialQCoordinate.y = 1 + i * 4;
                    printPieceOutside(initialQCoordinate, queue[i + 1]);
                }
                initialQCoordinate = new Coord(20, 1);
            } else
            {
                for (int i = 0; i < numberInQ; i++)
                {
                    initialQCoordinate2.y = 1 + i * 4;
                    printPieceOutside(initialQCoordinate2, queue2[i + 1]);
                }
                initialQCoordinate2 = new Coord(40, 1);
            }
        }

        public void Draw()
        {
            //before drawing something specific, change color
            //ConsoleColor original = Console.ForegroundColor;
            Console.SetCursorPosition(0, 0);

            printBoardArea();
            printHoldArea(1);
            printQArea(1);
            printQPieces(1);

            if (shadow != null)
            {
                Console.ForegroundColor = shadowColor;
                PrintPiece(shadowPos, shadow);
            }

            if (players ==2)
            {
                printHoldArea(2);
                printQArea(2);
                printQPieces(2);

                if (shadow2 != null)
                {
                    Console.ForegroundColor = shadowColor;
                    PrintPiece(shadowPos2, shadow2);
                }
            }


            // for testing nina's queue
            
            Console.ForegroundColor = queue[0].color;
            PrintPiece(piecePosition, currentPiece);
            if(queue2.Count > 0)
            {
                Console.ForegroundColor = queue2[0].color;
                PrintPiece(piecePosition2, currentPiece2);
            }
            
            Console.BackgroundColor = backgroundColor;
            Console.SetCursorPosition(0, height + 1);
            Console.ForegroundColor = placedColor;
            Console.Write(score);

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