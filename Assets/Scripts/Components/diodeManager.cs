using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class diodeManager : BaseComponent {

    GameObject water0, water2, bubble;
    GameObject piston;
    float q0, q2;
    float xp;

    public override void Reset_i_p()
    {
        base.Reset_i_p();
        q0 = q2 = 0;
    }

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p0 = p[0];
        p2 = p[2];

        q0 += (i[0] - f) / C * dt;
        q2 += (i[2] + f) / C * dt;

        f += (p[0] - p[2]) / L * dt;

        if (f >= 0)                   //Diode de 2 vers 0
            f = 0;

        p[0] = (q0 + (i[0] - f) * R);
        p[2] = (q2 + (i[2] + f) * R);

        i[0] = (f + (p0 - q0) / R);
        i[2] = (-f + (p2 - q2) / R);

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[1] = i[3] = 0;
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
        piston = transform.Find("Piston").gameObject;
        bubble = transform.Find("Bubble").gameObject;
    }

    private void Update()
    {
        water0.GetComponent<Image>().color = PressureColor(p0);
        water2.GetComponent<Image>().color = PressureColor(p2);

        xp = 0.9f * xp + 0.1f * Mathf.Clamp(-f, 0, 0.1f);


        piston.transform.localPosition = new Vector3(xp*100, 0, 0);

        bubble.GetComponent<Animator>().SetFloat("speed", -SpeedAnim());

    }
}
