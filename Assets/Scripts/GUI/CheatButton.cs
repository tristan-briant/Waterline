using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CheatButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    float timeDown;
    const float cheatTime = 2.0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        timeDown = Time.fixedTime;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        Debug.Log(Time.fixedTime);
        if (Time.fixedTime > timeDown + cheatTime)
        {
            Debug.Log("Cheat !!");

            LevelManager LVM = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            LVM.hacked = !LVM.hacked;

            GameObject.Find("MenuController").GetComponent<MenuManager>().GenerateMenu();

            if (!LVM.hacked)
                GetComponent<Text>().color = Color.white;
            else
                GetComponent<Text>().color = Color.red;


        }

    }
}
