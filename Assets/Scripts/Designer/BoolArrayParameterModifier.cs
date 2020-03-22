using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoolArrayParameterModifier : MonoBehaviour
{

    public string ParameterName; // Name of the property
    public Image[] checkMark;

    object ob;

    private void Start()
    {
        ob = transform.GetComponentInParent<ConfigPanel>().component;

        bool[] value = ((bool[])ob.GetType().GetProperty(ParameterName).GetValue(ob, null));

        for (int i = 0; i < 4; i++)
        {
            checkMark[i].enabled = value[i];//((bool[])ob.GetType().GetProperty(ParameterName).GetValue(ob, null))[i];
        }
    }

    public void Toogle(int k)
    {
        bool[] value = ((bool[])ob.GetType().GetProperty(ParameterName).GetValue(ob, null));

        value[k] = !value[k];
        ob.GetType().GetProperty(ParameterName).SetValue(ob, value, null);

        for (int i = 0; i < 4; i++)
        {
            checkMark[i].enabled = value[i];//((bool[])ob.GetType().GetProperty(ParameterName).GetValue(ob, null))[i];

        }

    }
}
