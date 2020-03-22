using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dilatation : MonoBehaviour
{
    BaseComponent bc;
    Material material;
    public int index = 0; // 
    const float alpha = 0.2f;
    protected float pressure = 0;

    protected void Start()
    {
        Material mat = GetComponent<Image>().material;

        material = GetComponent<Image>().material = new Material(mat);

        bc = GetComponentInParent<BaseComponent>();
    }

    private void Update()
    {
        pressure = alpha * bc.GetPressure(index) + (1 - alpha) * pressure;
        material.SetFloat("_DilationCoefficent", pressure);
    }
}
