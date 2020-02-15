﻿using UnityEngine;
using System.Collections;

public class CameraAnimationController : StateMachineBehaviour
{
	// OnStateExit is called before OnStateExit is called on any state inside this state machine
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RoleCameraManager.Instance.FinishCameraAnimation();
	}
}
