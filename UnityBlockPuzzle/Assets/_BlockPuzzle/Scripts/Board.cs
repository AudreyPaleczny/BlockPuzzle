using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width = 10;
    public int height = 23;
    public GameObject normalBlock;
    public Vector3 blockSize = Vector3.one;

    char[][] array;
    [TextArea(10, 20)]
    public string initial;

    public GameObject[][] objectMatrix;

    // Start is called before the first frame update
    void Start()
    {
        Score.Value = 0;
        objectMatrix = new GameObject[height][];

        for (int i = 0; i < objectMatrix.Length; ++i) objectMatrix[i] = new GameObject[width];


        array = new char[height][];

        for(int i = 0; i<array.Length; ++i)
        {
            array[i] = new char[width];
        }

        int r = 0, c = 0;

        for(int i = 0; i<initial.Length; ++i)
        {
            if(initial[i] == '\n')
            {
                r++;
                c = -1;
            }
            if(r < height && c < width && initial[i] != ' ' && initial[i] != '\n')
            {
                array[r][c] = initial[i];
            }
            c++;
        }

        //string output = "";
        //for(int row = 0; row<height; ++row)
        //{
        //    for (int col = 0; col < width; ++col)
        //    {
        //        if(array[row][col] != 0)
        //        {
        //            output += array[row][col];
        //        }
        //        else
        //        {
        //            output += '.';
        //        }
        //    }
        //    output += '\n';
        //}
        //Debug.Log(output);

        for (int col = 0; col<width; col++)
        {
            for (int row = 0; row < height; row++)
            {
                if(array[row][col] != 0)
                {
                    GameObject go = Instantiate(normalBlock);
                    go.transform.SetParent(transform);
                    go.transform.localPosition = new Vector3(col * blockSize.x, row * blockSize.y, 0);
                }
                
            }
        }

        controls[KeyCode.Escape] = () =>
        {
            Application.Quit();
            UnityEditor.EditorApplication.isPlaying = false;
            //this is how you quit in the editor
        };

        //controls[KeyCode.LeftArrow] = () =>
        //{
        //    Debug.Log("Go left");
        //};
    }

    Dictionary<KeyCode, Action> controls = new Dictionary<KeyCode, Action>();


    // Update is called once per frame
    void Update()
    {
        foreach(KeyValuePair<KeyCode, Action> kvp in controls)
        {
            //see if the key is being pressed
            if (Input.GetKey(kvp.Key))
            {
                kvp.Value();
            }
        }
    }
}
