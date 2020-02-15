using UnityEngine;
using Fm_ClientNet.Interface;
using System.Collections.Generic;
using SysUtils;

/// <summary>
/// 动态对象基类
/// </summary>
public class CGameObject : IObject
{
    public CGameObject()
    {

    }

    ~CGameObject()
    {
        UtilTools.DestroyGameObject(mFBX);
        UtilTools.DestroyGameObject(mHorse);
        UtilTools.DestroyGameObject(mHeadBillBoard);
        UtilTools.DestroyGameObject(mDamageNumber);
    }

    #region <>基础类属性<>

    /// <summary>
    /// 名字
    /// </summary>
    public string mPlayerName = string.Empty;

    /// <summary>
    /// 等级
    /// </summary>
    public int mLevel = 0;
    
    /// <summary>
    /// 对象阵营
    /// </summary>
    public int mCamp = -1;

    /// <summary>
    /// 国家
    /// </summary>
    private int mRoleNation = 0;

    /// <summary>
    /// 获取国家
    /// </summary>
    public int Nation { get { return mRoleNation; } }

    #endregion

    #region <>状态类属性<>

    /// <summary>
    /// 移动状态
    /// </summary>
    public bool mCanMove = true;

    /// <summary>
    /// 攻击状态
    /// </summary>
    public bool mCanAttack = true;

    /// <summary>
    /// 被攻击状态
    /// </summary>
    public bool mCanBeAttack = true;

    /// <summary>
    /// 沉默(只能释放普攻，不能放技能)
    /// </summary>
    public bool mSilent = false;

    /// <summary>
    /// 是否死亡 1死亡　0正常
    /// </summary>
    public bool mDead = false;

    #endregion

    #region <>数值类属性<>

    /// <summary>
    /// 物体朝向
    /// </summary>
    public float orient = 0f;

    #endregion

    #region <>资源类属性<>

    /// <summary>
    /// 对象模型号
    /// </summary>
    public string mstrModel = string.Empty;

    /// <summary>
    /// 资源路径
    /// </summary>
    public string mstrRes = string.Empty;

    /// <summary>
    /// 人物FBX模型
    /// </summary>
    public GameObject mFBX = null;

    /// <summary>
    /// 马FBX模型
    /// </summary>
    public GameObject mHorse = null;

    /// <summary>
    /// 头顶公告板
    /// </summary>
    public GameObject mHeadBillBoard = null;

    /// <summary>
    /// 伤害飘动
    /// </summary>
    public GameObject mDamageNumber = null;

    #endregion

    #region <>引擎类属性<>

    /// <summary>
    /// Config
    /// </summary>
    public string mConfig = string.Empty;

    /// <summary>
    /// Script
    /// </summary>
    public string mstrScript = string.Empty;

    #endregion

    /// <summary>
    /// 游戏对象类型
    /// </summary>
    public enum OBJECT_TYPES_ENUM : int
    {
        TYPE_SCENE = 1,		// 场景
        TYPE_PLAYER = 2,	// 玩家
        TYPE_NPC = 4,		// NPC
        TYPE_ITEM = 8,		// 物品
        TYPE_HELPER = 16,	// 辅助对象
        TYPE_WEAKBOX = 32,	// 弱关联容器
    };

    /// <summary>
    /// 特效映射关系
    /// </summary>
    private Dictionary<string/*特效id*/, GameObject/*特效*/> mEffectDic = new Dictionary<string, GameObject>();

    /// <summary>
    /// 对象公用变量定义
    /// </summary>
    public OBJECT_TYPES_ENUM miType;

    /// <summary>
    /// 显示对象
    /// </summary>
    public UnityEngine.Transform mVisualTrans = null;

    /// <summary>
    /// 数据对象
    /// </summary>
    public IGameSceneObj mDataObject = null;

    /// <summary>
    /// 马模型号
    /// </summary>
    public string _mstrAssembleHorse = string.Empty;

    public string mstrAssembleHorse
    {
        get
        {
            return _mstrAssembleHorse;
        }
        set
        {
            _mstrAssembleHorse = value;
            if (string.IsNullOrEmpty(_mstrAssembleHorse))
            {
                //mLocalRideVo = null;
            }
            else
            {
                //mLocalRideVo = MountManager.GetLocalRideById(_mstrAssembleHorse);
            }
        }
    }

    /// <summary>
    /// 人模型号
    /// </summary>
    public string mstrAssembleModel = string.Empty;

    /// <summary>
    /// 武器模型号
    /// </summary>
    public string mstrAssembleWeapon = string.Empty;

    /// <summary>
    /// 武器
    /// </summary>
    public GameObject moWeapon = null;

    public bool mJoyControl = false;
    /// <summary>
    /// 启用摇杆控制
    /// </summary>
    public bool JoyControl
    {
        get
        {
            return mJoyControl;
        }
        set
        {
            mJoyControl = value;
            if (!mJoyControl)
            {
                if (this is CRoleObject)
                {
                    //if (ObjectManager.mRole != null)
                        //ObjectManager.mRole.BreakAutoMove();
                }
                else
                {
                }
            }
        }
    }

    virtual public void OnAddObject()
    {
        try
        {
            if (mDataObject == null)
            {
                return;
            }

            OnObjectPropertyChange("Name");
            OnObjectPropertyChange("Level");
        }
        catch (System.Exception e)
        {
            LogSystem.LogError(e.ToString());
        }
    }

    virtual public void OnObjectProperty()
    {

    }

    virtual public void OnObjectPropertyChange(string strPropName)
    {
        if (mDataObject == null)
        {
            return;
        }
        
        switch (strPropName)
        {
            case "Name":
                {
                    string name = "";
                    mDataObject.QueryPropStringW(strPropName, ref name);
                    mObjName = name;
                }
                break;

            case "Level":
                mDataObject.QueryPropInt(strPropName, ref mLevel);
                break;

            default:
                break;
        }
    }

    protected virtual void OnActive(bool flag)
    {

    }

    public virtual void OnRemoveObjectBefore(bool bImmediatelyDelete = false)
    {

    }

    public virtual void OnRemoveObject()
    {

    }

    protected virtual void CreateState(bool flag)
    {

    }

    public virtual void OnMoving()
    {

    }

    public virtual void StopMove()
    {

    }

    public virtual void OnLocation()
    {

    }

    protected virtual void MoveState(bool flag)
    {

    }

    public virtual void OnHorse()
    {

    }

    public virtual void DownHorse()
    {

    }

    protected virtual void Upgrade()
    {

    }
}