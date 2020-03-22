using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InletJellyManager : BaseFrontier {

    public float pset;
    public float imax = 1;
    //float pp = 0;
    float ii = 0;
    GameObject jelly0, jelly2, arrow;
    //public bool isSuccess = false;
    public float Capa=100;
    bool full = false;
    bool entered = false;


    protected override void Start()
    {
        base.Start();
        jelly0 = this.transform.Find("Jelly0").gameObject;
        jelly2 = this.transform.Find("Jelly2").gameObject;

        jelly0.GetComponent<Image>().fillAmount = 0;
        jelly2.GetComponent<Image>().fillAmount = 0;

        arrow = this.transform.Find("Arrow").gameObject;

        audios = GameObject.Find("PlaygroundHolder").GetComponents<AudioSource>();
        audios[7].Play();
    }

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {
        if (pset > 0)
            ii = 1;
        else
            ii = 0;

        q += (i[0] + ii) / Capa;

        if (q > 0 && !entered)
        {
            entered = true;
            if (p[0] > 0) f = 1;
            else f = -1;
        }

        if (q >= 1 && !full)
        {             //pipe is full
            full = true;
        }
        
        if (f < 0 && q >= 1)
        {
            p[0] = 1;
            i[0] = -1;
        }

        if (isSuccess)
        {
            if (full)
                success = 1;
            else
                success = 0;
        }
        else if (q > 1 + 2 / Capa / alpha)
        {
            fail = 1;
            ii = i[0] = 0; f = 0;
        }


        /*if (q > 1 + 10 / Capa )
            fail = 1;*/

    }

    // Update is called once per frame
    private void Update()
    {
        if (f > 0) jelly0.GetComponent<Image>().fillAmount = q;
        if (f < 0) jelly2.GetComponent<Image>().fillAmount = q;

        if (fail >= 1)// && success < 1) //(q > 1+2/Capa)
        {
            jelly0.GetComponent<Image>().color = Color.Lerp(jelly0.GetComponent<Image>().color, new Color(1, 1, 1), 0.1f);
            jelly2.GetComponent<Image>().color = Color.Lerp(jelly2.GetComponent<Image>().color, new Color(1, 1, 1), 0.1f);
            audios[7].Stop();
        }
       
        

        if (pset <= 0)
        {
            arrow.transform.localScale = new Vector3(-1, 1, 1);
            arrow.transform.localPosition = new Vector3(25 - (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100, 0, 0);

        }
        else
            arrow.transform.localPosition = new Vector3(25 + (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100, 0, 0);
    }
}
