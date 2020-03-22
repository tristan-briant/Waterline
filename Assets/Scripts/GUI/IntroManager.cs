using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour {

    public LevelManager LVM;
    public GameObject TipButton;
    //public GameObject introScreen;
    GameController gc; // le moteur du jeu à invoquer parfois

    private void Awake()
    {

        LVM = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        gc = (GameController)GameObject.Find("gameController").GetComponent(typeof(GameController)); //find the game engine


        foreach (Transform go in transform)
            GameObject.DestroyImmediate(go.gameObject);

        transform.localPosition = new Vector3(0, 0, 0);
       
        /// Prepare intro if any
        string levelName = LVM.GetPlaygroundName(LVM.currentLevel);
        Object obj = Resources.Load("Intro/" + levelName + "_intro", typeof(GameObject));

        if (obj == null)
        {
            removeIntro();
            TipButton.SetActive(false);
        }
        else
        {
            TipButton.SetActive(true);
            GameObject intro = Instantiate(obj) as GameObject;
            intro.transform.SetParent(gameObject.transform);
            intro.transform.localScale = new Vector3(1, 1, 1);
            intro.transform.localPosition = new Vector3(0, 0, 0);


            if (LVM.currentLevel > LVM.completedLevel)
            {
                StartCoroutine("IntroScreen");
            }
            else
            {
                removeIntro();
            }
        }
    }

    public void Update()
    {
        if(gameObject.GetComponent<Canvas>().enabled) gc.Pause(true);
    }

    public void removeIntro()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
        gc.Pause(false);
    }

    public void PlayIntro() {
        StartCoroutine("IntroScreen");
    }

    public IEnumerator IntroScreen()
    {
        gameObject.GetComponent<Canvas>().enabled = true;
        yield return new WaitForSeconds(40f);
        removeIntro();
    }
}
