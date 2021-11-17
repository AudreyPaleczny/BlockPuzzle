using System;
namespace BlockPuzzle
{
    public partial class MainClass
    {
        public int score = 0;
        int linesCleared = 0;
        int maxLevel = 10;
        int level = 1;
        int comboCount = 0;
        int previousLinesCleared = 0;

        public void ChangeLevel()
        {
            if(level <= maxLevel)
            {
                level = (linesCleared / 10) + 1; // magic number here: 10 lines to go to next level, i assumed we wouldn't change that
            }
        }

        public void ClearLines()
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
            if(rowCount > 0)
            {
                if(previousLinesCleared > 0 && previousLinesCleared < 4) // combo
                {
                    comboCount++;
                    score += rowCount * rowCount * 100 * level;
                    score += 50 * comboCount * level;
                }
                else if (rowCount == 4 && previousLinesCleared == 4) // double tetris - special case because more points than regular combo
                {
                    comboCount++;
                    score += rowCount * rowCount * 100 * level * 3/2;
                }
                else // not a combo
                {
                    score += rowCount * rowCount * 100*level;
                    previousLinesCleared = rowCount;
                    comboCount++;
                }
            }
            else // no lines cleared
            {
                previousLinesCleared = 0;
                comboCount = 0;
            }

            linesCleared += rowCount;

        }
    }
}
