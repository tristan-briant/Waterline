using System;
using UnityEngine;
using UnityEngine.UI;

public class TransistorManager : BaseComponent
{

    GameObject water, water1, water2, water3, bubble1, bubble2;

    float q1, q2, q3;
    float f13, f2;
    public bool NPN = true;

    public float gain = 10;
    public float Gain { get => gain; set { gain = value; UpdateValue(); } }

    override public void Awake()
    {
        tubeEnd[1] = tubeEnd[2] = tubeEnd[3] = true;
        configPanel = Resources.Load("ConfigPanel/ConfigTransistor") as GameObject;
        UpdateValue();
    }

    protected override void Start()
    {
        base.Start();
        water = transform.Find("Holder/Water").gameObject;
        water1 = transform.Find("Holder/Water1").gameObject;
        water2 = transform.Find("Holder/Water2").gameObject;
        water3 = transform.Find("Holder/Water3").gameObject;
        bubble1 = transform.Find("Holder/Bubble1").gameObject;
        bubble2 = transform.Find("Holder/Bubble2").gameObject;
        bubble1.GetComponent<Animator>().SetFloat("speed", 0);
        bubble2.GetComponent<Animator>().SetFloat("speed", 0);
    }

    public void UpdateValue()
    {
        GetComponentInChildren<Text>().text = "x" + (Mathf.Round(10 * gain) / 10).ToString();
    }

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {

        if (mirror)
        {
            float e = p[0];
            p[0] = p[2];
            p[2] = e;
            e = i[0];
            i[0] = i[2];
            i[2] = e;
        }

        p1 = p[1];
        p2 = p[2];
        p3 = p[3];

        if (NPN)
        {
            q1 += (i[1] - f13) / C * dt;
            q2 += (i[2] - f2) / C * dt;
            q3 += (i[3] + f13 + f2) / C * dt;

            f13 += (p[1] - p[3]) / L * dt;
            f2 += (p[2] - p[3]) / L * dt;

            if (f2 <= 0)
                f2 = 0;

            //f1 = Mathf.Clamp(f1,0,gain*f2);
            f13 = Mathf.Min(f13, gain * f2);   // Courant retour autorisé

            p[1] = (q1 + (i[1] - f13) * R);
            p[2] = (q2 + (i[2] - f2) * R);
            p[3] = (q3 + (i[3] + f13 + f2) * R);

            i[1] = (f13 + (p1 - q1) / R);
            i[2] = (f2 + (p2 - q2) / R);
            i[3] = (-f13 - f2 + (p3 - q3) / R);
        }
        else
        {
            q1 += (i[1] + f2 - f13) / C * dt;
            q2 += (i[2] - f2) / C * dt;
            q3 += (i[3] + f13) / C * dt;

            f2 += (p[2] - p[1]) / L * dt;
            f13 += (p[1] - p[3]) / L * dt;

            if (f2 >= 0)
                f2 = 0;

            f13 = Mathf.Min(f13, -gain * f2);   // Courant retour autorisé

            p[1] = (q1 + (i[1] + f2 - f13) * R);
            p[2] = (q2 + (i[2] - f2) * R);
            p[3] = (q3 + (i[3] + f13) * R);

            i[1] = (-f2 + f13 + (p1 - q1) / R);
            i[2] = (f2 + (p2 - q2) / R);
            i[3] = (-f13 + (p3 - q3) / R);
        }

        if (mirror)
        {
            float e = p[0];
            p[0] = p[2];
            p[2] = e;
            e = i[0];
            i[0] = i[2];
            i[2] = e;
        }

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        if (mirror)
            i[2] = 0;
        else
            i[0] = 0;
    }

    public override void Rotate()
    {
        //transform.localRotation = Quaternion.Euler(0, 0, dir * 90);
        if (!mirror)
        {
            tubeEnd[1] = tubeEnd[2] = tubeEnd[3] = true;
            tubeEnd[0] = false;

            transform.Find("Holder").localScale = Vector3.one;
            PutStoppers();
        }
        else
        {
            tubeEnd[1] = tubeEnd[0] = tubeEnd[3] = true;
            tubeEnd[2] = false;

            transform.Find("Holder").localScale = new Vector3(-1, 1, 1);
            PutStoppers();
        }


    }

    public override void OnClick()
    {
        mirror = !mirror;
        Rotate();
        audios[0].Play();
    }

    private void Update()
    {
        water1.GetComponent<Image>().color = PressureColor(p1);
        water2.GetComponent<Image>().color = PressureColor(p2);
        water3.GetComponent<Image>().color = PressureColor(p3);
        water.GetComponent<Image>().color = PressureColor((p1 + p2 + p3) / 3);

        GetComponent<Animator>().SetFloat("open", Mathf.Clamp(NPN ? f2 : -f2, 0, 0.99f));

        bubble1.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f13));
        bubble2.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f2));

    }

    override public String ToString(char name) //name not used
    {
        String str = base.ToString(NPN ? 'T' : 't');
        str += ((int)(gain * 10)).ToString("D4");
        return str;
    }

    public bool FromString(String str)
    {
        int index = 0;
        if (str.Substring(index++, 1) == "T") NPN = true;
        if (str.Substring(index, 1) == "L") { locked = true; index++; }
        int x = int.Parse(str.Substring(index++, 1));
        dir = x % 4;
        mirror = (x / 4 == 1);
        gain = 0.1f * int.Parse(str.Substring(index, 4));

        return true; //success
    }
}
