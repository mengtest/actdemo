using UnityEngine;
using System.Collections;

public class ToppleController : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("ToppleState", true);
        animator.SetBool("Blow", false);

        AudioManager.Instance.PlayBattleEffectAudio(animator, stateInfo);
        EffectManager.Instance.SkillEffect(animator, stateInfo);
    }
    
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("InjuredState", false);
	}
}