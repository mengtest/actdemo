using UnityEngine;
using System.Collections;

public class AnimatedVegetationEffect_Radius : AnimatedVegetationEffect 
{
    private float delay = 0.0f;

    private float duration = 0.0f;

    private float radius = 0.0f;

    [AnimatedVegetationEffectInit]
    public AnimatedVegetationEffect Init(float radius, float delay, float duration)
    {
        base.Init();

        this.delay = delay;
        this.duration = duration;
        this.radius = radius;

        m_type = AnimatedVegetationEffectType.Radius;

        return this;
    }

    public override void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
        }
        else
        {
            if (agent != null)
            {
                agent.radiusOverride = radius;
            }

            duration -= Time.deltaTime;
        }
    }

    public override bool IsComplete()
    {
        return delay <= 0.0f && duration < 0.0f;
    }

    public override void Destroy()
    {
        if (agent != null)
        {
            agent.radiusOverride = 0.0f;
        }

        base.Destroy();
    }

    public override void InitTarget(AnimatedVegetationEffect target)
    {
        if (target is AnimatedVegetationEffect_Radius)
        {
            (target as AnimatedVegetationEffect_Radius).Init(radius, delay, duration);
        }
    }
}
