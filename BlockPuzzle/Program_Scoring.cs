using System;
namespace BlockPuzzle
{
    public partial class MainClass
    {
        int score = 0;
        public void clearLines()
        {
            int count = 0, clearRow, rowCount = 0;
            for (int row = 0; row < height; ++row)
            {
                for (int col = 0; col < width; ++col)
                {
                    if (board[row][col] != boardCharacter)
                    {
                        ++count;
                    }

                }
                
                if (count == width)
                {
                    // Nina: probably unecessary 
                    clearRow = row;
                    for (int col = 0; col < width; ++col)
                    {
                        board[row][col] = boardCharacter;
                    }
                    // for everyline under top to shift down by 1 row
                    for (int shiftRow = clearRow; shiftRow > 0; --shiftRow)
                    {
                        for (int shiftCol = 0; shiftCol < width; ++shiftCol)
                        {
                            board[shiftRow][shiftCol] = board[shiftRow - 1][shiftCol];
                        }
                    }
                    
                    rowCount++;
                }
                count = 0;

            }
            if (rowCount == 1)
            {
                score += 100;
            }
            else if (rowCount == 2)
            {
                score += 400;
            }
            else if (rowCount == 3)
            {
                score += 900;
            }
            else if (rowCount == 4)
            {
                score += 1600;
            }
        }
    }
}
