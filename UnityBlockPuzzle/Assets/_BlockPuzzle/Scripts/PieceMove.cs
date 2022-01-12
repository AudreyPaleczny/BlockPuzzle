using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PieceMove : MonoBehaviour
{
    float keyTimer = 0.0f;
    const float keyDelay = 1f/8;
    public Text debugText;
    public Board board;
    public PieceMaker makePiece;

    // Start is called before the first frame update
    void Start()
    {
        debugText = GameObject.Find("Debug Text")?.GetComponent<Text>();
        //debugText ??= GameObject.Find("Debug Text")?.GetComponent<Text>();
        //if(debugText==null) debugText = GameObject.Find("Debug Text")?.GetComponent<Text>();
        //debugText = GameObject.Find("Debug Text") != null
        //    ? GameObject.Find("Debug Text").GetComponent<Text>()
        //    : null;
    }

    // Update is called once per frame
    void Update()
    {
        //if(keyTimer > 0)
        //{
        //    keyTimer -= Time.deltaTime;
        //    if(keyTimer > 0)
        //    {
        //        return;
        //    }
        //}

        //KeyValuePair<KeyCode, Vector3>[] keyMoves = new KeyValuePair<KeyCode, Vector3>[]
        //{
        //    new KeyValuePair<KeyCode, Vector3> (KeyCode.DownArrow, Vector3.down),
        //    new KeyValuePair<KeyCode, Vector3> (KeyCode.UpArrow, Vector3.up),
        //    new KeyValuePair<KeyCode, Vector3> (KeyCode.LeftArrow, Vector3.left),
        //    new KeyValuePair<KeyCode, Vector3> (KeyCode.RightArrow, Vector3.right),
        //};

        //for(int i = 0; i < keyMoves.Length; i++)
        //{
        //    if (Input.GetKey(keyMoves[i].Key))
        //    {
        //        transform.position += keyMoves[i].Value;
        //        keyTimer = keyDelay;
        //    }
        //}

        //if (Input.GetKey(KeyCode.Space))
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for(int i = 0; i < transform.childCount; i++)
        //    {
        //        //Debug.Log("Child " + i + " " + transform.GetChild(i).position);
        //        sb.Append("Child " + i + " board xPos: " + (transform.GetChild(i).position.x - 0.5) +
        //            " board yPos:" + (transform.GetChild(i).position.y - 3.5)*-1).Append("\n");
        //    }
        //    Debug.Log(sb);
        //    if(debugText) debugText.text = sb.ToString();
        //    keyTimer = keyDelay;
        //}
        //if (Input.GetKey(KeyCode.Z))
        //{
        //    transform.Rotate(0, 0, -90);
        //    keyTimer = keyDelay;
        //}
        //if (Input.GetKey(KeyCode.P))
        //{
        //    for(int i = 0; i < transform.childCount; ++i)
        //    {
        //        Transform mino = transform.GetChild(i);

        //        if (mino.GetComponent<Light>()) continue; // if it's the light, go away

        //        int boardXPos = (int)(mino.position.x - 0.5f), boardYPos = (int)((mino.position.y - 3.5f)*-1);
        //        board.objectMatrix[boardYPos][boardXPos] = mino.gameObject;
        //        mino.SetParent(board.transform);
        //    }
        //    makePiece.makeAnotherOne();
        //    Destroy(gameObject);
        //    keyTimer = keyDelay;
        //}
    }
}
