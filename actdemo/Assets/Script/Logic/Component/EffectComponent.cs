using UnityEngine;
using System.Collections;

public class EffectComponent : IComponent {

	public void AttackEffect(int effect)
	{
		string pathName ="Effect/Skill";
	
		GameObject effectGame = GetEffectPrefab(pathName);
		Destroy(effectGame,2f);
	}

	public void AttackEffect(string effectName ,float time = 2)
	{
		//Debug.Log("effectName = "+effectName);
		string pathName ="Effect/Skill/"+effectName;
	
		GameObject effectGame = GetEffectPrefab(pathName);
		Destroy(effectGame,time);
	}


	GameObject GetEffectPrefab(string pathName)
	{
		GameObject obj = Resources.Load(pathName , typeof(GameObject))as GameObject;
		GameObject effect = Instantiate(obj,this.transform.position,this.transform.rotation)as GameObject;
		effect.transform.localScale = new Vector3(1.2f,1.2f,1.2f);
		return effect;
	}



}
