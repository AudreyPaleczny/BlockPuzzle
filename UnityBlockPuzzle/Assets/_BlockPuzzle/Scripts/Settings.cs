using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Settings : MonoBehaviour
{
    GameObject particleButton;
    GameObject noisesButton;
    GameObject musicButton;
    bool noisesOn;
    bool particleOn;
    bool musicOn;
    public void particleOnOff(GameObject txt)
    {
        if (particleOn)
        {
            PlayerPrefs.SetString("Particle", "Off");
            txt.GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString("Particle");
            particleOn = false;
            PlayerPrefs.SetInt("Particle", 1);
        }
        else
        {
            PlayerPrefs.SetString("Particle", "On");
            txt.GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString("Particle");
            particleOn = true;
            PlayerPrefs.SetInt("Particle", 0);
        }
    }

    public void noisesOnOff(GameObject txt)
    {
        if (noisesOn)
        {
            PlayerPrefs.SetString("Noise", "Off");
            txt.GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString("Noise");
            noisesOn = false;
            PlayerPrefs.SetInt("Noise", 1);
        }
        else
        {
            PlayerPrefs.SetString("Noise", "On");
            txt.GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString("Noise");
            noisesOn = true;
            PlayerPrefs.SetInt("Noise", 0);
        }
    }
    public void musicOnOff(GameObject txt)
    {
        if (musicOn)
        {
            PlayerPrefs.SetString("Music", "Off");
            txt.GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString("Music");
            musicOn = false;
            PlayerPrefs.SetInt("Music", 1);
        }
        else
        {
            PlayerPrefs.SetString("Music", "On");
            txt.GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString("Music");
            musicOn = true;
            PlayerPrefs.SetInt("Music", 0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        noisesOn = (PlayerPrefs.GetInt("Noise") == 0);
        particleOn = (PlayerPrefs.GetInt("Particle") == 0);
        musicOn = (PlayerPrefs.GetInt("Music") == 0);
        PlayerPrefs.SetString("Particle", particleOn ? "On" : "Off");
        PlayerPrefs.SetString("Noise", noisesOn ? "On" : "Off");
        PlayerPrefs.SetString("Music", musicOn ? "On" : "Off");
        GameObject.Find("noisesText").GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString("Noise");
        GameObject.Find("particlesText").GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString("Particle");
        GameObject.Find("musicText").GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString("Music");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
