using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class ConfigPanelCapacitor : ConfigPanel {

    /*capacitorManager Cap;

    public override void Start()
    {
        Cap = component as capacitorManager;

        ParameterIncrementer.Add(x => Cap.Cin += x); // lambda expression
        ParameterToggler.Add(x => Cap.locked ^= x);
        
        base.Start();
    }

    [ContextMenu("get property")]
    public void GetPropertyValue()
    {
        component.GetType().GetProperty("toto").SetValue(component,6,null);

        Type t = component.GetType();
        Debug.Log(t);
        System.Reflection.PropertyInfo propertyInfo = t.GetProperty("toto");
        //Debug.Log(t.GetProperties()[20]);
        Debug.Log(propertyInfo.GetValue(component, null));

    }*/

}
