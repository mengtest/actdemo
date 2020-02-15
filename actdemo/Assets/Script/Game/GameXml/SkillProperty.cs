using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 技能索引
/// </summary>
public class SkillIndex
{
    public SkillIndex(int skillId, int skillLevel)
    {
        mSkillId = skillId;
        mSkillLevel = skillLevel;
    }
    
    /// <summary>
    /// 技能Id
    /// </summary>
    public int mSkillId;

    /// <summary>
    /// 技能等级
    /// </summary>
    public int mSkillLevel;

    public override bool Equals(object obj)
    {
        SkillIndex s = (SkillIndex)obj;
        if (mSkillId == s.mSkillId && mSkillLevel == s.mSkillLevel)
        {
            return true;
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return mSkillId * 100 + mSkillLevel;
    }
}

/// <summary>
/// 技能属性
/// </summary>
public class SkillProperty
{
    /// <summary>
    /// 技能Id
    /// </summary>
    public int mSkillId;

    /// <summary>
    /// 技能名
    /// </summary>
    public string mSkillName;

    /// <summary>
    /// 技能释放者Id
    /// </summary>
    public int mSkillOwnerId;

    /// <summary>
    /// 技能位置索引
    /// </summary>
    public int mSkillIndex;

    /// <summary>
    /// 技能CD
    /// </summary>
    public float mSkillSpace;

    /// <summary>
    /// 技能消耗类型
    /// </summary>
    public SkillConsumptionType mSkillConsumptionType;

    /// <summary>
    /// 技能消耗值
    /// </summary>
    public int mSkillConsumptionValue;

    /// <summary>
    /// 技能描述
    /// </summary>
    public string mSkillDes;

    /// <summary>
    /// 事件触发效果;
    /// </summary>
    public AnimatorSkillProperty mAnimatorSkillProperty;
}

public class AnimatorSkillProperty
{
	//技能Id;
	public int mySkillId;

	/// <summary>
	/// 技能动画;
	/// </summary>
	public int mySkillAction;

	/// <summary>
	/// 事件数量;
	/// </summary>
	public int mySkillActionCount;

	/// <summary>
	/// 事件触发效果;
	/// </summary>
	public Dictionary<int, SkillActionProperty> mSkillActionProperty; 
}

public class SkillActionProperty
{
	/// <summary>
	/// 第几次事件帧;
	/// </summary>
	public int ActionNumberId;

	/// <summary>
	/// 作用对象;
	/// </summary>
	public SkillTargetType mSkillTargetType;

    /// <summary>
    /// 技能释放类型
    /// </summary>
    public SkillReleaseType mSkillReleaseType;

    /// <summary>
    /// 飞行特效资源
    /// </summary>
    public string mSkillFlyRes;

    /// <summary>
    /// 技能伤害类型;
    /// </summary>
    public SkillType mSkillType;

	/// <summary>
	/// 击中表现的数据;
	/// </summary>
	public List<float> mSkillTypeParams;

	/// <summary>
	/// 攻击类型;( 1.物理攻击 2.魔法攻击)计算伤害的不同公式;
	/// </summary>
	public int mDamageType;

	/// <summary>
	/// 伤害值计算类型; 1.基础攻击倍率-伤害倍率 2.直接伤害-伤害数值
	/// </summary>
	public int mDamageMath; 

	/// <summary>
	/// 伤害值 
	/// </summary>
	public float mDamageNumber;

	/// <summary>
	/// 受伤暂停时间（硬直时间）
	/// </summary>
	public float mPauseTime;

	/// <summary>
	/// 技能攻击位置偏移位移;(沿着人物正方向偏移;)
	/// </summary>
	public Vector3 mCenterOffset;

	/// <summary>
	/// 技能释放范围;
	/// </summary>
	public SkillRangeType mSkillRangeType;

	/// <summary>
	/// 计算范围值;
	/// </summary>
	public List<float> mSkillRangParametr;

	/// <summary>
	/// 受伤表现的特效;
	/// </summary>
	public int mHurtEffectId;

	/// <summary>
	/// 特殊伤害表现;
	/// </summary>
	public int mDamageStatus;

    /// <summary>
    /// 攻击附带debuffId
    /// </summary>
    public int mDebuffId;

	/// <summary>
	/// 攻击者 硬直 时间;
	/// </summary>
	public float mAttackerPauseTime;

	/// <summary>
	/// 震屏次数；
	/// </summary>
	public int mCameraShakeNumber;

	/// <summary>
	/// 震屏幅度；
	/// </summary>
	public float mCameraShakeDistance;

	/// <summary>
	/// 震屏速度
	/// </summary>
	public float mCameraShakeSpeed;

    /// <summary>
    /// 相机离心距离
    /// </summary>
    public float mShakeRotationX;

    /// <summary>
    /// 震屏衰减速度
    /// </summary>
    public float mShakeDecay;

	/// <summary>
	///击退方向;
	/// </summary>
	public int mRepelType;

	/// <summary>
	/// 受伤方向;
	/// </summary>
	public int mHurtDir;

	/// <summary>
	/// 地面受击;
	/// </summary>
	public int mTopplebackType;
}

/// <summary>
/// 飞行技能参数
/// </summary>
public class FlySkillParams
{
    /// <summary>
    /// 箭
    /// </summary>
    public GameObject self;
    /// <summary>
    /// 攻击目标
    /// </summary>
    public GameObject target;
    /// <summary>
    /// 技能参数
    /// </summary>
    public SkillActionProperty property;
}