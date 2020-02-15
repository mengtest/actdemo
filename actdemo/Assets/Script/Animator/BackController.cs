using UnityEngine;
using System.Collections;

public class BackController : StateMachineBehaviour
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
			animator.SetInteger(AnimatorComponent.AttackIndex, 0);
		}

		animator.SetBool("BackState",true);

        AudioManager.Instance.PlayBattleEffectAudio(animator, stateInfo);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("BackState",false);
        animator.SetBool("InjuredState",false);
	}
}
