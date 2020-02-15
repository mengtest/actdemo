using UnityEngine;
using System.Collections;

public class BeatenSpeed : StateMachineBehaviour {

	public float isStopTime = 0;

	public bool isPlay = true;

	public bool isStop = false;
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponent<EffectComponent>().AttackEffect("100008_back");
	}


	//刷新动画每一帧调用；
	//// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		//受击
//		if(stateInfo.IsName("back"))
//		{
			if(isPlay)
			{
				animator.speed = 2;

				if(stateInfo.normalizedTime < 0.35f)
				{
					isStop   = true;
				}
			}
			if(isStopTime >0 && stateInfo.normalizedTime >= 0.35f && isStop)
			{
				isStop = false;
				animator.speed = 0.0001f;
				isPlay = false;
			}
			else if(isStopTime <= 0)
			{
				animator.speed = 1;
				isPlay = false;
			}

			if(animator.speed == 0.0001f)
			{
				isStopTime -= Time.deltaTime;
			}
//		}
	}

	//动画播放完毕的时；
//	 OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
		if(isPlay)// && stateInfo.IsName("back"))
		{
			if(stateInfo.IsName("back_0"))
			{
				animator.Play("back_0");
			}
			else if(stateInfo.IsName("back_1"))
			{
				animator.Play("back_1");
			}
			else if(stateInfo.IsName("back_2"))
			{
				animator.Play("back_2");
			}
			else if(stateInfo.IsName("back_3"))
			{
				animator.Play("back_3");
			}
			else if(stateInfo.IsName("back_4"))
			{
				animator.Play("back_4");
			}
			else if(stateInfo.IsName("back_5"))
			{
				animator.Play("back_5");
			}
		}
		else
		{
	//		animator.SetBool("StandState",true);
			animator.SetBool("InjuredState",false);
		}

	}
}
