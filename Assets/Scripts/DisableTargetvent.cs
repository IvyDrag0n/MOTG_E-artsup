using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableTargetvent : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.SetActive(false);    //Once the animation is over, deactivate the object
    }
}
