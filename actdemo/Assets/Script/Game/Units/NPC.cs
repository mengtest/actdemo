using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	//AI
	//Animator
	//Attack
	//寻路

	AnimatorComponent m_AnimatorManager;

	NpcAICtrl m_NpcAICtrl;

	InjuredComponent m_InjuredComponent;

	float npc_Hp = 0;
	float npc_Atk = 0;
	string npc_id = "0";
	//Npc 类型;
	int npc_type = 0;

	void Start()
	{
		if(this.gameObject.GetComponent<AnimatorComponent>() == null)
		{
			m_AnimatorManager = this.gameObject.AddComponent<AnimatorComponent>();
		}
		else
		{
			m_AnimatorManager = this.gameObject.GetComponent<AnimatorComponent>();
		}
		//
		if(this.gameObject.GetComponent<InjuredComponent>() == null)
		{
			m_InjuredComponent = this.gameObject.AddComponent<InjuredComponent>();
		}
		else
		{
			m_InjuredComponent = this.gameObject.GetComponent<InjuredComponent>();
		}

		if(this.gameObject.GetComponent<NpcAICtrl>() == null)
		{
			m_NpcAICtrl = this.gameObject.AddComponent<NpcAICtrl>();
		}
		else
		{
			m_NpcAICtrl = this.gameObject.GetComponent<NpcAICtrl>();
		}
		m_NpcAICtrl.NpcAICtrlStart(this);
	}

	public void Npc_Move(Vector3 vector)
	{
		m_AnimatorManager.RoleMoveCtrl();
		if(m_AnimatorManager.GetRotation())
		{
			//移动;
			Vector3 directionVector = vector - this.transform.position;
			
			Vector3 forwardVector = this.transform.forward.normalized;
			
			forwardVector -= new Vector3(0,forwardVector.y,0);
			
			this.transform.forward = Vector3.Lerp(forwardVector ,directionVector,0.3f);
		}
	}

	public void  Npc_Stand()
	{
		m_AnimatorManager.Stand();
	}

	public void Npc_Attack()
	{
		m_AnimatorManager.Stand();

		m_AnimatorManager.AttackCtrl();

	}
	public void Npc_Skill()
	{

	}
	public void Npc_Dead()
	{

	}

	void Update()
	{

	}



}
