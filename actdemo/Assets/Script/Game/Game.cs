using UnityEngine;
using Fm_ClientNet;
using Fm_ClientNet.Interface;
using SysUtils;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    private static Game _Instance = null;
    public static Game Instance
    {
        get { return _Instance; }
    }

    /// <summary>
    /// 断线重连时间间隔
    /// </summary>
    public const int mfIntervalTime = 2;
    public IGameClient mGameClient;  ///引擎客户端数据管理对象
    public IGameSock mGameSock;      ///网络管理对象
    public IGameReceiver mGameRecv;  ///数据接收者对象
    public IGameSender mGameSender;  ///数据发送者对象

    public ConnectStage mConnectStage;
    public LoginStage mLoginStage;
    public WorldStage mWorldStage;

    public string mstrValidateString = string.Empty;
    public bool mbNetInit = false;
    public bool mbCanReconnect = false;
    public ClientNet mClientNet = new ClientNet();  ///网络模块对象
    public bool mbPaused = false;//是否在后台

    /// <summary>
    /// 心跳间隔时间 服务端发送过来
    /// </summary>
    private uint m_BeatDuration = 0;
    private float m_BeatStartTime = 0;
    private const uint MINDURATION = 5;

    public float mfLastTime = 0.0f;

    void Awake()
    {
        _Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        mfLastTime = Time.realtimeSinceStartup;

        // 平台初始化成功后，开始加载游戏
        PlatformManager.Instance.Init(GlobalData.mScreenOrientation, 
                                      GlobalData.mDebug, 
                                      GlobalData.mAnySDK,
                                      OnPlatformInitFinished);
    }

    /// <summary>
    /// 注册绑定验证库
    /// </summary>
    private void InitVerify()
    {
        fxVerify.SetFxVerify_EncodeAccount(UnityVerify.UVFxVerify_EncodeAccount);
        fxVerify.SetFxVerify_EncodePassword(UnityVerify.UVFxVerify_EncodePassword);
        fxVerify.SetFxVerify_GetChooseRoleVerify(UnityVerify.UVFxVerify_GetChooseRoleVerify);
        fxVerify.SetFxVerify_GetCustomVerify(UnityVerify.UVFxVerify_GetCustomVerify);
        //fxVerify.SetFxVerify_GetInterface(UnityVerify.UVFxVerify_GetInterface);
        fxVerify.SetFxVerify_GetLoginVerify(UnityVerify.UVFxVerify_GetLoginVerify);
        fxVerify.SetFxVerify_GetRetEncodeVerify(UnityVerify.UVFxVerify_GetRetEncodeVerify);
        fxVerify.SetFxVerify_GetSelectVerify(UnityVerify.UVFxVerify_GetSelectVerify);

        fxVerify.SetFxVerify_GetVersion(UnityVerify.UVFxVerify_GetVersion);
        fxVerify.SetFxVerify_Release(UnityVerify.UVFxVerify_Release);
        fxVerify.SetFxVerify_fnSetClientCode(UnityVerify.UVFxVerify_fnSetClientCode);
        fxVerify.SetFxVerify_fnSetClientCond(UnityVerify.UVFxVerify_fnSetClientCond);
    }

    /// <summary>
    /// 初始化网络模块
    /// </summary>
    public void InitNetWork()
    {
        if (mbNetInit)
        {
            mClientNet.ShutDown();
            mClientNet.UnInit();
        }

        try
        {
            InitVerify();

            mbNetInit = mClientNet.Init(GlobalData.mDeviceMac, null, null);
            mGameSock = mClientNet.GetGameSock();
            mGameClient = mClientNet.GetGameClient();
            mGameRecv = mGameSock.GetGameReceiver();
            mGameSender = mGameSock.GetGameSender();
            GameCommand.SetSender(mGameSender);
        }
        catch (System.Exception e)
        {
            LogSystem.LogError("InitNetWork::", e.ToString());
        }
    }

    /// <summary>
    /// 初始化游戏世界控制
    /// </summary>
    public void InitGameWord()
    {
        ConnectStage.RegistCallback();
        LoginStage.RegistCallback();
        WorldStage.RegistCallback();
    }

    void OnPlatformInitFinished()
    {
        PlatformManager.Instance.GetDeviceMac(OnPlatformGetDeviceMac);
    }

    void OnPlatformGetDeviceMac(string mac)
    {
        InitNetWork();

        InitGameWord();
    }

    /// <summary>
    /// 重新启动接口
    /// </summary>
    /// <param name="strServerIP">服务器地址</param>
    /// <param name="iServerPort">服务器端口</param>
    /// <param name="strUserName">用户名称</param>
    /// <param name="strPassword">用户密码</param>
    /// <param name="strValidate">一次性串</param>
    public void QuitGame()
    {
//         GameSceneManager.on_exit_scene(new SysUtils.VarList());
//         if (GameSceneManager.scene != null)
//         {
//             GameSceneManager.scene.UpdateView(Vector3.one * 1000);
//             GameSceneManager.scene = null;
//         }
// 
//         GUIManager.DestroyAllView();
//         TimerManager.ClearTimer();
// 
//         DontDestroy[] oObjects = GameObject.FindObjectsOfType<DontDestroy>();
//         if (oObjects != null)
//         {
//             for (int i = 0; i < oObjects.Length; i++)
//             {
//                 DestroyUtils.GameDestory(oObjects[i]);
//             }
//         }
// 
//         Instance.Get<SoundControl>().StopMusicBack();
    }
    /// <summary>
    /// 重新登录初始化
    /// </summary>
    /// <param name="strServer"></param>
    /// <param name="iPort"></param>
    /// <param name="strUserName"></param>
    /// <param name="strPassword"></param>
    /// <param name="strValidateString"></param>
    public void RestartGame(string strServer, int iPort, string strUserName, string strPassword, string strValidateString = "", int iAutoLogin = 2)
    {
        LogSystem.Log("Game::RestartGame");
        UnInitNetWork();

        //初始化网络
        InitNetWork();
        //初始化游戏世界
        InitGameWord();
//        Instance.Get<PlayerHeadControl>().InitPanel();

        ///发动重新连接
        if (!string.IsNullOrEmpty(strServer) && !string.IsNullOrEmpty(strUserName) && !string.IsNullOrEmpty(strPassword))
        {
//             ConnectStage.ConnectTo(strServer, iPort, strUserName, strPassword, strValidateString, iAutoLogin);
//             ///设定使用上次登录角色重登
//             LoginStage.miAutoLogin = iAutoLogin;
        }
    }

    /// <summary>
    /// 返回登录界面
    /// </summary>
    public void GoBackLogin()
    {
        LogSystem.Log("Game::GoBackLogin");
        UnInitNetWork();

        //初始化网络
        InitNetWork();
        //初始化游戏世界
        InitGameWord();
    }

    /// <summary>
    /// anysdk登出回调(渠道触发)
    /// </summary>
    /// <param name="ret"></param>
    /// <param name="msg"></param>
    public void AnySdkLogOut(string ret, string msg)
    {
        LogSystem.LogError("\tGame::\tAnySdkLogOut");
        OnReChooseRoleFailed();
    }


    /// <summary>
    /// 关闭网络
    /// </summary>
    public void UnInitNetWork()
    {
        if (mbNetInit)
        {
            try
            {
                GameCommand.SetSender(null);
                mClientNet.ShutDown();
                mClientNet.UnInit();
                mbNetInit = false;
            }
            catch (System.Exception e)
            {
                LogSystem.LogError("UnInitNetWork :: ", e.ToString());
            }
        }
    }

    /// <summary>
    /// 重新选择角色成功后，清理数据
    /// </summary>
    public void ReChooseRoleClear()
    {
        //断线重新选择角色成功,将重新选择角色标识符 制为false 恢复到正常状态
//        ConnectStage.isChangeRole = false;

        try
        {
//             GameSceneManager.on_exit_scene(new SysUtils.VarList());
//             //GameSceneManager.Clear();
//             WorldStage.RegistCallback();
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError("ReChooseRole on_exit_scene catch error", ex.ToString());
        }
        try
        {
//             GUIManager.DestroyAllView();
//             TimerManager.ClearTimer();
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError("rechooseRole DestroyAllView catch error", ex.ToString());
        }

//        Instance.Get<PlayerHeadControl>().InitPanel();
    }

    /// <summary>
    /// 重新选角色
    /// </summary>
    public void ReChooseRole()
    {
//         Config.isFirstLogin = false;
//         ConnectStage.isChangeRole = true;
// 
//         LogSystem.Log("Game::ReChooseRole");
// 
//         SystemWaitPanel.Start();
// 
//         GameCommand.SendCustom(CustomHeader.CLIENT_CUSTOMMSG_CHECKED_SELECTROLE);
// 
//         TimerManager.AddDelayTimer("ReChooseRoleFailedTime", 5f, OnReChooseRoleFailed);

    }

    /// <summary>
    /// 5秒钟以内 如果断线重连还是没有成功 则客户端认为已经断线 直接退出登录场景
    /// </summary>
    private void OnReChooseRoleFailed()
    {
        ReChooseRoleClear();    //清除数据

        GoBackLogin();
    }

    //2秒断线重连
    private void OnRestNet()
    {
        NGUIDebug.Log("Game::OnRestNet 断线重连");
        //断线重连需要 一直显示转圈 不显示提示
        //SystemWaitPanel.Start(99999999);
        VarList varlist = VarList.GetVarList();
        //ConnectStage.on_connect_block(varlist);
        varlist.Collect();
    }

    /// <summary>
    /// 停止计时
    /// </summary>
    public void RestTime()
    {
        NGUIDebug.Log("Game::RestTime 断线重连停止计时");
        //TimerManager.Destroy("ReChooseRoleFailedTime");
    }

    public float mLastHeartTime = 0.0f;
    public float mfDelayTime = 0.0f;
    public float mfDisplayTime_Ping= 0.0f;
    public int miIndex = 0;
    Dictionary<int, float> mDelayTime = new Dictionary<int, float>();

    /// <summary>
    /// 获取编号对应发送时间
    /// </summary>
    /// <param name="iKey">编号键值</param>
    /// <returns></returns>
    public float GetDelayTime(int iKey)
    {
        if (mDelayTime.ContainsKey(iKey))
        {
            return mDelayTime[iKey];
        }

        return Time.realtimeSinceStartup - 1.0f;
    }
    public bool mbEnterGame = false;
    int iTick = 0;

    void Update()
    {
        if (!mbNetInit || mClientNet == null)
        {
            return;
        }

        iTick++;
        if (iTick % 2 == 0)
        {
            mClientNet.Excutue();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            GameCommand.SendCustom((CustomHeader)141, 0, 3, 1);
        }

//         if (ObjectManager.mRole != null && !WorldStage.mbReConnected)
//         {
//             SendGameHeartBeat();
//         }
    }

    /// <summary>
    /// 定时发送游戏心跳
    /// </summary>
    private void SendGameHeartBeat()
    {
        if (m_BeatDuration == 0)
        {
            m_BeatStartTime = Time.realtimeSinceStartup;
            m_BeatDuration = mGameRecv.GetBeatInterval();
            if (m_BeatDuration < MINDURATION)
            {
                m_BeatDuration = (uint)MINDURATION;
            }
        }

        float time = Time.realtimeSinceStartup;
        if (time - m_BeatStartTime >= m_BeatDuration)
        {
            m_BeatStartTime = time;
            GameCommand.SendCustom(CustomHeader.CLIENT_CUSTOMMSG_HEART_BEAT);
        }
    }

    /// <summary>
    /// 应用程序退出回调
    /// </summary>
    public void OnDestroy()
    {
        if (mClientNet != null)
        {
            mClientNet.UnInit();
            mClientNet = null;
        }
    }

    /// <summary>
    /// 手动调用释放，onDestroy不及时
    /// </summary>
    public void FreeOnDestroy()
    {
        if (mClientNet != null)
        {
            mClientNet.ShutDown();
            mClientNet.UnInit();
            mClientNet = null;
        }
    }
#if DEV
    public bool mbShowFps = true;
    public bool mbServerNet = true;
#else
#endif
    public bool mbCGfps = false;
    
    public bool mbEffectWeight = true;
    public string mstrServerNetInfo = string.Empty;

#if DEV && FPS
    void OnGUI()
    {
        if (mbShowFps)
        {
            if (GameScene.mainScene != null)
            {
                GUI.Label(new Rect(0, 100, 100, 30), "fps:" + GameScene.mainScene.fps);
            }
        }

        if (mbServerNet && mfDisplayTime_Ping> 0.001f)
        {
            GUI.Label(new Rect(0, 130, 200, 300), "nfps:" + mfDisplayTime_Ping+"ms");
        }
    }
#endif
}
