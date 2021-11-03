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
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
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

        public void printQArea()
        {
            Console.SetCursorPosition(20, 0);
            for (int row = 0; row < 21; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    Console.Write(qArea[row][col]);
                }
                Console.SetCursorPosition(20, row);
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
                    if(ch == boardCharacter)
                    {
                        Console.ForegroundColor = backgroundColor;
                    }
                    Console.Write(ch);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                sout("\n");
            }
        }

        // stop it from rotating or smth
        public void printQPieces()
        {
            for (int i = 0; i < numberInQ; i++)
            {
                initialQCoordinate.y = 1 + i * 4;
                printPieceOutside(initialQCoordinate, queue[i+1]);
            }
            initialQCoordinate = new Coord(20, 1);
        }

        public void Draw()
        {
            //before drawing something specific, change color
            //ConsoleColor original = Console.ForegroundColor;
            Console.SetCursorPosition(0, 0);

            // for testing nina's queue
            /*
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
            */

            printBoardArea();
            printHoldArea();
            printQArea();
            printQPieces();

            if (shadow != null)
            {
                Console.ForegroundColor = shadowColor;
                PrintPiece(shadowPos, shadow);
            }

            // for testing nina's queue
            
            Console.ForegroundColor = queue[0].color;
            //BUG: WHEN I TOUCH ANY END OF THE BOARD IT LOSES ITS COLOR (turns black)
            //ONLY HAPPENS WHEN YOU MOVE THE PIECE SO THAT IT WOULD BE OOB
            PrintPiece(piecePosition, currentPiece);
            // Window's command line is black so i can't see that's why im commenting this out sorry
            // the following line makes sure nina can see things
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