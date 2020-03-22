using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResistorManager : BaseComponent {

    GameObject water,water0, water2,waterIn, bubble;
    //public float x_bulle = 0;
    //float r_bulle = 0.1f;
    public float res=10;

    //public Sprite[] TubeVariant;
    //public Sprite[] WaterVariant;
    //Sprite[] Variant;

    public float Res { get => res; set{ res = Mathf.Round( value*100)/100; UpdateValue(); } }

    protected override void Start()
    {
        base.Start();
        water = transform.Find("Water").gameObject;
        waterIn = transform.Find("WaterIn").gameObject;
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;

        bubble = transform.Find("Bubble").gameObject;
        f = 0;
        bubble.gameObject.SetActive(true); //May be deactivated if from designer
        bubble.GetComponent<Animator>().SetFloat("speed", -f / fMinBubble);
        
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

    override public void Awake()
    {
        tubeEnd[0] = tubeEnd[2] = true;
        //Variant = Resources.LoadAll<Sprite>("Components/Resistor-Variants");
        configPanel = Resources.Load("ConfigPanel/ConfigResistor") as GameObject;
        UpdateValue();
    }

    public override void Rotate()
    {
        base.Rotate();
        //transform.Find("Value").rotation = Quaternion.identity;
        transform.Find("Value").localRotation = Quaternion.Euler(0,0,-90*dir);

    }

    float Sature(float x)
    {
        if (x > 0)
            return x*x / (1 + x*x);
        else
            return x*x / (1 - x*x);
    }

    protected void UpdateValue()
    {
        float size = Sature(2f / Res);
        GetComponent<Animator>().SetFloat("size", size);

        GetComponentInChildren<Text>().text = res.ToString();

        /*if (res > 3.0f)
        {
            transform.Find("Tube").GetComponent<Image>().sprite = Variant[0];
            transform.Find("Water").GetComponent<Image>().sprite = Variant[3];
        }
        else if(res > 1.5f)
        {
            transform.Find("Tube").GetComponent<Image>().sprite = Variant[1];
            transform.Find("Water").GetComponent<Image>().sprite = Variant[4];
        }
        else
        {
            transform.Find("Tube").GetComponent<Image>().sprite = Variant[2];
            transform.Find("Water").GetComponent<Image>().sprite = Variant[5];
        }*/
            
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

        bubble.GetComponent<Animator>().SetFloat("speed", - SpeedAnim());

        //if (Mathf.Abs(f) > fMinBubble)
        //{

            /*float x_max = 0.5f - r_bulle;

            if (x_bulle > x_max) x_bulle = -x_max;
            if (x_bulle < -x_max) x_bulle = x_max;


            bubble.transform.localPosition = new Vector3(x_bulle * 100, 0, 0);
            float scale = Mathf.Max(Mathf.Abs(2 * x_bulle), 0.5f);
            bubble.transform.localScale = new Vector3(scale, scale, 1);*/


       /*     bubble.GetComponent<Animator>().SetFloat("speed",-5*f);
            bubble.SetActive(true);
        }
        else
        {
            bubble.SetActive(false);
        }*/

    }
}
