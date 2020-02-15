using UnityEngine;
using System.Collections;

public class AnimatedVegetationEffect_Wave : AnimatedVegetationEffect 
{
    private float waveSpeed = 0.0f;

    private float waveRadius = 0.0f;

    private float delay = 0.0f;

    private float radius = 0.0f;

    // 0: wait
    // 1: expand
    // 2: contract
    // 3: complete
    private int state = 0;

    [AnimatedVegetationEffectInit]
    public AnimatedVegetationEffect Init(float delay, float waveRadius, float waveSpeed)
    {
        base.Init();

        state = 0;

        this.delay = delay;
        this.waveSpeed = waveSpeed;
        this.waveRadius = waveRadius;

        m_type = AnimatedVegetationEffectType.Wave;

        return this;
    }

    public override void Update()
    {
        if (state == 0)
        {
            delay -= Time.deltaTime;
            if (delay < 0.0f)
            {
                state = 1;
            }
        }
        if (state == 1)
        {
            radius += waveSpeed * Time.deltaTime;
            if (agent != null)
            {
                agent.radiusOverride = radius;
            }
            if (radius > waveRadius)
            {
                state = 2;
            }
        }
        if (state == 2)
        {
            radius -= waveSpeed * Time.deltaTime;
            if (agent != null)
            {
                agent.radiusOverride = radius;
                if (radius < 0.0f)
                {
                    state = 3;
                }
            }
        }
    }

    public override bool IsComplete()
    {
        return state == 3;
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
        if (target is AnimatedVegetationEffect_Wave)
        {
            (target as AnimatedVegetationEffect_Wave).Init(delay, waveRadius, waveSpeed);
        }
    }
}
