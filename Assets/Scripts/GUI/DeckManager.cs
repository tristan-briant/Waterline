using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public Text text;

    private void Start()
    {
        DrawDeck();
    }

    public void DrawDeck()
    {
        bool designerMode =LevelManager.designerMode;

        if (designerMode)
        {
            foreach (Transform slot in transform)
            {
                slot.gameObject.SetActive(true);
                slot.GetComponent<CreateComponent>().designermode = true;

                if (slot.childCount > 1)
                {
                    slot.GetChild(1).GetComponent<BaseComponent>().enabled = false;
                    slot.GetComponentInChildren<BaseComponent>().StartCoroutine("Awake");
                }

            }

            transform.parent.parent.parent.GetComponent<Animator>().SetTrigger("HideImmediate");
            transform.parent.Find("Toggle").gameObject.SetActive(true);
        }
        else
        {
            int count = 0;
            foreach (Transform slot in transform)
            {
                if (slot.childCount == 1 || slot.GetChild(1).name.Contains("empty"))
                {
                    slot.gameObject.SetActive(false);
                }
                else
                {
                    slot.gameObject.SetActive(true);
                    slot.GetChild(1).GetComponent<BaseComponent>().enabled = false;
                    slot.GetComponentInChildren<BaseComponent>().StartCoroutine("Awake");
                    slot.GetComponent<CreateComponent>().designermode = false;
                    count++;
                }
            }

            ResetTrigger();
            transform.parent.parent.parent.GetComponent<Animator>().SetTrigger("HideImmediate");
            if (count > 0)
                transform.parent.parent.parent.GetComponent<Animator>().SetTrigger("Show");
            

            transform.parent.Find("Toggle").gameObject.SetActive(false);
        }
    }

    private void ResetTrigger()
    {
        Animator anim = transform.parent.parent.parent.GetComponent<Animator>();
        foreach (AnimatorControllerParameter param in anim.parameters)
            anim.ResetTrigger(param.name);
        
    }



    public void ShowDeck(bool visible=true)
    {
        ResetTrigger();

        if (visible)
            transform.parent.parent.parent.GetComponent<Animator>().SetTrigger("Show"); 
        else
            transform.parent.parent.parent.GetComponent<Animator>().SetTrigger("Hide");
    }


    public void ToggleShowDeck()
    {
        ResetTrigger();
        //bool visible = transform.parent.parent.parent.GetComponent<Animator>().GetBool("visible");
        //transform.parent.parent.parent.GetComponent<Animator>().SetBool("visible", !visible);
        transform.parent.parent.parent.GetComponent<Animator>().SetTrigger("Toggle");
    }
}
