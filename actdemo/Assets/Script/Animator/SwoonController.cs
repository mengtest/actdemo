using UnityEngine;
using System.Collections;

public class SwoonController : StateMachineBehaviour
{
	public float swoonTime = 0;

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
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		if(animator.GetBool("Swoon")  &&  swoonTime > 0)
		{
			swoonTime -= Time.deltaTime;
		}
		else
		{
			animator.SetBool("Swoon", false);
		}
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		animator.SetBool("InjuredState", false);
	}
}
