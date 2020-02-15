using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcAICtrl : MonoBehaviour {


	Vector3[] point = new Vector3[2]{new Vector3(-94.3f,5f,-80.5f),new Vector3(-94.3f,5f,-80.5f)};

	NPC m_Npc;
	public void NpcAICtrlStart(NPC npc)
	{
		this.m_Npc = npc;
	}

	//攻击；
	//寻路；
	//位移；

	/// <summary>
	/// 攻击距离;
	/// </summary>
	float atkDistance = 5;
	/// <summary>
	/// 视野范围;
	/// </summary>
	float searchDistance;
	/// <summary>
	/// 攻击目标 攻击保证第一目标;
	/// </summary>
	List<Transform> myTargets = new List<Transform>();
	/// <summary>
	/// Npc最终目标地;
	/// </summary>
	Vector3 finalTaget;

	private Transform npc_Target;

	


	public enum NPCSTATE
	{
		STAND,//待机
		MOVE, //移动;
		ATTCK, //攻击;
		CHASE, //追杀;
	}

	private NPCSTATE my_npcState;

	/// <summary>
	/// 根据目标点判断NPC行为;
	/// </summary>
	/// <param name="vector">Vector.</param>
	void NpcUpdate( Vector3 vector)
	{
		if(Vector3.Distance(this.transform.position ,vector ) > atkDistance)
		{
			my_npcState = NPCSTATE.MOVE;
			m_Npc.Npc_Move(vector);
		}
		else
		{
			my_npcState = NPCSTATE.ATTCK;
			if(index == 0)
			{
				index =1;
			}
			else
			{
				index =0;
			}
		}
	}

	void Start () {

	}

	int index = 0;
	void Update () {
	
		if(npc_Target == null)
		{
			if(GameObject.Find("Player") != null)
			{
				npc_Target = GameObject.Find("Player").transform;
			}
			else
			{
				return;
			}
		}

		if(npc_Target)
		{
			NpcUpdate(new Vector3(-94.3f,5f,-80.5f));
		}


	}

}
