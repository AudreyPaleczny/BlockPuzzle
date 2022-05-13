using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                _instance = FindObjectOfType<Score>();
                if (_instance != null) return _instance;
                _instance = new GameObject("Score").AddComponent<Score>();
            }
            return _instance;
        }
    }

    public Text scoreText;
    private int _score, _level;

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
            scoreText.text = "Score: " + _score.ToString();
        }
    }

    public int level
    {
        get
        {
            return _level;
        }

        set
        {
            _level = value;
            PlayerPrefs.SetInt("Level", _level);
            scoreText.text = "Level: " + _level.ToString();
        }
    }

    public static int Level
    {
        get => Instance.level;
        set => Instance.level = value;
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
