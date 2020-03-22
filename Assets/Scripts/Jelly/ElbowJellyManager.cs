using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElbowJellyManager : BaseComponent {

    GameObject jelly3, jelly2, bubble;
    public float x_bulle = 0;
    float r_bulle = 0.2f;
    public float Capa = 1.0f;
    bool full = false;
    bool entered = false;

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {

        q += (i[3] + i[2]) / Capa ; // Without alpha to have a constant filling time

        if (q > 0 && !entered)
        {
            locked = true; //Once the jelly enter the pipe, the pipe cannot be moved
            entered = true;
            if (p[3] > 0) f = 1;
            else f = -1;
        }

        if (q >= 1 && !full)
        {             //pipe is full
            full = true;
        }

        if (f > 0 && q >= 1)
        {
            p[2] = 1;
            i[2] = -1;
        }
        if (f < 0 && q >= 1)
        {
            p[3] = 1;
            i[3] = -1;
        }


        //i[1]=i[3]=0;
        i[1] = p[1] / Rground;
        i[0] = p[0] / Rground;

        x_bulle -= 0.01f * f * 100 / Capa;

        if (q > 1 + 2 / Capa / alpha)
        {
            fail = 1;
            i[3] = i[2] = 0; f = 0;
        }

        
    }

    protected override void Start()
    {
        base.Start();
        jelly3 = this.transform.Find("Jelly3").gameObject;
        jelly2 = this.transform.Find("Jelly2").gameObject;

        bubble = this.transform.Find("Bubble").gameObject;
        jelly3.GetComponent<Image>().fillAmount = 0;
        jelly2.GetComponent<Image>().fillAmount = 0;
    }

    private void Update()
    {
        if (f > 0) jelly3.GetComponent<Image>().fillAmount = q;
        if (f < 0) jelly2.GetComponent<Image>().fillAmount = q;

        if (fail>=1) //(q > 1 + 2 / Capa / alpha)
        {
            jelly3.GetComponent<Image>().color = Color.Lerp(jelly3.GetComponent<Image>().color , new Color(1, 1, 1),0.1f);
            jelly2.GetComponent<Image>().color = Color.Lerp(jelly2.GetComponent<Image>().color, new Color(1, 1, 1), 0.1f);
        }

        if (full && Mathf.Abs(f) > 0.01f){

            float x_max = 0.5f - r_bulle / 2f;

            if (x_bulle > x_max) x_bulle = -x_max; /// 3 for PI
            if (x_bulle < -x_max) x_bulle = x_max;


            bubble.transform.localPosition = new Vector3((Mathf.Cos((x_bulle - 0.5f) * Mathf.PI / 2) * 0.5f - 0.5f) * 100, (-Mathf.Sin((x_bulle - 0.5f) * Mathf.PI / 2) * 0.5f - 0.5f) * 100, 0);
            bubble.SetActive(true);
            }
            else
            {
                bubble.SetActive(false);
            }
    }
}
