using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResistorManager : BaseComponent
{

    GameObject water, water0, water2, waterIn, bubble;
    public float res = 10;
    public float Res { get => res; set { res = Mathf.Round(value * 100) / 100; UpdateValue(); } }

    override public void Awake()
    {
        tubeEnd[0] = tubeEnd[2] = true;
        configPanel = Resources.Load("ConfigPanel/ConfigResistor") as GameObject;
        UpdateValue();
    }
    protected override void Start()
    {
        base.Start();
        water = transform.Find("Water").gameObject;
        waterIn = transform.Find("WaterIn").gameObject;
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;
        bubble = transform.Find("Bubble").gameObject;
        bubble.GetComponent<Animator>().SetFloat("speed", 0);
    }

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p0 = p[0];
        p2 = p[2];

        q += (i[0] + i[2]) / C * dt;
        f = (i[0] - i[2]) / 2;

        p[0] = (q + i[0] * Res * 0.5f);
        p[2] = (q + i[2] * Res * 0.5f);

        i[0] = (p0 - q) / Res * 2;
        i[2] = (p2 - q) / Res * 2;
    }

    public override void Rotate()
    {
        base.Rotate();
        transform.Find("Value").localRotation = Quaternion.Euler(0, 0, -90 * dir);
    }

    protected void UpdateValue()
    {
        float size = Sature(4f / (Res*Res));
        GetComponent<Animator>().SetFloat("size", size);
        GetComponentInChildren<Text>().text = res.ToString();
    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[1] = i[3] = 0;
    }


    private void Update()
    {
        water.GetComponent<Image>().color = PressureColor(q);
        waterIn.GetComponent<Image>().color = PressureColor(q);
        water0.GetComponent<Image>().color = PressureColor(p0);
        water2.GetComponent<Image>().color = PressureColor(p2);

        bubble.GetComponent<Animator>().SetFloat("speed", -SpeedAnim());
    }
}
