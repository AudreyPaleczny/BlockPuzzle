using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMove : MonoBehaviour
{
    float keyTimer = 0.0f;
    const float keyDelay = 1f/8;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(keyTimer > 0)
        {
            keyTimer -= Time.deltaTime;
            if(keyTimer > 0)
            {
                return;
            }
        }

        KeyValuePair<KeyCode, Vector3>[] keyMoves = new KeyValuePair<KeyCode, Vector3>[]
        {
            new KeyValuePair<KeyCode, Vector3> (KeyCode.DownArrow, Vector3.down),
            new KeyValuePair<KeyCode, Vector3> (KeyCode.UpArrow, Vector3.up),
            new KeyValuePair<KeyCode, Vector3> (KeyCode.LeftArrow, Vector3.left),
            new KeyValuePair<KeyCode, Vector3> (KeyCode.RightArrow, Vector3.right)
        };

        for(int i = 0; i < keyMoves.Length; i++)
        {
            if (Input.GetKey(keyMoves[i].Key))
            {
                transform.position += keyMoves[i].Value;
                keyTimer = keyDelay;
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Debug.Log("Child " + i + " " + transform.GetChild(i).position);
            }
        }
    }
}
