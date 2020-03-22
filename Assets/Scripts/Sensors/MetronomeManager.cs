using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class MetronomeManager : BaseComponent {

    GameObject water0, water2, bubble,cadranMin,cadranMax,arrow,shine,pendulum;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    public float setPointHigh, setPointLow, iMax,freqMax;
    float SPH, SPL, successSpeed=0.005f;
    public int mode = 0;
    public float periode = 0,time=0,direction=0;//,phase = 0;
    float t_shine = 0;
    //public new float f;
    float ff=0;
    const float beta=0.2f;
    Vector3 arrowStartPosition;
    float rate = 0;
    float angle = 0.0f;

    public override void OnClick()
    {
        //Do not rotate
    }

    /*void calculateSetPoint()
    {
        float mean, tolerance, t;
        switch (mode)
        {
            case 0 :
                SPH = setPointHigh;
                SPL = setPointLow;
                successSpeed = 0.005f;
                break;
            case 1 :
                 mean = 0.5f * (setPointHigh + setPointLow);
                 tolerance = (setPointHigh - setPointLow) * 0.5f;
                 t = 2 * Mathf.PI * Time.time;

                SPL = mean * Mathf.Sin(t / periode + Mathf.PI * phase / 180.0f) - tolerance;
                SPH = SPL + 2 * tolerance;
                successSpeed = 0.01f / periode / 2;
                break;
            case 2:
                mean = 0.5f * (setPointHigh + setPointLow);
                tolerance = (setPointHigh - setPointLow) * 0.5f;
                t = 2 * Mathf.PI * Time.time;

                SPL = mean * theta(Mathf.Sin(t / periode + Mathf.PI * phase / 180.0f)) - tolerance;
                SPH = SPL + 2 * tolerance;
                successSpeed = 0.01f / periode / 2;
                break;

        }
    }*/


    float theta(float x) {
        if( x < 0) return 0;
        return x;
    }

    public override void Reset_i_p()
    {
        base.Reset_i_p();
        success = 0;
        time = 0;
    }

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {
        float a = p[0], b = p[2];

        q += (i[0] + i[2]) * alpha; 
        f += (p[0] - p[2]) / L * alpha;

        p[0] = (q / C + (i[0] - f) * R);
        p[2] = (q / C + (i[2] + f) * R);

        i[0] = (f + (a - q / C) / R);
        i[2] = (-f + (b - q / C) / R);

        Calcule_i_p_blocked(p, i, alpha, 1);
        Calcule_i_p_blocked(p, i, alpha, 3);

        x_bulle -= 0.05f * f;

        ff = (1 - beta) * ff + beta * f;


        if ( SPL < 1 / periode  && 1 / periode < SPH && itemBeingDragged == null)
            success = Mathf.Clamp(success + successSpeed, 0, 1);
        else
            success = Mathf.Clamp(success - 0.05f, 0, 1);

    }

    protected override void Start()
    {
        base.Start();
        success = 0;
        water0 = this.transform.Find("Water0").gameObject;
        water2 = this.transform.Find("Water2").gameObject;

        bubble = this.transform.Find("Bubble").gameObject;
        cadranMin = this.transform.Find("Cadran Min").gameObject;
        cadranMax = this.transform.Find("Cadran Max").gameObject;

        arrow = this.transform.Find("Arrow").gameObject;
        pendulum = this.transform.Find("Pendulum").gameObject;
        shine = this.transform.Find("Shine").gameObject;

        arrowStartPosition = arrow.transform.localPosition;

        time = Time.time;

        SPH = setPointHigh;
        SPL = setPointLow;

        cadranMax.GetComponent<Image>().fillAmount = (SPH / freqMax) * (SPH / freqMax);
        cadranMin.GetComponent<Image>().fillAmount = (SPL / freqMax) * (SPL / freqMax);

        successSpeed = 0.01f * ((SPH + SPL) / 2.0f) / 6;  // 6 * expected periode 

    }


    private void Update()
    {

        water0.GetComponent<Image>().color = PressureColor(pin[0]);
        water2.GetComponent<Image>().color = PressureColor(pin[2]);

        if (Mathf.Abs(f) > fMinBubble)
        {
            float x_max = 0.5f - r_bulle;

            if (x_bulle > x_max) x_bulle = -x_max;
            if (x_bulle < -x_max) x_bulle = x_max;


            bubble.transform.localPosition = new Vector3(x_bulle * 100, 0, 0);
            bubble.SetActive(true);
        }
        else
        {
            bubble.SetActive(false);
        }

        const float ANGLEMAX = 180 / 4.8f;

        if (Time.time < time + 4.0f)
            angle = 0.2f * angle + 0.8f * Mathf.Atan(ff / iMax ) * ANGLEMAX;//Mathf.Clamp(ff / iMax * ANGLEMAX, -ANGLEMAX, ANGLEMAX);
        else
            angle = 0.2f * angle;

        //float angleH = Mathf.Clamp((SPH) / iMax, -1, 1);
        //float angleL = Mathf.Clamp((SPL) / iMax, -1, 1);

        pendulum.transform.localEulerAngles= new Vector3(0, 0, angle);

        // compute the periode :
        
        if (Mathf.Abs(f) > iMax * 0.1 && Mathf.Sign(f) != Mathf.Sign(direction) && Time.time > time + 0.5f) 
        {
            direction = Mathf.Sign(f);
            periode = Time.time-time;
            time = Time.time;
        }

        if (Time.time > time + 4.0f)
        {
            periode = Mathf.Clamp(periode + 0.2f, 0, 1000);
            direction = 0;
        }

        if (periode <= 0.1f) periode = 10000f;


        rate = 0.9f * rate + 0.1f * Mathf.Clamp((1 / periode) / (freqMax), 0, 1);
        Vector3 pos = new Vector3(0, rate * rate * 56.6f, 0);

        arrow.transform.localPosition = arrowStartPosition + pos;


        


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
