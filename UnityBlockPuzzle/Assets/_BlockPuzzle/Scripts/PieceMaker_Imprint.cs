using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piece
{
    public partial class PieceMaker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns> whether the piece can be imprinted as is </returns>
        public bool ImprintPiece()
        {
            if (currentPiece == null || isColliding()) return false;
            bool success = true;
            Vector2Int[] MinosPos = minoCoords();

            checkIfGameOver();

            while (currentPiece.transform.childCount > 0)
            {
                Transform mino = currentPiece.transform.GetChild(0);
                if (!mino.GetComponent<Light>())
                {
                    int boardXPos = (int)Mathf.Round(mino.position.x - 0.5f), boardYPos = (int)Mathf.Round((mino.position.y - 3.5f) * -1);
                    if (isMinoOOB(boardXPos, boardYPos)) success = false;
                    else board.objectMatrix[boardYPos][boardXPos] = mino.gameObject;
                }

                mino.SetParent(board.transform);
            }
            
            Destroy(currentPiece.gameObject);
            Destroy(currentGhostPiece.gameObject);
            canPieceBeHeld = true;
            makeAnotherOne();
            dirtyGhost = true;
            return success;
        }
    }

}