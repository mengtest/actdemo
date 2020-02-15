using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyBinaryXml;
using System;
using System.Reflection;

public class AnimatedVegetationXml
{
    private static AnimatedVegetationXml s_instance = null;

    public static AnimatedVegetationXml GetInstance()
    {
        if (s_instance == null)
        {
            s_instance = new AnimatedVegetationXml();
        }
        return s_instance;
    }

    private Dictionary<int, AnimatedVegetationXmlTriggerProperty> aveTriggers = null;

    private Dictionary<int, AnimatedVegetationXmlEffectProperty> aveEffects = null;

    public bool Init()
    {
        if (aveTriggers != null && aveEffects != null)
        {
            return true;
        }

        byte[] asset = ResourceManager.Instance.GetXml("AnimatedVegetationEffect");
        if (asset == null)
        {
            return false;
        }

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return false;
        }

        aveTriggers = new Dictionary<int, AnimatedVegetationXmlTriggerProperty>();
        {
            List<TbXmlNode> triggerNodes = docNode.GetNodes("Root/AnimEventTriggers/Trigger");
            int numTriggerNodes = triggerNodes == null ? 0 : triggerNodes.Count;
            for (int triggerIndex = 0; triggerIndex < numTriggerNodes; ++triggerIndex)
            {
                TbXmlNode triggerNode = triggerNodes[triggerIndex];
                int triggerId = triggerNode.GetIntValue("id");
                List<TbXmlNode> effectNodes = triggerNode.GetNodes("Effect");
                int numEffectNodes = effectNodes == null ? 0 : effectNodes.Count;
                if (numEffectNodes > 0)
                {
                    int[] effectIds = new int[numEffectNodes];
                    for (int effectIndex = 0; effectIndex < numEffectNodes; ++effectIndex)
                    {
                        effectIds[effectIndex] = effectNodes[effectIndex].GetIntValue("id");
                    }
                    aveTriggers[triggerId] = new AnimatedVegetationXmlTriggerProperty() { triggerId = triggerId, effectIds = effectIds };
                }
            }
        }

        aveEffects = new Dictionary<int, AnimatedVegetationXmlEffectProperty>();
        {
            Assembly assembly = typeof(AnimatedVegetationSceneManager).Assembly;
            if (assembly == null)
            {
                Debug.LogError("Error:Assembly is null.");
            }
            List<TbXmlNode> effectNodes = docNode.GetNodes("Root/Effects/Effect");
            int numEffectNodes = effectNodes == null ? 0 : effectNodes.Count;
            for (int effectIndex = 0; effectIndex < numEffectNodes; ++effectIndex)
            {
                TbXmlNode effectNode = effectNodes[effectIndex];
                int effectId = effectNode.GetIntValue("id");
                string effectTypeStr = "AnimatedVegetationEffect_" + effectNode.GetStringValue("type");
                Type effectType = assembly == null ? null : assembly.GetType(effectTypeStr);
                if (effectType == null)
                {
                    Debug.LogError("Error:Type is null." + effectTypeStr);
                }
                else
                {
                    MethodInfo[] methodInfos = effectType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
                    int numMethodInfos = methodInfos == null ? 0 : methodInfos.Length;
                    bool isInitComplete = false;
                    for (int methodInfoIndex = 0; methodInfoIndex < numMethodInfos; ++methodInfoIndex)
                    {
                        MethodInfo methodInfo = methodInfos[methodInfoIndex];
                        object[] attributes = methodInfo.GetCustomAttributes(false);
                        int numAttributes = attributes == null ? 0 : attributes.Length;
                        for (int attributeIndex = 0; attributeIndex < numAttributes; ++attributeIndex)
                        {
                            if (attributes[attributeIndex] is AnimatedVegetationEffectInitAttribute)
                            {
                                try
                                {
                                    AnimatedVegetationXmlEffectProperty aveEffectProperty = new AnimatedVegetationXmlEffectProperty();
                                    aveEffectProperty.effectId = effectId;
                                    AnimatedVegetationEffect effect = (AnimatedVegetationEffect)Activator.CreateInstance(effectType);
                                    ParameterInfo[] paramInfos = methodInfo.GetParameters();
                                    int numParams = paramInfos == null ? 0 : paramInfos.Length;
                                    object[] parameters = new object[numParams];
                                    for (int paramIndex = 0; paramIndex < numParams; ++paramIndex)
                                    {
                                        ParameterInfo paramInfo = paramInfos[paramIndex];
                                        parameters[paramIndex] = effectNode.GetFloatValue(paramInfo.Name);
                                    }
                                    methodInfo.Invoke(effect, parameters);
                                    aveEffectProperty.effect = effect;
                                    aveEffects[effectId] = aveEffectProperty;
                                    isInitComplete = true;
                                }
                                catch (Exception exception)
                                {
                                    isInitComplete = false;
                                    if (exception != null)
                                    {
                                        Debug.LogError(exception.Message);
                                        Debug.LogError(exception.StackTrace);
                                    }
                                }
                                break;
                            }
                        }
                        if (isInitComplete)
                        {
                            break;
                        }
                    }
                    if (!isInitComplete)
                    {
                        Debug.LogError("Error:Init fail." + effectTypeStr);
                    }
                }

            }
        }

        return true;
    }

    public AnimatedVegetationXmlTriggerProperty GetTrigger(int triggerId)
    {
        if (aveTriggers == null)
        {
            return null;
        }

        AnimatedVegetationXmlTriggerProperty trigger = null;
        if (aveTriggers.TryGetValue(triggerId, out trigger))
        {
            return trigger;
        }
        return null;
    }

    public AnimatedVegetationXmlEffectProperty GetEffect(int effectId)
    {
        if (aveEffects == null)
        {
            return null;
        }

        AnimatedVegetationXmlEffectProperty effect = null;
        if (aveEffects.TryGetValue(effectId, out effect))
        {
            return effect;
        }
        return null;
    }
}
