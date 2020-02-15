using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 技能范围计算;
/// </summary>
public class AttackRangeDefine  
{
	/// <summary>
	/// 计算技能作用的对象;
	/// </summary>
	/// <returns><c>true</c>, if range sector was set, <c>false</c> otherwise.</returns>
	public List<Transform> GetTargets(Vector3 center , Vector3 direction, int skillId)
	{
		SkillRangeType my_SkillRangeType = SkillRangeType.SkillRangeType_Unknown;
		switch(my_SkillRangeType)
		{
		case SkillRangeType.SkillRangeType_Unknown:
			break;
		case SkillRangeType.SkillRangeType_SelfSector:
			break;
		case SkillRangeType.SkillRangeType_SelfLine:
			break;
		case SkillRangeType.SkillRangeType_SelfRound:
			break;
		default :
			break;
		}
		return null;
	}

	/// <summary>
	/// 是否在圆形范围内;
	/// </summary>
	/// <returns><c>true</c> if this instance is direction; otherwise, <c>false</c>.</returns>
	public static  bool IsRound(Vector3 target , Vector3 center , float distance)
	{
		return Vector3.Distance(target , center) <= distance;
	}
	/// <summary>
	/// 计算是否在扇形范围内;
	/// </summary>
	/// <returns><c>true</c> if this instance is sector the specified center direction radian; otherwise, <c>false</c>.</returns>
	/// <param name="center">扇形点顶.</param>
	/// <param name="direction">扇形中心方向.</param>
	/// <param name="radian">扇形弧度.</param>
	public static bool IsSector(Vector3 target , Vector3 center , Vector3 direction, float radian , float distance)                                                                            
	{
		if(IsRound(target ,center ,distance ))
		{
			float cos = Mathf.Cos(radian/360*Mathf.PI);

			Vector3 forward = (target-center).normalized;

			return Vector3.Dot(direction.normalized ,forward ) >= cos;
		}
		else
		{
			return false;
		}
	}
	/// <summary>
	///计算是否在区域前方区域内范围;
	/// </summary>
	/// <returns><c>true</c> if this instance is line the specified target center direction width length; otherwise, <c>false</c>.</returns>
	/// <param name="target">目标.</param>
	/// <param name="center">本体.</param>
	/// <param name="direction">本体方向ction.</param>
	/// <param name="width">宽度.</param>
	/// <param name="length">长度.</param>
	public static bool IsLine(Vector3 target , Vector3 center ,Vector3 direction, float width, float length)
	{
		Vector3 v1;
		if(length >= 0)
		{
			v1 = target-center;
		}
		else
		{
			v1 = center-target;
		}
        		
		Vector3 v2  = Vector3.Project(v1,direction);

		Vector3 v3 = v2 + center;

		float f = Vector3.Dot(v2 , direction);

		if(f >= 0 && f<= Mathf.Abs(length) &&  Vector3.Distance(v3 ,target ) <= width)
		{
			return 	true;
		}
		else
		{
			return false;
		}
	}


}
