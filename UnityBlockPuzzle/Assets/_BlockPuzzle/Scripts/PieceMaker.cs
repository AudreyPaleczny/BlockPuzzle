using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PieceMaker : MonoBehaviour
{
    [Tooltip("Put a thing in here to create!"), ContextMenuItem("Spawn", "makeAnotherOne"), ContextMenuItem("DESTROY THE LAST THING", "destroyTheLastOne")]
    public List<GameObject> prefabsOfPieces = new List<GameObject>();
    public float delay;
    public List<GameObject> listOfObjects = new List<GameObject>();
    public Board board;
    public Light[] pieceLight = new Light[4];
    float keyTimer = 0.0f;
    const float keyDelay = 1f / 8;
    public Text debugText;
    public GameObject currentPiece;
    public GameObject holdPiece;
    public long then;
    public long fallCounter = 0;

    public class LightUnattacher : MonoBehaviour
    {
        public Light light;
        private void OnDestroy()
        {
            light.transform.SetParent(null);
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

    // 7 bag implementation here
    // the pieces are not random everytime we launch the game because the seed is the same

    public int[] nums;
    public int numsLeft;
    // public double p = Random.Range(0f, 1f);

    public void initNums(int n)
    {
        nums = new int[n];
        for (int i = 0; i < n; i++)
        {
            nums[i] = i;

            numsLeft = n;
        }

        shuffle(nums);
    }

    public void shuffle(int[] a)
    {
        for (int i = a.Length-1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i);
            int temp = a[j];
            a[j] = a[i];
            a[i] = temp;
        }
    }

    public int nextInt()
    {
        // If there are no numbers left, just return 0
        if (numsLeft == 0) return 7;

        // Get a random index amongst the remaining numbers
        // int index = (int) (numsLeft * p);

        // The number of numbers left is decreased and numsLeft now
        // references the last index in the array in which a unique value
        // can be returned
        numsLeft--;

        // Save the return value
        //int temp = nums[index];

        // Overwrite the element in position index with the
        // element in the last position containing a number
        // that has yet to be returned
        ///nums[index] = nums[numsLeft];

        // Return the selected value
        return nums[numsLeft];
    }

    public int randomizer()
    {
        int rgn = nextInt();
        if (rgn == 7)
        {
            initNums(7);
            rgn = nextInt();
        }

        return rgn;
    }

    public List<GameObject> queue = new List<GameObject>(5);
    public Vector3 initialPosition = new Vector3(18, 4.5f, 4);
    public void makeQueue()
    {   
        GameObject pieceOne = Instantiate(prefabsOfPieces[randomizer()]);
        GameObject pieceTwo = Instantiate(prefabsOfPieces[randomizer()]);
        GameObject pieceThree = Instantiate(prefabsOfPieces[randomizer()]);
        GameObject pieceFour = Instantiate(prefabsOfPieces[randomizer()]);
        GameObject pieceFive = Instantiate(prefabsOfPieces[randomizer()]);

        queue.Add(pieceOne);
        queue.Add(pieceTwo);
        queue.Add(pieceThree);
        queue.Add(pieceFour);
        queue.Add(pieceFive);

        pieceOne.transform.position = initialPosition;
        pieceTwo.transform.position = initialPosition + Vector3.down*4;
        pieceThree.transform.position = initialPosition + Vector3.down * 8;
        pieceFour.transform.position = initialPosition + Vector3.down * 12;
        pieceFive.transform.position = initialPosition + Vector3.down * 16;
    }
    public int n = 1;
    public void swapHold()
    {
        GameObject temp;

        if (n == 1) // first time THIS WORKS BTW WOOOOHOOOO
        {
            holdPiece = currentPiece;
            holdPiece.transform.position = new Vector3(-5, 4.5f, 4);
            makeAnotherOne();
            n++;
        } else if (n == 2) // from then on
        {
            temp = holdPiece;
            holdPiece = currentPiece;
            currentPiece = temp;
            holdPiece.transform.position = new Vector3(-5, 4.5f, 4); // magic number that is the position of the hold piece
            currentPiece.transform.position = new Vector3(4.5f, 4.5f, 4); // or transform.position? this is the position of piecemaker
            // add an update current piece funtion so the current piece changes
        }
    }

    public void makeAnotherOne()
    {
        GameObject newOne = Instantiate(queue[0]);
        newOne.transform.position = transform.position;
        newOne.transform.SetParent(transform);
        listOfObjects.Add(newOne);

        for (int i = 0; i < 4; i++)
        {
            pieceLight[i].transform.SetParent(newOne.transform.GetChild(i));
            pieceLight[i].transform.localPosition = Vector3.zero;
            pieceLight[i].transform.SetParent(newOne.transform);
        }

        currentPiece = newOne;
        updateQueue();
        printQueue();
        //currentPiece.board = board;
    }

    public void updateQueue()
    {
        Destroy(queue[0]);
        queue.Remove(queue[0]);
        GameObject temp = Instantiate(prefabsOfPieces[randomizer()]);
        queue.Add(temp);
    }

    public void printQueue()
    {
        queue[0].transform.position += Vector3.up * 4;
        queue[1].transform.position += Vector3.up * 4;
        queue[2].transform.position += Vector3.up * 4;
        queue[3].transform.position += Vector3.up * 4;
        queue[4].transform.position = initialPosition + Vector3.down * 16; 
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

        foreach(Vector2Int mino in minoCoords())
        {
            if(isMinoOOB(mino.x,mino.y)) return true;
        }
        return false;
    }

    public bool isMinoOOB(int boardXPos, int boardYPos)
    {
        return boardXPos < 0 ||
            //boardYPos < 0 ||
            boardXPos >= board.width||
            boardYPos >= board.height;
    }

    Vector2Int[] minoCoords() => minoCoords(currentPiece.transform);
    Vector2Int[] minoCoords(Transform pieceTransform)
    {
        Vector2Int[] coords = new Vector2Int[4];
        int index = 0;
        for(int i = 0; i < pieceTransform.childCount; ++i)
        {
            Transform mino = pieceTransform.GetChild(i);
            if (!mino.GetComponent<Light>())
            {
                int boardXPos = (int)(mino.position.x - 0.5f), boardYPos = (int)((mino.position.y - 3.5f) * -1);
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
        if (isColliding()) return false;
        bool success = true;
        Vector2Int[] MinosPos = minoCoords();

        while (currentPiece.transform.childCount > 0)
        {
            Transform mino = currentPiece.transform.GetChild(0);
            if (!mino.GetComponent<Light>())
            {
                int boardXPos = (int)(mino.position.x - 0.5f), boardYPos = (int)((mino.position.y - 3.5f) * -1);
                if (isMinoOOB(boardXPos, boardYPos)) success = false;
                else board.objectMatrix[boardYPos][boardXPos] = mino.gameObject;
            }

            mino.SetParent(board.transform);
        }
        Destroy(currentPiece.gameObject);
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
        double iterationDelay = 1000; 

        if (fallCounter >= iterationDelay)
        {
            currentPiece.transform.position += Vector3.down;
            if (findBottom() == board.height)
            {
                currentPiece.transform.position += Vector3.up;
                ImprintPiece(); // add delay later
            }

            else if (isColliding()) {
                currentPiece.transform.position += Vector3.up;
                ImprintPiece();
            }

            fallCounter -= (long)iterationDelay;
        }
    }

    public enum PieceCollision { none, blockedV, blockedH, oobLeft, oobRight, oobTop, oobBottom };

    public KeyCode currentKey = KeyCode.None;
    public PieceCollision kindOfPieceCollision() // is piece colliding at a specific position
    {
        // if (isPieceOOB(currentPiece)) return true;

        for (int i = 0; i < currentPiece.transform.childCount; i++)
        {
            int x = (int)(currentPiece.transform.GetChild(i).position.x - 0.5);
            int y = (int)((currentPiece.transform.GetChild(i).position.y - 3.5) * -1);
            if (y < 0) return PieceCollision.oobTop;
            if (y >= board.height) return PieceCollision.oobBottom;
            if (x < 0) return PieceCollision.oobLeft;
            if (x >= board.width) return PieceCollision.oobRight;

            if (board.objectMatrix[y][x] != null)
            {
                if (currentKey == KeyCode.LeftArrow || currentKey == KeyCode.RightArrow) return PieceCollision.blockedH;
                return PieceCollision.blockedV;
            }
        }
        return PieceCollision.none;
    }

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

    public void placePieceIfCollision()
    {
        int bottomOfPiece = findBottom();
        PieceCollision col = kindOfPieceCollision();
        if (col == PieceCollision.blockedV)
        {
            currentPiece.transform.position += Vector3.up;
            ImprintPiece();
        } else if (col == PieceCollision.oobBottom)
        {   currentPiece.transform.position += Vector3.up;
            ImprintPiece();
        } else if (col == PieceCollision.blockedH)
        {
            if (currentKey == KeyCode.LeftArrow)
            {
                currentPiece.transform.position += Vector3.right;
            } else currentPiece.transform.position += Vector3.left;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        then = UTCMS();
        debugText = GameObject.Find("Debug Text")?.GetComponent<Text>();
        //Random.InitState(System.Environment.TickCount);
        initNums(7);
        makeQueue();
        makeAnotherOne();
    }

    [TextArea(4, 4)]
    public string debugPosition;
    void Update()
    {
        debugPosition = string.Join("\n", minoCoords());
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

        //KeyValuePair<KeyCode, Vector3>[] keyMoves = new KeyValuePair<KeyCode, Vector3>[]
        //{
        //    new KeyValuePair<KeyCode, Vector3> (KeyCode.DownArrow, Vector3.down),
        //    new KeyValuePair<KeyCode, Vector3> (KeyCode.UpArrow, Vector3.up),
        //    new KeyValuePair<KeyCode, Vector3> (KeyCode.LeftArrow, Vector3.left),
        //    new KeyValuePair<KeyCode, Vector3> (KeyCode.RightArrow, Vector3.right),
        //};

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

            [KeyCode.Space] = () => ImprintPiece(), // <- thats a function
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
                if (isPieceOOB(currentPiece) || isColliding() ) currentPiece.transform.position += Vector3.left;
            },
        };

        currentKey = KeyCode.None;

        //for (int i = 0; i < keyMoves.Length; i++)
        //{
        //    if (Input.GetKey(keyMoves[i].Key))
        //    {
        //        currentKey = keyMoves[i].Key;
        //        currentPiece.transform.position += keyMoves[i].Value;
        //        if (isPieceOOB(currentPiece)) {
        //            currentPiece.transform.position -= keyMoves[i].Value;
        //        }
        //        keyTimer = keyDelay;
        //    }
        //}

        //placePieceIfCollision();

        foreach (KeyValuePair<KeyCode, Action> kvp in controls)
        {
            if (Input.GetKey(kvp.Key))
            {
                kvp.Value.Invoke();
                keyTimer = keyDelay;
            }
        }
        //ClearLines();
    }
}
