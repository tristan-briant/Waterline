using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoLoader : MonoBehaviour
{
    void Start()
    {
        LevelManager lvl = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        string lvlname = lvl.GetPlaygroundName(lvl.currentLevel);
        GameObject tuto = Resources.Load("Tutos/" + lvlname) as GameObject;

        if (tuto == null)
            Debug.Log("no tuto");
        else
        {
            GameObject t= Instantiate(tuto);
            Transform vp = transform.Find("ViewPort");
            foreach (Transform child in vp)
                Destroy(child.gameObject);

                t.transform.SetParent(vp);
                t.transform.localPosition=Vector3.zero;
                t.transform.localScale=Vector3.one;

        }
    }
}
