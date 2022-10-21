using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class KeyChanger : MonoBehaviour
{
    public string[] keysToBind = null;
    public KeyTest whatToChange;
    public int whichKey;
    public KeyCode GetPressed()
    {
        for(int i = 0; i<512; ++i)
        {
            KeyCode k = (KeyCode)i;
            if (Input.GetKeyDown(k))
            {
                return k;
            }
        }
        return KeyCode.None;
    }

    private void Start()
    {
        keysToBind = new string[] { nameof(whatToChange.up), "left", "down", "right" };
        Debug.Log(string.Join(", ", keysToBind));
        //whatToChange.youDontSeeMe = 500;
        System.Type keyTestType = whatToChange.GetType();
        FieldInfo evilVoodooMagic = keyTestType.GetField("youDontSeeMe", BindingFlags.Instance | BindingFlags.NonPublic);
        Debug.Log(evilVoodooMagic.GetValue(whatToChange));
        evilVoodooMagic.SetValue(whatToChange, 500);
    }

    // Update is called once per frame
    void Update()
    {
        KeyCode k = GetPressed();
        if(k != KeyCode.None)
        {
            Debug.Log(k);
            System.Type keyTestType = whatToChange.GetType();
            string nameOfKeyVar = keysToBind[whichKey];
            FieldInfo keyField = keyTestType.GetField(nameOfKeyVar);
            if(keyField != null)
            {
                keyField.SetValue(whatToChange, k);
            }
        }
    }
}
