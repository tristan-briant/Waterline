using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    //public float incrementMultiplier;
    float longPressDuration = 0.3f;
    bool pressing = false;
    public UnityEvent OnClick;
    public UnityEvent OnShortClick;
    public UnityEvent OnLongPress;
    public UnityEvent OnLongClick;

    //public FloatParameterModifier fpm;
   

    float startPressTime;
    bool longclick;

 
    void Awake()
    {
        if (OnLongPress == null)
            OnLongPress = new UnityEvent();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressing = true;
        longclick = false;
        startPressTime = Time.time;
        OnClick.Invoke();

        if (Time.time <= startPressTime + longPressDuration) //Long click
        {
            OnShortClick.Invoke();
        }
    }

   public void OnPointerUp(PointerEventData eventData)
    {
        /*if (Time.time > startPressTime + longPressDuration) //Long click
        {
            OnLongClick.Invoke();
        }*/
        pressing = false;
    }

    void Update()
    {
        if (pressing && (Time.time > startPressTime + longPressDuration)) //continuous press
        {
            OnLongPress.Invoke();

            if (!longclick)
            {
                OnLongClick.Invoke();
                longclick = true;
            }
        }
    }
}
