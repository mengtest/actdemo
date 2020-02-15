using UnityEngine;
using System.Collections;

public class FloatController : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Blow", true);

        AudioManager.Instance.PlayBattleEffectAudio(animator, stateInfo);
    }
	
	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		if (animator.GetBool("FloatBackState"))
		{
			animator.SetBool("FloatBackState", false);
		}

        animator.SetBool("Blow", false);
	}
}