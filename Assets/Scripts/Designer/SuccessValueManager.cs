using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuccessValueManager : MonoBehaviour
{

    float value;
    const float scoreSpeed = 2;

    float score = 0;
    protected BaseComponent component;
    protected Animator animator;
    bool valueVisible = false;
    protected Text textValue;

    private void Start()
    {
        component = GetComponentInParent<BaseComponent>();
        animator = transform.Find("Value").GetComponent<Animator>();
        textValue = transform.Find("Value").GetComponent<Text>();

        textValue.gameObject.SetActive(false);
        valueVisible = false;
    }

    void Update()
    {
        if (!component.isSuccess || !component.enabled)
        {
            if(valueVisible)
            {
                textValue.gameObject.SetActive(false);
                valueVisible = false;
            }
            return;
        }

        if (!valueVisible)
        {
            textValue.gameObject.SetActive(true);
            valueVisible = true;
        }

        value = component.success;

        if (score < value)
        {
            score += Time.deltaTime * scoreSpeed;
            if (score > value) score = value;
        }
        else if (score > value)
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


        textValue.text = Mathf.RoundToInt(score * 100) + "%";
    }
}
