using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatedVegetationEffect_LineVirtualAgent : AnimatedVegetationEffect 
{
    private float delay = 0.0f;

    private float duration = 0.0f;

    private float radius = 0.0f;

    private float velocityFactor = 0.0f;

    private float volumeFactor = 0.0f;

    private float degree = 0.0f;

    private float distance = 0.0f;

    private float length = 0.0f;

    private float stepSize = 0.0f;

    private float stepTime = 0.0f;

    private int stepIndex = 0;

    private float stepTimePoint = 0.0f;

    private bool isComplete = false;

    private Vector3 startPosition;

    private Vector3 dir;

    private List<AnimatedVegetationEffect> subEffects = null;

    [AnimatedVegetationEffectInit]
    public AnimatedVegetationEffect Init(float degree, float distance, float length, float radius, float velocityFactor, float volumeFactor, float delay, float duration, float stepSize, float stepTime)
    {
        base.Init();

        stepIndex = 0;
        stepTimePoint = 0.0f;
        isComplete = false;
        if (subEffects != null)
        {
            subEffects.Clear();
        }

        this.degree = degree;
        this.distance = distance;
        this.delay = delay;
        this.duration = duration;
        this.length = length;
        this.radius = radius;
        this.velocityFactor = velocityFactor;
        this.volumeFactor = volumeFactor;
        this.stepSize = stepSize;
        this.stepTime = stepTime;
        m_isSpecificTarget = false;

        m_type = AnimatedVegetationEffectType.LineVirtualAgent;

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
            if(!isComplete && AnimatedVegetationSceneManager.instance != null && reference != null)
            {
                if (stepIndex == 0)
                {
                    startPosition = reference.position;

                    dir = reference.forward;
                    dir.y = 0;
                    dir = Quaternion.AngleAxis(degree, Vector3.up) * dir;
                    dir.Normalize();
                }

                if (stepTimePoint <= 0)
                {
                    bool doInOneFrame = stepTime < (1.0f / 30.0f);

                    do
                    {
                        Vector3 pos = startPosition + dir * stepSize * stepIndex;
                        if (Vector3.Distance(pos, startPosition) > length)
                        {
                            isComplete = true;
                            return;
                        }
                        else
                        {
                            stepTimePoint = stepTime;
                            ++stepIndex;

                            var subEffect = (AnimatedVegetationSceneManager.instance.NewAgentEffect(AnimatedVegetationEffectType.VirtualAgent) as AnimatedVegetationEffect_VirtualAgent).Init(pos, radius, velocityFactor, volumeFactor, 0, duration);
                            if (subEffects == null)
                            {
                                subEffects = new List<AnimatedVegetationEffect>();
                            }
                            subEffects.Add(subEffect);

                            AnimatedVegetationSceneManager.instance.AddAgentEffect(null, subEffect);
                        }
                    }
                    while(doInOneFrame);
                }

                stepTimePoint -= Time.deltaTime;
            }
        }
    }

    public override bool IsComplete()
    {
        bool allSubEffectsComplete = true;
        int numSubEffects = subEffects == null ? 0 : subEffects.Count;
        for(int i = 0; i < numSubEffects; ++i)
        {
            var subEffect = subEffects[i];
            if(subEffect != null && !subEffect.IsComplete())
            {
                allSubEffectsComplete = false;
                break;
            }
        }

        return isComplete && allSubEffectsComplete;
    }

    public override void Destroy()
    {
        if(subEffects != null)
        {
            int numSubEffects = subEffects == null ? 0 : subEffects.Count;
            for(int i = 0; i < numSubEffects; ++i)
            {
                var subEffect = subEffects[i];
                if(subEffect != null)
                {
                    subEffect.markAsDestroy = true;
                }
            }
            subEffects.Clear();
        }

        base.Destroy();
    }

    public override void InitTarget(AnimatedVegetationEffect target)
    {
        if (target is AnimatedVegetationEffect_LineVirtualAgent)
        {
            (target as AnimatedVegetationEffect_LineVirtualAgent).Init(degree, distance, length, radius, velocityFactor, volumeFactor, delay, duration, stepSize, stepTime);
        }
    }
}
