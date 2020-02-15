using UnityEngine;
using System.Collections;

public class TopplebackController : StateMachineBehaviour
{
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.Instance.PlayBattleEffectAudio(animator, stateInfo);
    }
}