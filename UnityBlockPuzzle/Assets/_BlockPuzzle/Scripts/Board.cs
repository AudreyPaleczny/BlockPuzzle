using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    public int width = 10;
    public int height = 23;
    public GameObject normalBlock;
    public Vector3 blockSize = Vector3.one;
    public bool isGameLoaded = false;
    public TMP_Text countdownText;
    public UnityEvent onStart;

    char[][] array;
    [TextArea(10, 20)]
    public string initial;

    public GameObject[][] objectMatrix;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(StartGameCountdown());
        //StartGame();
    }

    public IEnumerator StartGameCountdown()
    {
        if (PlayerPrefs.GetInt("Noise") == 1)
        {
            Debug.Log("3"); 
            yield return new WaitForSeconds(1);
            Debug.Log("2"); 
            countdownText.text = "2";
            yield return new WaitForSeconds(1);
            Debug.Log("1");
            countdownText.text = "1";
            yield return new WaitForSeconds(1);
            Debug.Log("boomshakalaka");
            countdownText.text = "Start!";
            onStart.Invoke();
            yield return new WaitForSeconds(0.5f);
            countdownText.text = "";
            StartGame();
        } else
        {
            Debug.Log("3");
            Noisy.PlaySound("lowerStartgame");
            yield return new WaitForSeconds(1);
            Debug.Log("2");
            Noisy.PlaySound("lowerStartgame");
            countdownText.text = "2";
            yield return new WaitForSeconds(1);
            Debug.Log("1");
            Noisy.PlaySound("lowerStartgame");
            countdownText.text = "1";
            yield return new WaitForSeconds(1);
            Debug.Log("boomshakalaka");
            countdownText.text = "Start!";
            Noisy.PlaySound("startgame");
            onStart.Invoke();
            yield return new WaitForSeconds(0.5f);
            countdownText.text = "";
            StartGame();
        }
    }


    void StartGame()
    { 
        Score.Value = 0;
        Score.Level = PlayerPrefs.GetInt("Level");
        Score.LinesCleared = 0;
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
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//this is how you quit in the editor
#endif

        };

        isGameLoaded = true;
    }

    Dictionary<KeyCode, Action> controls = new Dictionary<KeyCode, Action>();

    void Update()
    {
        foreach (KeyValuePair<KeyCode, Action> kvp in controls)
        {
            //see if the key is being pressed
            if (Input.GetKey(kvp.Key))
            {
                kvp.Value();
            }
        }
    }
}

    


