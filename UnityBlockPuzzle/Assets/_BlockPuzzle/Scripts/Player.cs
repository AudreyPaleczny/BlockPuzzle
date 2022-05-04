using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Piece
{
    public class Player : MonoBehaviour
    {
        public GameObject currentGhostPiece;
        public GameObject currentPiece;
        public bool canPieceBeHeld = true;
        public int howManyTimesPieceBeenHeld = 1;
        public GameObject holdPiece;
        public Vector3 holdPosition = new Vector3(-5, 0.5f, 4);

        public BlockQueue blockQueue;
        public List<GameObject> listOfObjects = new List<GameObject>();
        public Light[] pieceLight = new Light[4];
        public bool dirtyGhost = true;

        public Vector3 startingPos;

        public long fallCounter = 0;

        public Board board;

        Vector2Int[] minoCoords() => minoCoords(currentPiece.transform);
        public Vector2Int[] minoCoords(Transform pieceTransform)
        {
            Vector2Int[] coords = new Vector2Int[4];
            int index = 0;
            for (int i = 0; i < pieceTransform.childCount; ++i)
            {
                Transform mino = pieceTransform.GetChild(i);
                if (!mino.GetComponent<Light>())
                {
                    int boardXPos = (int)Mathf.Round(mino.position.x - 0.5f), boardYPos = (int)Mathf.Round((mino.position.y - 3.5f) * -1);
                    coords[index++] = new Vector2Int(boardXPos, boardYPos);
                }
            }
            return coords;
        }

        public int findBottom(Transform pieceTransform)
        {
            Vector2Int[] minoPos = minoCoords(pieceTransform);
            int highest = minoPos[0].y; // the bottom mino with the highest y coord, not the highest mino

            for (int i = 1; i < minoPos.Length; i++)
            {
                int y = minoPos[i].y;

                if (y > highest) { highest = y; }
            }
            return highest;
        }

        bool isColliding(GameObject p) => CollisionDetection.isColliding(minoCoords(), board.objectMatrix);


        public bool isMinoOOB(int boardXPos, int boardYPos)
        {
            return boardXPos < 0 ||
                //boardYPos < 0 ||
                boardXPos >= board.width ||
                boardYPos >= board.height;
        }

        public bool isPieceOOB(GameObject piece)
        {
            //for (int i = 0; i < piece.transform.childCount; ++i)
            //{
            //    Transform mino = piece.transform.GetChild(i);
            //    int minoXPos = (int)(mino.position.x - 0.5f),
            //        minoYPos = (int)((mino.position.y - 3.5f) * -1);
            //    if (isMinoOOB(minoXPos, minoYPos)) return true;
            //}

            foreach (Vector2Int mino in minoCoords(piece.transform))
            {
                if (isMinoOOB(mino.x, mino.y)) return true;
            }
            return false;
        }

        public void placeGhostPiece()
        {
            currentGhostPiece.transform.position = currentPiece.transform.position;
            while ((findBottom(currentGhostPiece.transform) != board.height) && (!CollisionDetection.isColliding(minoCoords(currentGhostPiece.transform), board.objectMatrix)))
            {
                currentGhostPiece.transform.position += Vector3.down;
            }
            currentGhostPiece.transform.position += Vector3.up;
        }

        public void makeAnotherOne()
        {
            GameObject newOne = Instantiate(blockQueue.queue[0]);
            newOne.transform.position = startingPos;
            newOne.transform.SetParent(transform);
            listOfObjects.Add(newOne);

            // make ghostPiece transparent
            makeGhost(newOne);

            for (int i = 0; i < 4; i++)
            {
                pieceLight[i].transform.SetParent(newOne.transform.GetChild(i));
                pieceLight[i].transform.localPosition = Vector3.zero;
                pieceLight[i].transform.SetParent(newOne.transform);
            }

            currentPiece = newOne;
            blockQueue.updateQueue();
            blockQueue.printQueue();
        }

        public void makeGhost(GameObject newOne)
        {
            GameObject ghostPiece = Instantiate(newOne);
            for (int i = 0; i < ghostPiece.transform.childCount; i++)
            {
                Transform ghostMino = ghostPiece.transform.GetChild(i);
                Transform minoModel = ghostMino.GetChild(0);
                Color ghostMinoColor = minoModel.GetComponent<Renderer>().material.color;
                ghostMinoColor.a = 0.25f;
                minoModel.GetComponent<Renderer>().material.color = ghostMinoColor;
            }

            currentGhostPiece = ghostPiece;
        }

        public void resetRotation(GameObject p)
        {
            p.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        public void swapHold()
        {
            GameObject temp, tempGhost;
            
            if (canPieceBeHeld)
            {
                if (currentGhostPiece) Destroy(currentGhostPiece.gameObject);
                if (howManyTimesPieceBeenHeld == 1) // first time THIS WORKS BTW WOOOOHOOOO
                {
                    holdPiece = currentPiece;
                    holdPiece.transform.position = holdPosition;
                    resetRotation(holdPiece);
                    makeAnotherOne();
                    dirtyGhost = true;
                    //placeGhostPiece();
                    // currentPiece.transform.position = new Vector3(4.5f, 2.5f, 4);
                    howManyTimesPieceBeenHeld++;
                    canPieceBeHeld = false;
                }
                else if (howManyTimesPieceBeenHeld == 2) // from then on
                {
                    temp = holdPiece;
                    //Vector3 prevPos = currentPiece.transform.position;
                    holdPiece = currentPiece;
                    currentPiece = temp;
                    holdPiece.transform.position = holdPosition; // magic number that is the position of the hold piece
                    currentPiece.transform.position = startingPos; // or transform.position? this is the position of piecemaker
                    resetRotation(holdPiece);
                    resetRotation(currentPiece);
                    makeGhost(currentPiece);
                    dirtyGhost = true;
                    //placeGhostPiece();
                    canPieceBeHeld = false;
                }

                Noisy.PlaySound("Pop");
            }
        }

        public bool ImprintPiece()
        {
            if (currentPiece == null || isColliding(currentPiece)) return false;
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

        public void pieceFallOnTime()
        {
            // double iterationDelay = ((11 - level) * 0.1) * 1000;  // [seconds] used to be 0.05
            if (currentPiece == null) return;

            double iterationDelay = 1000;

            if (fallCounter >= iterationDelay)
            {
                currentPiece.transform.position += Vector3.down;
                if (findBottom(currentPiece.transform) == board.height)
                {
                    currentPiece.transform.position += Vector3.up;
                    ImprintPiece(); // add delay later
                }

                else if (isColliding(currentPiece))
                {
                    currentPiece.transform.position += Vector3.up;
                    ImprintPiece();
                }

                fallCounter -= (long)iterationDelay;
            }
        }

        private int topOfBoard = 2;
        private void checkIfGameOver()
        {
            Vector2Int[] minoPos1 = minoCoords(currentPiece.transform);
            foreach (Vector2Int coord in minoPos1)
            {
                int y = coord.y;

                //start is @2
                if (y <= topOfBoard)
                {
                    //go to next scene
                    SceneManager.LoadScene("GameOverScreen");
                }
            }
        }

        public void HardDrop()
        {
            while (findBottom(currentPiece.transform) != board.height && !isColliding(currentPiece))
            {
                currentPiece.transform.position += Vector3.down;
            }
            currentPiece.transform.position += Vector3.up;
            ImprintPiece();
        }
    }
}