using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatorComponent : IComponent
{
    public static string Topple = "topple";
    public static string Toppleback = "toppleback_";
    public static string Back = "back_";
    public static string Attack = "attack_";
    public static string AttackIndex = "AttackIndex";
    public static string Float = "float_";

	private enum AnimatorType
	{
		RunType,
		StandType,
		SkillType,
		AttackType,
	}

	IAttackController my_AttackCtrl;
	/// <summary>
	/// 普通攻击的个数;
	/// </summary>
	int mMaxAttackCount;

	ISkillController my_SkillCtrl;

	BackController my_BackCtrl;

	SwoonController my_SwoonCtrl;

    FloatController mFloatController;

	AnimatorStateInfo stateInfo;

	//cd 时间;
	float icrossTime;

	/// <summary>
	/// 动画控制器;
	/// </summary>
	public Animator animator;

    private SkillComponent mSkillComponent;
    
    public override void SetObject(IObject obj)
    {
        base.SetObject(obj);

        mMaxAttackCount = obj.GetProperty("AttackActionCount").GetInt();

        animator = obj.mGameObject.GetComponent<Animator>();
        my_AttackCtrl = animator.GetBehaviour<IAttackController>();
        my_BackCtrl = animator.GetBehaviour<BackController>();
        my_SkillCtrl = animator.GetBehaviour<ISkillController>();
        my_SwoonCtrl = animator.GetBehaviour<SwoonController>();
        mFloatController = animator.GetBehaviour<FloatController>();
        mSkillComponent = obj.GetComponent<SkillComponent>();
    }

    private float i_MoveSpeed;

	private float i_AcceleratedSpeed = 2f;

	/// <summary>
	/// 判断是否受伤状态;
	/// </summary>
	/// <returns><c>true</c>, if injured state was gotten, <c>false</c> otherwise.</returns>
	public bool GetInjuredState()
	{
		if (animator.GetBool("InjuredState"))
		{
			animator.SetFloat("Speed", 0);
			animator.SetInteger(AttackIndex, 0);

            return true;
		}
		else
        {
			return false;
		}
	}

	public void SetRotation(Quaternion rotaion)
	{
		if (animator.GetBool("InjuredState"))
		{
			return;
		}
		if (animator.GetInteger(AttackIndex) != 0)
		{
			return ;
		}

		if (animator.GetInteger("Skill") != 0)
		{
			return ;
		}

		this.transform.rotation = rotaion;
	}

    public void SetRotation(Vector3 dir)
    {
        if (animator.GetBool("InjuredState"))
        {
            return;
        }

        if (animator.GetInteger(AttackIndex) != 0)
        {
            return;
        }

        if (animator.GetInteger("Skill") != 0)
        {
            return;
        }
        
        this.transform.forward = dir;
    }
    
    public bool GetRotation()
	{
		if (animator.GetBool("InjuredState"))
		{
			return false;
		}

		if (animator.GetInteger("Skill") != 0)
		{
			return false;
		}

		if (animator.GetInteger(AttackIndex) != 0)
		{
			return false;
		}
		return true;
	}
    
	/// <summary>
	/// 站立;
	/// </summary>
	public void Stand()
	{
		if (i_MoveSpeed > 1)
		{
			i_MoveSpeed = 1;
		}

		if (GetInjuredState())
        {
			animator.SetFloat("Speed", 0);
		}
		else
		{
			animator.SetFloat("Speed", 0);
		}
	}

	/// <summary>
	/// 位移;
	/// </summary>
	public void RoleMoveCtrl()
	{
		if (i_MoveSpeed <= 0)
		{
			i_MoveSpeed = 0;
		}

        if (GetInjuredState())
        {
			animator.SetFloat("Speed", 0);
		}
        else
		{
			animator.SetFloat("Speed", 1.3f);
        }
	}

	float pauseTime = 2f;
	bool isPauseAnim = false;
	public void SkillCtrl(int skillId, int type)
	{
		if (animator.GetBool("InjuredState"))
		{
			return; //受击状态不能攻击;
		}

		animator.SetInteger(AttackIndex , 0);
		if (type == 3)
		{
			if (animator.GetInteger("Skill") != 3)
			{
				animator.SetInteger("Skill", type);
				animator.SetBool("PauseSkill", false);
				pauseTime = 2f;
				isPauseAnim = true;
			}
			else
			{
				animator.SetBool("PauseSkill",true);
				isPauseAnim = false;
			}
		}
	  	else if (type == 4)
		{
			SetSpecialSkill(skillId , type);
		}
		else
        {
		    //设置播放的动作普通攻击;
			animator.SetInteger("Skill", type);
		}
    }

	public void AttackCtrl()
	{
		if (animator.GetBool("InjuredState"))
		{
            return; //受击状态不能攻击;
		}

		if (animator.GetInteger(AttackIndex) != 0)
		{
            int attackIndex = animator.GetInteger(AttackIndex);
            int nextAttackIndex = attackIndex + 1;
            if (nextAttackIndex <= mMaxAttackCount)
            {
                animator.SetInteger(AttackIndex, nextAttackIndex);
            }
		}
		else
		{
			animator.SetInteger(AttackIndex , 1);
		}

		animator.SetInteger("Skill" , 0);
    }

	/// <summary>
	/// 翻滚;
	/// </summary>
	public void RollState()
	{
		if (animator.GetBool("InjuredState"))
		{
			return; //受击状态不能攻击;
		}

		animator.SetInteger(AttackIndex , 0);

		//设置播放的动作普通攻击;
		animator.SetInteger("Skill" ,99);
	}


	int i_SpecialSkillIndex;

	public void SetSpecialSkill(int skillId , int type)
	{
		if (animator.GetInteger("Skill") != 4)
		{
			i_SpecialSkillIndex = 0;
			animator.SetInteger("Skill" ,4);
		}

		i_SpecialSkillIndex++;

		animator.SetInteger("SpecialSkillIndex",i_SpecialSkillIndex);

		//设置播放的动作普通攻击;
	
		ISkillController skillController = animator.GetBehaviour<ISkillController>();
		if (skillController != null)
		{
			//skillController.iSkillId = skillId;
		}
	}


	//-------------------------------------------------------------!------------------------------------------------------------------------
	/// <summary>
	/// 播放受击动作;
	/// </summary>
	/// <param name="time">硬直时间</param>
	/// <param name="type">受击动作类型</param>
	public void PlayBackAnimator(SkillActionProperty sap)
	{
        if (animator.GetBool("ToppleState"))
        {
            PlayToppleback();

            return;
        }

        if (animator.GetBool("Blow"))
        {
            PlayFloatback();

            return;
        }

		if (!animator.GetBool("InjuredState"))
		{
			animator.SetBool("InjuredState" , true);
		}

		if (animator.GetBool("BackState") && !animator.GetBool("Blow"))
		{
			iStopTime = sap.mPauseTime;
			animator.speed = 1f;
			iStop = false;

            iBackName = "back_" + sap.mHurtDir.ToString();
            animator.Play(iBackName, -1, 0.0f);
		}
		else if (!animator.GetBool("Blow"))
		{
			iStopTime = sap.mPauseTime;
			animator.speed = 1f;
			iStop = false;

			iBackName = "back_" + sap.mHurtDir.ToString();
            animator.Play(iBackName, -1, 0.0f);
		}
	}

	bool iStop = false;
	float iStopTime;
	float normalizedTime = 0.35f;
	string iBackName;
	private void UpdateBackStop(float deltaTime)
	{
		if (!iStop && iStopTime <= 0)
		{
			return;
		}

		if (!stateInfo.IsName(iBackName))
		{
			return;
		}

		if (iStopTime > 0 && stateInfo.normalizedTime >= normalizedTime && !iStop)
		{
			animator.speed = 0.0001f;
			iStop = true;
		}
		
		if (iStop)
		{
			iStopTime -= deltaTime;
			if (iStopTime <= 0)
			{
				animator.speed = 1f;
				iStop = false;
			}
		}
	}

	#region<击退>
	private bool  isBlowExit;
	private float iBlowExitTime; //退的时间;
	private float iBlowExitSpeed;
	/// <summary>
	/// 击退;
	/// </summary>
	/// <param name="speed">击退速度.</param>
	/// <param name="distance">击退距离.</param>
	/// <param name="time">时间.</param>
	/// <param name="type">受击动作类型.</param>
	public void BlowExitAnim(SkillActionProperty sap, int type)
	{
		if (!animator.GetBool("InjuredState"))
		{
			animator.SetBool("InjuredState", true);
		}

        if (animator.GetBool("Blow"))
        {
            PlayFloatback();
        }

		isBlowExit = true;

        if (sap.mSkillRangParametr.Count < 2)
        {
            return;
        }

		iBlowExitSpeed = sap.mSkillTypeParams[0];
		iBlowExitTime = sap.mSkillTypeParams[1] / iBlowExitSpeed;
		PlayBackAnimator(sap);
	}

	/// <summary>
	/// 击退位移变化;
	/// </summary>
	private void UpdateBlowExit(float deltaTime)
	{
		if (!isBlowExit)
		{
			return;
		}

        if (iBlowExitTime <= 0)
        {
            isBlowExit = false;

            return;
        }

        iBlowExitTime -= deltaTime;
        this.transform.Translate(new Vector3(0, 0, -iBlowExitSpeed * deltaTime), Space.Self);
	}

    #endregion

	#region<击飞>

    /// <summary>
    /// 击飞
    /// </summary>
    /// <param name="floatType"></param>
	public void BlowupAnim(int floatType)
	{
        if (animator.GetBool("ToppleState"))
        {
            PlayToppleback();

            return;
        }

        if (animator.GetBool("Blow"))
        {
            PlayFloatback();

            return;
        }

        if (floatType < 1)
        {
            return;
        }

        if (!animator.GetBool("InjuredState"))
		{
			animator.SetBool("InjuredState", true);
		}

        string floatName = UtilTools.StringBuilder("float", floatType);
        animator.Play(floatName, -1, 0.0f);
        animator.Update(0.0f);
    }

    void PlayFloatback()
    {
        animator.Play("floatback", -1, 0.0f);
        animator.Update(0.0f);
    }

	#endregion

	public void Testanimator()
	{
		animator.CrossFade("back_1", 0.5f);
	}

	#region<眩晕>
	/// <summary>
	/// (被动)眩晕;
	/// </summary>
	public void SwoonAnim(float time)
	{
		if (!animator.GetBool("InjuredState"))
		{
			animator.SetBool("InjuredState",true);
		}

        // 眩晕时间不是叠加的
        my_SwoonCtrl.swoonTime = time;
		
		if (!animator.GetBool("Swoon"))
		{
			animator.SetBool("Swoon",true);
            animator.Play("swoon", -1, 0.0f);
        }
	}
	#endregion

	#region<死亡>

	/// <summary>
	///  (被动） 死亡;
	/// </summary>
	public void DeathAnim()
	{
		animator.SetBool("Born", false);
        animator.Play("diedown", -1, 0.0f);
	}

	#endregion

	#region<开启>

	/// <summary>
	///  (被动） 开启;
	/// </summary>
	public void OpenAnim()
	{
		animator.SetBool("Born", true);
	}

	#endregion

	#region<特效>

	private string i_animName;

	private void UpdatePlayerEffect()
	{

	}

	#endregion

	#region<吸附>
	public void SetAdsorbAnim()
	{

	}
	#endregion

	#region<击倒>

	int iToppleback; 
	public void SetToppleAnim(int toppleBack)
	{
        if (toppleBack < 1)
        {
            return;
        }

        iToppleback = toppleBack;
    }

    void PlayToppleback()
    {
        animator.Play(Toppleback + iToppleback, -1, 0.0f);
        //animator.Update(0.0f);
    }

    #endregion

    // Update is called once per frame
    void Update ()
	{
		if (animator)
		{
			stateInfo = animator.GetCurrentAnimatorStateInfo(0);

			if (isPauseAnim && pauseTime > 0)
			{
				pauseTime -= Time.deltaTime;
			}
			else if (isPauseAnim)
			{
				animator.SetBool("PauseSkill",true);
				isPauseAnim = false;
			}

			UpdateBackStop(Time.deltaTime);

			UpdateBlowExit(Time.deltaTime);

			// 主角技能打击 硬直时间 暂停;
			if (isPause)
			{
				if (pauseAnimtor > 0 )
				{
					pauseAnimtor -= Time.deltaTime;
					animator.speed = 0.01f;
				}
				else
				{
					animator.speed = 1f;
					isPause = false;
				}
			}
		}
	}

	private float pauseAnimtor;
	private bool isPause;
	public void PauseAnimtor(float time)
	{
		pauseAnimtor = time;
		isPause = true;
	}

    /// <summary>
    /// 动画事件帧
    /// </summary>
    /// <param name="strSkillId">格式为：技能Id,当前事件帧(60005,1)</param>
	public void PlayAnimatorEvent(string strSkillId)
	{
        if (string.IsNullOrEmpty(strSkillId))
        {
            return;
        }

        int skillId = UtilTools.IntParse(UtilTools.GetStringSplit(strSkillId)[0]);
        int eventId = UtilTools.IntParse(UtilTools.GetStringSplit(strSkillId)[1]);

        mSkillComponent.SkillRelease(skillId, eventId);
	}
}