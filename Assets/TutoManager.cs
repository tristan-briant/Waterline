using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class TutoManager : MonoBehaviour, IPointerDownHandler
{

    public void OnPointerDown(PointerEventData ev)
    {
        Animator anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("next");
    }

    public void CloseTuto(){
        SceneManager.UnloadSceneAsync("Tuto");
    }
}
