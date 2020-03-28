using UnityEngine;
using System.Collections;

public class SkillController : StateMachineBehaviour {
	public bool isend  = false;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
		if(stateInfo.IsName("skill_03_a"))
		{
			animator.GetComponent<EffectComponent>().AttackEffect("100008_skill03_a");
		}
		else if(stateInfo.IsName("skill_03_b"))
		{
			animator.GetComponent<EffectComponent>().AttackEffect("100008_skill03_b");
		}
		else if(stateInfo.IsName("skill_03_c"))
		{
			animator.GetComponent<EffectComponent>().AttackEffect("100008_skill03_c");
		}
		else if(stateInfo.IsName("skill_01_b"))
		{
			animator.GetComponent<EffectComponent>().AttackEffect("100008_skill_01_b");
		}
		else if(stateInfo.IsName("skill_01_c"))
		{
			animator.GetComponent<EffectComponent>().AttackEffect("100008_skill_01_c");
		}
		else if(stateInfo.IsName("skill_02_b"))
		{
			animator.GetComponent<EffectComponent>().AttackEffect("100008_skill02_a");
		}
		else if(stateInfo.IsName("skill_02_c"))
		{
			//Debug.Log("========================!!");
			animator.GetComponent<EffectComponent>().AttackEffect("100008_skill02_c");
		}
		else if(stateInfo.IsName("skill_04"))
		{
			animator.GetComponent<EffectComponent>().AttackEffect("100008_skill_04_a",6);
		}
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(animator.GetBool("InjuredState"))
		{
			animator.SetInteger("Skill",0);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(isend)
			animator.SetInteger("Skill",0);
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
