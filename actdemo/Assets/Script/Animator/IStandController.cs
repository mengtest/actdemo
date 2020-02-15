using UnityEngine;
using System.Collections;

public class IStandController : StateMachineBehaviour
{
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		if (animator.GetInteger("Skill") != 0)
		{
			animator.SetInteger("Skill" , 0);
		}

        if (animator.GetInteger(AnimatorComponent.AttackIndex) != 0)
		{
			animator.SetInteger(AnimatorComponent.AttackIndex , 0);
		}

		if (animator.GetBool("Swoon"))
		{
			animator.SetBool("Swoon",false);
		}

		if (animator.GetBool("ToppleState"))
		{
			animator.SetBool("ToppleState",false);
		}

		if (animator.GetBool("InjuredState"))
		{
			animator.SetBool("InjuredState" , false);
		}

		if (animator.speed != 1)
		{
			animator.speed = 1;
		}
	}
}
