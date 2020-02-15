using UnityEngine;
using System.Collections;

public class AnimatedVegetationEffectObjectPoolManager
{
    private AnimatedVegetationEffectObjectPool[] objectPools = null;

    public AnimatedVegetationEffectObjectPoolManager()
    {
        objectPools = new AnimatedVegetationEffectObjectPool[(int)AnimatedVegetationEffectType.NUM_TYPES];
        objectPools[(int)AnimatedVegetationEffectType.Radius] = new AnimatedVegetationEffectObjectPool(newObject_Radius, AnimatedVegetationEffectType.Radius);
        objectPools[(int)AnimatedVegetationEffectType.VelocityFactor] = new AnimatedVegetationEffectObjectPool(newObject_VelocityFactor, AnimatedVegetationEffectType.VelocityFactor);
        objectPools[(int)AnimatedVegetationEffectType.VolumeFactor] = new AnimatedVegetationEffectObjectPool(newObject_VolumeFactor, AnimatedVegetationEffectType.VolumeFactor);
        objectPools[(int)AnimatedVegetationEffectType.VirtualAgent] = new AnimatedVegetationEffectObjectPool(newObject_VirtualAgent, AnimatedVegetationEffectType.VirtualAgent);
        objectPools[(int)AnimatedVegetationEffectType.LineVirtualAgent] = new AnimatedVegetationEffectObjectPool(newObject_LineVirtualAgent, AnimatedVegetationEffectType.LineVirtualAgent);
        objectPools[(int)AnimatedVegetationEffectType.Wave] = new AnimatedVegetationEffectObjectPool(newObject_Wave, AnimatedVegetationEffectType.Wave);
        objectPools[(int)AnimatedVegetationEffectType.RndMoveVirtualAgent] = new AnimatedVegetationEffectObjectPool(newObject_RndMoveVirtualAgent, AnimatedVegetationEffectType.RndMoveVirtualAgent);
    }

    public AnimatedVegetationEffectObjectPool GetEffectObjectPool(AnimatedVegetationEffectType effectType)
    {
        if (objectPools == null)
        {
            return null;
        }
        return objectPools[(int)effectType];
    }

    private AnimatedVegetationEffect newObject_Radius()
    {
        return new AnimatedVegetationEffect_Radius();
    }

    private AnimatedVegetationEffect newObject_VelocityFactor()
    {
        return new AnimatedVegetationEffect_VelocityFactor();
    }

    private AnimatedVegetationEffect newObject_VolumeFactor()
    {
        return new AnimatedVegetationEffect_VolumeFactor();
    }

    private AnimatedVegetationEffect newObject_VirtualAgent()
    {
        return new AnimatedVegetationEffect_VirtualAgent();
    }

    private AnimatedVegetationEffect newObject_LineVirtualAgent()
    {
        return new AnimatedVegetationEffect_LineVirtualAgent();
    }

    private AnimatedVegetationEffect newObject_Wave()
    {
        return new AnimatedVegetationEffect_Wave();
    }

    private AnimatedVegetationEffect newObject_RndMoveVirtualAgent()
    {
        return new AnimatedVegetationEffect_RndMoveVirtualAgent();
    }

    public void Destroy()
    {
        if (objectPools != null)
        {
            int numObjectPools = objectPools.Length;
            for (int i = 0; i < numObjectPools; ++i)
            {
                var objectPool = objectPools[i];
                objectPools[i] = null;
                if (objectPool != null)
                {
                    objectPool.Destroy();
                }
            }
            objectPools = null;
        }
    }
}
