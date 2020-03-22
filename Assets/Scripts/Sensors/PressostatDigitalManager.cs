using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressostatDigitalManager : BaseComponent
{


    GameObject water, water2, timer;//, successValue;// //red, green, arrow, shine;
    GameObject[] red = new GameObject[4];
    GameObject[] green = new GameObject[4];
    public bool[] setPoint = { false, true, false, true };
    bool[] successes = { false, false, false, false };
    float PMax=1, PMin=0;
    public float periode = 16;


    public bool[] SetPoint { get => setPoint; set { setPoint = value; InitializePanel(); } }


    override public void Awake()
    {
        InitializePanel();
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
        base.Start();
        water = transform.Find("Water").gameObject;
        water2 = transform.Find("Water2").gameObject;
        timer = transform.Find("Timer").gameObject;
        //successValue = transform.Find("SuccessValue").gameObject;

        for (int i = 0; i < 4; i++)
        {
            red[i] = transform.Find("Red" + (i + 1)).gameObject;
            green[i] = transform.Find("Green" + (i + 1)).gameObject;
        }
        IsSuccess = true;
        success = 0;
        configPanel = Resources.Load("ConfigPanel/ConfigPressostatDigital") as GameObject;
    }

    public override void Rotate()
    {
        dir = 0;
        gc.StopperChanged = true;
    }

    public void InitializePanel()
    {
        for (int i = 0; i < 4; i++)
        {
            transform.Find("Red" + (i + 1)).gameObject.SetActive(!setPoint[i]);
            transform.Find("Green" + (i + 1)).gameObject.SetActive(setPoint[i]);
        }
    }

    public override void ResetSuccess()
    {
        ClearPanel();
    }

    protected void UpdatePanel()
    {
        success = 0;
        for (int i = 0; i < 4; i++)
        {
            red[i].transform.GetChild(0).gameObject.SetActive(successes[i]);
            green[i].transform.GetChild(0).gameObject.SetActive(successes[i]);

            if (successes[i])
                success += 0.25f;
        }
    }

    protected void ClearPanel()
    {

        for (int i = 0; i < 4; i++)
            successes[i] = false;

        UpdatePanel();
    }

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p2 = p[2];
        C = 0.05f;
        R = 1;
        q += i[2] / C * dt;
        q *= 0.99f;
       
        p[2] = q  + i[2] * R;
        i[2] = (p2 - q) / R;

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[0] = i[1] = i[3] = 0;
    }


    int step = 0; // 0 à 
    private void Update()
    {
        water2.GetComponent<Image>().color = PressureColor(p2);

        float rate = Mathf.Clamp((q - PMin) / (PMax - PMin) * 0.5f + 0.25f, 0, 0.99f);

        GetComponent<Animator>().SetFloat("rate", rate);

        if (itemBeingDragged) ClearPanel();

        switch (step)
        {
            case 0:
                ClearPanel();
                timer.GetComponent<Animator>().SetInteger("step", 0);
                success = 0;
                step++;
                break;
            case 1:
                if (8 * (Time.time / periode % 1) > 1)
                {
                    if (((setPoint[0] ? 1 : -1 )* (q - 0.5f)) > 0.2f) successes[0]=true;
                    UpdatePanel();
                    step++;
                }
                break;
            case 2:
                if (8 * (Time.time / periode % 1) > 2)
                {
                    timer.GetComponent<Animator>().SetInteger("step", 1);
                    step++;
                }
                break;
            case 3:
                if (8 * (Time.time / periode % 1) > 3)
                {
                    if (((setPoint[1] ? 1 : -1 )* (q - 0.5f)) > 0.2f) successes[1] = true;
                    UpdatePanel();
                    step++;
                }
                break;
            case 4:
                if (8 * (Time.time / periode % 1) > 4)
                {
                    timer.GetComponent<Animator>().SetInteger("step", 2);
                    step++;
                }
                break;
            case 5:
                if (8 * (Time.time / periode % 1) > 5)
                {
                    if ((setPoint[2] ? 1 : -1) * (q - 0.5f) > 0.2f) successes[2] = true;
                    UpdatePanel();
                    step++;
                }
                break;
            case 6:
                if (8 * (Time.time / periode % 1) > 6)
                {
                    timer.GetComponent<Animator>().SetInteger("step", 3);
                    step++;
                }
                break;
            case 7:
                if (8 * (Time.time / periode % 1) > 7)
                {
                    if ((setPoint[3] ? 1 : -1) * (q - 0.5f) > 0.2f) successes[3] = true;
                    UpdatePanel();
                    step++;
                }
                break;
            case 8:
                if (successes[0] && successes[1] && successes[2] && successes[3])
                    success = 1;
                if (8 * (Time.time / periode % 1) < 1) step = 0;
                break;
        }



    }

}
