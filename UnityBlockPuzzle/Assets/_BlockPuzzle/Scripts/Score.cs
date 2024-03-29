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
    private int _score, _level, _linesCleared, _combo;

    public int value
    {
        get
        {
            return _score;
        }

        set
        {
            bool isNewValue = _score != value;
            _score = value;
            if (isNewValue) PlayerPrefs.SetInt("Score", _score);
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
            bool isNewValue = _level != value;
            _level = value;
            //if (isNewValue) PlayerPrefs.SetInt("Level", _level);
            GameObject.Find("LevelText").GetComponent<Text>().text = "Level: " + _level.ToString();
        }
    }

    public int linesCleared
    {
        get
        {
            return _linesCleared;
        }
        set
        {
            bool isNewValue = _linesCleared != value;
            _linesCleared = value;
            if (isNewValue) PlayerPrefs.SetInt("LinesCleared", _linesCleared);
        }
    }

    public int combo
    {
        get
        {
            return _combo;
        }
        set
        {
            bool isNewValue = _combo != value;
            _combo = value;
            if (isNewValue) PlayerPrefs.SetInt("Combo", _combo);
        }
    }

    public static int Level
    {
        get => Instance.level;
        set => Instance.level = value;
    }

    public static int LinesCleared
    {
        get => Instance.linesCleared;
        set => Instance.linesCleared = value;
    }

    public static int Value
    {
        get => Instance.value;
        set => Instance.value = value;
    }

    public static int Combo
    {
        get => Instance.combo;
        set => Instance.combo = value;
    }

    // Awake called right after add component
    void Awake()
    {
        _score = PlayerPrefs.GetInt("Score");
        _level = PlayerPrefs.GetInt("Level");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
