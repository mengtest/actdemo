using UnityEngine;
using System.Collections;

public class AnimatedVegetationEffect_VirtualAgent : AnimatedVegetationEffect 
{
    protected float delay = 0.0f;

    protected float duration = 0.0f;

    protected float radius = 0.0f;

    protected float velocityFactor = 0.0f;

    protected float volumeFactor = 0.0f;

    protected Vector3 position;

    protected float degree = 0.0f;

    protected float distance = 0.0f;

    protected bool isAzimuth = false;

    public AnimatedVegetationEffect Init(Vector3 position, float radius, float velocityFactor, float volumeFactor, float delay, float duration)
    {
        base.Init();

        isAzimuth = false;
        this.position = position;
        this.delay = delay;
        this.duration = duration;
        this.radius = radius;
        this.velocityFactor = velocityFactor;
        this.volumeFactor = volumeFactor;
        m_isSpecificTarget = false;

        m_type = AnimatedVegetationEffectType.VirtualAgent;

        return this;
    }

    [AnimatedVegetationEffectInit]
    public AnimatedVegetationEffect Init(float degree, float distance, float radius, float velocityFactor, float volumeFactor, float delay, float duration)
    {
        base.Init();

        isAzimuth = true;
        this.degree = degree;
        this.distance = distance;
        this.delay = delay;
        this.duration = duration;
        this.radius = radius;
        this.velocityFactor = velocityFactor;
        this.volumeFactor = volumeFactor;
        m_isSpecificTarget = false;

        m_type = AnimatedVegetationEffectType.VirtualAgent;

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
            if(AnimatedVegetationSceneManager.instance != null)
            {
                if (agent == null)
                {
                    Transform target = AnimatedVegetationSceneManager.instance.virtualAgents.Get();
                    if (target != null)
                    {
                        if (isAzimuth)
                        {
                            if (reference != null)
                            {
                                Vector3 dir = reference.forward;
                                dir.y = 0;
                                dir = Quaternion.AngleAxis(degree, Vector3.up) * dir;
                                dir.Normalize();
                                target.position = reference.position + dir * distance;
                            }
                        }
                        else
                        {
                            target.position = position;
                        }
                        agent = AnimatedVegetationSceneManager.instance.AddAgent(target, radius, velocityFactor, volumeFactor);
                    }
                }
            }
            UpdateInvoke();
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
            if(AnimatedVegetationSceneManager.instance != null)
            {
                AnimatedVegetationSceneManager.instance.RemoveAgent(agent.target);
                
                if(AnimatedVegetationSceneManager.instance.virtualAgents != null)
                {
                    AnimatedVegetationSceneManager.instance.virtualAgents.Release(agent.target);
                }
            }
            agent = null;
        }

        base.Destroy();
    }

    protected virtual void UpdateInvoke()
    {
        // Do nothing
    }

    public override void InitTarget(AnimatedVegetationEffect target)
    {
        if (target is AnimatedVegetationEffect_VirtualAgent)
        {
            var obj = target as AnimatedVegetationEffect_VirtualAgent;
            if (isAzimuth)
            {
                obj.Init(degree, distance, radius, velocityFactor, volumeFactor, delay, duration);
            }
            else
            {
                obj.Init(position, radius, velocityFactor, volumeFactor, delay, duration);
            }
        }
    }
}
