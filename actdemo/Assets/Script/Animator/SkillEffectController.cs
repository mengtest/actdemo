using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillEffectController : StateMachineBehaviour
{
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EffectManager.Instance.SkillEffect(animator, stateInfo);
        AudioManager.Instance.PlaySkillAudio(animator, stateInfo);

        if (stateInfo.IsName("skill_04_c"))
        {
            CameraAnimation.Instance.Animation("skill_4");
        }
    }

	// OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called before OnStateExit is called on any state inside this state machine
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("skill_3_b"))
        {
            AudioManager.Instance.StopSkillAudio(animator, stateInfo);
        }        

        Dictionary<int, EffectProperty> dic = XmlManager.Instance.GetEffectProperty();
        if (dic == null)
        {
            return;
        }

        foreach (KeyValuePair<int, EffectProperty> kvp in dic)
        {
            if (kvp.Value.mEffectRemovePath == null ||
                string.IsNullOrEmpty(kvp.Value.mEffectRemovePath))
            {
                continue;
            }

            if (stateInfo.IsName(kvp.Value.mEffectAction))
            {
                EffectManager.Instance.RemoveSkillEffect(kvp.Value.mEffectRemovePath, animator);

                return;
            }
        }
    }

	// OnStateMove is called before OnStateMove is called on any state inside this state machine
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called before OnStateIK is called on any state inside this state machine
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMachineEnter is called when entering a statemachine via its Entry Node
	//override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash){
	//
	//}

	// OnStateMachineExit is called when exiting a statemachine via its Exit Node
	//override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
	//
	//}
}
