using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigitalManager : BaseFrontier
{

    public float pset = 1;
    public float Pset
    {
        get
        {
            return pset;
        }

        set
        {
            pset = value;
        }
    }

    public float imax = 0.1f;
    public float Imax
    {
        get
        {
            return imax;
        }

        set
        {
            imax = value;
        }
    }

    public bool[] setPoint = { false, true, false, true };
    public bool[] SetPoint { get => setPoint; set { setPoint = value; InitializePanel(); } }


    GameObject water =null, water0=null, arrow=null, bubble=null, value, timer;
    GameObject[] red = new GameObject[4];
    GameObject[] green = new GameObject[4];

    [SerializeField]
    protected float periode = 8;  
    public float Periode { get => periode; set => periode = value; }

    override public void PutStopper(int direction) // Put a stopper
    {
        GameObject stopper = Instantiate(Resources.Load("Components/Stopper"), transform) as GameObject;
        stopper.transform.localPosition = Vector3.zero;
        stopper.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
    }

    public override void Awake()
    {
        base.Awake();
        tubeEnd[3] = true;
    }

    public override void Rotate()
    {
        if (dir == 3)
        {
            transform.localScale = new Vector3(1, -1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.Euler(0, 0, dir * 90);
        }

        Transform holder = transform.Find("Panel");
        if (dir == 3)
            holder.localScale = new Vector3(-1, 1, 1);
        else
            holder.localScale = new Vector3(1, 1, 1);

        if (dir == 0)
        {
            holder.localRotation = Quaternion.Euler(0,0,0);
        }
        else
        {
            holder.localRotation = Quaternion.Euler(0, 0,-90);
        }

        //gc.StopperChanged = true;
    }

    float ppset;
    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        R = 0.5f;
        C = 0.1f;
        const float epsilon = 0.02f;

        if (setPoint[Mathf.FloorToInt(4 * (Time.time / periode % 1))])
        {
            ppset = (1- epsilon) * ppset + epsilon * (pset + 2 * R * f); // contreréaction pour atteindre pset en sortie
        }
        else
            ppset = (1 - epsilon) * ppset + epsilon * ( 2 * R * f); // contreréaction pour atteindre pset en sortie

        p0 = p[0];

        f = Mathf.Clamp((ppset - q) / R, -Imax, Imax); ;
        q += (i[0] + f) / C * dt;

        p[0] = q + i[0] * R;
        i[0] = (p0 - q) / R;

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
    }

    public void InitializePanel()
    {
        for (int i = 0; i < 4; i++)
        {
            transform.Find("Panel/Red" + (i + 1)).gameObject.SetActive(!setPoint[i]);
            transform.Find("Panel/Green" + (i + 1)).gameObject.SetActive(setPoint[i]);
        }
    }

    protected void UpdatePanel()
    {
        for (int i = 0; i < 4; i++)
        {
            red[i].transform.GetChild(0).gameObject.SetActive(i == step-1);
            green[i].transform.GetChild(0).gameObject.SetActive(i == step-1);
        }
    }

    protected void ClearPanel()
    {

        for (int i = 0; i < 4; i++)
        {
            red[i].transform.GetChild(0).gameObject.SetActive(false);
            green[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    protected override void Start()
    {
        base.Start();

        if (transform.Find("Water"))
            water = transform.Find("Water").gameObject;
        if (transform.Find("Water0"))
            water0 = transform.Find("Water0").gameObject;

        if (transform.Find("Arrow"))
            arrow = transform.Find("Arrow").gameObject;
        if (transform.Find("Bubble"))
            bubble = transform.Find("Bubble").gameObject;

        timer = transform.Find("Panel/Timer").gameObject;

        for (int i = 0; i < 4; i++)
        {
            red[i] = transform.Find("Panel/Red" + (i + 1)).gameObject;
            green[i] = transform.Find("Panel/Green" + (i + 1)).gameObject;
        }

        value = transform.Find("Value").gameObject;

        configPanel = Resources.Load("ConfigPanel/ConfigInletDigital") as GameObject;

        InitializePanel();
        UpdatePanel();
        R = 10f;
    }


    int step = 0; // 0 à 4

    private void Update()
    {

        water.GetComponent<Image>().color = PressureColor(ppset);
        water0.GetComponent<Image>().color = PressureColor(p0);

        bubble.GetComponent<Animator>().SetFloat("speed", SpeedAnim());


        bool isGreen = Time.time / periode % 1 < 0.5;

        value.GetComponent<Text>().text = f.ToString("F2");


        if (arrow)
        {
            if (ppset <= 0)
                arrow.GetComponent<Animator>().SetBool("Negative", true);
            else
                arrow.GetComponent<Animator>().SetBool("Negative", false);
        }

        switch (step)
        {
            case 0:
                timer.GetComponent<Animator>().SetInteger("step", 0);
                step++;
                UpdatePanel();
                break;
            case 1:
                if (4 * (Time.time / periode % 1) > 1)
                {
                    timer.GetComponent<Animator>().SetInteger("step", 1);
                    step++;
                    UpdatePanel();
                }
                break;
            case 2:
                if (4 * (Time.time / periode % 1) > 2)
                {
                    timer.GetComponent<Animator>().SetInteger("step", 2);
                    step++;
                    UpdatePanel();
                }
                break;
            case 3:
                if (4 * (Time.time / periode % 1) > 3)
                {
                    timer.GetComponent<Animator>().SetInteger("step", 3);
                    step++;
                    UpdatePanel();
                }
                break;
            case 4:
                if (4 * (Time.time / periode % 1) < 1)
                    step = 0;
                break;

        }


        if (Mathf.Abs(f) > fMinBubble)
        {

            if (!audios[3].isPlaying && !audios[4].isPlaying && !audios[5].isPlaying)
            {
                int r = Random.Range(0, 3);
                audios[3 + r].Play();
            }
            audios[3].volume = audios[4].volume = audios[5].volume = Mathf.Abs(f) / fMinBubble * 0.1f;

        }
    }

}
