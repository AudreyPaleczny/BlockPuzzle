using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTest : MonoBehaviour
{
    public KeyCode up = KeyCode.I, left = KeyCode.J, down = KeyCode.K, right = KeyCode.L;
    [SerializeField]private int youDontSeeMe = 5;
    public void goUp()
    {
        Debug.Log("up");
    }

    public void goLeft()
    {
        Debug.Log("left");
    }

    public void goDown()
    {
        Debug.Log("down");
    }

    public void goRight()
    {
        Debug.Log("right");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(up))
        {
            goUp();
        }
        if (Input.GetKeyDown(down))
        {
            goDown();
        }
        if (Input.GetKeyDown(left))
        {
            goLeft();
        }
        if (Input.GetKeyDown(right))
        {
            goRight();
        }
    }
}
