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
    public Piece currentPiece;

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

    public void makeAnotherOne()
    {
        int p = Random.Range(0, prefabsOfPieces.Count);

        GameObject newOne = Instantiate(prefabsOfPieces[p]);
        newOne.transform.position = transform.position;
        newOne.transform.SetParent(transform);
        listOfObjects.Add(newOne);

        for (int i = 0; i < 4; i++)
        {
            pieceLight[i].transform.SetParent(newOne.transform.GetChild(i));
            pieceLight[i].transform.localPosition = Vector3.zero;
            pieceLight[i].transform.SetParent(newOne.transform);
        }

        currentPiece = newOne.GetComponent<Piece>();
        //currentPiece.board = board;
    }

    // Start is called before the first frame update
    void Start()
    {

        makeAnotherOne();
    }

    // Update is called once per frame
    void Update()
    {
        if (keyTimer > 0)
        {
            keyTimer -= Time.deltaTime;
            if (keyTimer > 0)
            {
                return;
            }
        }

        KeyValuePair<KeyCode, Vector3>[] keyMoves = new KeyValuePair<KeyCode, Vector3>[]
        {
            new KeyValuePair<KeyCode, Vector3> (KeyCode.DownArrow, Vector3.down),
            new KeyValuePair<KeyCode, Vector3> (KeyCode.UpArrow, Vector3.up),
            new KeyValuePair<KeyCode, Vector3> (KeyCode.LeftArrow, Vector3.left),
            new KeyValuePair<KeyCode, Vector3> (KeyCode.RightArrow, Vector3.right),
        };

        for (int i = 0; i < keyMoves.Length; i++)
        {
            if (Input.GetKey(keyMoves[i].Key))
            {
                currentPiece.transform.position += keyMoves[i].Value;
                keyTimer = keyDelay;
            }
        }

        if (Input.GetKey(KeyCode.Space))
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
            keyTimer = keyDelay;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            currentPiece.transform.Rotate(0, 0, -90);
            keyTimer = keyDelay;
        }
        if (Input.GetKey(KeyCode.P))
        {
            while(currentPiece.transform.childCount > 0)
            {
                Transform mino = currentPiece.transform.GetChild(0);

                if (!mino.GetComponent<Light>()) {
                    int boardXPos = (int)(mino.position.x - 0.5f), boardYPos = (int)((mino.position.y - 3.5f) * -1);
                    board.objectMatrix[boardYPos][boardXPos] = mino.gameObject;
                }

                mino.SetParent(board.transform);
            }
            Destroy(currentPiece.gameObject);
            makeAnotherOne();
            keyTimer = keyDelay;
        }
    }
}
