using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Fm_ClientNet.Interface;
using SysUtils;

/// <summary>
/// 对象类型
/// </summary>
public enum ObjectType
{
    ObjectType_Unknown  = 0,    // 未知类型
    ObjectType_Player   = 1,    // 玩家
    ObjectType_Npc      = 2,    // NPC
    ObjectType_Monster  = 3,    // PVE怪物
    ObjectType_Soldier  = 4,    // 小兵
    ObjectType_Creeps   = 5,    // 野怪
    ObjectType_Tower    = 6,    // 塔
    ObjectType_Wall     = 7,    // 墙
    ObjectType_Hall     = 8,    // 基地
    ObjectType_Well     = 9,    // 泉水
    ObjectType_Born     = 10,   // 刷兵点
    ObjectType_Grass    = 11,   // 草丛
}

public class IObject
{
    public IObject()
    {
        
    }

    ~IObject()
    {
        mGameObject = null;
        mComponentList = null;
    }

    /// <summary>
    /// 服务器对象唯一标示号
    /// </summary>
    public string mStrIdent = string.Empty;
    public string StrIdent
    {
        set { mStrIdent = value; }
        get { return mStrIdent; }
    }

    private int mObjectId;
    /// <summary>
    /// 唯一Id
    /// </summary>
    public int mObjId
    {
        get
        {
            return mObjectId;
        }

        set
        {
            mObjectId = value;

            InitProperty();
        }
    }

    public int mObjModel = 0;

    /// <summary>
    /// 对象GO
    /// </summary>
    public GameObject mGameObject = null;

    /// <summary>
    /// 对象Transform
    /// </summary>
    public Transform mTransform = null;

    /// <summary>
    /// 对象名
    /// </summary>
    public string mObjName = string.Empty;

    /// <summary>
    /// AOI范围 单位:米
    /// </summary>
    public float mAOIRange = 20.0f;

    /// <summary>
    /// AOI范围列表
    /// </summary>
    public List<IObject> mAOIList = new List<IObject>();

    /// <summary>
    /// 组件数量
    /// </summary>
    private int mComponentCount = 0;
    public int ComponentCount
    {
        private set { }
        get { return mComponentCount; }
    }

    /// <summary>
    /// 对象组件列表
    /// </summary>
    private List<string> mComponentList = new List<string>();
    public List<string> ComponentList
    {
        private set { }
        get
        {
            return mComponentList;
        }
    }

    /// <summary>
    /// 数据对象
    /// </summary>
    public IGameSceneObj mGameSceneObj = null;

    /// <summary>
    /// 对象类型
    /// </summary>
    public ObjectType mObjectType = ObjectType.ObjectType_Unknown;

    /// <summary>
    /// 对象资源
    /// </summary>
    public string mObjectRes = string.Empty;

    #region <>属性数据<>

    /// <summary>
    /// 对象属性表
    /// </summary>
    public Dictionary<string, Var> mIObjectPropDic = new Dictionary<string, Var>();

    /// <summary>
    /// 当前血量
    /// </summary>
    public long mHP = -1;

    /// <summary>
    /// 最大血量
    /// </summary>
    public long mMaxHP = -1;

    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool mIsDead = false;

    /// <summary>
    /// 
    /// </summary>
    public string mConfig = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string mScript = string.Empty;

    /// <summary>
    /// 对象位置
    /// </summary>
	public Vector3 mPosition = new Vector3(-94.3f, 5f, -80.5f);

    /// <summary>
    /// 对象朝向
    /// </summary>
    public Vector3 mDirection = Vector3.forward;

    /// <summary>
    /// 索引，用于区分同类型对象
    /// </summary>
    public int mIndex;

    #endregion

    private AnimatorComponent mAnimatorManager = null;
	public AnimatorComponent ObjAnimatorManager
    {
        set
        {
            mAnimatorManager = value;
        }

        get
        {
            return mAnimatorManager;
        }
    }

    public void AddComponent<T>() where T : IComponent
    {
        T t = mGameObject.GetComponent<T>();
        if (t != null)
        {
            return;
        }

        t = mGameObject.AddComponent<T>();
        t.SetObject(this);
    }

    public T GetComponent<T>() where T : IComponent
    {
        if (mGameObject.GetComponent<T>() == null)
        {
            mGameObject.AddComponent<T>().SetObject(this);
        }

        return mGameObject.GetComponent<T>();
    }

    void InitProperty()
    {
        if (mObjectId < 1)
        {
            return;
        }

        List<Property> list = XmlManager.Instance.GetProperty(mObjectId);
        if (list == null)
        {
            return;
        }

        int count = list.Count;
        if (count < 1)
        {
            return;
        }

        for (int i = 0; i < count; ++i)
        {
            string propName = list[i].mPropName;
            Var var = list[i].mValue;

            SetProperty(propName, var);
        }
    }

    /// <summary>
    /// 设置属性
    /// </summary>
    /// <param name="propName"></param>
    /// <param name="value"></param>
    public void SetProperty(string propName, Var value)
    {
        if (!mIObjectPropDic.ContainsKey(propName))
        {
            mIObjectPropDic.Add(propName, Var.zero);
        }

        mIObjectPropDic[propName] = value;
    }

    /// <summary>
    /// 获取属性
    /// </summary>
    /// <param name="propName"></param>
    /// <returns></returns>
    public Var GetProperty(string propName)
    {
        if (!mIObjectPropDic.ContainsKey(propName))
        {
            return Var.zero;
        }

        return mIObjectPropDic[propName];
    }

    /// <summary>
    /// 朝向目标
    /// </summary>
    /// <param name="obj"></param>
    public void LookAt(IObject obj)
    {
        if (obj == null)
        {
            return;
        }

        Vector3 dir = UtilTools.Vec3Direction(obj, this);
        GetComponent<AnimatorComponent>().SetRotation(dir);
    }

    #region <>对象回调<>

    virtual public void OnAddObject()
    {
        if (mGameSceneObj == null)
        {
            return;
        }

        float x = 0.0f, y = 0.0f, z = 0.0f;
        x = mGameSceneObj.GetPosiX();
        y = mGameSceneObj.GetPosiY();
        z = mGameSceneObj.GetPosiZ();

        mPosition = new Vector3(x, y, z);

        // 属性回调
        OnObjectPropertyChange("Name");
        OnObjectPropertyChange("MoveSpeed");
        OnObjectPropertyChange("HP");
        OnObjectPropertyChange("MaxHP");
        OnObjectPropertyChange("Config");
        OnObjectPropertyChange("Script");
    }

    virtual public void OnActive()
    {

    }

    /// <summary>
    /// 显示对象创建或删除
    /// </summary>
    /// <param name="flag"></param>
    virtual public void CreateState(bool flag)
    {
        
    }

    /// <summary>
    /// 移动或停止
    /// </summary>
    /// <param name="flag"></param>
    virtual public void MoveState(bool flag)
    {

    }

    virtual public void OnHorse()
    {

    }

    virtual public void DownHorse()
    {

    }

    virtual public void StopAudio()
    {

    }

    virtual public void ModelChanged()
    {

    }

    virtual public void AddObjName()
    {

    }

    virtual public void SetBuffSize(params object[] args)
    {

    }

    virtual public void OnObjectProperty()
    {

    }

    /// <summary>
    /// IObject基础属性回调
    /// </summary>
    /// <param name="propName"></param>
    virtual public void OnObjectPropertyChange(string propName)
    {
        if (mGameSceneObj == null)
        {
            return;
        }

        switch (propName)
        {
            case "Name":
                mGameSceneObj.QueryPropStringW(propName, ref mObjName);
                break;

            case "HP":
                mGameSceneObj.QueryPropInt64(propName, ref mHP);
                break;

            case "MaxHP":
                mGameSceneObj.QueryPropInt64(propName, ref mMaxHP);
                break;

            case "Config":
                mGameSceneObj.QueryPropString(propName, ref mConfig);
                break;

            case "Script":
                mGameSceneObj.QueryPropString(propName, ref mScript);
                break;
        }
    }

    virtual public void Upgrade()
    {

    }

    virtual public void OnRemoveObjectBefore(bool bImmediatelyDelete = false)
    {

    }

    virtual public void OnRemoveObject()
    {

    }

    virtual public void OnLocation()
    {
        mPosition = new Vector3(mGameSceneObj.GetPosiX(), mGameSceneObj.GetPosiY(), mGameSceneObj.GetPosiZ());
    }

    virtual public void OnMoving()
    {

    }

    virtual public void StopMove()
    {

    }

    virtual public void RemoveName()
    {

    }

    virtual public void DisplayInfo(string hurt)
    {

    }

    virtual public void DoAction(string actionName)
    {

    }

    virtual public void DoIdleAction()
    {

    }

    virtual public void PlayEffect()
    {

    }

    virtual public void PlaySkill()
    {

    }
    
    #endregion
}