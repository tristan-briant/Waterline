using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueManager : MonoBehaviour
{
    public float value;
    float success;
    const float scoreSpeed = 1;
    public bool drawSetPoint = true;
    public bool lowResolution = false; // Only 1 digit even when value <1

    float score = 0;
    BaseComponent component;
    Animator animator;
    Text textValue;

    void Awake()
    {
        component = GetComponentInParent<BaseComponent>();
        animator = GetComponent<Animator>();
        textValue = transform.Find("Value").GetComponent<Text>();
    }

    public void ReDraw(float setpoint = 0)
    {
        component = GetComponentInParent<BaseComponent>();
        if (component.isSuccess && drawSetPoint)
        {
            transform.Find("SetPoint").gameObject.SetActive(true);
            transform.Find("Dash").gameObject.SetActive(true);
            float setPoint = Mathf.Round(100 * setpoint) / 100;
            transform.Find("SetPoint").GetComponent<Text>().text = setPoint.ToString();
        }
        else
        {
            transform.Find("SetPoint").gameObject.SetActive(false);
            transform.Find("Dash").gameObject.SetActive(false);
        }
    }

    float val;

    void Update()
    {
        success = component.success;

        if (score < success)
        {
            score += Time.deltaTime * scoreSpeed;
            if (score > success) score = success;
        }
        else if (score > success)
        {
            score -= Time.deltaTime * scoreSpeed;
            if (score < 0) score = 0;
        }

        if (score == 1)
            animator.SetTrigger("success");
        else
        {
            animator.ResetTrigger("success");
            animator.SetFloat("value", score);
        }

        if (!float.IsNaN(value))
            val = 0.9f * val + 0.1f * value;

        float v;
        if (Mathf.Abs(val) < 1 && !lowResolution)
            v = Mathf.Round(100 * val) / 100;
        else
            v = Mathf.Round(10 * val) / 10;
        textValue.text = v.ToString();

    }
}
