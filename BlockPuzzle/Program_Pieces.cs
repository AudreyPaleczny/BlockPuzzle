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
        public Piece currentPiece = null; //new Piece(new Coord(3, 2), " # ###");
        RandP randomGenerator = new RandP(7);
        int counter = 0;
        int numberInQ = 5;

        public List<Piece> queue = new List<Piece>(5);

        // make sure first piece of queue isn't the one on board
        public void initQ()
        {
            queue.Add(generatePiece());
            queue.Add(generatePiece());
            queue.Add(generatePiece());
            queue.Add(generatePiece());
            queue.Add(generatePiece());
            queue.Add(generatePiece());
        }

        public void updateQ()
        {
            queue[5]=generatePiece();
        }

        public Piece generatePiece()
        {
            if (counter == 7)
            {
                randomGenerator = new RandP(7);
                counter = 0;
            }
            counter++;
            
            return listOfPieces[randomGenerator.nextInt()];
        }

        public void choosePiece()
        {
            for (int i = 0; i < numberInQ; i++)
            {
                queue[i] = queue[i+1];
            }

            updateQ();
            currentPiece = queue[0].clone();
        }

        // PIECE IN HOLD
        Piece holdPiece = null;

        public bool canhold;

        public void swapHold()
        {
            if (canhold) {
                if (holdPiece == null)
                {
                    holdPiece = queue[0];
                    choosePiece();
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