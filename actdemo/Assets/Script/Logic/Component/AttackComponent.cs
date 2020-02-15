using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 攻击组件
/// </summary>
public class AttackComponent : IComponent
{
    private AnimatorComponent mAnimatorManager;
    private SkillComponent mSkillComponent;

    private string mKey = string.Empty;
    private IObject mTarget;
    private int mAttackId;
    private float mAttackSpace;
    private float mLastAttackTime = 0.0f;

    public override void SetObject(IObject obj)
    {
        base.SetObject(obj);

		mAnimatorManager = mObj.GetComponent<AnimatorComponent>();
        if (mAnimatorManager == null)
        {
			mObj.AddComponent<AnimatorComponent>();
        }

        mSkillComponent = mObj.GetComponent<SkillComponent>();
        if (mSkillComponent == null)
        {
            mObj.AddComponent<SkillComponent>();
        }

        if (Obj == null)
        {
            return;
        }

        mAttackSpace = mObj.GetProperty("AttackSpace").GetFloat();

        // 获取怪物普攻技能
        mAttackId = mObj.GetProperty("AttackId").GetInt();

        mKey = string.Format("{0}_{1}", mObj.mObjId, mObj.mIndex);
    }

	/// <summary>
	/// 普通攻击;
	/// </summary>
	public void DoAttack(IObject target)
	{
        if (target == null)
        {
            return;
        }

        mTarget = target;

        // 如果当前攻击间隔大于指定攻击间隔，先执行一次攻击后，开启心跳
        if (Time.time - mLastAttackTime > mAttackSpace)
        {
            OnAttackLoop();

            // 播放攻击动作
            TimerManager.AddTimerRepeat(mKey, mAttackSpace, OnAttackLoop);
        }
        else
        {
            float space = mAttackSpace - (Time.time - mLastAttackTime);
            TimerManager.AddTimer(mKey, space, OnAttackLoopSpace);
        }
	}

    void OnAttackLoopSpace()
    {
        TimerManager.Destroy(mKey);

        OnAttackLoop();

        // 播放攻击动作
        TimerManager.AddTimerRepeat(mKey, mAttackSpace, OnAttackLoop);
    }

    void OnAttackLoop()
    {
        mAnimatorManager.AttackCtrl();

        mLastAttackTime = Time.time;
    }

    public void StopAttack()
    {
        mTarget = null;

        TimerManager.Destroy(mKey);
    }

    void Update()
    {
        if (mObj.mIsDead)
        {
            TimerManager.Destroy(mKey);

            mTarget = null;

            return;
        }

        if (mTarget == null)
        {
            return;
        }

        mObj.LookAt(mTarget);
    }
}
