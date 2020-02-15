using UnityEngine;
using System.Collections;
/// <summary>
/// 人物移动方向类型;
/// 在屏幕坐标中表示X = 坐标X轴  Y =坐标系Y轴  I= 1 , O = 0 ,H = 1/2 , T = -1/2 ;  
/// </summary>
public class UnitMoveState  {

	/// <summary>
	/// 获取移动方向
	/// </summary>
	/// <returns>The direction.</returns>
	/// <param name="axisX">Axis x.</param>
	/// <param name="axisY">Axis y.</param>
 	public static Vector3 MoveDirection(float axisX , float axisY)
	{
	//	Vector2 v = new Vector2(axisX , axisY).normalized;
		return new Vector3(axisX , 0 ,axisY).normalized;
	}
}
