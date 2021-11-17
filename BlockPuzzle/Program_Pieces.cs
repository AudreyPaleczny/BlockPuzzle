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

    }


}