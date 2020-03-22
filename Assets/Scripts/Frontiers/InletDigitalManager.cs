using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InletDigitalManager : BaseFrontier
{

    public float pset;
    float ppset;
    public float imax = 1;
    float pp = 0;
    float ii = 0;
    GameObject water, water0, arrow, green, red;
    //public bool isSuccess = false;
    public bool jelly = false;
    Color jellyColor = new Color(0xFF / 255.0f, 0x42 / 255.0f, 0x6A / 255.0f);
    Color jellyColorBg = new Color(0x42 / 255.0f, 0x42 / 255.0f, 0x42 / 255.0f);
    public int mode = 0;
    public float periode = 2;



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


    //float Rin = 2; //resistance interne
    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {

        float p0 = p[0];

        switch (mode)
        {
            case 0:  // Mode normal
                ppset = pset;
                break;
            case 1: //mode périodique
                if (Time.time % periode < periode / 2)
                    ppset = pset;
                else
                    ppset = 0;
                //ppset = pset * Mathf.Sin(2 * Mathf.PI * Time.time / periode);
                break;

        }

        R = 1;
        q += (i[0] + ii) / C * alpha;
        f += (p[0] - pp) / L * alpha * 0;

        pp = ppset;
        p[0] = (q + (i[0] - f) * R);
        i[0] = (f + (p0 - q) / R);
        ii = (-f + (pp - q) / R);


        if (isSuccess)
        {
            if (Mathf.Abs(f) > 0.1)
                success = Mathf.Clamp(success + 0.005f, 0, 1);
            else
                success = Mathf.Clamp(success - 0.05f, 0, 1);
        }

    }

    protected override void Start()
    {
        base.Start();
        water = transform.Find("Water").gameObject;
        water0 = transform.Find("Water0").gameObject;

        arrow = transform.Find("Arrow").gameObject;
        red = transform.Find("Red").gameObject;
        green = transform.Find("Green").gameObject;

        if (jelly)
        {
            if (pset > 0)
            {
                water.GetComponent<Image>().color = jellyColor;
                water0.GetComponent<Image>().color = jellyColor;
            }
            else
            {
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


        switch (mode)
        {
            case 0:  // Mode normal statique
                if (ppset <= 0)
                {
                    arrow.transform.localScale = new Vector3(-1, 1, 1);

                }
                else
                    arrow.transform.localScale = new Vector3(1, 1, 1);
                break;
            case 1: //mode périodique
            case 2:
                arrow.transform.localScale = new Vector3(Mathf.Clamp(ppset, -1, 1), 1, 1);
                break;
        }

        if (ppset <= 0)
        {
            arrow.transform.localPosition = new Vector3(25 - (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100, 0, 0);
            red.SetActive(true);
            green.SetActive(false);

        }
        else
        {
            arrow.transform.localPosition = new Vector3(25 + (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100, 0, 0);
            red.SetActive(false);
            green.SetActive(true);
        }
    }


}
