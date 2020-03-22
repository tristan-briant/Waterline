using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InletDebimetreTriggerManager : InletDebimeterManager
{
    public override void Awake()
    {
        base.Awake();
        open = false;
        transform.Find("CadranHolder/Cadre").GetComponent<Image>().color = Color.gray;
    }

    public void TriggerStart()
    {
        open = true;
        transform.Find("CadranHolder/Cadre").GetComponent<Image>().color = Color.white;
    }

    public void TriggerEnd()
    {
        open = false;
        transform.Find("CadranHolder/Cadre").GetComponent<Image>().color = Color.gray;
    }
}
