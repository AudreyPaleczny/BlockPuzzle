using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Piece
{
    public partial class PieceMaker : MonoBehaviour
    {
        [Tooltip("Put a thing in here to create!"), ContextMenuItem("Spawn", "makeAnotherOne"), ContextMenuItem("DESTROY THE LAST THING", "destroyTheLastOne")]
        public float delay;
        public List<GameObject> listOfObjects = new List<GameObject>();
        public Board board;
        public BlockQueue blockQueue;
        public Light[] pieceLight = new Light[4];
        float keyTimer = 0.0f;
        const float keyDelay = 1f / 8;
        public Text debugText;
        public GameObject currentPiece;
        public GameObject currentGhostPiece, previousGhostPiece;
        public long then;
        private int level = 0;
        private int topOfBoard = 2;

        public long fallCounter = 0;
        double iterationDelay = 1000;

        bool delayTheImprintForSoftDrop = false;

        private bool dirtyGhost = true;

        public class LightUnattacher : MonoBehaviour
        {
            public Light _light;
            private void OnDestroy()
            {
                _light.transform.SetParent(null);
            }
        }

        public void ClearLines()
        {
            int rowsCleared = 0;
            int countColumns = 0, clearRow;
            for (int row = 0; row < board.height; ++row)
            {
                for (int col = 0; col < board.width; ++col)
                {
                    if (board.objectMatrix[row][col]) // change when we add shadow pieces and whatever
                    {
                        ++countColumns;
                    }
                }
                //
                //
                if (countColumns == board.width)
                {
                    clearRow = row;
                    rowsCleared++;
                    for (int col = 0; col < board.width; ++col)
                    {
                        Destroy(board.objectMatrix[row][col]);
                    }

                    for (int shiftRow = clearRow; shiftRow > 0; --shiftRow)
                    {
                        for (int shiftCol = 0; shiftCol < board.width; ++shiftCol)
                        {
                            board.objectMatrix[shiftRow][shiftCol] = board.objectMatrix[shiftRow - 1][shiftCol];
                            if (board.objectMatrix[shiftRow][shiftCol])
                            {
                                board.objectMatrix[shiftRow][shiftCol].transform.position += Vector3.down;
                            }
                        }
                    }
                    for (int r = 0; r < board.height; ++r)
                    {
                        for (int c = 0; c < board.width; ++c)
                        {
                            GameObject mino = board.objectMatrix[r][c];
                            if (mino == null) { continue; }
                            Vector3 expectedPosition = new Vector3(c + 0.5f, -(r - 3.5f), mino.gameObject.transform.position.z);
                            if (Vector3.Distance(expectedPosition, mino.transform.position) > 0.5f)
                            {
                                mino.gameObject.GetComponentInChildren<Renderer>().material.color = Color.black;
                            }
                        }
                    }
                }

                countColumns = 0;
            }

            updateScore(rowsCleared, level);
        }

        private void updateScore(int rowsCleared, int level)
        {
            switch (rowsCleared)
            {
                case 0:
                    break;

                case 1: Score.Instance.value += (40 * (level + 1));
                    break;

                case 2: Score.Instance.value += (100 * (level + 1));
                    break;

                case 3:
                    Score.Instance.value += (300 * (level + 1));
                    break;

                default:
                    Score.Instance.value += (1200 * (level + 1));
                    break;
            }

        }

        public void destroyTheLastOne()
        {
            int last = listOfObjects.Count - 1;
            Destroy(listOfObjects[last]);
            listOfObjects.RemoveAt(last);
        }

        public void placeGhostPiece()
        {
            currentGhostPiece.transform.position = currentPiece.transform.position;
            //Debug.Log( string.Join(", ",minoCoords()) );
            while ((findBottom(currentGhostPiece.transform) != board.height) && (!CollisionDetection.isColliding(minoCoords(currentGhostPiece.transform), board.objectMatrix)))
            {
                currentGhostPiece.transform.position += Vector3.down;
            }
            currentGhostPiece.transform.position += Vector3.up;
        }

        private void checkIfGameOver()
        {
            Vector2Int[] minoPos = minoCoords(currentPiece.transform);
            foreach(Vector2Int coord in minoPos)
            {
                int y = coord.y;

                //start is @2
                if(y <= topOfBoard)
                {
                    //go to next scene
                    SceneManager.LoadScene("GameOverScreen");
                }
            }
        }

        public void makeAnotherOne()
        {
            GameObject newOne = Instantiate(blockQueue.queue[0]);
            newOne.transform.position = transform.position;
            newOne.transform.SetParent(transform);
            listOfObjects.Add(newOne);

            // make ghostPiece transparent
            GameObject ghostPiece = Instantiate(newOne);
            for(int i = 0; i < ghostPiece.transform.childCount; i++)
            {
                Transform ghostMino = ghostPiece.transform.GetChild(i);
                Transform minoModel = ghostMino.GetChild(0);
                Color ghostMinoColor = minoModel.GetComponent<Renderer>().material.color;
                ghostMinoColor.a = 0.25f;
                minoModel.GetComponent<Renderer>().material.color = ghostMinoColor;
            }

            currentGhostPiece = ghostPiece;

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

        public int howManyTimesPieceBeenHeld = 1;
        public bool canPieceBeHeld = true;
        public GameObject holdPiece;

        public void swapHold()
        {
            if (!canPieceBeHeld) return;
            GameObject temp;

            resetRotation(currentGhostPiece);
            resetRotation(currentPiece);

            if (howManyTimesPieceBeenHeld == 1) // first time THIS WORKS BTW WOOOOHOOOO
            {
                previousGhostPiece = currentGhostPiece;
                previousGhostPiece.SetActive(false);
                holdPiece = currentPiece;
                makeAnotherOne();
                howManyTimesPieceBeenHeld++;
            }
            else if (howManyTimesPieceBeenHeld == 2) // from then on
            {
                temp = holdPiece; holdPiece = currentPiece; currentPiece = temp; // swaps the hold piece and current piece
                currentPiece.transform.position = transform.position; 

                temp = previousGhostPiece; previousGhostPiece = currentGhostPiece; currentGhostPiece = temp; // swaps ghost piece and old ghost piece

                currentGhostPiece.SetActive(true);
                previousGhostPiece.SetActive(false);
                currentGhostPiece.transform.position = transform.position;
            }
            
            Noisy.PlaySound("Pop");
            dirtyGhost = true;
            holdPiece.transform.position = new Vector3(-5, 2.5f, 4); // magic number that is the position of the hold piece
            canPieceBeHeld = false;
            
        }

        public void resetRotation(GameObject p)
        {
            p.transform.eulerAngles = new Vector3(0, 0, 0);
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

            foreach (Vector2Int mino in minoCoords())
            {
                if (isMinoOOB(mino.x, mino.y)) return true;
            }
            return false;
        }

        public bool isMinoOOB(int boardXPos, int boardYPos)
        {
            return boardXPos < 0 ||
                //boardYPos < 0 ||
                boardXPos >= board.width ||
                boardYPos >= board.height;
        }

        Vector2Int[] minoCoords() => minoCoords(currentPiece.transform);
        Vector2Int[] minoCoords(Transform pieceTransform)
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

        bool isColliding() => CollisionDetection.isColliding(minoCoords(), board.objectMatrix);

        public static long UTCMS()
        {
            return System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        }

        public void pieceFallOnTime()
        {
            // double iterationDelay = ((11 - level) * 0.1) * 1000;  // [seconds] used to be 0.05
            if (currentPiece == null) return;

            if (fallCounter >= iterationDelay)
            {
                currentPiece.transform.position += Vector3.down;
                if (findBottom(currentPiece.transform) == board.height)
                {
                    currentPiece.transform.position += Vector3.up;
                    ImprintPiece(); // add delay later
                    delayTheImprintForSoftDrop = false;
                }

                else if (isColliding())
                {
                    currentPiece.transform.position += Vector3.up;
                    ImprintPiece();
                    delayTheImprintForSoftDrop = false;
                }

                fallCounter -= (long)iterationDelay;
            }
        }

        public enum PieceCollision { none, blockedV, blockedH, oobLeft, oobRight, oobTop, oobBottom };

        public KeyCode currentKey = KeyCode.None;

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

        public void HardDrop()
        {
            while(findBottom(currentPiece.transform) != board.height && !isColliding())
            {
                currentPiece.transform.position += Vector3.down;
            }
            currentPiece.transform.position += Vector3.up;
            ImprintPiece();
        }

        // Start is called before the first frame update
        void Start()
        {
            then = UTCMS();
            debugText = GameObject.Find("Debug Text")?.GetComponent<Text>();
            //Random.InitState(System.Environment.TickCount);
            blockQueue.initNums(7);
            blockQueue.makeQueue();
            makeAnotherOne();
        }


        public Dictionary<KeyCode, Action> _controls;

        Dictionary<KeyCode, Action> controls =>
            _controls != null
                ? _controls
                : _controls = new Dictionary<KeyCode, Action>()
                {
                    [KeyCode.P] = () =>
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < currentPiece.transform.childCount; i++)
                        {
                            //Debug.Log("Child " + i + " " + transform.GetChild(i).position);
                            sb.Append("Child " + i + " board xPos: " + (currentPiece.transform.GetChild(i).position.x - 0.5) +
                                " board yPos:" + (currentPiece.transform.GetChild(i).position.y - 3.5) * -1).Append("\n");
                        }
                        Debug.Log(sb);
                        if (debugText) debugText.text = sb.ToString();
                    },
                    [KeyCode.Z] = () =>
                    {
                        currentPiece.transform.Rotate(0, 0, 90);
                        currentGhostPiece.transform.Rotate(0, 0, 90);
                        dirtyGhost = true;
                    },
                    [KeyCode.X] = () =>
                    {
                        currentPiece.transform.Rotate(0, 0, -90);
                        currentGhostPiece.transform.Rotate(0, 0, -90);
                        dirtyGhost = true;
                    },
                    [KeyCode.R] = () =>
                    {
                        resetRotation(currentPiece);
                    },
                    [KeyCode.Space] = () => {
                        if (!Input.GetKeyDown(KeyCode.Space)) return;
                        HardDrop();
                    },
                    //[KeyCode.Space] = () => ImprintPiece(), // <- thats a function
                    [KeyCode.C] = () =>
                    {
                        swapHold();
                    },
                    [KeyCode.UpArrow] = () =>
                    {
                        currentPiece.transform.position += Vector3.up;
                    },
                    [KeyCode.DownArrow] = () =>
                    {
                        currentPiece.transform.position += Vector3.down;

                        if (isPieceOOB(currentPiece) || isColliding())
                        {
                            currentPiece.transform.position += Vector3.up;
                            if (!delayTheImprintForSoftDrop)
                            {
                                fallCounter = 0;
                                delayTheImprintForSoftDrop = true;
                            }
                        }
                    },
                    [KeyCode.LeftArrow] = () =>
                    {
                        currentPiece.transform.position += Vector3.left;
                        if (isPieceOOB(currentPiece) || isColliding()) currentPiece.transform.position += Vector3.right;
                        dirtyGhost = true;
                    },
                    [KeyCode.RightArrow] = () =>
                    {
                        currentPiece.transform.position += Vector3.right;
                        if (isPieceOOB(currentPiece) || isColliding()) currentPiece.transform.position += Vector3.left;
                        dirtyGhost = true;
                    },
                };


        [TextArea(4, 4)]
        public string debugPosition;
        void Update()
        {
            if (currentPiece)
            {
                debugPosition = string.Join("\n", minoCoords());
            }
            if (dirtyGhost)
            {
                placeGhostPiece();
                //Debug.Log("moved");
                dirtyGhost = false;
            }
            if (keyTimer > 0)
            {
                keyTimer -= Time.deltaTime;
                if (keyTimer > 0)
                {
                    return;
                }
            }

            long now = UTCMS();
            //time passed
            long passed = now - then;
            then = now;
            fallCounter += passed;
            pieceFallOnTime();

            currentKey = KeyCode.None;

            //placePieceIfCollision();

            foreach (KeyValuePair<KeyCode, Action> kvp in controls)
            {
                if (Input.GetKey(kvp.Key))
                {
                    kvp.Value.Invoke();
                    keyTimer = keyDelay;
                }
            }
            ClearLines();
        }
    }
}