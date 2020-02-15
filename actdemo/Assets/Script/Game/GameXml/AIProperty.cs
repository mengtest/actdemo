using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SysUtils;

/// <summary>
/// AI阶段类型
/// </summary>
public enum AIStageType
{
    AIStageType_Unknown         = 0,

    AIStageType_Think           = 1,
    AIStageType_SelfHp          = 2,
    AIStageType_TargetHp        = 3,
    AIStageType_BornTime        = 4,
    AIStageType_PreStageTime    = 5,
}

/// <summary>
/// AI行为
/// </summary>
public enum AIActionType
{
    AIActionType_Unknown = 0,

    AIActionType_Wait = 1,
    AIActionType_Pursuit = 2,
    AIActionType_Escape = 3,
    AIActionType_Attack = 4,
    AIActionType_Skill = 5,
}

/// <summary>
/// AI属性
/// </summary>
public class AIProperty
{
    public int mAIId;
    public List<AIStage> mAIStageList;
}

/// <summary>
/// AI阶段
/// </summary>
public class AIStage
{
    public AIStageType mAIStageType;        // 当前AI类型
    public Var mAIStageValue;               // 当前AI触发值
    public AIActionType mAIActionType;      // 当前AI行为
    public Var mAIActionTypeValue;          // 当前AI行为值
}