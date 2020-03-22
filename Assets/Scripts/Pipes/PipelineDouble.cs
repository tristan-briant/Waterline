using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PipelineDouble : BaseComponent
{

    GameObject water0, water1, water2, water3, bubble1, bubble2;
    float f0, f1, q0, q1;

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p0 = p[0];
        p1 = p[1];
        p2 = p[2];
        p3 = p[3];

        q0 += (i[0] + i[2]) / C * dt;
        f0 += (p0 - p2) / L * dt;

        q1 += (i[1] + i[3]) / C * dt;
        f1 += (p1 - p3) / L * dt;

        p[0] = (q0 + (i[0] - f0) * R);
        p[2] = (q0 + (i[2] + f0) * R);

        p[1] = (q1 + (i[1] - f1) * R);
        p[3] = (q1 + (i[3] + f1) * R);

        i[0] = (f0 + (p0 - q0) / R);
        i[2] = (-f0 + (p2 - q0) / R);

        i[1] = (f1 + (p1 - q1) / R);
        i[3] = (-f1 + (p3 - q1) / R);

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
    }

    public override void Awake()
    {
        base.Awake();
        tubeEnd[0] = tubeEnd[1] = tubeEnd[2] = tubeEnd[3] = true;
    }

    protected override void Start()
    {
        base.Start();
        water0 = transform.Find("Water0").gameObject;
        water1 = transform.Find("Water1").gameObject;
        water2 = transform.Find("Water2").gameObject;
        water3 = transform.Find("Water3").gameObject;

        bubble1 = transform.Find("Bubble1").gameObject;
        bubble2 = transform.Find("BubbleHolder/Bubble2").gameObject;

    }

    private void Update()
    {
        SetPressure(Mathf.Clamp(0.25f * (p0 + p2), -1f, 1f));
        SetPressure(Mathf.Clamp(0.25f * (p1 + p3), -1f, 1f), 1);

        water0.GetComponent<Image>().color = PressureColor(p0);
        water1.GetComponent<Image>().color = PressureColor(p1);
        water2.GetComponent<Image>().color = PressureColor(p2);
        water3.GetComponent<Image>().color = PressureColor(p3);

        bubble1.GetComponent<Animator>().SetFloat("speed", -SpeedAnim(f0));
        bubble2.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f1));
    }

}

