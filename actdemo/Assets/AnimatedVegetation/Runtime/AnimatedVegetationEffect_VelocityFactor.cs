using UnityEngine;
using System.Collections;

public class AnimatedVegetationEffect_VelocityFactor : AnimatedVegetationEffect 
{
    private float delay = 0.0f;

    private float duration = 0.0f;

    private float velocityFactor = 0.0f;

    [AnimatedVegetationEffectInit]
    public AnimatedVegetationEffect Init(float velocityFactor, float delay, float duration)
    {
        base.Init();

        this.delay = delay;
        this.duration = duration;
        this.velocityFactor = velocityFactor;

        m_type = AnimatedVegetationEffectType.VelocityFactor;

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
                agent.velocityFactorOverride = velocityFactor;
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
            agent.velocityFactorOverride = 0.0f;
        }

        base.Destroy();
    }

    public override void InitTarget(AnimatedVegetationEffect target)
    {
        if (target is AnimatedVegetationEffect_VelocityFactor)
        {
            (target as AnimatedVegetationEffect_VelocityFactor).Init(velocityFactor, delay, duration);
        }
    }
}
