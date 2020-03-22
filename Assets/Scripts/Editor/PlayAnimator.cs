using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PlayAnimator : MonoBehaviour
{
    public Animator animator;

    
    public void Update()
    {
        animator.Update(Time.deltaTime);
    }
}
