/// <summary>
/// 三方玩家
/// </summary>
public class CPlayerObject : IObject
{
    public static readonly string[] DieMenSounds = new string[] { "die", "die1", "die2", "die3" };
    public static readonly string[] DieWomenSounds = new string[] { "womendie", "womendie1", "womendie2", "womendie3" };
    
    /// <summary>
    /// 增加对象
    /// </summary>
    override public void OnAddObject()
    {
        base.OnAddObject();

//         if (mDynamicUnit != null)
//         {
//             mDynamicUnit.isCollider = !GameSceneManager.IsCollideScene();
//             mDynamicUnit.dontComputeCollision = true;
//             mDynamicUnit.moveListener = MoveState;
//         }
    }

    /// <summary>
    /// 对象属性接入
    /// </summary>
    override public void OnObjectProperty()
    {
        base.OnObjectProperty();
    }
    /// <summary>
    /// 对象单个属性变化
    /// </summary>
    /// <param name="strPropName">变化的属性名</param>
    override public void OnObjectPropertyChange(string strPropName)
    {
        base.OnObjectPropertyChange(strPropName);
    }

    /// <summary>
    /// 对象删除前回调
    /// </summary>
    override public void OnRemoveObjectBefore(bool bImmediatelyDelete = false)
    {
        base.OnRemoveObjectBefore(bImmediatelyDelete);
    }
    /// <summary>
    /// 对象删除回调
    /// </summary>
    override public void OnRemoveObject()
    {
        base.OnRemoveObject();
    }

    /// <summary>
    /// 移动处理
    /// </summary>
    override public void OnMoving()
    {
        base.OnMoving();
    }

    /// <summary>
    /// 收到对象位置信息
    /// </summary>
    override public void OnLocation()
    {
        base.OnLocation();
    }

    /// <summary>
    /// 移动状态
    /// </summary>
    /// <param name="flag"></param>
    public override void MoveState(bool flag)
    {
        base.MoveState(flag);
    }

}
