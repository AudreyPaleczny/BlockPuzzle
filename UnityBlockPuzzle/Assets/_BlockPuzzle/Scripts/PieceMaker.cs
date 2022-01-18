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

    public void makeAnotherOne()
    {
        int rgn = nextInt();
        if (rgn == 7)
        {
            initNums(7);
            rgn = nextInt();
        }

        GameObject newOne = Instantiate(prefabsOfPieces[rgn]);
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
        //currentPiece.board = board;
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

    //TODO: need to test more
    public bool isColliding()
    {
        Vector2Int[] minoPos = minoCoords();
        //4 minos per piece
        for(int i = 0; i < 4; i++)
        {
            int x = minoPos[i].x;
            int y = minoPos[i].y;

            if (y < 0) return true;

            if (board.objectMatrix[y][x] != null)
            {
                Debug.Log("Is Colliding");
                return true;
            }
        }
        
        return false;
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

    public bool ImprintPiece()
    {
        if (isColliding()) return false;
        bool success = true;
        //if (isPieceOOB(currentPiece)) return;
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

    public void fallCounterUpdate()
    {
        // double iterationDelay = ((11 - level) * 0.1) * 1000;  // [seconds] used to be 0.05
        double iterationDelay = 1000; 

        if (fallCounter >= iterationDelay)
        {
            currentPiece.transform.position += Vector3.down;
            fallCounter -= (long)iterationDelay;
        }
    }

    public bool isPieceCaSP(int position) // is piece colliding at a specific position
    {
        if (isPieceOOB(currentPiece)) return true;
        for (int i = 0; i < currentPiece.transform.childCount; i++)
        {
            int x = (int)(currentPiece.transform.GetChild(i).position.x - 0.5);
            int y = (int)((currentPiece.transform.GetChild(i).position.y - 3.5) * -1);
            if (y<0 || board.objectMatrix[y][x] != null) return true;
        }
        return false;
    }

    public void placePieceIfCollision()
    {
        int bottomOfPiece = (int) currentPiece.transform.position.y;
        if (bottomOfPiece > board.height || isPieceCaSP((int)currentPiece.transform.position.y))
        {
            currentPiece.transform.position += Vector3.up;
            ImprintPiece();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        then = UTCMS();
        debugText = GameObject.Find("Debug Text")?.GetComponent<Text>();
        //Random.InitState(System.Environment.TickCount);
        initNums(7);
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
        fallCounterUpdate();
        placePieceIfCollision();

        KeyValuePair<KeyCode, Vector3>[] keyMoves = new KeyValuePair<KeyCode, Vector3>[]
        {
            new KeyValuePair<KeyCode, Vector3> (KeyCode.DownArrow, Vector3.down),
            new KeyValuePair<KeyCode, Vector3> (KeyCode.UpArrow, Vector3.up),
            new KeyValuePair<KeyCode, Vector3> (KeyCode.LeftArrow, Vector3.left),
            new KeyValuePair<KeyCode, Vector3> (KeyCode.RightArrow, Vector3.right),
        };

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
            [KeyCode.Space] = ()=> ImprintPiece() // <- thats a function
        };

        for (int i = 0; i < keyMoves.Length; i++)
        {
            if (Input.GetKey(keyMoves[i].Key))
            {
                currentPiece.transform.position += keyMoves[i].Value;
                if (isPieceOOB(currentPiece)) currentPiece.transform.position -= keyMoves[i].Value;
                keyTimer = keyDelay;
            }
        }

        foreach(KeyValuePair<KeyCode, Action> kvp in controls)
        {
            if (Input.GetKey(kvp.Key))
            {
                kvp.Value.Invoke();
                keyTimer = keyDelay;
            }
        }
    }
}
