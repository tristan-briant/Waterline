using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigPanelInlet : ConfigPanel {


    public override void Start()
    {
        base.Start();
        InletManager Inlet = component as InletManager;

        //ParameterIncrementer.Add(x => (Inlet.pset += x)); // lambda expression
        //ParameterIncrementer.Add(x => (Inlet.imax += x));
    }

   
}
