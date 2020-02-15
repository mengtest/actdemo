using UnityEngine;
using System.Collections;

/// <summary>
/// 组件
/// 组件内部调用相关逻辑，外部仅调用组件接口
/// </summary>
public class IComponent : MonoBehaviour
{
    /// <summary>
    /// 组件父对象
    /// </summary>
    public IObject mObj = null;

    public IObject Obj
    {
        set { mObj = value; }
        get { return mObj; }
    }

    public virtual void SetObject(IObject obj)
    {
        mObj = obj;
    }
}