using UnityEngine;
using System.Collections;

public class AttackController : StateMachineBehaviour {

	

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(animator.GetBool("InjuredState"))
		{
			animator.SetInteger(AnimatorComponent.AttackIndex, 0);
		}

		if(animator.GetInteger("Skill") >0)
		{
		//	animator.SetInteger(AttackIndex , 0);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(stateInfo.IsName("attack_04"))
		{
	//		animator.SetBool("StandState",true);
			animator.SetInteger(AnimatorComponent.AttackIndex, 0);
		}
		else if(stateInfo.IsName("attack_03") && animator.GetInteger(AnimatorComponent.AttackIndex) <= 3)
		{
		//	animator.SetBool("StandState",true);
			animator.SetInteger(AnimatorComponent.AttackIndex , 0);
		}
		else if(stateInfo.IsName("attack_02") && animator.GetInteger(AnimatorComponent.AttackIndex) <=2)
		{
	//		animator.SetBool("StandState",true);
			animator.SetInteger(AnimatorComponent.AttackIndex , 0);
		}
		else if(stateInfo.IsName("attack_01") && animator.GetInteger(AnimatorComponent.AttackIndex) <=1)
		{
			animator.SetBool("StandState",true);
			animator.SetInteger(AnimatorComponent.AttackIndex , 0);
		}
		
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
