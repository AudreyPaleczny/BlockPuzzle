using System;
using System.Collections.Generic;

namespace BlockPuzzle
{
    public partial class MainClass
    {
        public Piece[] listOfPieces = { zPiece, sPiece, jPiece, lPiece, oPiece, iPiece, tPiece };
        public String[] namesOfPieces = {"zPiece", "sPiece", "jPiece", "lPiece", "oPiece", "iPiece", "tPiece"};

        static Piece zPiece = new Piece(new Coord(3, 2), "##  ##", ConsoleColor.Red);
        static Piece sPiece = new Piece(new Coord(3, 2), " #### ", ConsoleColor.Green);
        static Piece jPiece = new Piece(new Coord(3, 2), "###  #", ConsoleColor.DarkBlue);
        static Piece lPiece = new Piece(new Coord(3, 2), "####  ", ConsoleColor.DarkYellow);
        static Piece oPiece = new Piece(new Coord(2, 2), "####", ConsoleColor.Yellow);
        static Piece iPiece = new Piece(new Coord(4, 1), "####", ConsoleColor.Cyan);
        static Piece tPiece = new Piece(new Coord(3, 2), " # ###", ConsoleColor.Magenta);

        // CURRENT PIECE
        // choosing a random piece
        public Piece currentPiece = null;
        public Piece currentPiece2 = null;
        RandP randomGenerator = new RandP(7);

        // this can be ignored if we want both players to have the same random pieces
        RandP randomGenerator2 = new RandP(7);
        int counter = 0;
        int counter2 = 0;
        int numberInQ = 5;

        public List<Piece> queue = new List<Piece>(5);
        public List<Piece> queue2 = new List<Piece>(5);

        // make sure first piece of queue isn't the one on board
        public void initQ()
        {
            queue.Add(generatePiece(1));
            queue.Add(generatePiece(1));
            queue.Add(generatePiece(1));
            queue.Add(generatePiece(1));
            queue.Add(generatePiece(1));
            queue.Add(generatePiece(1));
        }
        public void initQ2()
        {
            queue2.Add(generatePiece(2));
            queue2.Add(generatePiece(2));
            queue2.Add(generatePiece(2));
            queue2.Add(generatePiece(2));
            queue2.Add(generatePiece(2));
            queue2.Add(generatePiece(2));
        }

        public void updateQ(int p)
        {
            if (p == 1)
            {
                queue[5] = generatePiece(1);
            } else
            {
                queue2[5] = generatePiece(2);
            }
        }

        public Piece generatePiece(int p)
        {
            if (p == 1)
            {
                if (counter == 7)
                {
                    randomGenerator = new RandP(7);
                    counter = 0;
                }
                counter++;

                return listOfPieces[randomGenerator.nextInt()];
            } else
            {
                if (counter2 == 7)
                {
                    randomGenerator2 = new RandP(7);
                    counter2 = 0;
                }
                counter2++;

                return listOfPieces[randomGenerator2.nextInt()];
            }
            
        }

        public void choosePiece(int p)
        {
            if (p == 1)
            {
                for (int i = 0; i < numberInQ; i++)
                {
                    queue[i] = queue[i + 1];
                }

                updateQ(1);
                currentPiece = queue[0].clone();
            } else
            {
                for (int i = 0; i < numberInQ; i++)
                {
                    queue2[i] = queue2[i + 1];
                }

                updateQ(2);
                currentPiece2 = queue2[0].clone();
            }
        }

        // PIECE IN HOLD
        Piece holdPiece = null;
        Piece holdPiece2 = null;

        public bool canhold;
        public bool canhold2;

        public void swapHold(int p)
        {
            if (p == 1)
            {
                if (canhold)
                {
                    if (holdPiece == null)
                    {
                        holdPiece = queue[0];
                        choosePiece(1);
                    }
                    else
                    {
                        Piece temp = holdPiece;
                        holdPiece = queue[0];
                        currentPiece = temp;
                    }
                }
                canhold = false;
                piecePosition = new Coord(4, 0);
            } else
            {
                if (canhold2)
                {
                    if (holdPiece2 == null)
                    {
                        holdPiece2 = queue2[0];
                        choosePiece(2);
                    }
                    else
                    {
                        Piece temp = holdPiece2;
                        holdPiece2 = queue2[0];
                        currentPiece2 = temp;
                    }
                }
                canhold2 = false;
                piecePosition = new Coord(4, 0);
            }
            
        }

        /*
        public void placePiece()
        {
            for (int row = 0; row < currentPiece.Height; ++row)
            {
                for (int col = 0; col < currentPiece.Width; ++col)
                {
                    int x = piecePosition.x + col, y = piecePosition.y + row;
                    bool oob = x < 0 || x >= width || y < 0 || y >= height;
                    if (!oob && currentPiece[row, col] != ' ')
                    {
                        board[piecePosition.y + row][piecePosition.x + col] = currentPiece[row, col];
                    }
                }
            }
        }
        */

    }


}