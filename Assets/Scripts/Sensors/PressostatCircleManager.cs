using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressostatCircleManager : BaseComponent {


    GameObject  cadranMin, cadranMax, arrow, shine, value, connector, gauge;
   
    //Vector3 arrowStartPosition;
    float t_shine = 0;

    public float setPointHigh;
    public float setPointLow;
    public float pMax=1;
    public float pMin=0;

    public float SetPointHigh {
        get => setPointHigh;
        set
        {
            if (value > setPointLow) { setPointHigh = value; isSuccess = true; }
            else { setPointHigh = setPointLow; isSuccess = false; }
        }
    }
    public float SetPointLow {
        get => setPointLow;
        set
        {
            if (value < setPointHigh) { setPointLow = value; isSuccess = true; }
            else { setPointLow = setPointHigh; isSuccess = false; }
        }
      
    }
    public float PMax { get => pMax; set => pMax = value; }
    public float PMin { get => pMin; set => pMin = value; }


    const float decreaseRate = 0.99f;

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {
        float b = p[2];
        C = 0.05f;
        R = 1;
        q += (i[2])  * alpha;
        q *= decreaseRate;
        //f += (p[0] - p[2]) / L * 0;
        //f = 0;

        //p[0] = (1-alpha)*p[0] + alpha*( q + (i[0]-f)*R);
        p[2] = q / C + i[2]  * R;

        //i[0] = (1-alpha)*i[0] + alpha*( f + (a-q)/R);
        i[2] =  (b - q / C) / R;



        if ( setPointLow <q / C &&  q / C < setPointHigh  && itemBeingDragged == null)
            success = Mathf.Clamp(success + 0.005f, 0, 1);
        else
            success = Mathf.Clamp(success - 0.05f, 0, 1);


    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[0] = i[1] = i[3] = 0;
    }

    public override void Awake()
    {
        success = 0;
        tubeEnd[2] = true;
    }

    override public void PutStopper(int direction) // Put a stopper
    {
        /*GameObject stopper = Instantiate(Resources.Load("Components/Stopper"), transform) as GameObject;
        stopper.transform.localPosition = Vector3.zero;
        stopper.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 * (direction)));*/
    }

    protected override void Start()
    {
       
        cadranMin = transform.Find("Gauge/Cadran Min").gameObject;
        cadranMax = transform.Find("Gauge/Cadran Max").gameObject;

        shine = transform.Find("Gauge/Shine").gameObject;
        value = transform.Find("Gauge/Value").gameObject;
        arrow = transform.Find("Gauge/Arrow").gameObject;

        gauge = transform.Find("Gauge").gameObject;
        connector = transform.Find("Connector").gameObject;

        base.Start();
        success = 0;
        
        //shine.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        configPanel = Resources.Load("ConfigPanel/ConfigPressostat") as GameObject;
    }

    public override void Rotate()
    {
        switch (dir%4)
        {
            case 0:
                gauge.transform.localPosition = new Vector3(10,0,0);
                connector.transform.localRotation = Quaternion.Euler(0,0,-90);
                break;
            case 1:
                gauge.transform.localPosition = new Vector3(0, 10, 0);
                connector.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2:
                gauge.transform.localPosition = new Vector3(-10, 0, 0);
                connector.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case 3:
                gauge.transform.localPosition = new Vector3(0, -10, 0);
                connector.transform.localRotation = Quaternion.Euler(0, 0, 180);
                break;
        }

        gc.StopperChanged = true;

    }

    float angle;

    private void Update()
    {
        
        float rate = Mathf.Clamp((q / C / decreaseRate - PMin) / (PMax - PMin), -0.02f, 1.02f);

        float rateH = Mathf.Clamp((setPointHigh - PMin) / (PMax - PMin), 0, 1);
        float rateL = Mathf.Clamp((setPointLow - PMin) / (PMax - PMin) , 0, 1);

        // Vector3 pos= new Vector3(0, rate* 55.3f, 0);
        const float angleMax = 150;
        arrow.transform.localRotation = Quaternion.Euler(0, 0, -(2 * rate - 1) * angleMax);

        angle = -(2 * rate - 1) * angleMax;

        cadranMax.GetComponent<Image>().fillAmount = -(2 * rateL - 1) * 150 / 360 + 0.5f;
        cadranMin.GetComponent<Image>().fillAmount = -(2 * rateH - 1) * 150 / 360 + 0.5f;


        float v = Mathf.Round(20 * q/C) / 20;
        value.GetComponent<Text>().text = v.ToString();


        float alpha;

        if (success < 1)
        {
            alpha = success;
            t_shine = 0;
        }
        else
        {
            t_shine += Time.deltaTime;
            alpha = 0.8f + 0.2f * Mathf.Cos(t_shine * 5.0f);

        }

        shine.GetComponent<Image>().color = new Color(1, 1, 1, alpha);

    }

}
