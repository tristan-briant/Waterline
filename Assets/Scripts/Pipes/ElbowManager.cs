using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElbowManager : BaseComponent {
    GameObject water2, water3, bubble;
    


    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p2 = p[2];
        p3 = p[3];

        q += (i[2] + i[3]) /C * dt;
        f += (p2 - p3) / L * dt;

        p[2] = (q + (i[2] - f) * R);
        p[3] = (q + (i[3] + f) * R);

        i[2] = (f + (p2 - q) / R);
        i[3] = (-f + (p3 - q) / R);
 
    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[0] = i[1] = 0;
    }

    public override void Awake()
    {
        base.Awake();
        tubeEnd[3] = tubeEnd[2] = true;
    }


    protected override void Start()
    {
        base.Start();
        tubeEnd[3] = tubeEnd[2] = true;
        water2 = transform.Find("Water2").gameObject;
        water3 = transform.Find("Water3").gameObject;
        bubble = transform.Find("Bubble").gameObject;
       
        bubble.gameObject.SetActive(true);
        bubble.GetComponent<Animator>().SetFloat("speed", 0);
    }

    private void Update()
    {
        SetPressure(Mathf.Clamp(0.25f * (p2 + p3), -1f, 1f));

        water2.GetComponent<Image>().color = PressureColor(p2);
        water3.GetComponent<Image>().color = PressureColor(p3);
        
        bubble.GetComponent<Animator>().SetFloat("speed", SpeedAnim());
        
    }

}
