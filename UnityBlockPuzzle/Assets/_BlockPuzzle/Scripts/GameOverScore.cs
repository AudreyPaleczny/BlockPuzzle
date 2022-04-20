using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScore : MonoBehaviour
{
    Text scoreText;
    [ContextMenuItem("setScore", nameof(setScore))]
    public int debugScore;

    public void setScore()
    {
        PlayerPrefs.SetInt("scoreValue", debugScore);
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<Text>();
        int scoreValue = PlayerPrefs.GetInt("Score");
        scoreText.text = scoreText.text + "" + scoreValue;
    }

}
