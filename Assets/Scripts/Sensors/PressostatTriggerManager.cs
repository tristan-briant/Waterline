using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PressostatTriggerManager : PressostatManager {

    public float raiseTime = 5.0f;
    public float RaiseTime { get => raiseTime; set => raiseTime = Mathf.Clamp(value, 1f, 20f); }

    public float pLim = 1f;
    public float PLim { get => pLim; set => pLim = Mathf.Clamp(value, 0.1f, PMax); }

    float tolerance = 0.15f;
    bool rising = false;

    public override void Awake()
    {
        symmetric = false;
        tubeEnd[2] = true;
    }

    protected override void Start()
    {
        base.Start();
        configPanel = Resources.Load("ConfigPanel/ConfigPressostatTrigger") as GameObject;
        IsSuccess = true;
        C = 0.1f;
    }

    void CalculateSetPoints(float time)
    {
        if (time >= 0)
        {
            setPointHigh = pLim * (time / raiseTime + tolerance);
            setPointLow = pLim * (time / raiseTime - tolerance);
        }
        else
        {
            setPointHigh = -1;
            setPointLow = 0;
        }
        DrawCadran();
    }

    public override void Reset_i_p()
    {
        TriggerEnd();
        Debug.Log("Reset");
        base.Reset_i_p();
    }

    IEnumerator coroutine;
    public void TriggerStart(float timeOut)
    {
        RaiseTime = timeOut;
        coroutine = CountDown();
        Debug.Log("message reçu");
        StartCoroutine(coroutine);
        rising = true;
    }

    public void TriggerEnd()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        CalculateSetPoints(-1);
        rising = false;
    }

    IEnumerator CountDown() {
        float time = 0;

        while (true)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            CalculateSetPoints(time);
        }
        
    }

    override public void UpdateSuccess()
    {
 

        if (!rising || itemBeingDragged!=null)
            success = 0;
        else if (setPointLow < qq && qq < setPointHigh)
            success = Mathf.Clamp01(success + Time.deltaTime / RaiseTime * 1.2f);
    }

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
       
        p2 = p[2];

        q += i[2] / C * dt;
        //q *= 0.9995f;

        p[2] = q + i[2] * R;
        i[2] = (p2 - q) / R;
    }

 
}
