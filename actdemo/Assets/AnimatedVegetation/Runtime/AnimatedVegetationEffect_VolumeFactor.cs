using UnityEngine;
using System.Collections;

public class AnimatedVegetationEffect_VolumeFactor : AnimatedVegetationEffect 
{
    private float delay = 0.0f;

    private float duration = 0.0f;

    private float volumeFactor = 0.0f;

    [AnimatedVegetationEffectInit]
    public AnimatedVegetationEffect Init(float volumeFactor, float delay, float duration)
    {
        base.Init();

        this.delay = delay;
        this.duration = duration;
        this.volumeFactor = volumeFactor;

        m_type = AnimatedVegetationEffectType.VolumeFactor;

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
                agent.volumeFactorOverride = volumeFactor;
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
            agent.volumeFactorOverride = 0.0f;
        }

        base.Destroy();
    }

    public override void InitTarget(AnimatedVegetationEffect target)
    {
        if (target is AnimatedVegetationEffect_VolumeFactor)
        {
            (target as AnimatedVegetationEffect_VolumeFactor).Init(volumeFactor, delay, duration);
        }
    }
}
