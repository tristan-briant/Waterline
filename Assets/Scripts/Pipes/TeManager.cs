using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TeManager : BaseComponent
{

    GameObject water0, water2, water3, bubble20, bubble23, bubble03;
    float f0,  f2, f3;
    float i0 = 0,  i2 = 0, i3 = 0;


    public override void Reset_i_p()
    {
        base.Reset_i_p();
        i0 = i2 = i3 = f0 = f2 = f3 = 0;
    }

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p0 = p[0];
        p2 = p[2];
        p3 = p[3];

        q += (i[0] + i[2] + i[3]) / C * dt;

        f0 += (p[0] - q) / L * dt;
        //f1 += (p[1] - q) / L * dt;
        f2 += (p[2] - q) / L * dt;
        f3 += (p[3] - q) / L * dt;

        p[0] = (q + (i[0] - f0) * R);
        //p[1] = (q + (i[1] - f1) * R);
        p[2] = (q + (i[2] - f2) * R);
        p[3] = (q + (i[3] - f3) * R);

        i[0] = (f0 + (p0 - q) / R);
        //i[1] = (f1 + (p1 - q) / R);
        i[2] = (f2 + (p2 - q) / R);
        i[3] = (f3 + (p3 - q) / R);

    }


    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[1] = 0;
    }

    public override void Awake()
    {
        base.Awake();
        tubeEnd[0] = tubeEnd[2] = tubeEnd[3] = true;
    }

    protected override void Start()
    {
        base.Start();
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;
        water3 = transform.Find("Water3").gameObject;

        bubble20 = transform.Find("Bubble20").gameObject;
        bubble23 = transform.Find("Bubble23").gameObject;
        bubble03 = transform.Find("Bubble03/Bubble").gameObject;

    }


    private void Update()
    {
        SetPressure(Mathf.Clamp(0.5f * (p0 + p2 + p3) / 3f, -1f, 1f));

        water0.GetComponent<Image>().color = PressureColor(p0);
        water2.GetComponent<Image>().color = PressureColor(p2);
        water3.GetComponent<Image>().color = PressureColor(p3);

        bubble20.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f2, f0));
        bubble23.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f2, f3));
        bubble03.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f0, f3));

    }

}

