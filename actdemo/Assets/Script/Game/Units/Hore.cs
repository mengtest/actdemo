using UnityEngine;
using System.Collections;
using UnityStandardAssets.Cameras;

public class Hore : MonoBehaviour {

	PlayerCtrlManager m_PlayerCtrlManager;

	AnimatorComponent m_AnimatorManager;

	void Start () 
	{
		if(this.gameObject.GetComponent<PlayerCtrlManager>() == null)
		{
			m_PlayerCtrlManager = this.gameObject.AddComponent<PlayerCtrlManager>();
		}
		else
		{
			m_PlayerCtrlManager = this.gameObject.GetComponent<PlayerCtrlManager>();
		}

		if(this.gameObject.GetComponent<AnimatorComponent>() == null)
		{
			m_AnimatorManager = this.gameObject.AddComponent<AnimatorComponent>();
		}
		else
		{
			m_AnimatorManager = this.gameObject.GetComponent<AnimatorComponent>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
