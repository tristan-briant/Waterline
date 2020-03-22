using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InletManager : BaseFrontier {

    public float pset;
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
    float ppset;

    public float imax = 1;
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



    float pp = 0;
    float ii=0;
    GameObject water,water0, arrow;
    //public bool isSuccess=false;
    public bool jelly = false;
    Color jellyColor = new Color(0xFF / 255.0f, 0x42 / 255.0f, 0x6A / 255.0f);
    Color jellyColorBg = new Color(0x42 / 255.0f, 0x42 / 255.0f, 0x42 / 255.0f);
    public int mode = 0;
    public float periode=2;



    IEnumerator generateRandom()
    {
        int s = 1;

        while (true)
        {
            ppset = pset * s;
            float time = Random.Range(0, periode);
            yield return new WaitForSeconds(time);

            const float N = 10;
            for (int i = 0; i < N; i++)
            {
                ppset = pset * (1 - 2 * i / N) * s;
                yield return new WaitForSeconds(0.02f);  // retournement en douceur
            }
            s = -s;
        }
    }


    public float Rin = 0; //resistance interne


    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {

        float p0 = p[0];

        switch (mode)
        {
            case 0:  // Mode normal
                ppset = pset;
                break;
            case 1: //mode périodique
                ppset = pset * Mathf.Sin(2 * Mathf.PI * Time.time / periode);
                break;

        }


        q += (i[0] + ii) / C * dt;
        f += (p[0] - pp) / L * dt;

        p[0] = (q + (i[0] - f) * R);
        //p[0] = ppset;
        i[0] = (f + (p0 - q) / R);
        ii = (-f + (pp - q) / R);

        if (-imax < ii && ii < imax)
        {
            //pp=ppset;      
            Rin = Mathf.Clamp(Rin - 0.05f, 0, 20);
            //Rin = 0;
        }
        else
        {
            Rin = Mathf.Clamp(Rin + 0.01f, 0, 20);

            // pp = 0.9f * pp;
            /*if(ii > imax)
                ii = imax;
            else
                ii = -imax;

            pp = p[0];*/
        }
        //Rin = 0;
        pp = ppset - ii * Rin;


        if (isSuccess)
        {
            if (Mathf.Abs(f) > 0.1)
                success = Mathf.Clamp(success + 0.005f, 0, 1);
            else
                success = Mathf.Clamp(success - 0.05f, 0, 1);
        }

    }

    public override void Calcule_i_p_blocked(float[] p, float[] i, float dt, int index)
    {
        
    }

    protected override void Start()
    {
        base.Start();
        tubeEnd[3] = true;
        water = this.transform.Find("Water").gameObject;
        water0 = this.transform.Find("Water0").gameObject;

        arrow = this.transform.Find("Arrow").gameObject;

        if (jelly)
        {
            if (pset > 0)
            {
                water.GetComponent<Image>().color = jellyColor;
                water0.GetComponent<Image>().color = jellyColor;
            }
            else {
                water.GetComponent<Image>().color = jellyColorBg;
                water0.GetComponent<Image>().color = jellyColorBg;
            }
        }

        if (mode == 2) StartCoroutine(generateRandom()); 
    }
        

    private void Update()
    {
       
        if (!jelly)
        {
            water.GetComponent<Image>().color = PressureColor(ppset);
            water0.GetComponent<Image>().color = PressureColor(pin[0]);
        }

        if (ppset <= 0)
            arrow.GetComponent<Animator>().SetBool("Negative", true);
        else
            arrow.GetComponent<Animator>().SetBool("Negative", false);


        /*switch (mode)
        {
            case 0:  // Mode normal
                if (ppset <= 0)
                    arrow.transform.localScale = new Vector3(-1, 1, 1);
                else
                    arrow.transform.localScale = new Vector3(1, 1, 1);
                break;
            case 1: //mode périodique
            case 2:
                arrow.transform.localScale = new Vector3(Mathf.Clamp(ppset,-1,1), 1, 1);
                break;
        }

        if (ppset <= 0)
        {
            arrow.transform.localPosition = new Vector3(25 - (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100, 0, 0);

        }
        else
            arrow.transform.localPosition = new Vector3(25+ (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100 ,0,0);
            */
    }


}
