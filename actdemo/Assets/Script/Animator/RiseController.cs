using UnityEngine;
using System.Collections;

public class RiseController : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("ToppleState", false);
    }
}