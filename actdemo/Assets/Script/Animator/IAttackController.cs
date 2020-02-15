using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
// 普通攻击 命名规则为 attack_1 attack_2 以此类推;
 ***/
public class IAttackController : StateMachineBehaviour
{ 
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EffectManager.Instance.SkillEffect(animator, stateInfo);
        AudioManager.Instance.PlaySkillAudio(animator, stateInfo);
    }

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//被打断攻击;
		if(animator.GetBool("InjuredState"))
		{
			animator.SetInteger(AnimatorComponent.AttackIndex , 0);
		}
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        AnimatorComponent mAnimatorComponent = animator.GetComponent<AnimatorComponent>();
        if (mAnimatorComponent == null)
        {
            return;
        }

        IObject obj = mAnimatorComponent.mObj;
        if (obj == null)
        {
            return;
        }

        if (obj is CRoleObject)
        {
            if (stateInfo.IsName("attack_4"))
            {
                animator.SetInteger("AttackIndex", 0);
            }
        }
        else if (obj is CMonsterObject)
        {
            if (stateInfo.IsName("attack_2"))
            {
                animator.SetInteger("AttackIndex", 0);
            }
        }
    }
}
