using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    GameObject particleButton;
    bool particleOn = true;
    GameObject noisesButton;
    bool noisesOn = true;

    public void particleOnOff(GameObject txt)
    {
        if (particleOn)
        {
            txt.GetComponent<TMPro.TMP_Text>().text = "Off";
            particleOn = false;
        }
        else
        {
            txt.GetComponent<TMPro.TMP_Text>().text = "On";
            particleOn = true;
        }
        Debug.Log("this is supposed to turn off particles");
    }

    public void noisesOnOff(GameObject txt)
    {
        if (noisesOn)
        {
            txt.GetComponent<TMPro.TMP_Text>().text = "Off";
            noisesOn = false;
        }
        else
        {
            txt.GetComponent<TMPro.TMP_Text>().text = "On";
            noisesOn = true;
        }
        Debug.Log("this is supposed to turn off noises");
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
