using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Keybinds : MonoBehaviour
{
    public TMPro.TMP_Text button_label;
    public Button[] singlePlayerButtons;

    public class SinglePlayerControls
    {
        public KeyCode
            left = KeyCode.LeftArrow,
            down = KeyCode.DownArrow,
            right = KeyCode.RightArrow,
            hardDrop = KeyCode.Space,
            rotateLeft = KeyCode.Z,
            rotateLeft2 = KeyCode.UpArrow,
            rotateRight = KeyCode.X,
            hold = KeyCode.C;
    }
    public KeyCode GetPressed()
    {
        for (int i = 0; i < 512; ++i)
        {
            KeyCode k = (KeyCode)i;
            if (Input.GetKeyDown(k))
            {
                return k;
            }
        }
        return KeyCode.None;
    }

    private TMPro.TMP_Text ButtonTextCompGivenIndex(int idx) { return singlePlayerButtons[idx].GetComponentInChildren<TMPro.TMP_Text>(); }
    struct keyBinding
    {
        public string name;
        public string initial;
        public keyBinding(string name, string initial)
        {
            this.name = name;
            this.initial = initial;
        }

        public void loadKeyDataFromPlayerPrefsIntoUI(TMPro.TMP_Text text)
        {
            if (PlayerPrefs.HasKey(name)) { text.text = PlayerPrefs.GetString(name); }
            else
            {
                text.text = initial;
                PlayerPrefs.SetString(name, initial);
            }
        }
    }

    private static Dictionary<Piece.playerActions, KeyCode> defaultKeyBinding = new Dictionary<Piece.playerActions, KeyCode>() {
        [Piece.playerActions.SingleMoveLeft] = KeyCode.LeftArrow,
        [Piece.playerActions.SingleMoveRight] = KeyCode.RightArrow,
        [Piece.playerActions.SingleSoftDrop] = KeyCode.DownArrow,
        [Piece.playerActions.SingleHardDrop] = KeyCode.Space,
        [Piece.playerActions.SingleRotateLeft] = KeyCode.Z,
        [Piece.playerActions.SingleRotateRight] = KeyCode.X,
        [Piece.playerActions.SingleHold] = KeyCode.C,

        [Piece.playerActions.Coop1MoveLeft] = KeyCode.A,
        [Piece.playerActions.Coop1MoveRight] = KeyCode.D,
        [Piece.playerActions.Coop1SoftDrop] = KeyCode.S,
        [Piece.playerActions.Coop1HardDrop] = KeyCode.Space,
        [Piece.playerActions.Coop1RotateLeft] = KeyCode.Q,
        [Piece.playerActions.Coop1RotateRight] = KeyCode.E,
        [Piece.playerActions.Coop1Hold] = KeyCode.C,

        [Piece.playerActions.Coop2MoveLeft] = KeyCode.LeftArrow,
        [Piece.playerActions.Coop2MoveRight] = KeyCode.RightArrow,
        [Piece.playerActions.Coop2SoftDrop] = KeyCode.DownArrow,
        [Piece.playerActions.Coop2HardDrop] = KeyCode.Period,
        [Piece.playerActions.Coop2RotateLeft] = KeyCode.K,
        [Piece.playerActions.Coop2RotateRight] = KeyCode.L,
        [Piece.playerActions.Coop2Hold] = KeyCode.Comma
    };

    /*
     * Coop1RotateLeft,
        Coop1RotateRight,
        Coop1HardDrop,
        Coop1Hold,
        Coop1SoftDrop,
        Coop1MoveLeft,
        Coop1MoveRight,

        Coop2RotateLeft,
        Coop2RotateRight,
        Coop2HardDrop,
        Coop2Hold,
        Coop2SoftDrop,
        Coop2MoveLeft,
        Coop2MoveRight,
     */

    private static keyBinding[] singleControlNames = new keyBinding[]
    {
        new keyBinding(Piece.playerActions.SingleMoveLeft.ToString(), KeyCode.LeftArrow.ToString()),
        new keyBinding(Piece.playerActions.SingleMoveRight.ToString(), KeyCode.RightArrow.ToString()),
        new keyBinding(Piece.playerActions.SingleSoftDrop.ToString(), KeyCode.DownArrow.ToString()),
        new keyBinding(Piece.playerActions.SingleHardDrop.ToString(), KeyCode.Space.ToString()),
        new keyBinding(Piece.playerActions.SingleRotateLeft.ToString(), KeyCode.Z.ToString()),
        new keyBinding(Piece.playerActions.SingleRotateRight.ToString(), KeyCode.X.ToString()),
        new keyBinding(Piece.playerActions.SingleHold.ToString(), KeyCode.C.ToString())
    };

    private static keyBinding[] coop1ControlNames = new keyBinding[]
    {
        new keyBinding(Piece.playerActions.Coop1MoveLeft.ToString(), KeyCode.LeftArrow.ToString()),
        new keyBinding(Piece.playerActions.Coop1MoveRight.ToString(), KeyCode.RightArrow.ToString()),
        new keyBinding(Piece.playerActions.Coop1SoftDrop.ToString(), KeyCode.DownArrow.ToString()),
        new keyBinding(Piece.playerActions.Coop1HardDrop.ToString(), KeyCode.Space.ToString()),
        new keyBinding(Piece.playerActions.Coop1RotateLeft.ToString(), KeyCode.Z.ToString()),
        new keyBinding(Piece.playerActions.Coop1RotateRight.ToString(), KeyCode.X.ToString()),
        new keyBinding(Piece.playerActions.Coop1Hold.ToString(), KeyCode.C.ToString())
    };

    public static Dictionary<Piece.playerActions, KeyCode> loadControls()
    {
        Dictionary<Piece.playerActions, KeyCode> result = new Dictionary<Piece.playerActions, KeyCode>();
        for(int i = 0; i < (int)Piece.playerActions.count; ++i)
        {
            string controlName = ((Piece.playerActions)i).ToString();
            if (!PlayerPrefs.HasKey(controlName))
            {
                //defaults
                result[(Piece.playerActions)i] = defaultKeyBinding[(Piece.playerActions)i];
                continue;
            }
            string keyname = PlayerPrefs.GetString(controlName);
            result[(Piece.playerActions)i] = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyname);
        }
        //foreach(KeyValuePair<Piece.playerActions, KeyCode> kvp in result)
        //{
        //    Debug.Log(kvp.Key + " : " + kvp.Value);
        //}
        return result;
    }

    private void Start()
    {
        for(int i = 0; i<singleControlNames.Length; ++i)
        {
            singleControlNames[i].loadKeyDataFromPlayerPrefsIntoUI(ButtonTextCompGivenIndex(i));           
        }
    }

    public void ChangeMoveLeft()
    {
        button_label = singlePlayerButtons[0].GetComponentInChildren<TMPro.TMP_Text>();
        ChangeButtonText();
    }
    public void ChangeMoveRight()
    {
        button_label = singlePlayerButtons[1].GetComponentInChildren<TMPro.TMP_Text>();
        ChangeButtonText();
    }
    public void ChangeSoftDrop()
    {
        button_label = singlePlayerButtons[2].GetComponentInChildren<TMPro.TMP_Text>();
        ChangeButtonText();
    }
    public void ChangeHardDrop()
    {
        button_label = singlePlayerButtons[3].GetComponentInChildren<TMPro.TMP_Text>();
        ChangeButtonText();
    }
    public void ChangeRotateLeft()
    {
        button_label = singlePlayerButtons[4].GetComponentInChildren<TMPro.TMP_Text>();
        ChangeButtonText();
    }
    public void ChangeRotateRight()
    {
        button_label = singlePlayerButtons[5].GetComponentInChildren<TMPro.TMP_Text>();
        ChangeButtonText();
    }
    public void ChangeHoldPiece()
    {
        button_label = singlePlayerButtons[6].GetComponentInChildren<TMPro.TMP_Text>();
        ChangeButtonText();
    }

    SinglePlayerControls spc = new SinglePlayerControls();
    public void ChangeButtonText() { StartCoroutine(ChangeButtonTextCoroutine()); }
    public IEnumerator ChangeButtonTextCoroutine()
    {
        bool keyHasNotBeenChangedYet = true;
        KeyCode keyPressed = GetPressed();
        long whenToStop = System.Environment.TickCount + 1000 * 3;
        while (keyHasNotBeenChangedYet)
        {
            button_label.text = "Press Any Key";

            keyPressed = GetPressed();
            if(keyPressed != KeyCode.None)
            {
                spc.left = keyPressed;
                string json = JsonUtility.ToJson(spc);
                Debug.Log(json);
                keyHasNotBeenChangedYet = false;
            }
            if(System.Environment.TickCount >= whenToStop)
            {
                yield break;
            }
            yield return null;
        }
        button_label.text = keyPressed.ToString();
    }

    public void ChangeToStartScreen()
    {
        SceneManager.LoadScene("RetroStart");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
