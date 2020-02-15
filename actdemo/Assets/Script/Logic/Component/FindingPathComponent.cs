using UnityEngine;
using System.Collections;

/// <summary>
/// 寻路组件
/// </summary>
public class FindingPathComponent : IComponent
{
    public UnityEngine.AI.NavMeshAgent mNavMeshAgent;

    public override void SetObject(IObject obj)
    {
        base.SetObject(obj);

        GameObject go = obj.mGameObject;
        if (go == null)
        {
            return;
        }
        
        mNavMeshAgent = go.AddComponent<UnityEngine.AI.NavMeshAgent>();
        mNavMeshAgent.stoppingDistance = obj.GetProperty("AttackRange").GetFloat();
        mNavMeshAgent.autoBraking = true;
    }
}