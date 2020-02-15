using UnityEngine;
using System.Collections;

/// <summary>
/// 追击组件
/// </summary>
public class PursuitComponent : IComponent
{
    private Vector3 mObjInitPosition;
    private float mMaxPursuitRange = 20.0f;
    private float mAttackRange = 3.0f;
    private int mAoiListCount;
    private IObject mPursuitTarget;
    private bool isPursuitBack = false;

    private AttackComponent mObjAttackComponent;
    private MoveComponent mObjMoveComponent;
	private AnimatorComponent mObjAnimatorComponent;

    public enum PursuitType
    {
        PursuitType_Wait = 1,
        PursuitType_Pursuit = 2,
        PursuitType_Attack = 3,
        PursuitType_Back = 4,
    }
    public PursuitType mPursuitType = PursuitType.PursuitType_Wait;

    public override void SetObject(IObject obj)
    {
        base.SetObject(obj);

        if (mObj == null)
        {
            return;
        }

        mObjInitPosition = mObj.mPosition;

        mObjAttackComponent = mObj.GetComponent<AttackComponent>();
        mObjMoveComponent = mObj.GetComponent<MoveComponent>();
        mObjAnimatorComponent = mObj.GetComponent<AnimatorComponent>();

        mMaxPursuitRange = obj.GetProperty("PursuitRange").GetFloat();
        mAttackRange = obj.GetProperty("AttackRange").GetFloat();
    }

    /// <summary>
    /// 追击
    /// </summary>
    void Pursuit()
    {
        if (mPursuitTarget == null || mObj.mIsDead)
        {
            return;
        }

        // 如果移动超出追击范围，回到初始点
		if (UtilTools.Vec2Distance(mObj.mPosition, mObjInitPosition) > mMaxPursuitRange && mPursuitType != PursuitType.PursuitType_Back)
        {
            mPursuitType = PursuitType.PursuitType_Back;
            ChangeState();

            return;
        }

        // 如果双方距离小于攻击距离，执行攻击操作
        if (UtilTools.Vec2Distance(mObj, mPursuitTarget) <= mAttackRange && mPursuitType != PursuitType.PursuitType_Attack)
        {
            mPursuitType = PursuitType.PursuitType_Attack;
            ChangeState();
        }

        // 否则，执行移动操作
        else if (UtilTools.Vec2Distance(mObj, mPursuitTarget) > mAttackRange &&
            UtilTools.Vec2Distance(mObj, mPursuitTarget) < mMaxPursuitRange &&
            mPursuitType != PursuitType.PursuitType_Pursuit)
        {
            mPursuitType = PursuitType.PursuitType_Pursuit;
            ChangeState();
        }
    }

    /// <summary>
    /// 到达初始点，追击返回为false
    /// </summary>
    void OnArrive()
    {
        mPursuitType = PursuitType.PursuitType_Wait;
        ChangeState();
    }

    void ChangeState()
    {
        if (mPursuitType != PursuitType.PursuitType_Attack)
        {
            mObjAttackComponent.StopAttack();
        }

        switch (mPursuitType)
        {
            case PursuitType.PursuitType_Wait:
                mObjAnimatorComponent.Stand();
                break;

            case PursuitType.PursuitType_Pursuit:
                mObjMoveComponent.MoveToTarget(mPursuitTarget, OnArrive);
                break;

            case PursuitType.PursuitType_Attack:
                mObjAttackComponent.DoAttack(mPursuitTarget);
                break;

            case PursuitType.PursuitType_Back:
                mPursuitTarget = null;
                mObjMoveComponent.mMoveTarget = null;
                mObjMoveComponent.MoveToPos(mObjInitPosition, OnArrive);
                break;

            default:
                break;
        }
    }

    void Update()
    {
        if (mObj == null || mObj.mIsDead)
        {
            mPursuitTarget = null;

            return;
        }

		if (mObjAnimatorComponent.GetInjuredState()) 
		{
			return;
		}

        // 如果当前有追击目标，执行追击操作
        if (mPursuitTarget != null)
        {
			Pursuit();
			return;
        }

        // 当前没有追击目标，开启巡逻模式
        mAoiListCount = mObj.mAOIList.Count;
        if (mAoiListCount < 1)
        {
            return;
        }

        for (int i = 0; i < mAoiListCount; ++i)
        {
            IObject obj = mObj.mAOIList[i];
            if (obj == null)
            {
                continue;
            }

            if (obj is CRoleObject)
            {
                if (UtilTools.Vec2Distance(obj, mObj) <= mMaxPursuitRange &&
                    mPursuitType != PursuitType.PursuitType_Back)
                {
                    mPursuitTarget = obj;
                }

                return;
            }
        }
    }
}