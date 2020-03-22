using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class WatchManager : BaseComponent {

    protected Animation Animation;
    public float timeOut = 4.0f;
    public float TimeOut { get => timeOut; set { timeOut = value; ChangeValue(timeOut); } }
    bool running = false;
    int timeValue;

    public override void Awake()
    {
        ChangeValue(timeOut);

    }

    protected override void Start()
    {
        base.Start();
        configPanel = Resources.Load("ConfigPanel/ConfigWatch") as GameObject;
    }

    public void ChangeValue(float value)
    {
        if (value < 0)
            transform.Find("Value").GetComponent<Text>().text = ((int)timeOut).ToString();
        else
            transform.Find("Value").GetComponent<Text>().text = ((int)value).ToString();
    }

    public override void OnClick()
    {
        if (running) return;

        audios[2].Play();
     
        GetComponent<Animator>().SetTrigger("Start");
        GetComponent<Animator>().ResetTrigger("Reset");
        GetComponent<Animator>().ResetTrigger("Stop");

        GetComponent<Animator>().SetFloat("speed", 1 / timeOut);

        GameObject.Find("Playground").BroadcastMessage("TriggerStart", timeOut, SendMessageOptions.DontRequireReceiver);
        GameObject.Find("CanvasDragged").BroadcastMessage("TriggerStart", timeOut , SendMessageOptions.DontRequireReceiver);

        Invoke("EndTrig", timeOut);
        timeValue = (int) timeOut;
        //InvokeRepeating("DecreaseTime", 1, 1);
        running = true;
    }



    public void DecreaseTime()
    {
        timeValue--;
        ChangeValue(timeValue);
    }


    void EndTrig()
    {
        GameObject.Find("Playground").BroadcastMessage("TriggerEnd", SendMessageOptions.DontRequireReceiver);
        GameObject.Find("Playground").BroadcastMessage("HighLightFlush", SendMessageOptions.DontRequireReceiver);
        GameObject.Find("CanvasDragged").BroadcastMessage("TriggerEnd", SendMessageOptions.DontRequireReceiver);
        GetComponent<Animator>().SetTrigger("Stop");
        CancelInvoke("DecreaseTime");
        running = false;
    }

    public override void Reset_i_p()
    {
        base.Reset_i_p();
        CancelInvoke("EndTrig");
        CancelInvoke("DecreaseTime");

        ChangeValue(timeOut);
        GetComponent<Animator>().SetTrigger("Reset");
        EndTrig();
    }

}
