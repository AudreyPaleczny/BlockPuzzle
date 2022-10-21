using UnityEngine;
public class JsonTest : MonoBehaviour
{
    [System.Serializable]
    public class Controls
    {
        public KeyCode up = KeyCode.W, left = KeyCode.A, down = KeyCode.S, right = KeyCode.D;
    }
    [ContextMenuItem(nameof(Test), nameof(Test))]
    public Controls test;
    void Test()
    {
        Controls controls = new Controls
        {
            up = KeyCode.I,
            left = KeyCode.I,
            down = KeyCode.I,
            right = KeyCode.I
        };
        string json = JsonUtility.ToJson(controls);
        Debug.Log(json);
        Controls otherControls = new Controls();
        Debug.Log(JsonUtility.ToJson(otherControls));
        JsonUtility.FromJsonOverwrite(json, otherControls);
        Debug.Log(JsonUtility.ToJson(otherControls));
    }
}