using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotManager : MonoBehaviour , IDropHandler {

    public bool isSlotFrontier = false;
    public int type=0; //for frontier
    public int dir=0;


    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        if (BaseComponent.itemBeingDragged)
        {
            if (BaseComponent.itemBeingDragged.GetComponent<BaseComponent>().isFrontiers == isSlotFrontier)
            { // component and slot of the same type
                BaseComponent.endParent = transform;
            }
        }
    }


    public void InitializeSlot(int d)
    { //set the Frontier slot

        //ChangeSlotImage();
        Transform bg = transform.GetChild(0);

        dir = d;
        
        if (dir == 3)
        {
            bg.localScale = new Vector3(-1, 1, 1);
            bg.localRotation = Quaternion.Euler(0, 0, -90f);
        }
        else if (dir == 5)
        {
            bg.localScale = new Vector3(-1, 1, 1);
            bg.localRotation = Quaternion.Euler(0, 0, 180f);
        }
        else if (dir == 6)
        {
            bg.localScale = new Vector3(-1, 1, 1);
            bg.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
            bg.localRotation = Quaternion.Euler(0, 0, 90f * dir); // C'est le slot qui tourne

    }

    public void ChangeSlotImage(int t)
    {
        type = t;
        //Transform slot = transform.parent;
        Sprite[] sprites = Resources.LoadAll<Sprite>("Field/BorderAtlas");

        int index = 0;
        switch (type)
        {
            case 0: index = 7; break;
            case 1: index = (int)Random.Range(5, 6.999f); break;
            case 2: index = 4; break;
            case 3: //index = 3; break;
            case 4: index = (int)Random.Range(1, 2.999f); break;
            case 5: index = 0; break;
        }
        GetComponentInChildren<Image>().sprite = sprites[index];
    }
}

