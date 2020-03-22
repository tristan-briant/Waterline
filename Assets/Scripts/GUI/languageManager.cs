using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class languageManager : MonoBehaviour {

    [TextArea]
    public string text_fr = "";
    [TextArea]
    public string text_en = "";
    GameObject LVM;

    // Use this for initialization
    void Awake() {
        LVM = GameObject.Find("LevelManager");
        SwitchLanguage();
    }

    public string GetText()
    {
        switch (LVM.GetComponent<LevelManager>().language)
        {
            case "english":
            case "English":
            default:
                return text_en;
            case "french":
            case "French":
                return text_fr;
        }
    }

    public void SwitchLanguage()
    {
         switch (LVM.GetComponent<LevelManager>().language) {
            case "english":
            case "English":
            default:
                GetComponent<Text>().text = text_en;
                break;
            case "french":
            case "French":
                GetComponent<Text>().text = text_fr;
                break;
        }
    }
	
}
