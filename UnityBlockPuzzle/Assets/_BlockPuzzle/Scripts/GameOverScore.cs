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
        PlayerPrefs.SetInt("DeathCount", PlayerPrefs.GetInt("DeathCount") + 1);
        scoreText = GetComponent<Text>();
        int scoreValue = PlayerPrefs.GetInt("Score");
        if (PlayerPrefs.GetInt("HighScore") < scoreValue)
        {
            PlayerPrefs.SetInt("HighScore", scoreValue);
            scoreText.text = "HIGHSCORE: " + scoreValue;
        }
        else scoreText.text = "Score: " + scoreValue;
        
    }

}
