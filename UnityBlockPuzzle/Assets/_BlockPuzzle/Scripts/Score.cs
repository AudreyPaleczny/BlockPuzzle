using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    //singleton
    private static Score _instance;

    public static Score Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new GameObject("Score").AddComponent<Score>();
            }
            return _instance;
        }
    }

    private int _score;

    public int value
    {
        get
        {
            return _score;
        }

        set
        {
            _score = value;
            PlayerPrefs.SetInt("Score", _score);
        }
    }

    public static int Value
    {
        get => Instance.value;
        set => Instance.value = value;
        
    }

    // Awake called right after add component
    void Awake()
    {
        _score = PlayerPrefs.GetInt("Score");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
