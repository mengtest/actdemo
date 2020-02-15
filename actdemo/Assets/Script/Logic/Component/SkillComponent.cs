using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 技能组件
/// </summary>
public class SkillComponent : IComponent
{
    #region <>技能属性<>

    /// <summary>
    /// 角色释放技能列表
    /// </summary>
    private Dictionary<int, Skill> mSkillDic = new Dictionary<int, Skill>();
    public Dictionary<int, Skill> SkillDic
    {
        get
        {
            return mSkillDic;
        }
    }
    
    private float mDirectionRange = 3.0f;

    #endregion

    private IObject mTarget;

    private AnimatorComponent mAnimatorManager;

    /// <summary>
    /// 初始化 添加角色技能
    /// </summary>
    public override void SetObject(IObject obj)
    {
        base.SetObject(obj);

        mAnimatorManager = obj.GetComponent<AnimatorComponent>();

        List<string> skillList = UtilTools.GetStringSplit(obj.GetProperty("SkillList").GetString());

        for (int i = 0; i < skillList.Count; ++i)
        {
            int skillId = UtilTools.IntParse(skillList[i]);
            if (skillId < 11)
            {
                continue;
            }

            Skill skill = new Skill(mObj, skillId);
            mSkillDic.Add(skillId, skill);
        }
    }

	public void DoAttack()
	{
        mAnimatorManager.AttackCtrl();
	}

	public void DoRoll()
	{
        mAnimatorManager.RollState();
	}

    /// <summary>
    /// 释放技能
    /// </summary>
    public void DoSkill(int skillId)
    {
        if (!mSkillDic.ContainsKey(skillId))
        {
            return;
        }

        Skill skill = mSkillDic[skillId];

		// 检测技能释放条件
        if (!skill.Do())
        {
            return;
        }

        // 目标超过视野范围，设置目标为空，重新寻找新的目标
        if (mTarget != null && UtilTools.Vec2Distance(mTarget, mObj) > skill.mSkillRange)
        {
            mTarget = null;
        }

        if (mTarget == null)
        {
            List<IObject> objList = RangeTools.GetEnemyByAOI(mObj, SkillRangeType.SkillRangeType_SelfRound, mDirectionRange, 0.0f);
            int objCount = objList.Count;

            float distance = float.MaxValue;
            IObject minDistanceObj = null;

            // 检测距离最近的攻击目标，调整朝向
            for (int i = 0; i < objCount; ++i)
            {
                IObject enemy = objList[i];
                if (enemy == null)
                {
                    continue;
                }

                float tempDistance = UtilTools.Vec2Distance(mObj, enemy);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    minDistanceObj = enemy;
                }
            }

            mTarget = minDistanceObj;
        }

        if (mTarget != null)
        {
            Vector3 dir = UtilTools.Vec3Direction(mTarget, mObj);
            mAnimatorManager.SetRotation(dir);
        }

        int type = skill.mSkillProperty.mAnimatorSkillProperty.mySkillAction;

        if (type == 0)
        {
            DoAttack();
        }
        else
        {
            mAnimatorManager.SkillCtrl(skillId, type);
        }
    }

    /// <summary>
    /// 技能释放
    /// 判断释放方式
    /// 立即命中 - 检测技能范围内敌人，实施效果
    /// 飞行命中 - 创建飞行特效，追踪敌人，抵达敌人位置后，实施效果
    /// </summary>
    public void SkillRelease(int skillId, int eventId)
    {
        if (!mSkillDic.ContainsKey(skillId))
        {
            return;
        }

        Skill skill = mSkillDic[skillId];

        AnimatorSkillProperty mAnimator = skill.mSkillProperty.mAnimatorSkillProperty;
        if (!mAnimator.mSkillActionProperty.ContainsKey(eventId))
        {
            return;
        }

        SkillActionProperty skillActionProperty = mAnimator.mSkillActionProperty[eventId];
        if (skillActionProperty == null)
        {
            return;
        }

        if (skillActionProperty.mSkillReleaseType == SkillReleaseType.SkillReleaseType_Immediately)
        {
            SkillHit(skillActionProperty);
        }
        else if (skillActionProperty.mSkillReleaseType == SkillReleaseType.SkillReleaseType_Flying)
        {
            List<IObject> list = GetTargets(skillActionProperty);
            if (list == null || list.Count != 1)
            {
                return;
            }

            EffectManager.Instance.CreateFlyingEffect(mObj, skillActionProperty, list[0]);
        }
    }

    /// <summary>
    /// 获取技能目标
    /// </summary>
    /// <param name="skillActionProperty"></param>
    /// <returns></returns>
    List<IObject> GetTargets(SkillActionProperty skillActionProperty)
    {
        float width = 0.0f;
        if (skillActionProperty.mSkillRangParametr.Count > 1)
        {
            width = skillActionProperty.mSkillRangParametr[1];
        }

        if (skillActionProperty.mSkillRangeType == SkillRangeType.SkillRangeType_SelfLine && skillActionProperty.mSkillRangParametr.Count > 2)
        {
            if (skillActionProperty.mSkillRangParametr[2] == -1 && skillActionProperty.mSkillRangParametr[0] >= 0)
            {
                skillActionProperty.mSkillRangParametr[0] = skillActionProperty.mSkillRangParametr[0] * skillActionProperty.mSkillRangParametr[2];
            }
        }

        return RangeTools.GetEnemyByAOI(mObj, skillActionProperty.mSkillRangeType, skillActionProperty.mSkillRangParametr[0], width);
    }

    /// <summary>
    /// 技能命中
    /// </summary>
    /// <param name="skillActionProperty"></param>
    void SkillHit(SkillActionProperty skillActionProperty)
    {
        List<IObject> objList = GetTargets(skillActionProperty);
        int objCount = objList.Count;
        if (objCount < 0)
        {
            return;
            //PauseAnimtor(skillActionProperty.mAttackerTime);
        }

        if (skillActionProperty.mCameraShakeNumber > 0)
        {
            _CameraShake.Shake(skillActionProperty.mCameraShakeNumber,
                               skillActionProperty.mCameraShakeDistance,
                               skillActionProperty.mCameraShakeSpeed,
                               skillActionProperty.mShakeRotationX,
                               skillActionProperty.mShakeDecay);
        }

        for (int i = 0; i < objCount; ++i)
        {
            IObject enemy = objList[i];
            if (enemy == null)
            {
                continue;
            }

            InjuredComponent injCom = enemy.GetComponent<InjuredComponent>();
            if (injCom == null)
            {
                continue;
            }

            // 如果是主角达到敌人，增加连击数
            if (mObj is CRoleObject)
            {
                ObjectManager.mRole.IncComboCount();
                FightManager.Instance.mFightUI.SetComboNumber(ObjectManager.mRole.ComboCount);
            }

            EffectManager.Instance.SkillHitEffect(skillActionProperty.mHurtEffectId, injCom.mObj.mGameObject);
            EffectManager.Instance.SkillHitHighLightEffect(injCom.mObj.mGameObject);
            StartCoroutine(RemoveHighlight(injCom.mObj.mGameObject));
            
            if (skillActionProperty.mDamageStatus == 0)
            {
                injCom.InjuredSkillType(mObj, skillActionProperty);
            }
            else if (skillActionProperty.mDamageStatus == 1)
            {
                injCom.MoveSkillInjured(0.9f, mObj);
            }
            else if (skillActionProperty.mDamageStatus == 2)
            {
                injCom.HeightSkillInjured(0.2f, mObj.mTransform, skillActionProperty);
            }
        }
    }

    IEnumerator RemoveHighlight(GameObject go)
    {
        yield return new WaitForSeconds(0.05f);

        EffectManager.Instance.RemoveSkillHitHighLightEffect(go);
    }


    /// <summary>
    /// 飞行特效结束回调
    /// </summary>
    /// <param name="go"></param>
    void OniTweenComplete(FlySkillParams param)
    {
        // 这里使用内存池，暂时这么处理
        //Destroy(go);

        float dis = UtilTools.Vec2Distance(param.self.transform.position, param.target.transform.position);
        Destroy(param.self);
        // 箭跟目前的距离
        if (dis <= 2.5f)
        {
            SkillHit(param.property);
        }

        
    }
}