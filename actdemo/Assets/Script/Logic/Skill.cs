using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 技能类型
/// </summary>
public enum SkillType
{
    SkillType_Unknown = 0,

    SkillType_Damage = 1,           // 伤害型
    SkillType_Dot = 2,              // 持续伤害型

    SkillType_Restore = 3,          // 恢复型

    SkillType_Deceleration = 4,     // 减速
    SkillType_Vertigo = 5,          // 眩晕
    SkillType_Repel = 6,            // 击退
    SkillType_BlowUp = 7,           // 击飞
    SkillType_Close = 8,            // 拉近距离
    SkillType_Devour = 9,           // 吞噬
    SkillType_Blindness = 10,       // 致盲

    SkillType_Deformation = 11,     // 变形
}

/// <summary>
/// 技能释放方式
/// </summary>
public enum SkillReleaseFunc
{
    SkillReleaseFunc_Unknown = 0,

    SkillReleaseFunc_Instantaneous = 1,     // 瞬时
    SkillReleaseFunc_Sustained = 2,         // 持续
    SkillReleaseFunc_Delayed = 3,           // 延迟

    SkillReleaseFunc_MaxCount,
}

/// <summary>
/// 技能释放范围
/// </summary>
public enum SkillRangeType
{
    SkillRangeType_Unknown = 0,

    SkillRangeType_Self = 1,            // 自身
    SkillRangeType_Single = 2,          // 单体
    SkillRangeType_SelfRound = 3,       // 自身为圆形
    SkillRangeType_SelfSector = 4,      // 自身为扇形
    SkillRangeType_SelfLine = 5,        // 自身为线形

    SkillRangeType_MaxCount,
}

/// <summary>
/// 技能释放类型
/// </summary>
public enum SkillReleaseType
{
    SkillReleaseType_Unknown = 0,

    SkillReleaseType_Immediately    = 1,         // 范围内立即生效
    SkillReleaseType_Flying         = 2,         // 范围内飞行
    SkillReleaseType_Random         = 3,         // 范围内随机
}

/// <summary>
/// 技能目标类型
/// </summary>
public enum SkillTargetType
{
    SkillTargetType_Unknown = 0,

    SkillTargetType_Self = 1,           // 自身
    SkillTargetType_Enemy = 2,          // 敌人
    SkillTargetType_Friend = 3,         // 队友
    SkillTargetType_Neutral = 4,        // 中立
    SkillTargetType_Building = 5,       // 建筑
}

/// <summary>
/// 技能消耗类型
/// </summary>
public enum SkillConsumptionType
{
    SkillConsumptionType_Unknown = 0,

    SkillConsumptionType_None = 1,      // 无消耗
    SkillConsumptionType_Hp = 2,        // 生命
    SkillConsumptionType_Mp = 3,        // 魔法
    SkillConsumptionType_Eng = 4,       // 能量
    SkillConsumptionType_Buff = 5,      // buff
    SkillConsumptionType_Debuff = 6,    // debuff
    SkillConsumptionType_MaxCount = 7,  // 最大类型
}

public enum SkillHitType
{
    SkillHitType_Unknown    = 0,

    SkillHitType_Hit        = 1,        // 命中
    SkillHitType_Miss       = 2,        // 闪避
    SkillHitType_Ignore     = 3,        // 免疫
}

/// <summary>
/// 技能基类
/// </summary>
public class Skill
{
    /// <summary>
    /// 技能Id
    /// </summary>
    public int mSkillId;

    /// <summary>
    /// 技能等级
    /// </summary>
    public int mSkillLevel;

    /// <summary>
    /// 技能释放距离
    /// </summary>
    public float mSkillRange;
    public float mSkillMinRange;

    /// <summary>
    /// 技能上一次释放时间
    /// </summary>
    public float mSkillLastTime = 0.0f;

    /// <summary>
    /// 技能飞行速度（不一定有）
    /// </summary>
    public float mSkillFlySpeed;

    /// <summary>
    /// 技能飞行列表
    /// </summary>
    public List<GameObject> mSkillFlyList = new List<GameObject>();

    /// <summary>
    /// 技能释放方式
    /// </summary>
    public SkillReleaseFunc mSkillReleaseFunc;

    /// <summary>
    /// 技能释放范围
    /// </summary>
    public SkillRangeType mSkillRangeType;

    /// <summary>
    /// 技能属性
    /// </summary>
    public SkillProperty mSkillProperty;

    /// <summary>
    /// 技能释放者
    /// </summary>
    public IObject mSelfObj;

    public Skill(IObject obj, int skillId)
    {
        if (mSkillProperty != null)
        {
            return;
        }

        mSelfObj = obj;
        mSkillId = skillId;

        mSkillProperty = XmlManager.Instance.GetSkillProperty(mSkillId);
    }

    /// <summary>
    /// 技能消耗检测
    /// </summary>
    /// <returns></returns>
    public bool CheckSkillConsumption()
    {
        if (mSkillProperty == null)
        {
            return false;
        }

        SkillConsumptionType type = mSkillProperty.mSkillConsumptionType;
        int value = mSkillProperty.mSkillConsumptionValue;
        if (type <= SkillConsumptionType.SkillConsumptionType_Unknown ||
            type >= SkillConsumptionType.SkillConsumptionType_MaxCount)
        {
            return false;
        }

        if (value < 0)
        {
            return false;
        }

        long currentValue = 0;

        switch (type)
        {
            case SkillConsumptionType.SkillConsumptionType_None:

                return true;

            case SkillConsumptionType.SkillConsumptionType_Hp:

                break;

            case SkillConsumptionType.SkillConsumptionType_Mp:

                currentValue = mSelfObj.GetProperty("CurMp").GetInt64();

                break;

            case SkillConsumptionType.SkillConsumptionType_Eng:
                
                break;

            case SkillConsumptionType.SkillConsumptionType_Buff:

                break;

            case SkillConsumptionType.SkillConsumptionType_Debuff:

                break;

            default:
                return false;
        }

        if (currentValue < value)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 技能冷却检测
    /// </summary>
    /// <returns></returns>
    public bool CheckSkillCD()
    {
        if (mSkillProperty == null)
        {
            return false;
        }

        float spaceTime = mSkillProperty.mSkillSpace;
        if (spaceTime < 0.0f)
        {
            return false;
        }

        if (mSkillLastTime > 0.0f && Time.time - mSkillLastTime < spaceTime)
        {
            return false;
        }

        mSkillLastTime = Time.time;

        // 如果当前技能是主角技能
        if (mSelfObj is CRoleObject)
        {
            FightUI ui = FightManager.Instance.mFightUI;
            if (ui != null)
            {
                ui.SetSkillCD(mSkillId, spaceTime);
            }
        }

        return true;
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    /// <returns></returns>
    public bool Do()
    {
//         // 检测当前技能消耗
//         if (!CheckSkillConsumption())
//         {
//             return false;
//         }
// 
//         // 检测当前技能CD
//         if (!CheckSkillCD())
//         {
//             return false;
//         }

        return true;
    }

    public void Update()
    {
        int count = mSkillFlyList.Count;
        if (count < 1)
        {
            return;
        }

        Hashtable table = new Hashtable();
    }
        
    public void Destroy()
    {

    }
}