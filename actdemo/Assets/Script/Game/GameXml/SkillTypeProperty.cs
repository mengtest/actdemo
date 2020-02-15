using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 技能类型
/// </summary>
public class SkillTypeProperty
{
    /// <summary>
    /// 技能类型
    /// </summary>
    public SkillType mSkillType;

    /// <summary>
    /// 技能生效延迟（单位：毫秒）
    /// </summary>
    public float mSkillDelay;

    /// <summary>
    /// 技能持续时间（单位：毫秒）
    /// </summary>
    public float mSkillContinue;

    /// <summary>
    /// 技能增量（有可能为负数）
    /// </summary>
    public float mSkillIncrementalValue;

    #region <>伤害类技能<>

    /// <summary>
    /// 技能伤害值
    /// </summary>
    public float mSkillDamage;

    #endregion

    #region <>Dot类技能<>

    /// <summary>
    /// Dot每次所受伤害
    /// </summary>
    public float mSkillDotDamage;

    /// <summary>
    /// Dot生效间隔
    /// </summary>
    public float mSkillDotSpace;

    #endregion <>Dot类技能<>

    #region <>恢复类技能<>

    /// <summary>
    /// 技能恢复类型
    /// </summary>
    public SkillConsumptionType mSkillRestoreType;

    /// <summary>
    /// 恢复技能恢复量
    /// </summary>
    public float mSkillRestoreValue;

    #endregion

    #region <>减速类技能<>

    /// <summary>
    /// 减速量
    /// </summary>
    public float mSkillDecelerationValue;

    #endregion

    #region <>眩晕<>

    #endregion

    #region <>击退<>

    /// <summary>
    /// 击退距离（单位：米）
    /// </summary>
    public float mSkillRepelDistance;

    /// <summary>
    /// 击退速度
    /// </summary>
    public float mSkillRepelSpeed;

    #endregion

    #region <>击飞<>

    #endregion

    #region <>拉近距离<>

    /// <summary>
    /// 拉近距离速度
    /// </summary>
    public float mSkillCloseSpeed;

    #endregion

    #region <>吞噬<>
    #endregion

    #region <>致盲<>
    #endregion

    #region <>变形<>
    #endregion
}