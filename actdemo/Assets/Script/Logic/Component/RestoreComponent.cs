using UnityEngine;
using System.Collections;
using SysUtils;

/// <summary>
/// 生命/魔法/能量等回复组件
/// </summary>
public class RestoreComponent : IComponent
{
    /// <summary>
    /// 当前血量,最大血量
    /// </summary>
    private long mCurrentHp = 10000;
    private long mMaxHp = 10000;

    /// <summary>
    /// 当前魔法值,最大魔法值
    /// </summary>
    private long mCurrentMp;
    private long mMaxMp;

    /// <summary>
    /// 当前能量值,最大能量值
    /// </summary>
    private long mCurrentEng;
    private long mMaxEng;

    /// <summary>
    /// 回复间隔
    /// </summary>
    private float mRestoreSpaceTime;

	private AnimatorComponent mAnimatorManager;

    private UISlider mBar;
    private UILabel mValue;

    public override void SetObject(IObject obj)
    {
        base.SetObject(obj);

        if (mObj == null)
        {
            return;
        }

        mCurrentHp = mObj.GetProperty("CurHp").GetInt64();
        mMaxHp = mObj.GetProperty("InitHp").GetInt64();

        mCurrentMp = mObj.GetProperty("CurMp").GetInt64();
        mMaxMp = mObj.GetProperty("InitMp").GetInt64();

        mCurrentEng = mObj.GetProperty("CurEng").GetInt64();
        mMaxEng = mObj.GetProperty("InitEng").GetInt64();

        mAnimatorManager = mObj.GetComponent<AnimatorComponent>();
        if (mAnimatorManager == null)
        {
			mObj.AddComponent<AnimatorComponent>();
        }

		mAnimatorManager = mObj.GetComponent<AnimatorComponent>();

        //UIManager.Instance.mHUDUIPanel.Bar(obj);
    }

    /// <summary>
    /// 改变血量
    /// </summary>
    /// <param name="hp"></param>
    public void ChangeHp(long hp)
    {
        if (mObj.mIsDead)
        {
            return;
        }

        mCurrentHp += hp;
        if (mCurrentHp >= mMaxHp)
        {
            mCurrentHp = mMaxHp;
        }
        else if (mCurrentHp < 0)
        {
            mCurrentHp = 0;
        }

        mObj.SetProperty("CurHp", new Var(VarType.Int64, mCurrentHp));
    }

    public void ChangeMp(long mp)
    {
        if (mObj.mIsDead)
        {
            return;
        }

        mCurrentMp += mp;

        if (mCurrentMp > mMaxMp)
        {
            mCurrentMp = mMaxMp;
        }
        else if (mCurrentMp < 0)
        {
            mCurrentMp = 0;
        }

        mObj.SetProperty("CurMp", new Var(VarType.Int64, mCurrentMp));
    }

    public void ChangeEng(long eng)
    {
        if (mObj.mIsDead)
        {
            return;
        }

        mCurrentEng += eng;

        if (mCurrentEng > mMaxEng)
        {
            mCurrentEng = mMaxEng;
            //mBar.value = 1.0f;
        }
        else if (mCurrentEng < 0)
        {
            mCurrentEng = 0;
            //mBar.value = 0.0f;
        }
        else
        {
            //mBar.value = (float)mCurrentMp / (float)mMaxMp;
        }
    }

    void Update()
    {
        if (mObj == null)
        {
            return;
        }

        if (mObj.mIsDead)
        {
            return;
        }

        if (mCurrentHp < 1 && !mObj.mIsDead)
        {
            mAnimatorManager.DeathAnim();
            mObj.mIsDead = true;

            RangeTools.MotifyRoleAoi();

            TweenAlpha tween = TweenAlpha.Begin(mObj.mGameObject, 3.0f, 1.0f);
            tween.AddOnFinished(EnemyDisappare);
        }
    }

    void EnemyDisappare()
    {
        ObjectManager.DestroyObject(mObj);
    }
}