using System;
using System.Collections.Generic;

namespace BlockPuzzle {
    public class Player {
        public List<Piece> queue = new List<Piece>(5);
        public Coord holdCoordinate;
        public Coord initialQCoordinate;
        public Coord restartPos;
        public Coord piecePosition = Coord.ZERO;
        public Piece holdPiece;
        public Piece shadow;
        public Coord shadowPos;
        public bool canhold = true;
        public Piece currentPiece = null;
        RandP randomGenerator = null;
        public int counter = 0;
        public Coord oldPiecePos;
        public Piece oldPiece;
        public int bottomOfPiece;
        public int numberInQ = 5;
        public Random r;

        public Player(Coord hc, Coord ic, Coord rp) {
            holdCoordinate = hc;
            initialQCoordinate = ic;
            restartPos = rp;
        }

        public void printHoldArea(MainClass Game)
        {
            for (int row = 0; row < 5; row++)
            {
                Console.SetCursorPosition(holdCoordinate.x, row + 4);
                for (int col = 0; col < 5; col++)
                {
                    Console.Write(Game.background);
                }
            }         
            if (holdPiece != null)
            {
                Game.printPieceOutside(holdCoordinate, holdPiece);
            }
        }
        public void printQArea(MainClass Game)
        {
            for (int row = 0; row < 21; row++)
            {
                Console.SetCursorPosition(initialQCoordinate.x, row);
                for (int col = 0; col < 5; col++)
                {
                    Console.Write(Game.background);
                }
            }
        }
         public void printQPieces(MainClass Game)
        {
            Coord temp = initialQCoordinate;
            for (int i = 0; i < numberInQ; i++)
            {
                initialQCoordinate.y = 1 + i * 4;
                Game.printPieceOutside(initialQCoordinate, queue[i + 1]);
            }
            initialQCoordinate = temp;
            
        }

        public void initQ(MainClass Game, int p)
        {
            r = new Random(p);
            randomGenerator = new RandP(7, r);
            queue.Add(generatePiece(Game));
            queue.Add(generatePiece(Game));
            queue.Add(generatePiece(Game));
            queue.Add(generatePiece(Game));
            queue.Add(generatePiece(Game));
            queue.Add(generatePiece(Game));
        }
          
        public void updateQ(MainClass Game)
        {
            queue[5] = generatePiece(Game);
        }  

        public Piece generatePiece(MainClass Game)
        {
            if (counter == 7)
            {
                randomGenerator = new RandP(7, r);
                counter = 0;
            }
            counter++;

            return Game.listOfPieces[randomGenerator.nextInt()];
        }

        public void choosePiece(MainClass Game)
        {
            for (int i = 0; i < numberInQ; i++)
            {
                queue[i] = queue[i + 1];
            }

            updateQ(Game);
            currentPiece = queue[0].clone();
        }
        public void swapHold(MainClass Game)
        {
            if (canhold)
            {
                if (holdPiece == null)
                {
                    holdPiece = queue[0];
                    choosePiece(Game);
                }
                else
                {
                    Piece temp = holdPiece;
                    holdPiece = queue[0];
                    currentPiece = temp;
                }

                canhold = false;
                piecePosition = restartPos;
            }
            
        }

        public void hardDrop(MainClass Game)
        {
            Game.score += dropCalculation(Game).y - piecePosition.y;
            piecePosition = new Coord(dropCalculation(Game));
            imprintPiece((char)currentPiece.color, Game);
            RestartPiece(Game);
        }

        public Coord dropCalculation(MainClass Game)
        {
            Coord endPos = piecePosition;
            while (!isPieceCollidingWithBoardAtSpecificPos(endPos, Game))
            {
                ++endPos.y;
                if (endPos.y + currentPiece.Height > Game.height)
                {
                    break;
                }
            }
            endPos.y--;
            return endPos;
        }

        public void softDrop(MainClass Game)
        {
            piecePosition.y++;
            ++Game.score;
        }

        public void updateShadow(MainClass Game) {
            shadow = currentPiece.clone().changeChars(Game.pieceCharacter, Game.shadowCharacter);
            shadowPos = new Coord(dropCalculation(Game));
        }

        public void placePieceDownWhatever(MainClass Game) {
            if (isPieceOOB(Game) || isPieceCollidingWithBoardAtSpecificPos(piecePosition, Game))
            {
                // move back to old position
                piecePosition.x = oldPiecePos.x;
                piecePosition.y = oldPiecePos.y;
                // rotate back to old rotation i think
                currentPiece = oldPiece;
                if(Game.key.Key == ConsoleKey.DownArrow)
                {
                --Game.score;
                }
            }
        }
        public void fallCounterUpdate(MainClass Game)
        {
            piecePosition.y++;
            Game.fallCounter -= 1000;
        }


        public void idkhowthisisdifferentbutitsoksothisissoftdropplacedownthinginprogramdotcs(MainClass Game) {
            bottomOfPiece = piecePosition.y + currentPiece.Height;
            if (bottomOfPiece > Game.height || isPieceCollidingWithBoardAtSpecificPos(piecePosition, Game))
            {
                piecePosition.y--;
                imprintPiece((char)currentPiece.color, Game); 
                RestartPiece(Game);
            }
        }

        public void RestartPiece(MainClass Game)
        {
            piecePosition = restartPos;
            choosePiece(Game);
            if (isPieceCollidingWithBoardAtSpecificPos(piecePosition, Game))
            {
                Game.gameOver = true;
            }
            canhold = true;
        }

        public bool isPieceCollidingWithBoardAtSpecificPos(Coord pos, MainClass Game)
        {
            if (isPieceOOB(Game)) return true;
            for (int r = 0; r < currentPiece.Height; r++)
            {
                for (int c = 0; c < currentPiece.Width; c++)
                {
                    if (Game.board[pos.y + r][pos.x + c] != Game.boardCharacter && currentPiece.map[0 + r, 0 + c] == Game.pieceCharacter)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool isPieceOOB(MainClass Game)
        {
            return piecePosition.x < 0 || piecePosition.y < 0 || piecePosition.x +
                currentPiece.Width > Game.width || piecePosition.y + currentPiece.Height > Game.height;
        }

         public void imprintPiece(char replace, MainClass Game)
        {
            currentPiece.changeChars(Game.pieceCharacter, replace);
            for (int r = 0; r < currentPiece.Height; ++r)
            {
                for (int c = 0; c < currentPiece.Width; c++)
                {
                    int x = piecePosition.x + c, y = piecePosition.y + r;
                    bool oob = x < 0 || y < 0 || x >= Game.width || y >= Game.height;
                    if (!oob && currentPiece[r, c] != ' ')
                    {
                        Game.board[y][x] = currentPiece[r, c];
                    }
                }
            }
            currentPiece.changeChars(replace, Game.pieceCharacter);
        }
        
    }
}
