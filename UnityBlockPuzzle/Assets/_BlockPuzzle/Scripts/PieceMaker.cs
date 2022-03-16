using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Piece
{
    public class PieceMaker : MonoBehaviour
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
        public long then;
        public long fallCounter = 0;

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
            int count = 0, clearRow;
            for (int row = 0; row < board.height; ++row)
            {
                for (int col = 0; col < board.width; ++col)
                {
                    if (board.objectMatrix[row][col]) // change when we add shadow pieces and whatever
                    {
                        ++count;
                    }
                }
                //
                //
                if (count == board.width)
                {
                    clearRow = row;
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
                count = 0;
            }
        }

        public void destroyTheLastOne()
        {
            int last = listOfObjects.Count - 1;
            Destroy(listOfObjects[last]);
            listOfObjects.RemoveAt(last);
        }

        public void makeAnotherOne()
        {
            GameObject newOne = Instantiate(blockQueue.queue[0]);
            newOne.transform.position = transform.position;
            newOne.transform.SetParent(transform);
            listOfObjects.Add(newOne);

            // make ghostPiece transparent
            //GameObject ghostPiece = Instantiate(blockQueue.queue[0]);
            //Color ghostPieceColor = ghostPiece.GetComponent<Renderer>().material.color;
            //ghostPieceColor.a = 0.5f;
            //ghostPiece.GetComponent<Renderer>().material.color = ghostPieceColor;
            //ghostPiece.transform.position = transform.position;

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

        public int n = 1;
        public int holdCounter = 0;
        public GameObject holdPiece;

        public void swapHold()
        {
            GameObject temp;
            while (holdCounter == 0)
            {
                if (n == 1) // first time THIS WORKS BTW WOOOOHOOOO
                {
                    holdPiece = currentPiece;
                    holdPiece.transform.position = new Vector3(-5, 2.5f, 4);
                    makeAnotherOne();
                    // currentPiece.transform.position = new Vector3(4.5f, 2.5f, 4);
                    n++;
                    holdCounter++;
                }
                else if (n == 2) // from then on
                {
                    temp = holdPiece;
                    Vector3 prevPos = currentPiece.transform.position;
                    holdPiece = currentPiece;
                    currentPiece = temp;
                    holdPiece.transform.position = new Vector3(-5, 2.5f, 4); // magic number that is the position of the hold piece
                    currentPiece.transform.position = prevPos; // or transform.position? this is the position of piecemaker
                    holdCounter++;
                }
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns> whether the piece can be imprinted as is </returns>
        public bool ImprintPiece()
        {
            if (currentPiece == null || isColliding()) return false;
            bool success = true;
            Vector2Int[] MinosPos = minoCoords();

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
            holdCounter = 0;
            makeAnotherOne();
            return success;
        }

        public static long UTCMS()
        {
            return System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        }

        public void pieceFallOnTime()
        {
            // double iterationDelay = ((11 - level) * 0.1) * 1000;  // [seconds] used to be 0.05
            if (currentPiece == null) return;

            double iterationDelay = 1000;

            if (fallCounter >= iterationDelay)
            {
                currentPiece.transform.position += Vector3.down;
                if (findBottom() == board.height)
                {
                    currentPiece.transform.position += Vector3.up;
                    ImprintPiece(); // add delay later
                }

                else if (isColliding())
                {
                    currentPiece.transform.position += Vector3.up;
                    ImprintPiece();
                }

                fallCounter -= (long)iterationDelay;
            }
        }

        public enum PieceCollision { none, blockedV, blockedH, oobLeft, oobRight, oobTop, oobBottom };

        public KeyCode currentKey = KeyCode.None;

        public int findBottom()
        {
            Vector2Int[] minoPos = minoCoords();
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
            while(findBottom() != board.height && !isColliding())
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

        [TextArea(4, 4)]
        public string debugPosition;
        void Update()
        {
            if (currentPiece)
            {
                debugPosition = string.Join("\n", minoCoords());
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

            Dictionary<KeyCode, Action> controls = new Dictionary<KeyCode, Action>()
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
                },
                [KeyCode.X] = () =>
                {
                    currentPiece.transform.Rotate(0, 0, -90);
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
                        ImprintPiece();
                    }
                },
                [KeyCode.LeftArrow] = () =>
                {
                    currentPiece.transform.position += Vector3.left;
                    if (isPieceOOB(currentPiece) || isColliding()) currentPiece.transform.position += Vector3.right;

                },
                [KeyCode.RightArrow] = () =>
                {
                    currentPiece.transform.position += Vector3.right;
                    if (isPieceOOB(currentPiece) || isColliding()) currentPiece.transform.position += Vector3.left;
                },
            };

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