using UnityEngine;
using System.Collections;

/// <summary>
/// 移动组件
/// </summary>
public class MoveComponent : IComponent
{
    #region <>移动属性<>

    /// <summary>
    /// 移动速度
    /// </summary>
    private float mMoveSpeed = 1.0f;

    /// <summary>
    /// 能否移动
    /// </summary>
    private bool mCanMove = true;

	private float mTargetDistance = 0.5f;
    public IObject mMoveTarget = null;
    public Vector3 mTargetPos = Vector3.zero;

    #endregion

	private AnimatorComponent mAnimatorManager;
    private FindingPathComponent mFindingPathComponent;

    public delegate void OnArrive();
    public OnArrive mOnArrive;

    public override void SetObject(IObject obj)
    {
        base.SetObject(obj);

		mAnimatorManager = mObj.GetComponent<AnimatorComponent>();
        if (mAnimatorManager == null)
        {
			mObj.AddComponent<AnimatorComponent>();
        }

        mFindingPathComponent = mObj.GetComponent<FindingPathComponent>();

        mTargetDistance = obj.GetProperty("AttackRange").GetFloat();
    }

    /// <summary>
    /// 移动
    /// </summary>
	public int DoMove(float mMoveSpeed)
    {
        if (Obj == null)
        {
            return 0;
        }

        Vector3 Position = Obj.mPosition;

		if(mMoveSpeed == 0)
		{
			Obj.ObjAnimatorManager.Stand();
		}
		else
		{
			Obj.ObjAnimatorManager.RoleMoveCtrl();
		}

        // 同步当前位置给服务端
		return 1;
    }

    public void MoveToPos(Vector3 pos, OnArrive arrive = null)
    {
        Vector3 dir = (pos - mObj.mPosition).normalized;
        mAnimatorManager.SetRotation(dir);

        mTargetPos = pos;
        mOnArrive = arrive;
    }

    public void MoveToTarget(IObject target, OnArrive arrive = null)
    {
        if (target == null)
        {
            return;
        }

        mMoveTarget = target;

        MoveToPos(mMoveTarget.mPosition, arrive);
    }

    void Update()
    {
        if (Obj == null)
        {
            return;
        }

        if (!mCanMove)
        {
            return;
        }

        Obj.mPosition = Obj.mGameObject.transform.position;
        if (Obj is CRoleObject)
        {
            RangeTools.MotifyRoleAoi();

            return;
        }

        if (!mTargetPos.Equals(Vector3.zero))
        {
            if (mMoveTarget != null)
            {
                mTargetPos = mMoveTarget.mPosition;
            }

            if (UtilTools.Vec2Distance(mObj.mPosition, mTargetPos) > mTargetDistance)
            {
                Vector3 dir = UtilTools.Vec3Direction(mTargetPos, mObj.mPosition);
                mAnimatorManager.SetRotation(dir);
                mAnimatorManager.RoleMoveCtrl();
            }
            else
            {
                mTargetPos = Vector3.zero;

                if (mOnArrive != null)
                {
                    mOnArrive();
                }
            }
        }
    }
}