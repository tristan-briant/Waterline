using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class optionManager : MonoBehaviour {

    LevelManager LVM;

	void Awake () {
        LVM = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

    }

    public void Start()
    {
        GameObject.Find("SliderVolume").GetComponent<Slider>().normalizedValue = LVM.Volume;
    }

    public void SetLanguage(string language)
    {
        LVM.Language=language;
        PlayerPrefs.SetString("Language",language);

        // updte text already present in scene
        foreach (Text t in FindObjectsOfType<Text>()) {
            if (t.transform.GetComponent<languageManager>())
            {
                t.transform.GetComponent<languageManager>().SwitchLanguage();
            }
        }
    }

    public void ResetGame()
    {
        PlayerPrefs.SetInt("Level Completed",0);
        LVM.completedLevel = 0;
        LVM.ResetGame();
    }

    bool firstTime = true;

    public void SetVolume() {
        //Debug.Log("bip");
        float vol = GameObject.Find("SliderVolume").GetComponent<Slider>().normalizedValue;
        PlayerPrefs.SetFloat("Volume", vol);
        LVM.Volume = vol;
        AudioListener.volume = LVM.Volume;
        AudioSource bip=GetComponent<AudioSource>();
        if(!bip.isPlaying && !firstTime) bip.Play();
        firstTime = false;
    }

}
