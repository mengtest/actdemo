using Fm_ClientNet.Interface;
using SysUtils;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 场景管理器
/// </summary>
public class GameSceneManager
{
    /// <summary>
    /// 场景加载状态 false正在加载 true没有加载
    /// </summary>
    public static bool mbSceneLoaded = true;

    /// <summary>
    /// 是否新手引导场景
    /// </summary>
    public static bool bGuideScene;

    /// <summary>
    /// 中心点
    /// </summary>
    public static Vector3 GSP = Vector3.zero;

    /// <summary>
    /// 宽度总个数
    /// </summary>
    public static int iGWC = 0;

    /// <summary>
    /// 高度总个数
    /// </summary>
    public static int iGHC = 0;

    public static float mfSceneProgress
    {
        get
        {
//             if (mbSceneLoaded)
//                 return 1f;
//             if (scene != null)
//                 return Mathf.Clamp01(scene.loadProgress);
            return 0f;
        }
    }

    ///当前场景路径
    public static string sceneFileDir = string.Empty;

    ///场景编号
    public static string mstrSceneID = string.Empty;

    ///区域地图服务器编号,需要转成地图编号;
    public static int mintRegionID = -1;

    public static int sceneID = -1;

    ///场景数据对象
    public static IGameScene mScene = null;

    /// 场景加载对象
    //public static GameScene scene = null;
    public static UserDelegate mSceneChanged = new UserDelegate();

    /// <summary>
    /// 清空场景
    /// </summary>
    public static void Clear()
    {
        mbSceneLoaded = false;
        mstrSceneID = string.Empty;                //场景编号
        mintRegionID = -1;                 //区域地图服务器编号,需要转成地图编号;
        mScene = null;  //场景数据对象
        //scene = null;
        sceneFileDir = string.Empty;

        mSceneChanged.ClearCalls();
        sceneID = -1;
    }

    /* @brief: 启动注册自定义消息监听
        @param: iGameRecv 消息注册接口
        @return void
   */
    public static void RegistCallback(IGameReceiver iGameRecv, bool bClear = true)
    {
        if (bClear)
        { 
            Clear();
        }

        iGameRecv.RegistCallBack("on_entry_scene", on_entry_scene);
        iGameRecv.RegistCallBack("on_exit_scene", on_exit_scene);
        iGameRecv.RegistCallBack("on_scene_property", on_scene_property);
    }

    /* @brief:进入场景消息入口点
       @param: args 该事件参数列表
       @return void
    */
    public static void on_entry_scene(VarList args)
    {
        ConnectStage.mbTryingConnect = false;
        ConnectStage.mbLoginSuccess = true;
        ConnectStage.isChangeRole = false;

        if (TimerManager.IsHaveTimer("OnNetBlockTimer"))
        {
            TimerManager.Destroy("OnNetBlockTimer");
        }

//         if (LoginStage.isNewCreateRole)
//         {
//             DataCollectionManager.Instance.SubmitCollect(DataCollectType.CREATE_ROLE);
// #if UNITY_IPHONE || UNITY_ANDROID
//                     Handheld.PlayFullScreenMovie("movie.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
//             DataCollectionManager.Instance.SubmitCollect(DataCollectType.DIS_CONNECT);
// #endif
// 
//             LoginStage.isNewCreateRole = false;
//         }

        if (ConnectStage.mbNeedReConnect)
        {
            ConnectStage.mbNeedReConnect = false;
        }
        if (ConnectStage.mbSecondTimeConnect)
        {
            ConnectStage.mbSecondTimeConnect = false;
        }

        LogSystem.Log("Begin on_entry_scene");

        bool isChangeScene = false;
        int iRegionId = 0;
        mScene = Game.Instance.mGameClient.GetCurrentScene();
        if (mScene != null)
        {
            if (mScene.QueryPropInt("ID", ref iRegionId))
            {
                if (sceneID != iRegionId)
                {
                    isChangeScene = true;
                }
                else if (!WorldStage.mbReConnected)
                {
                    //如果正常切场景、则相同场景也重新加载场景
                    isChangeScene = true;
                }
                else
                {
                    OnLeaveScene();
                }
            }
        }

        ObjectManager.RemovePlayerAll();    
        ObjectManager.bSceneExist = false;

        ///检查场景是否需要更新
        if (isChangeScene)
        {
            string strSceneID = string.Empty;
            mScene.QueryPropString("Resource", ref strSceneID);
            mstrSceneID = strSceneID;

            sceneID = iRegionId;
        }

        LogSystem.Log("End on_entry_scene");

        SceneManager.Instance.ChangeScene("Scene_Test");
    }

    /// <summary>
    /// 返回场景是否可穿人
    /// </summary>
    /// <returns></returns>
    public static bool GetSceneInsert(int RegionID)
    {
        string strInsert = "";//GameConfigManager.GetConfigDataById("SceneCanInsert");
        if (string.IsNullOrEmpty(strInsert))
            return false;
        string[] temps = strInsert.Split(',');
        for (int i = 0; i < temps.Length; i++)
        {
            if (RegionID == UtilTools.IntParse(temps[i]))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 设置场景质量
    /// </summary>
    /// <param name="args"></param>
    static void SetSceneQuality(params object[] args)
    {
        try
        {
//             if (scene != null)
//                 scene.ActivePostEffect(SystemSetting.ImageQuality != GameQuality.HIGH ? false : true);
        }
        catch (System.Exception e)
        {
            LogSystem.LogError("GameScene-->ActivePostEffect :: ", e.ToString());
        }
    }

    /// <summary>
    /// 场景加载完成
    /// </summary>
    public static void OnSceneLoadComplete()
    {

    }

    private static void ClearPoolManager()
    {
        //ObjectManager.ClearPoolManager();
    }


    static private List<string> cacheList = new List<string>();
    static private List<UnityEngine.Object> cachePreList = new List<UnityEngine.Object>();

    /* @brief:退出场景消息入口点
       @param: args 该事件参数列表
       @return void
  */
    public static void on_exit_scene(VarList args)
    {
        for (int i = 0; i < cachePreList.Count; i++)
            cachePreList[i] = null;
        cachePreList.Clear();

        ///leave scene recover rendersettings
        RenderSettings.ambientLight = Color.white;
		RenderSettings.fog = false;

        LogSystem.Log("Begin on_exit_scene");
        //ObjectManager.bSceneExist = false;
        
        ///删除场景前，释放所有数据单件
        OnLeaveScene();
        try
        {
            if (Camera.main != null)
            {
                Camera.main.backgroundColor = Color.black;
//                 FirstPersonCameraControl fpcc = Camera.main.GetComponent<FirstPersonCameraControl>();
//                 if(fpcc!=null)
//                 {
//                     fpcc.mbInterruption = true;
//                 }
            }
            mstrSceneID = string.Empty;
            mintRegionID = -1;
            mbSceneLoaded = false;
//             Instance.Get<WeatherControl>().ClearWeather();
//             TimerManager.Destroy("TalkTask");
//             BindPropManager.Clear();
//             ItemPickUp.DestroyAll();
//             AnimationControl.ListAnimation.Clear();
//             EffectLevel.ClearList();
//             ExploreControl.OnExitCurrentScene();
//             Game game = Game.Instance;
//             if (game != null && game.gameObject != null)
//             {
//                 SecretPassControl SecretContorl = game.gameObject.GetComponent<SecretPassControl>();
//                 //是否在关卡密境
//                 if (SecretContorl != null)
//                 {
//                     SecretContorl.DestoryInfo();
//                     DestroyUtils.GameDestory(SecretContorl);
//                 }
//             }
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError(ex.ToString());
        }

        try
        {
            //AssetLibrary.getBundle().RemoveAllAssets();
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError(ex.ToString());
        }

        try
        {
//             if (scene != null)
//                 scene.RemvoeWillRemoveDynUnits();
//             ObjectManager.RemovePlayerAll();
//             CacheObjects.ClearAllCache();
            ClearPoolManager();
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError(ex.ToString());
        }

        Game.Instance.mbCanReconnect = false;

        try
        {
            if (Game.Instance != null)
            {
//                SecretPassControl SecretContorl = Game.Instance.gameObject.GetComponent<SecretPassControl>();
                //是否在关卡密境
//                 if (SecretContorl != null)
//                 {
//                     SecretContorl.DestoryInfo();
//                     //DestroyUtils.GameDestory(SecretContorl);
//                 }
            }
            //EffectLoader.Clear();

            LogSystem.Log("scene destory before");
//             if (scene != null)
//             {
//                 scene.sceneLoadCompleListener = null;
//                 scene.sceneLoadingListener = null;
//                 scene.Destroy();
// 
//                 LogSystem.Log("scene destory end");
//             }
//             scene = null;
//             ObjectManager.mRole = null;
            
        }
        catch (System.Exception ex)
        {
//             scene = null;
//             ObjectManager.mRole = null;
            LogSystem.LogError(ex.ToString());
        }
        LogSystem.Log("End on_exit_scene");
    }

    public static void OnLeaveScene()
    {
        try
        {
            LogSystem.Log("Scene Exit CallBack PlayerMainViewPanle");
//             if (GUIManager.HasView<PlayerMainViewPanel>())
//             {
//                 LogSystem.Log("PlayerMainViewPanle  HasView");
//                 GUIManager.CallViewFunc<PlayerMainViewPanel>((IView view) =>
//                 {
//                     PlayerMainViewPanel pPanel = view as PlayerMainViewPanel;
//                     if (pPanel != null)
//                     {
//                         pPanel.OnSceneExit();
//                         LogSystem.Log("PlayerMainViewPanel OnSceneExit");
//                     }
//                 });
//             }
        }
        catch (System.Exception e)
        {
            LogSystem.LogError("Scene Exit ", e.ToString());
        }
        try
        {
//             if (SystemSetting.imageQualityCallback != null)
//                 SystemSetting.imageQualityCallback.RemoveCalls(SetSceneQuality);

            GameDefine.UnInit();

//             ///背包数据清理至初始化;
//             PackItemData.CleanInstance();
//             Instance.Clean<PackItemData>();
//             ///装备数据清理至初始化;
//             RoleEquipmentInfo.CleanInstance();
//             PetDataManager.CleanInstance();
//             Instance.Clean<RoleEquipmentInfo>();
//             ///快速装备卸载
//             Instance.Get<FastEquip>().UnInit();
//             Instance.Clean<FastEquip>();
//             //切换场景关闭大地图;
//             GUIManager.DestroyView<WorldMapPanel>();
// 
//             Instance.Get<SystemText>().UnInit();
// 
//             Instance.Get<SoundControl>().StopMusicBack();
//             TaskTrack.Clear();
//             HangUpManager.UnInit();
//             FightOrderData.UnInit();
//             MainViewRedPointManager.Clear();
//             Instance.Get<SkillManager>().ResourceUnLoad();
//             ///按钮冷却
//             Instance.Get<ButtonCoolTimerManager>().Clear();
//             //卸载任务数据服务类    数据服务类，切场景只清空，不删除
//             Instance.Get<TaskRecord>().Clear();
//             //卸载金币数据服务类     数据服务类，切场景只清空，不删除
//             Instance.Get<MoneyData>().Clear();
//             //卸载黑名单数据服务类    数据服务类，切场景只清空，不删除
//             Instance.Get<BlackFriendRecord>().Clear();
//             //卸载日常活动数据服务类   数据服务类，切场景只清空，不删除
//             Instance.Get<DailyRecord>().Clear();
//             //卸载组队信息表;
//             Instance.Get<TeamMemRecord>().Clear();
//             //卸载伙伴信息表;
//             Instance.Get<PartnerRecord>().Clear();
//             //卸载好友伙伴申请表;
//             Instance.Get<FriendApplyRecord>().Clear();
//             //卸载好友表;
//             Instance.Get<FriendMemRecord>().Clear();
//             Instance.Get<BufferManager>().Clear();
// 
//             LoginAwardManager.UnInit();
// 
//             //释放掉所有自己放进场景的对象
//             DestoryManager.Destory();
        }
        catch (System.Exception ex)
        {
            LogSystem.Log(ex.ToString());
        }

    }

    /* @brief:场景属性变化消息入口点
       @param: args 该事件参数列表
       @return void
    */
    public static void on_scene_property(VarList args)
    {
    }
}
