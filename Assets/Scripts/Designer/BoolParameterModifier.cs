using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoolParameterModifier : MonoBehaviour {

    public string ParameterName; // Name of the property
    public Image checkMark;

    object ob;

    private void Start()
    {
        ob = transform.GetComponentInParent<ConfigPanel>().component;
        checkMark.enabled = ((bool)ob.GetType().GetProperty(ParameterName).GetValue(ob, null));
    }

    public void Toogle()
    {
        bool a = (bool)ob.GetType().GetProperty(ParameterName).GetValue(ob, null);
        a = !a;
        ob.GetType().GetProperty(ParameterName).SetValue(ob, a, null);
        checkMark.enabled = ((bool)ob.GetType().GetProperty(ParameterName).GetValue(ob, null));
    }

}
