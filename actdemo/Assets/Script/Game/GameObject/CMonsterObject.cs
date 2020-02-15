using UnityEngine;
using System.Collections;

public class CMonsterObject : IObject
{
    /// <summary>
    /// 怪物攻击范围
    /// </summary>
    public float mAttackRange;

    public CMonsterObject()
    {
        
    }

    public override void OnAddObject()
    {
        base.OnAddObject();

        OnObjectPropertyChange("Attack");
    }

    public override void OnRemoveObject()
    {
        base.OnRemoveObject();
    }
}
