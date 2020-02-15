using UnityEngine;
using System.Collections;

/// <summary>
/// 受击组件
/// </summary>
public class InjuredComponent : IComponent
{
    private RestoreComponent mRestoreComponent;
    private AnimatorComponent mAnimatorComponent;

    public override void SetObject(IObject obj)
    {
        base.SetObject(obj);

        mRestoreComponent = obj.GetComponent<RestoreComponent>();
        mAnimatorComponent = obj.GetComponent<AnimatorComponent>();
    }

    public void Do(IObject sender, SkillActionProperty sap)
    {
        if (Obj == null || sender == null || sap == null)
        {
            return;
        }

        if (mObj.mIsDead)
        {
            return;
        }

        // 判断技能是否击中（如霸体，魔免等技能都被未击中）
        SkillHitType sht = CheckSkillHit();
        if (sht != SkillHitType.SkillHitType_Hit)
        {
            // 如果没有击中，显示对应字样

            return;
        }

        // 播放受击效果
        InjuredAction(sender, sap);

        // 计算技能是否暴击
        if (!CheckCrit())
        {
            return;
        }

        // 计算技能伤害
        int damage = (int)DamageManager.GetDamageByAttack(sender, mObj);
        if (damage > 0)
        {
            LostHp(damage + Random.Range(-20, 0));
        }

        // 播放受击特效和音效
        EffectManager.Instance.SkillHitEffect(0, mObj.mGameObject);
        AudioManager.Instance.PlayBeSkilledAudio(0, mObj.mTransform);
    }

    SkillHitType CheckSkillHit()
    {
        return SkillHitType.SkillHitType_Hit;
    }

    bool CheckCrit()
    {
        return true;
    }

    public void LostHp(int hp)
	{
        hp = hp < 0 ? hp : -hp;

        if (mObj is CRoleObject)
        {
            //Todo UGUI
    //        UIManager.Instance.mHUDUIPanel.HudNumber(hp, mObj, HUDNumberType.HUDNumberType_Lost);
        }
        else
        {
            //Todo UGUI
    //        UIManager.Instance.mHUDUIPanel.HudNumber(hp, mObj);
        }

        mRestoreComponent.ChangeHp(hp);
    }
    
    void Update()
    {
		MoveSkillUpdate();

		HeightSkillUpdate();
    }

    // 受击动画效果
    void InjuredAction(IObject sender, SkillActionProperty sap)
    {
        if (sap == null)
        {
            return;
        }
        
        switch (sap.mSkillType)
        {
            case SkillType.SkillType_Unknown:
                break;

            case SkillType.SkillType_Damage:
                mAnimatorComponent.SetToppleAnim(sap.mTopplebackType);
                mAnimatorComponent.PlayBackAnimator(sap);
                break;

            case SkillType.SkillType_Dot:
                break;

            case SkillType.SkillType_Restore:
                break;

            case SkillType.SkillType_Deceleration:
                break;

            case SkillType.SkillType_Vertigo:
                mAnimatorComponent.SwoonAnim(sap.mSkillTypeParams[0]);
                break;

            case SkillType.SkillType_Repel:
                mAnimatorComponent.SetToppleAnim(sap.mTopplebackType);
                Rotate(sender.mTransform, sap.mRepelType);
                mAnimatorComponent.BlowExitAnim(sap, sap.mHurtDir);
                break;

            case SkillType.SkillType_BlowUp:
                mAnimatorComponent.SetToppleAnim(sap.mTopplebackType);
                Rotate(sender.mTransform, sap.mRepelType);
                mAnimatorComponent.BlowupAnim((int)sap.mSkillTypeParams[0]);
                break;

            case SkillType.SkillType_Close:
                break;

            case SkillType.SkillType_Devour:
                break;

            case SkillType.SkillType_Blindness:
                break;

            case SkillType.SkillType_Deformation:
                break;

            default:
                break;
        }
    }

	/// <summary>
	/// 受击触发的类型;
	/// 受击技能的Id;
	/// 受击的帧数;
	/// </summary>
	public void InjuredSkillType(IObject attacker, SkillActionProperty skillActionProperty)
	{
        if (mObj.mIsDead)
        {
            return;
        }

        MoveSkillInjured(0, null);

		HeightSkillInjured(0, null, null);

        //计算掉血
        int damage = (int)DamageManager.GetDamageByAttack(attacker, mObj);

        if (damage > 0)
        {
            LostHp(damage + Random.Range(-20, 0));
        }

        int injuredAudio = attacker.GetProperty("HurtAudio").GetInt();
        if (injuredAudio > 0)
        {
            AudioManager.Instance.PlayBattleEffectAudio(injuredAudio, mObj);
        }

        if (mObj is CRoleObject)
        {
            if (attacker.mObjId == 120009 || attacker.mObjId == 120010 || attacker.mObjId == 120011)
            {

            }
            else
            {
                return;
            }
        }

        InjuredAction(attacker, skillActionProperty);
    }

	private void Rotate(Transform center ,int rotateType)
	{
		if(rotateType == 0)
		{
			Vector3 v = (center.position - this.transform.position).normalized;
			this.transform.forward = v;
		}
		else
		{
			this.transform.forward = -center.forward;
		}

	}

	#region<串击退后>

	/// <summary>
	/// 运动技能击中;
	/// 是否开启被选中在击中的范围内;
	/// 攻击者;
	/// 技能Id 和 技能等级;
	/// 受击的帧数;
	/// </summary>
	public void MoveSkillInjured(float isOpen , IObject attacker)
	{
		i_isOpen = isOpen;

		if(i_isOpen > 0)
		{
			i_attacker = attacker.mTransform;

			i_forward = (this.transform.position - attacker.mTransform.position).normalized;

			a_forward = i_attacker.transform.forward.normalized;

			i_distance = 2;

		}
		else
		{
			i_attacker = null;
		}
	}

	float i_isOpen;
	Transform i_attacker;
	float i_distance;
	Vector3 i_forward;
	Vector3 a_forward;
	/// <summary>
	/// 计算位移人物跟随击退效果;
	/// </summary>
	private void MoveSkillUpdate()
	{
		if(i_isOpen <= 0)
		{
			return;
		}

		i_isOpen -= Time.deltaTime;

		i_forward = this.transform.position - i_attacker.transform.position;

		Vector3 v1 =  Vector3.Project(i_forward , i_attacker.transform.forward);

		if (v1.magnitude <= i_distance)
		{
			this.transform.position += a_forward.normalized * i_distance;
            this.transform.LookAt(i_attacker.transform);
		}
	}
	#endregion

	#region<串击高>
	SkillActionProperty iskillHeight;
	float i_isHeightOpen;
	/// <summary>
	/// 运动技能击中;
	/// 是否开启被选中在击中的范围内;
	/// 攻击者;
	/// 技能Id 和 技能等级;
	/// 受击的帧数;
	/// </summary>
	public void HeightSkillInjured(float isOpen , Transform attacker, SkillActionProperty skillActionProperty)
	{
		i_isHeightOpen = isOpen;
		
		if(i_isHeightOpen >0)
		{
			i_attacker = attacker;
			
			i_forward =( this.transform.position - attacker.transform.position).normalized;
			
			a_forward = i_attacker.transform.forward.normalized;
			
			i_distance = 2;

            iskillHeight = skillActionProperty;
        }
		else
		{
			i_attacker = null;
		}
	}


	/// <summary>
	/// 计算位移人物跟随击退效果;
	/// </summary>
	private void HeightSkillUpdate()
	{
		if(i_isHeightOpen <= 0)
		{
			return;
		}
		
		i_isHeightOpen -= Time.deltaTime;
		
		i_forward = this.transform.position - i_attacker.transform.position;
		
		Vector3 v1 =  Vector3.Project(i_forward , i_attacker.transform.forward);
		
		if (v1.magnitude <= i_distance)
		{
			mAnimatorComponent.SetToppleAnim(iskillHeight.mTopplebackType);
			Rotate(i_attacker,iskillHeight.mRepelType);
			mAnimatorComponent.BlowupAnim((int)iskillHeight.mSkillTypeParams[0]);
		}
	}

	#endregion



	/// <summary>
	/// 受击播放不同方向的受击动作;
	/// 1.攻击者位置，2.被 攻击者位置 ，3.被攻击者站立的方向
	/// </summary>
	private int PlayBackType(Vector3 center , Transform target)
	{
		Vector3 forward = (center -target.position).normalized;
		
		float f1 = Vector3.Dot(forward,target.forward);
		float f2 = Vector3.Dot(forward , target.right);
		
		if(Mathf.Abs(f1) >=  Mathf.Abs(f2))
		{
			if(f1 > 0)
			{
				return  4; //打在前胸;
			}
			else
			{
				return 2;//打在后背;
			}
		}
		else
		{
			if(f2 > 0)
			{
				return  1; //打在右方;
			}
			else
			{
				return 3;//打在左方;
			}
		}
	}

    /// <summary>
    /// 飞行技能移动回调
    /// </summary>
    /// <param name="FlySkillParams"></param>
    void OniTweenUpdate(FlySkillParams paramy)
    {
        // 暂时不做处理
    }
}
