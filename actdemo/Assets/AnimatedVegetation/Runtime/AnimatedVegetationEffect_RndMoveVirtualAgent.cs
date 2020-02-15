using UnityEngine;
using System.Collections;

public class AnimatedVegetationEffect_RndMoveVirtualAgent : AnimatedVegetationEffect_VirtualAgent
{
    private float moveRadius = 0.0f;

    private float moveSpeed = 0.0f;

    private bool isFirstTime = true;

    private Vector3 centerPos;

    private Vector3 targetPos;

    public AnimatedVegetationEffect Init(Vector3 position, float radius, float velocityFactor, float volumeFactor, float delay, float duration, float moveRadius, float moveSpeed)
    {
        base.Init(position, radius, velocityFactor, volumeFactor, delay, duration);
        this.moveRadius = moveRadius;
        this.moveSpeed = moveSpeed;

        isFirstTime = true;

        m_type = AnimatedVegetationEffectType.RndMoveVirtualAgent;

        return this;
    }

    [AnimatedVegetationEffectInit]
    public AnimatedVegetationEffect Init(float degree, float distance, float radius, float velocityFactor, float volumeFactor, float delay, float duration, float moveRadius, float moveSpeed)
    {
        base.Init(degree, distance, radius, velocityFactor, volumeFactor, delay, duration);
        this.moveRadius = moveRadius;
        this.moveSpeed = moveSpeed;

        isFirstTime = true;

        m_type = AnimatedVegetationEffectType.RndMoveVirtualAgent;

        return this;
    }

    protected override void UpdateInvoke()
    {
        base.UpdateInvoke();

        if (agent != null)
        {
            if (isFirstTime)
            {
                if (agent.target != null)
                {
                    centerPos = agent.target.position;
                    targetPos = centerPos;
                }
                isFirstTime = false;
            }
            if(!isFirstTime)
            {
                if (agent.target != null)
                {
                    bool newTargetPos = (targetPos - agent.target.position).sqrMagnitude < 0.001f;
                    if (newTargetPos)
                    {
                        targetPos = centerPos + (Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Vector3.forward).normalized * moveRadius;
                    }
                    if (!newTargetPos)
                    {
                        Vector3 v = targetPos - agent.target.position;
                        float dist = v.magnitude;
                        dist = Mathf.Min(dist, moveSpeed * Time.deltaTime);
                        v.Normalize();
                        agent.target.position += dist * v;
                    }
                }
            }
        }
    }

    public override void InitTarget(AnimatedVegetationEffect target)
    {
        if (target is AnimatedVegetationEffect_RndMoveVirtualAgent)
        {
            var obj = target as AnimatedVegetationEffect_RndMoveVirtualAgent;
            if (isAzimuth)
            {
                obj.Init(degree, distance, radius, velocityFactor, volumeFactor, delay, duration, moveRadius, moveSpeed);
            }
            else
            {
                obj.Init(position, radius, velocityFactor, volumeFactor, delay, duration, moveRadius, moveSpeed);
            }
        }
    }
}
