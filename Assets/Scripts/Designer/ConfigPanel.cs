using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class ConfigPanel : MonoBehaviour {

    public BaseComponent component;
    public GameObject selection;

    GameObject selec;
    bool connected = false; //true if linked to a component

    public virtual void Start()
    {
        if (component)
        {
            selec = Instantiate(selection);
            selec.transform.SetParent(component.transform);
            selec.transform.SetAsFirstSibling();
            selec.transform.localScale = Vector3.one;
            selec.transform.localPosition = Vector3.zero;
            connected = true;
        }

        gameObject.AddComponent<GraphicRaycaster>();
    }

    public void Close()
    {
        Destroy(gameObject);
        if(selec)
            Destroy(selec);
    }

    public void Update()
    {
        if (connected && selec == null) // the component has been deleted
            Destroy(gameObject);
    }

}
