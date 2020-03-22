using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Pipeline : BaseComponent { 

    GameObject water0,water2,bubble;
   

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p0 = p[0];
        p2 = p[2];

        q += (i[0] + i[2]) / C * dt;
        f += (p0 - p2) / L * dt;

        p[0] = q + (i[0] - f) * R;
        p[2] = q + (i[2] + f) * R;

        i[0] = f + (p0 - q) / R;
        i[2] = -f + (p2 - q) / R;
        
    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[1] = 0;
        i[3] = 0;
    }

    public override void Awake()
    {
        base.Awake();
        tubeEnd[0] = tubeEnd[2] = true;
    }

    protected override void Start()
    {
        base.Start();
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;

        bubble = transform.Find("Bubble").gameObject;

    }

    private void Update()
    {
        SetPressure(Mathf.Clamp(0.25f * (p0 + p2), -1f, 1f));
        water0.GetComponent<Image>().color = PressureColor(p0);
        water2.GetComponent<Image>().color = PressureColor(p2);
        
        bubble.GetComponent<Animator>().SetFloat("speed", -SpeedAnim());
    }

}

