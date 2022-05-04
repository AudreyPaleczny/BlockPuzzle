using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Piece
{
    public class StartScreen : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

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
            AsyncOperation a = SceneManager.LoadSceneAsync("Board Scene");
            a.completed += f;
        }

        public void StartClassic() => startGame(whatToDoOnClassic);

        public void StartMultiplayer() => startGame(whatToDoOnMultiplayer);

    }
}


