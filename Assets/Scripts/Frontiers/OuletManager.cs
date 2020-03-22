using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OuletManager : BaseFrontier
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

    public float imax = 1f;
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


    GameObject water=null, water0=null, arrow=null, bubble=null, moulin;
    
    public bool periodic = false;
    public float periode = 2;
    public float Periode { get => periode; set => periode = value; }
    public bool Periodic { get => periodic; set => periodic = value; }

    public override void Awake()
    {
        tubeEnd[3] = true;
    }

    override public bool IsSuccess { get => isSuccess; set { isSuccess = value; InitializeSuccess(); } }

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
        if (transform.Find("Moulin"))
            moulin = transform.Find("Moulin").gameObject;

        if (isSuccess)
            success = 0;
        configPanel = Resources.Load("ConfigPanel/ConfigInlet") as GameObject;

        InitializeSuccess();
    }

    void InitializeSuccess()
    {
        GameObject successValue=null;

        if (transform.Find("ValueHolder/SuccessValue"))
            successValue = transform.Find("ValueHolder/SuccessValue").gameObject;

        success = 0;
        if (successValue)
        {
            if (IsSuccess)
            {
                successValue.SetActive(true);
            }
            else
                successValue.SetActive(false);
        }
        Rotate();
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


        Transform holder = transform.Find("ValueHolder");
        if (holder)
        {
            holder.rotation = Quaternion.identity;
            if (dir == 1)
                holder.localRotation = Quaternion.Euler(0, 0, -90f);
            else
                holder.localRotation = Quaternion.Euler(0, 0, 90f * dir);


            if (dir == 3)
                holder.localScale = new Vector3(-1, 1, 1);
            else
                holder.localScale = new Vector3(1, 1, 1);
        }
    }

    float ppset;
    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        if (periodic)
            ppset = pset * Mathf.Sin(2 * Mathf.PI * Time.time / periode);
        else
            ppset = pset;

        p0 = p[0];
        f = i[0]; //for bubble animation
        
    }

    float pp = 0;
    public override void Constraint(float[] p, float[] i, float dt)
    {
        if (Mathf.Abs(i[0]) < Imax)
        {
            pp = 0.01f * ppset + 0.99f * pp;
            p[0] = pp;
            //ii = 0.01f * i[0] + 0.99f * ii;
        }
        else
        {
            pp = 0.01f * p[0] + 0.99f * pp;
            //ii = 0.01f * 0 + 0.99f * ii;
            //i[0] = ii;
            i[0] = Mathf.Clamp(i[0], -Imax, Imax);
        }
    }

    public override void Reset_i_p()
    {
        base.Reset_i_p();
        pp = ppset = 0;
    }

    private void Update()
    {
        if (water)
            water.GetComponent<Image>().color = PressureColor(ppset);
        if (water0)
            water0.GetComponent<Image>().color = PressureColor(p0);
        
        if (bubble)
            bubble.GetComponent<Animator>().SetFloat("speed", -SpeedAnim());

        if (arrow)
        {
            if (ppset <= 0)
                arrow.GetComponent<Animator>().SetBool("Negative", true);
            else
                arrow.GetComponent<Animator>().SetBool("Negative", false);
        }

        if (moulin)
        {
            moulin.GetComponent<Animator>().SetFloat("speed", Mathf.Clamp(ppset,-3f,3f));
        }

        if (isSuccess)
        {
            const float timeSuccess = 4.0f;  
            if (f > 0.2f)
                success = Mathf.Clamp(success + Time.deltaTime/ timeSuccess, 0, 1);
            else
                success = Mathf.Clamp(success - 10 * Time.deltaTime/ timeSuccess, 0, 1);
        }

        if (Mathf.Abs(f) > fMinBubble)
        {

            if (!audios[3].isPlaying && !audios[4].isPlaying && !audios[5].isPlaying)
            {
                int r = UnityEngine.Random.Range(0, 3);
                audios[3 + r].Play();
            }
            audios[3].volume = audios[4].volume = audios[5].volume = Mathf.Abs(f) / fMinBubble * 0.1f;

        }


    }

}
