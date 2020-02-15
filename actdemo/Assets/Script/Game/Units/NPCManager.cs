using UnityEngine;
using System.Collections;

public class NPCManager  {

	private static NPCManager instance;

	public static NPCManager GetInstance
	{
		get
		{
			if(instance == null)
			{
				instance = new NPCManager();
			}
			return instance;
		}
	}

	/// <summary>
	/// 初始化Npc
	/// </summary>
	public void StartInif()
	{
		//读取NPC信息;
		//初始化生成点;
		//
	}

	
	public void NpcCtrl_Update () {
	
	}
}
