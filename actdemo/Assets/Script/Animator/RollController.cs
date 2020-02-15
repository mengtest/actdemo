using UnityEngine;
using System.Collections;

public class RollController : StateMachineBehaviour
{
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
//	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
//	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        return;

        if (animator.name == "100008")
        {
            RoleControllerManager.Instance.CreateRole(140008);
            FightManager.Instance.mFightUI.Init();
        }
        else
        {
            RoleControllerManager.Instance.CreateRole(100008);
            FightManager.Instance.mFightUI.Init();
        }
	}
}
