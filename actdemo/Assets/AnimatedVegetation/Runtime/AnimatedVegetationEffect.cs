using UnityEngine;
using System.Collections;

public abstract class AnimatedVegetationEffect
{
    public AnimatedVegetationAgent agent = null;

    public Transform reference = null;

    protected AnimatedVegetationEffectType m_type;
    public AnimatedVegetationEffectType type
    {
        get
        {
            return m_type;
        }
    }

    public bool markAsDestroy = false;

    protected bool m_isSpecificTarget = true;
    public bool isSpecificTarget
    {
        get
        {
            return m_isSpecificTarget;
        }
    }

    public AnimatedVegetationEffect()
    {

    }

    protected void Init()
    {
        markAsDestroy = false;
    }

    public abstract void Update();

    public abstract bool IsComplete();

    public virtual void Destroy()
    {
        agent = null;
    }

    public abstract void InitTarget(AnimatedVegetationEffect target);
}
