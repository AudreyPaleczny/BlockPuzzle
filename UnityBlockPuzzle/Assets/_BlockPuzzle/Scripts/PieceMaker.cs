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
        // public List<GameObject> listOfObjects = new List<GameObject>();
        public Board board;
        // public BlockQueue blockQueue;
        // public Light[] pieceLight = new Light[4];
        float keyTimer = 0.0f;
        const float keyDelay = 1f / 8;
        public Text debugText;
        public GameObject previousGhostPiece;
        public long then;
        private int level = 0;
        // private int topOfBoard = 2;

        //public long fallCounter = 0;
        //double iterationDelay = 1000;

        bool delayTheImprintForSoftDrop = false;

        // private bool dirtyGhost = true;

        public class LightUnattacher : MonoBehaviour
        {
            public Light _light;
            private void OnDestroy()
            {
                _light.transform.SetParent(null);
            }
        }

        public int numberOfPlayers = 1;
        public Player player1;
        public Player player2;

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

        //public void destroyTheLastOne()
        //{
        //    int last = listOfObjects.Count - 1;
        //    Destroy(listOfObjects[last]);
        //    listOfObjects.RemoveAt(last);
        //}

        //public void placeGhostPiece()
        //{
        //    currentGhostPiece.transform.position = currentPiece.transform.position;
        //    //Debug.Log( string.Join(", ",minoCoords()) );
        //    while ((findBottom(currentGhostPiece.transform) != board.height) && (!CollisionDetection.isColliding(minoCoords(currentGhostPiece.transform), board.objectMatrix)))
        //    {
        //        currentGhostPiece.transform.position += Vector3.down;
        //    }
        //    currentGhostPiece.transform.position += Vector3.up;
        //}

        //private void checkIfGameOver()
        //{
        //    Vector2Int[] minoPos = minoCoords(currentPiece.transform);
        //    foreach(Vector2Int coord in minoPos)
        //    {
        //        int y = coord.y;

        //        //start is @2
        //        if(y <= topOfBoard)
        //        {
        //            //go to next scene
        //            SceneManager.LoadScene("GameOverScreen");
        //        }
        //    }
        //}

        //public void makeAnotherOne()
        //{
        //    GameObject newOne = Instantiate(blockQueue.queue[0]);
        //    newOne.transform.position = transform.position;
        //    newOne.transform.SetParent(transform);
        //    listOfObjects.Add(newOne);

        //    // make ghostPiece transparent
        //    GameObject ghostPiece = Instantiate(newOne);
        //    for(int i = 0; i < ghostPiece.transform.childCount; i++)
        //    {
        //        Transform ghostMino = ghostPiece.transform.GetChild(i);
        //        Transform minoModel = ghostMino.GetChild(0);
        //        Color ghostMinoColor = minoModel.GetComponent<Renderer>().material.color;
        //        ghostMinoColor.a = 0.25f;
        //        minoModel.GetComponent<Renderer>().material.color = ghostMinoColor;
        //    }

        //    currentGhostPiece = ghostPiece;

        //    for (int i = 0; i < 4; i++)
        //    {
        //        pieceLight[i].transform.SetParent(newOne.transform.GetChild(i));
        //        pieceLight[i].transform.localPosition = Vector3.zero;
        //        pieceLight[i].transform.SetParent(newOne.transform);
        //    }

        //    currentPiece = newOne;
        //    blockQueue.updateQueue();
        //    blockQueue.printQueue();
        //}

        //public int howManyTimesPieceBeenHeld = 1;
        //public bool canPieceBeHeld = true;
        //public GameObject holdPiece;

        //public void swapHold()
        //{
        //    if (!canPieceBeHeld) return;
        //    GameObject temp;

        //    resetRotation(currentGhostPiece);
        //    resetRotation(currentPiece);

        //    if (howManyTimesPieceBeenHeld == 1) // first time THIS WORKS BTW WOOOOHOOOO
        //    {
        //        previousGhostPiece = currentGhostPiece;
        //        previousGhostPiece.SetActive(false);
        //        holdPiece = currentPiece;
        //        makeAnotherOne();
        //        howManyTimesPieceBeenHeld++;
        //    }
        //    else if (howManyTimesPieceBeenHeld == 2) // from then on
        //    {
        //        temp = holdPiece; holdPiece = currentPiece; currentPiece = temp; // swaps the hold piece and current piece
        //        currentPiece.transform.position = transform.position; 

        //        temp = previousGhostPiece; previousGhostPiece = currentGhostPiece; currentGhostPiece = temp; // swaps ghost piece and old ghost piece

        //        currentGhostPiece.SetActive(true);
        //        previousGhostPiece.SetActive(false);
        //        currentGhostPiece.transform.position = transform.position;
        //    }
            
        //    Noisy.PlaySound("Pop");
        //    dirtyGhost = true;
        //    holdPiece.transform.position = new Vector3(-5, 2.5f, 4); // magic number that is the position of the hold piece
        //    canPieceBeHeld = false;
        //    currentPiece.GetComponent<PieceInfo>().pieceOrientation = 0;
        //}

        public void resetRotation(GameObject p)
        {
            p.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        Vector2Int[] minoCoords(GameObject p) => minoCoords(p.transform);
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
        bool isColliding(GameObject p) => CollisionDetection.isColliding(minoCoords(p), board.objectMatrix);

        public static long UTCMS()
        {
            return System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        }

        public enum PieceCollision { none, blockedV, blockedH, oobLeft, oobRight, oobTop, oobBottom };

        public KeyCode currentKey = KeyCode.None;

        // Start is called before the first frame update
        void Start()
        {
            numberOfPlayers = 2;
            then = UTCMS();
            debugText = GameObject.Find("Debug Text")?.GetComponent<Text>();
            //Random.InitState(System.Environment.TickCount);
            player1.blockQueue.initNums(7);
            player1.blockQueue.initialPosition = new Vector3(18, 0.5f, 4);
            player1.startingPos = transform.position;
            player1.blockQueue.makeQueue();
            
            if (numberOfPlayers == 2)
            {
                player1.startingPos = transform.position + Vector3.left * 2;
                player1.holdPosition = new Vector3(-10, 0.5f, 4);
                player2.startingPos = transform.position + Vector3.right * 2;
                player2.blockQueue.initialPosition = new Vector3(23, 0.5f, 4);
                player2.blockQueue.initNums(7);
                player2.blockQueue.makeQueue();
                player2.makeAnotherOne();
            }
            player1.makeAnotherOne();
        }

        bool SpecialCollisionLogic(PieceInfo currentPieceInfo, int rotationRuleIndex)
        {
            for (int i = 0; i < currentPieceInfo.rules_I[rotationRuleIndex].test.Length; i++)
            {
                (int,int) v = currentPieceInfo.rules_I[rotationRuleIndex].test[i];
                Vector3Int piece_test = new Vector3Int(v.Item1, v.Item2, 0);
                player1.currentPiece.transform.position += piece_test;
                if (!(player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)))
                {
                    return true;
                }
                else player1.currentPiece.transform.position -= piece_test;
            }
            return false;
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
                        for (int i = 0; i < player1.currentPiece.transform.childCount; i++)
                        {
                            //Debug.Log("Child " + i + " " + transform.GetChild(i).position);
                            sb.Append("Child " + i + " board xPos: " + (player1.currentPiece.transform.GetChild(i).position.x - 0.5) +
                                " board yPos:" + (player1.currentPiece.transform.GetChild(i).position.y - 3.5) * -1).Append("\n");
                        }
                        Debug.Log(sb);
                        if (debugText) debugText.text = sb.ToString();
                    },
                    [KeyCode.Z] = () =>
                    {

                        player1.currentPiece.transform.Rotate(0, 0, 90);
                        PieceInfo currentPieceInfo = player1.currentPiece.GetComponent<PieceInfo>();
                        bool canRotate = true;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece))
                        {
                            canRotate = false;
                            if (currentPieceInfo.pieceType == PieceInfo.PieceType.I) // IPiece 
                            {                           
                                switch (currentPieceInfo.pieceOrientation)
                                {
                                    case 0:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 7);
                                        break;
                                    case 1:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 1);
                                        break;
                                    case 2:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 3);
                                        break;
                                    case 3:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 5);
                                        break;
                                }

                            }
                            else
                            {
                                switch (currentPieceInfo.pieceOrientation)
                                {
                                    case 0:
                                        Debug.LogError("Error: not working yet");
                                        break;
                                    case 1:
                                        Debug.LogError("Error: not working yet");
                                        break;
                                    case 2:
                                        Debug.LogError("Error: not working yet");
                                        break;
                                    case 3:
                                        Debug.LogError("Error: not working yet");
                                        break;
                                }
                            }
                        }
                        if (canRotate)
                        {
                            player1.currentGhostPiece.transform.Rotate(0, 0, 90);
                            currentPieceInfo.pieceOrientation--;
                            if (currentPieceInfo.pieceOrientation < 0) currentPieceInfo.pieceOrientation += 4;
                            player1.dirtyGhost = true;
                        }
                        else
                        {
                            player1.currentPiece.transform.Rotate(0, 0, -90);
                        }
                        
                    },
                    [KeyCode.X] = () =>
                    {
                        player1.currentPiece.transform.Rotate(0, 0, -90);
                        PieceInfo currentPieceInfo = player1.currentPiece.GetComponent<PieceInfo>();
                        bool canRotate = true;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece))
                        {
                            canRotate = false;
                            if (currentPieceInfo.pieceType == PieceInfo.PieceType.I) // IPiece 
                            {
                                switch (currentPieceInfo.pieceOrientation)
                                {
                                    case 0:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 0);
                                        break;
                                    case 1:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 2);
                                        break;
                                    case 2:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 4);
                                        break;
                                    case 3:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 6);
                                        break;
                                }

                            }
                            else
                            {
                                switch (currentPieceInfo.pieceOrientation)
                                {
                                    case 0:
                                        Debug.LogError("Error: not working yet");
                                        break;
                                    case 1:
                                        Debug.LogError("Error: not working yet");
                                        break;
                                    case 2:
                                        Debug.LogError("Error: not working yet");
                                        break;
                                    case 3:
                                        Debug.LogError("Error: not working yet");
                                        break;
                                }
                            }
                        }
                        if (canRotate)
                        {
                            player1.currentGhostPiece.transform.Rotate(0, 0, -90);
                            currentPieceInfo.pieceOrientation++;
                            if (currentPieceInfo.pieceOrientation > 3) currentPieceInfo.pieceOrientation -= 4;
                            player1.dirtyGhost = true;
                        }
                        else
                        {
                            player1.currentPiece.transform.Rotate(0, 0, 90);
                        }
                        
                    },
                    [KeyCode.A] = () =>
                    {
                        player1.currentPiece.transform.Rotate(0, 0, -180);
                        player1.currentGhostPiece.transform.Rotate(0, 0, -180);
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.Space] = () => {
                        if (!Input.GetKeyDown(KeyCode.Space)) return;
                        player1.HardDrop();
                    },
                    //[KeyCode.Space] = () => ImprintPiece(), // <- thats a function
                    [KeyCode.C] = () =>
                    {
                        player1.swapHold();
                    },
                    [KeyCode.UpArrow] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.up;
                    },
                    [KeyCode.DownArrow] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.down;

                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece))
                        {
                            player1.currentPiece.transform.position += Vector3.up;
                            if (!delayTheImprintForSoftDrop)
                            {
                                player1.fallCounter = 0;
                                delayTheImprintForSoftDrop = true;
                            }
                        }
                    },
                    [KeyCode.LeftArrow] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.left;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)) player1.currentPiece.transform.position += Vector3.right;
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.RightArrow] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.right;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)) player1.currentPiece.transform.position += Vector3.left;
                        player1.dirtyGhost = true;
                    },
                };

        public Dictionary<KeyCode, Action> _coopcontrols;
        Dictionary<KeyCode, Action> coopcontrols =>
            _coopcontrols != null
                ? _coopcontrols
                : _coopcontrols = new Dictionary<KeyCode, Action>()
                {
                    [KeyCode.Q] = () =>
                    {
                        player1.currentPiece.transform.Rotate(0, 0, 90);
                        player1.currentGhostPiece.transform.Rotate(0, 0, 90);
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.E] = () =>
                    {
                        player1.currentPiece.transform.Rotate(0, 0, -90);
                        player1.currentGhostPiece.transform.Rotate(0, 0, -90);
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.F] = () =>
                    {
                        player1.currentPiece.transform.Rotate(0, 0, -180);
                        player1.currentGhostPiece.transform.Rotate(0, 0, -180);
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.Space] = () => {
                        if (!Input.GetKeyDown(KeyCode.Space)) return;
                        player1.HardDrop();
                    },
                    //[KeyCode.Space] = () => ImprintPiece(), // <- thats a function
                    [KeyCode.C] = () =>
                    {
                        player1.swapHold();
                    },
                    [KeyCode.W] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.up;
                    },
                    [KeyCode.S] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.down;

                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece))
                        {
                            player1.currentPiece.transform.position += Vector3.up;
                            if (!delayTheImprintForSoftDrop)
                            {
                                player1.fallCounter = 0;
                                delayTheImprintForSoftDrop = true;
                            }
                        }
                    },
                    [KeyCode.A] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.left;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)) player1.currentPiece.transform.position += Vector3.right;
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.D] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.right;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)) player1.currentPiece.transform.position += Vector3.left;
                        player1.dirtyGhost = true;
                    },

                    
                    // second player
                    [KeyCode.K] = () =>
                    {
                        player2.currentPiece.transform.Rotate(0, 0, 90);
                        player2.currentGhostPiece.transform.Rotate(0, 0, 90);
                        player2.dirtyGhost = true;
                    },
                    [KeyCode.L] = () =>
                    {
                        player2.currentPiece.transform.Rotate(0, 0, -90);
                        player2.currentGhostPiece.transform.Rotate(0, 0, -90);
                        player2.dirtyGhost = true;
                    },
                    [KeyCode.O] = () =>
                    {
                        player2.currentPiece.transform.Rotate(0, 0, -180);
                        player2.currentGhostPiece.transform.Rotate(0, 0, -180);
                        player2.dirtyGhost = true;
                    },
                    [KeyCode.Period] = () => {
                        if (!Input.GetKeyDown(KeyCode.Period)) return;
                        player2.HardDrop();
                    },
                    //[KeyCode.Space] = () => ImprintPiece(), // <- thats a function
                    [KeyCode.Comma] = () =>
                    {
                        player2.swapHold();
                    },
                    [KeyCode.UpArrow] = () =>
                    {
                        player2.currentPiece.transform.position += Vector3.up;
                    },
                    [KeyCode.DownArrow] = () =>
                    {
                        player2.currentPiece.transform.position += Vector3.down;

                        if (player2.isPieceOOB(player2.currentPiece) || isColliding(player2.currentPiece))
                        {
                            player2.currentPiece.transform.position += Vector3.up;
                            if (!delayTheImprintForSoftDrop)
                            {
                                player2.fallCounter = 0;
                                delayTheImprintForSoftDrop = true;
                            }
                        }
                    },
                    [KeyCode.LeftArrow] = () =>
                    {
                        player2.currentPiece.transform.position += Vector3.left;
                        if (player2.isPieceOOB(player2.currentPiece) || isColliding(player2.currentPiece)) player2.currentPiece.transform.position += Vector3.right;
                        player2.dirtyGhost = true;
                    },
                    [KeyCode.RightArrow] = () =>
                    {
                        player2.currentPiece.transform.position += Vector3.right;
                        if (player2.isPieceOOB(player2.currentPiece) || isColliding(player2.currentPiece)) player2.currentPiece.transform.position += Vector3.left;
                        player2.dirtyGhost = true;
                    },
                };


        [TextArea(4, 4)]
        public string debugPosition;
        void Update()
        {
            //if (player1.currentPiece)
            //{
            //    debugPosition = string.Join("\n", minoCoords());
            //}
            if (player1.dirtyGhost)
            {
                player1.placeGhostPiece();
                //Debug.Log("moved");
                player1.dirtyGhost = false;
            }
            if (numberOfPlayers == 2 && player2.dirtyGhost)
            {
                player2.placeGhostPiece();
                //Debug.Log("moved");
                player2.dirtyGhost = false;
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
            player1.fallCounter += passed;
            player1.pieceFallOnTime();
            if (numberOfPlayers == 2)
            {
                player2.fallCounter += passed;
                player2.pieceFallOnTime();
                player2.pieceFallOnTime();
            }
            currentKey = KeyCode.None;

            if (numberOfPlayers == 2)
            {
                foreach (KeyValuePair<KeyCode, Action> kvp in coopcontrols)
                {
                    if (Input.GetKey(kvp.Key))
                    {
                        kvp.Value.Invoke();
                        keyTimer = keyDelay;
                    }
                }
            } else
            {
                foreach (KeyValuePair<KeyCode, Action> kvp in controls)
                {
                    if (Input.GetKey(kvp.Key))
                    {
                        kvp.Value.Invoke();
                        keyTimer = keyDelay;
                    }
                }
            }

            ClearLines();
        }
    }
}