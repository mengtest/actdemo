using UnityEngine;
using System.Collections;

public enum EffectDirectionType
{
    EffectDirectionType_RoleForward     = 1,
    EffectDirectionType_RoleBack        = 2,
    EffectDirectionType_RoleLeft        = 3,
    EffectDirectionType_RoleRight       = 4,
    EffectDirectionType_RoleUp          = 5,
}

public class EffectProperty
{
    public string mEffectPath;
    public string mEffectFile;
    public string mEffectRemovePath;
    public int mEffectId;
    public int mEffectModel;
    public string mEffectAction;
    public bool mEffectFollowAction;
    public EffectDirectionType mEffectDirectionType;
    public float mEffectDirectionDistance;
    public bool mEffectFollowRole;
    public bool mEffectFollowBip;
}