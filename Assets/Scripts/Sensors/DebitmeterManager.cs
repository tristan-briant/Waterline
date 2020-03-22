using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebitmeterManager : BaseComponent
{

    GameObject water, water0, water2, bubble, cadranMin, cadranMax, arrow, shine;
    float successTime = 2.0f; //Time to obtain a success
    ValueManager valueM;

    public float iMax;
    public float setPointHigh;
    public float setPointLow;

    public float SetPointHigh
    {
        get => setPointHigh;
        set
        {
            if (value <= SetPointLow) { setPointHigh = setPointLow; isSuccess = false; }
            else { setPointHigh = value; isSuccess = true; }
            DrawCadran();
        }
    }
    public float SetPointLow
    {
        get => setPointLow;
        set
        {
            if (value >= setPointHigh) { setPointLow = setPointHigh; isSuccess = false; }
            else { setPointLow = value; isSuccess = true; }
            DrawCadran();
        }
    }
    public float IMax { get => iMax; set { iMax = value; DrawCadran(); } }
    public bool symmetric = true;
    public bool Symmetric { get => symmetric; set { symmetric = value; DrawCadran(); } }
 
    float SPH, SPL, successSpeed;
    public int mode = 0;
    public float periode = 2, phase = 0;
    float t_shine = 0;

    float flux = 0;


    const float beta = 0.1f;

    public override void Awake()
    {

        DrawCadran();
        tubeEnd[0] = tubeEnd[2] = true;
    }

    void DrawCadran()
    {
        float angleH, angleL;
        if (symmetric)
        {
            angleH = Mathf.Clamp((setPointHigh) / iMax, -1, 1);
            angleL = Mathf.Clamp((setPointLow) / iMax, -1, 1);
            transform.Find("Convention1").GetComponent<Image>().color = Color.white;
            transform.Find("Convention2").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
        else
        {
            angleH = Mathf.Clamp(2 * setPointHigh / iMax - 1, -1, 1);
            angleL = Mathf.Clamp(2 * setPointLow / iMax - 1, -1, 1);
            transform.Find("Convention1").GetComponent<Image>().color = new Color(0, 0, 0, 0);
            transform.Find("Convention2").GetComponent<Image>().color = Color.white;
        }

        cadranMin = transform.Find("Cadran/Cadran Min").gameObject;
        cadranMax = transform.Find("Cadran/Cadran Max").gameObject;

        cadranMax.GetComponent<Image>().fillAmount = 0.5f - angleH * 0.32f;
        cadranMin.GetComponent<Image>().fillAmount = 0.5f - angleL * 0.32f;

        valueM = GetComponentInChildren<ValueManager>();
        valueM.ReDraw(Mathf.Round(100 * 0.5f * (setPointHigh + SetPointLow)) / 100);

        /*if (isSuccess)
        {
            transform.Find("Value/SetPoint").gameObject.SetActive(true);
            transform.Find("Value/Dash").gameObject.SetActive(true);
            float setPoint = Mathf.Round(100 * 0.5f * (setPointHigh + SetPointLow)) / 100;
            transform.Find("Value/SetPoint").GetComponent<Text>().text = setPoint.ToString();
            //transform.Find("SuccessValue").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Value/SetPoint").gameObject.SetActive(false);
            transform.Find("Value/Dash").gameObject.SetActive(false);
            //transform.Find("SuccessValue").gameObject.SetActive(false);
        }*/
    }

    protected override void Start()
    {
        base.Start();
        success = 0;
        water = transform.Find("Water").gameObject;
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;

        bubble = transform.Find("Bubble").gameObject;


        arrow = transform.Find("Cadran/Arrow").gameObject;
        shine = transform.Find("Shine").gameObject;
        valueM = GetComponentInChildren<ValueManager>();

        f = 0;
        bubble.GetComponent<Animator>().SetFloat("speed", 0);

        configPanel = Resources.Load("ConfigPanel/ConfigDebimeter") as GameObject;
        DrawCadran();
    }


    void CalculateSetPoint()
    {
        float mean, tolerance, t;
        switch (mode)
        {
            case 0:
                SPH = setPointHigh;
                SPL = setPointLow;
                successSpeed = 0.005f;
                break;
            case 1:
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
    }


    public override void Rotate()
    {
        base.Rotate();
        transform.Find("Value").rotation = Quaternion.identity;
        //transform.Find("SuccessValue").rotation = Quaternion.identity;
     
    }

    float theta(float x)
    {
        if (x < 0) return 0;
        return x;
    }

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p0 = p[0];
        p2 = p[2];

        q += (i[0] + i[2]) / C * dt;
        f += (p[0] - p[2]) / L * dt;

        p[0] = (q + (i[0] - f) * R);
        p[2] = (q + (i[2] + f) * R);

        i[0] = (f + (p0 - q) / R);
        i[2] = (-f + (p2 - q) / R);

        flux = (1 - beta) * flux + beta * f;

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[1] = i[3] = 0;
    }

    public virtual void UpdateSuccess()
    {
        if (setPointLow < -f && -f < setPointHigh && itemBeingDragged == null)
            success = Mathf.Clamp(success + Time.deltaTime / successTime, 0, 1);
        else
            success = Mathf.Clamp(success - 10 * Time.deltaTime / successTime, 0, 1);
    }


    private void Update()
    {

        UpdateSuccess();

        water.GetComponent<Image>().color = PressureColor(q);
        water0.GetComponent<Image>().color = PressureColor(p0);
        water2.GetComponent<Image>().color = PressureColor(p2);

        bubble.GetComponent<Animator>().SetFloat("speed", -SpeedAnim());

        const float ANGLEMAX = 180 / 4.8f;
        float angle;
        if (symmetric)
            angle = Mathf.Clamp(flux / iMax * ANGLEMAX, -ANGLEMAX * 1.2f, ANGLEMAX * 1.2f);
        else
            angle = Mathf.Clamp((2 * flux / iMax + 1) * ANGLEMAX, -ANGLEMAX * 1.2f, ANGLEMAX * 1.2f);

        arrow.transform.localEulerAngles = new Vector3(0, 0, angle);

       
        
        valueM.value=-f;

        float alpha;

        if (success < 1)
        {
            alpha = success;
            t_shine = 0;
        }
        else
        {
            t_shine += Time.deltaTime;
            alpha = 0.5f + 0.2f * Mathf.Cos(t_shine * 5.0f);
        }

        shine.GetComponent<Image>().color = new Color(1, 1, 1, alpha);

    }

}
