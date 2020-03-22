using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class FlushManager : BaseFrontier {

    protected Animation Animation;
    //gameController gc; // le moteur du jeu à invoquer parfois


    override public void Awake()
    {
        Animation = GetComponent<Animation>();
        audios = GameObject.Find("PlaygroundHolder").GetComponents<AudioSource>();
    }

    public override void OnClick()
    {

        //Animation.Play("FlushAnimation");
        GetComponent<Animator>().SetTrigger("flush");
        GameObject.Find("GameController").GetComponent<GameController>().ResetComponent();

        audios[6].Play();

    }

    public void HighLightFlush() {
        GetComponent<Animator>().SetTrigger("showHand");
    }

    public override void Rotate()
    {
        if (dir == 3)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = Vector3.one;
    }
}
