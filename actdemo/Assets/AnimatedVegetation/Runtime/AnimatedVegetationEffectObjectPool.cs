using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatedVegetationEffectObjectPool
{
    public delegate AnimatedVegetationEffect NewObject();

    private List<AnimatedVegetationEffect> workingList = null;

    private List<AnimatedVegetationEffect> freeList = null;

    private NewObject newObject = null;

    private AnimatedVegetationEffectType effectType;

    public AnimatedVegetationEffectObjectPool(NewObject newObject, AnimatedVegetationEffectType effectType)
    {
        workingList = new List<AnimatedVegetationEffect>();
        freeList = new List<AnimatedVegetationEffect>();
        this.newObject = newObject;
        this.effectType = effectType;
    }

    public void Destroy()
    {
        if (workingList != null)
        {
            workingList.Clear();
            workingList = null;
        }
        if (freeList != null)
        {
            freeList.Clear();
            freeList = null;
        }
    }

    public AnimatedVegetationEffect Get()
    {
        if (freeList == null || workingList == null)
        {
            return null;
        }

        AnimatedVegetationEffect obj = null;
        if (freeList.Count > 0)
        {
            obj = freeList[freeList.Count - 1];
            freeList.RemoveAt(freeList.Count - 1);
            //Debug.LogError(effectType + " reuse");
        }
        else
        {
            if(newObject != null)
            {
                //Debug.LogError(effectType + " new");
                obj = newObject();
            }
        }

        if (obj != null)
        {
            workingList.Add(obj);
        }

        return obj;
    }

    public void Release(AnimatedVegetationEffect obj)
    {
        if (freeList == null || workingList == null || obj == null)
        {
            return;
        }

        int index = workingList.IndexOf(obj);
        if (index == -1)
        {
            return;
        }

        //Debug.LogError(effectType + " release");

        workingList.RemoveAt(index);
        freeList.Add(obj);
    }
}
