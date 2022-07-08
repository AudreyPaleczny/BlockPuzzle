using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Piece
{
    public class StartScreen : MonoBehaviour
    {
        private void whatToDoOnClassic(AsyncOperation a)
        {
            PieceMaker p = FindObjectOfType<PieceMaker>();
            Debug.Log(p);
            p.numberOfPlayers = 1;
        }
        private void whatToDoOnMultiplayer(AsyncOperation a)
        {
            PieceMaker p = FindObjectOfType<PieceMaker>();
            Debug.Log(p);
            p.numberOfPlayers = 2;
        }

        private void startGame(System.Action<AsyncOperation> f)
        {
            AsyncOperation a = SceneManager.LoadSceneAsync("Minimal Board Scene");
            a.completed += f;
        }

        public void StartClassic() => startGame(whatToDoOnClassic);

        public void StartMultiplayer() => startGame(whatToDoOnMultiplayer);

        public void changeLevel()
        {
            int lvl = (int)GameObject.Find("Slider").GetComponent<Slider>().value;
            PlayerPrefs.SetInt("Level", lvl);
        }
        public void changeText()
        {
            // Debug.Log(GameObject.Find("slidertext").GetComponent<Text>().text);
            GameObject.Find("slidertext").GetComponent<Text>().text = "Level: " + (int)GameObject.Find("Slider").GetComponent<Slider>().value;
        }

        public void Stats()
        {
            Debug.Log("no worky");
        }

        public void Themes()
        {
            Debug.Log("no worky");
        }

        public void Start()
        {
            Noisy.PlaySound("Start noise");
            PlayerPrefs.SetInt("Level", 0);
        }

    }
}


