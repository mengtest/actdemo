using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fm_ClientNet.Interface;
using SysUtils;


public class FightManager : MonoBehaviour
{
    private static FightManager _Instance = null;
    public static FightManager Instance
    {
        private set { }
        get { return _Instance; }
    }

    /// <summary>
    /// 副本Id
    /// </summary>
    private int mCloneId = 0;

    /// <summary>
    /// 战斗开始时间
    /// </summary>
    private float mFightStartTime = 0.0f;

    /// <summary>
    /// 是否是PVE
    /// </summary>
    private bool mIsPve = true;

    /// <summary>
    /// 主角
    /// </summary>
    private CRoleObject mRole = null;

    /// <summary>
    /// 战斗UI
    /// </summary>
    public FightUI mFightUI = null;

    /// <summary>
    /// 战斗是否结束
    /// </summary>
    private bool mFightFinished = false;

    /// <summary>
    /// 预先加载预设对象
    /// </summary>
    private Dictionary<string, GameObject> mPreLoadObjectDic = new Dictionary<string, GameObject>();

    /// <summary>
    /// 索引，用于区分同类型对象
    /// </summary>
    private int mIndex = 0;

    /// <summary>
    /// 战斗准备时间
    /// </summary>
    private float mFightPrepareTime = 5.0f;

    void Awake()
    {
        _Instance = this;

        DontDestroyOnLoad(gameObject);

        mRole = ObjectManager.mRole;

        // 注册服务端消息
        RegisterCallback();
    }

    /// <summary>
    /// 注册服务端消息
    /// </summary>
    void RegisterCallback()
    {
        CustomSystem.RegistCustomCallback(0, OnMonsterKilled);
    }

    void OnMonsterKilled(VarList args)
    {

    }

    /// <summary>
    /// 初始化战斗
    /// </summary>
    public void InitFight(int cloneId)
    {
        if (cloneId < 1)
        {
            return;
        }
        mCloneId = cloneId;

        CloneSceneProperty cs = XmlManager.Instance.GetCloneSceneProperty(mCloneId);
        if (cs == null)
        {
            return;
        }

        // 预加载资源
        PreLoadObject(cs);

        // 初始化怪物信息
        InitMonster(cs.mCloneSceneMonsterInfoList);

        // 初始化战斗UI
        InitFightUI();

        // 战斗开始倒计时
        TimerManager.AddTimerCount("FightCountDown", mFightPrepareTime, FightCountDown);
    }

    void PreLoadObject(CloneSceneProperty cs)
    {
        // 预缓存怪物预设
        int count = cs.mCloneSceneMonsterInfoList.Count;
        for (int i = 0; i < count; ++i)
        {
            int monsterId = cs.mCloneSceneMonsterInfoList[i].mMonsterId;
        }

        // 预缓存主角技能特效预设
        
    }

    /// <summary>
    /// 战斗开始倒计时
    /// </summary>
    /// <param name="count"></param>
    /// <param name="args"></param>
    void FightCountDown(int count, params object[] args)
    {
        if (count > 0.0f)
        {
            
        }
        else
        {
            StartFight();
        }
    }

    public void InitMonster(List<CloneSceneProperty.CloneSceneMonsterInfo> list)
    {
        if (list == null)
        {
            return;
        }

        int count = list.Count;
        if (count < 1)
        {
            return;
        }

        GameObject monsterParent = NGUITools.AddChild(gameObject);
        monsterParent.transform.localPosition = Vector3.zero;
        monsterParent.name = "Monsters";

        for (int i = 0; i < count; ++i)
        {
            CloneSceneProperty.CloneSceneMonsterInfo csm = list[i];
            if (csm == null)
            {
                continue;
            }

            int monsterId = csm.mMonsterId;
            Vector3 postion = csm.mMonsterPosition;
            float rotation = csm.mMonsterRotationY;
            float scale = csm.mMonsterScale;
            if (scale.Equals(0.0f))
            {
                scale = 1.0f;
            }

            CMonsterObject cObj = new CMonsterObject();

            GameObject go = ResourceManager.Instance.GetMonsterPrefab(monsterId.ToString());
            if (go == null)
            {
                LogSystem.LogError("GetMonsterPrefab Failed, monsterId = " + monsterId);

                continue;
            }

            ++mIndex;

            cObj.mObjId = monsterId;
            cObj.mIndex = mIndex;

            go.name = monsterId.ToString();
            go.transform.parent = monsterParent.transform;
            go.transform.position = postion;
            go.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            go.transform.localScale = new Vector3(scale, scale, scale);

            cObj.mGameObject = go;
            cObj.mTransform = go.transform;

            cObj.mPosition = postion;

			cObj.AddComponent<AnimatorComponent>();

            cObj.AddComponent<RestoreComponent>();
            cObj.AddComponent<InjuredComponent>();

            cObj.AddComponent<FindingPathComponent>();
            cObj.AddComponent<MoveComponent>();
            cObj.AddComponent<AttackComponent>();
            cObj.AddComponent<HatredComponent>();
            cObj.AddComponent<PursuitComponent>();
            
            cObj.AddComponent<SkillComponent>();
			cObj.AddComponent<EffectComponent>();

            RangeTools.MotifyObjectAoi(ObjectManager.mRole, cObj);

            ObjectManager.mObjectsList.Add(cObj);
        }
    }

    void InitFightUI()
    {
        GameObject uiPrefab = ResourceManager.Instance.GetUIPrefab("FightUI");
        if (uiPrefab == null)
        {
            LogSystem.LogError("FightUI is null");
        }

        GameObject parent = GameObject.Find("UI Root/Camera");
        GameObject go = NGUITools.AddChild(parent, uiPrefab);
        go.name = "FightUI";
        go.AddComponent(System.Type.GetType("FightUI"));
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        mFightUI = go.GetComponent<FightUI>();
    }

    /// <summary>
    /// 开始战斗
    /// </summary>
    public void StartFight()
    {
        GlobalData.TimeScale = 1.0f;


    }

    /// <summary>
    /// 战斗暂停
    /// </summary>
    public void PauseFight()
    {
        GlobalData.TimeScale = 0.0f;
    }

    /// <summary>
    /// 最后一个敌人死亡
    /// </summary>
    void FinalEnemyDown()
    {
        // 开启慢动作
        GlobalData.TimeScale = 0.1f;

        // 开启模糊效果


        // 协程结束后战斗结算
        StartCoroutine("SettlementFight");
    }

    /// <summary>
    /// 显示战斗结算界面
    /// </summary>
    IEnumerator SettlementFight()
    {
        yield return new WaitForSeconds(2.0f);

        GlobalData.TimeScale = 1.0f;
    }

    /// <summary>
    /// 战斗结束
    /// </summary>
    public void FinishFight()
    {
        // 清理战场中缓存的资源
        ResourceManager.Instance.ClearCacheObject();

        mIndex = 0;

        mFightFinished = true;
    }

    void Update()
    {
        if (mFightFinished)
        {
            return;
        }


    }
}