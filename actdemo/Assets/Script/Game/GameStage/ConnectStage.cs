using SysUtils;
using Fm_ClientNet.Interface;
using Fm_ClientNet;

/// <summary>
/// 连接服务器状态管理
/// </summary>
public class ConnectStage
{
    /// <summary>
    /// 外部转储数据
    /// </summary>
    public static string mstrUser = string.Empty;
    public static string mstrPsd = string.Empty;
    public static string mstrServer = string.Empty;
    public static int miPort = 0;
    public static string mstrValidateString = string.Empty;
    public static string mstrRoleName = string.Empty;

    public static bool mbRecvMsgVersion = false;
    /// <summary>
    /// 内部变量，需要清空
    /// </summary>
    private static bool mbLoginReconnect = false;

    public static bool mbConnected = false;

    /// <summary>
    /// 是否被踢出游戏
    /// </summary>
    public static bool isGameOut = false;

    /// <summary>
    /// 选择角色操作  进行此操作、禁止断线重连，直接将用户送登陆界面 让用户重新登陆游戏
    /// </summary>
    public static bool isChangeRole = false;

    public static bool mbNeedReConnect = false;

    /// <summary>
    /// 是否在登录之后
    /// </summary>
    public static bool mbLoginSuccess = false;

    /// <summary>
    /// 是否尝试重连
    /// </summary>
    public static bool mbTryingConnect = false;

    /// <summary>
    /// 连接状态清空
    /// </summary>
    public static void Clear()
    {
        mbLoginReconnect = false;
        mbConnected = false;
    }
    /// <summary>
    /// 文本协议Socket消息接受，只用于获取服务器列表数据
    /// </summary>
    /// <param name="textSock"></param>
    /// <param name="args"></param>
    static void OnTextSockRecieveData(TextSock textSock, ref VarList args)
    {
        LogSystem.Log("OnTextSockRecieveData");
        int count = args.GetCount();
        if (count <= 0)
        {
            LogSystem.Log("OnTextSockRecieveData param null");
            return;
        }
        ///消息头
        string strHeader = args.GetString(0);
        LogSystem.Log("OnTextSockRecieveData", strHeader);
        if (strHeader.Equals("svrlist"))
        {
            ///服务器列表
            string strServerName = args.GetString(1);
            string strMemberAddr = args.GetString(2);
            int nMemberPort = args.GetInt(3);
            int iLoad = args.GetInt(4);
            LogSystem.Log(strServerName, "|", strMemberAddr, "|", nMemberPort, "|", iLoad);
            textSock.Disconnect();
            if (string.IsNullOrEmpty(strServerName))
            {
                mstrMemberAddress = strMemberAddr;
                mstrMemberPort = nMemberPort;
                miRecieveMemberType = -2;
            }
            else
            {
                mstrMemberAddress = strMemberAddr;
                mstrMemberPort = nMemberPort;
                miRecieveMemberType = 1;
            }

        }
        else if (strHeader.Equals("version_error"))
        {
            miRecieveMemberType = 2;
        }
    }
    static int miClientVersion = 1;
    /// <summary>
    /// 文本协议socket连接成功
    /// </summary>
    /// <param name="textSock"></param>
    static void OnTextSockConnected(TextSock textSock)
    {
        LogSystem.Log("OnTextSockConnected");
        string strSendInfo = "svrlist" + "" + GlobalData.Version;
        if (!textSock.Send(strSendInfo))
        {
            LogSystem.LogError("textSock Send failed");
        }
    }
    private static string mstrConnectServerIP = string.Empty;
    private static int miConnectServerPort = 4000;

    /// <summary>
    /// 文本协议socket连接成功
    /// </summary>
    /// <param name="textSock"></param>
    static void OnTextSockConnectFailed(TextSock textSock)
    {
        LogSystem.Log("OnTextSockConnectFailed");
        textSock.Disconnect();
        miRecieveMemberType = -1;

    }

    private static int miRecieveMemberType = 0;
    private static string mstrMemberAddress = string.Empty;
    private static int mstrMemberPort = 2001;
    private static string mstrMemberUserName = string.Empty;
    private static string mstrMemberPassword = string.Empty;
    private static string mstrMemberValidatestring = string.Empty;
    private static int miMemberReconnect = 0;
    static void CheckConnectToMember()
    {
        switch (miRecieveMemberType)
        {
            case 0:
                {
                    //未接收
                }
                break;
            case 1:
                {
                    ///正常接收到服务器列表
                    miRecieveMemberType = 0;
                    TimerManager.ClearTimerWithPrefix("CheckServerListen");
                    ///接收
                    ConnecToMember(mstrMemberAddress, mstrMemberPort, mstrMemberUserName, mstrMemberPassword, mstrMemberValidatestring, miMemberReconnect);
                }
                break;
            case 2:
                {
                    miRecieveMemberType = 0;
                    TimerManager.ClearTimerWithPrefix("CheckServerListen");

//                     ///提示版本错误
//                     SystemWaitPanel.End();
//                     string strHeader = TextManager.Instance.GetString("UI00031");
//                     PromptType pt = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);
//                     pt.layer = 3;
//                     pt.title = strHeader;
//                     pt.content = TextManager.Instance.GetUIString("UI18000");
//                     pt.style = PromptType.Style.OK;
//                     GUIManager.ShowView<SystemPrompt>(pt);
                }
                break;
            case -1:
                {
                    miRecieveMemberType = 0;
                    ///提示连接失败
                    TimerManager.ClearTimerWithPrefix("CheckServerListen");

//                     ///提示连接失败
//                     SystemWaitPanel.End();
//                     string strHeader = TextManager.Instance.GetString("UI00031");
//                     PromptType pt = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);
//                     pt.layer = 3;
//                     pt.title = strHeader;
//                     pt.content = TextManager.Instance.GetUIString("UI17999");
//                     pt.style = PromptType.Style.OK;
//                     GUIManager.ShowView<SystemPrompt>(pt);
                }
                break;
            case -2:
                {
                    miRecieveMemberType = 0;
                    ///提示连接失败
                    TimerManager.ClearTimerWithPrefix("CheckServerListen");

//                     ///提示连接失败
//                     SystemWaitPanel.End();
//                     string strHeader = TextManager.Instance.GetString("UI00031");
//                     PromptType pt = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);
//                     pt.layer = 3;
//                     pt.title = strHeader;
//                     pt.content = TextManager.Instance.GetUIString("UI17999");
//                     pt.style = PromptType.Style.OK;
//                     GUIManager.ShowView<SystemPrompt>(pt);
                }
                break;
        }
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="strServerIP">服务器地址</param>
    /// <param name="iPort">端口</param>
    /// <param name="strUserName">账号名</param>
    /// <param name="strPassword">密码</param>
    /// <returns>连接成败</returns>
    public static bool ConnectTo(string strServerIP, int iPort, string strUserName, string strPassword, string strValidatestring = "", int iReconnect = 2)
    {
        NGUIDebug.Log("ConnectTo::", strUserName, ":", strPassword, ":", strValidatestring);
        if (!string.IsNullOrEmpty(strServerIP))
        {
            TimerManager.AddTimerRepeat("CheckServerListen", 0.5f, CheckConnectToMember);
            mstrMemberUserName = strUserName;
            mstrMemberPassword = strPassword;
            mstrMemberValidatestring = strValidatestring;
            miMemberReconnect = iReconnect;

            TextSock textSock = new TextSock(OnTextSockConnected, OnTextSockConnectFailed, OnTextSockRecieveData);
            if (textSock != null)
            {
                return textSock.Connect(strServerIP, iPort);
            }
        }
        else
        {
            miRecieveMemberType = 0;
            ///提示连接失败
            TimerManager.ClearTimerWithPrefix("CheckServerListen");

//             ///提示连接失败
//             SystemWaitPanel.End();
//             string strHeader = TextManager.Instance.GetString("UI00031");
//             PromptType pt = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);
//             pt.layer = 3;
//             pt.title = strHeader;
//             pt.content = TextManager.Instance.GetUIString("UI17999");
//             pt.style = PromptType.Style.OK;
//             GUIManager.ShowView<SystemPrompt>(pt);
        }

        return false;
    }

    public static bool ConnecToMember(string strServerIP, int iPort, string strUserName, string strPassword, string strValidatestring = "", int iReconnect = 2)
    {
        Game game = Game.Instance;
        if (game == null || game.mGameSock == null)
        {
            LogSystem.Log("Error!! ConnectState::ConnectTo (game or game.mGameSock) is null");
            return false;
        }

        UserLoginClear();

        if (game.mGameSock.Connect(strServerIP, iPort))
        {
            Fm_ClientNet.fxVerify.SetLoginInfo(strServerIP, iPort);
            mstrServer = strServerIP;
            miPort = iPort;
            mstrUser = strUserName;
            mstrPsd = strPassword;
            mstrValidateString = strValidatestring;
            mbLoginReconnect = iReconnect > 0;

            return true;
        }

        return false;
    }

    /* @brief:系统启动注册事件
        @return void
  */
    public static void RegistCallback(bool bClear = true)
    {
        ConnectStage.Clear();

        IGameSock iSock = Game.Instance.mGameSock;
        if (iSock != null)
        {
            //连服务器成功后回调
            iSock.RegistCallBack("on_connected", on_connected);
            //连服务器失败后回调
            iSock.RegistCallBack("on_connect_fail", on_connect_fail);
            //网络阻塞
            iSock.RegistCallBack("on_connect_block", on_connect_block);
            //服务器主动关闭回调
            iSock.RegistCallBack("on_close", on_close);
        }
        else
        {
            LogSystem.Log("Error!! ConnectState::RegistCallback iSock is null");
        }
    }

    /* @brief:连接上服务器回调
        @param: args 回调参数列表
       @return void
 */
    public static void on_connected(VarList args)
    {
 
        mbConnected = true;
    
        UserLogin();
    }
    public static void UserLoginClear()
    {
        mbRecvMsgVersion = false;
        mbConnected = false;
    }
    /// <summary>
    /// 用户登录
    /// </summary>
    public static void UserLogin()
    {
        if (!mbRecvMsgVersion || !mbConnected)
            return;

        if (string.IsNullOrEmpty(mstrUser))
            return;

        if (string.IsNullOrEmpty(mstrValidateString) && string.IsNullOrEmpty(mstrPsd))
            return;

        Game game = Game.Instance;
        if (game == null || game.mGameSender == null)
        {
            LogSystem.Log("Error!! ConnectState::UserLogin (game or game.mGameSender) is null");
            return;
        }

        string strMacAddress = GlobalData.mDeviceMac;
        if (string.IsNullOrEmpty(strMacAddress))
        {
            strMacAddress = "1234455677";
        }
        bool bSend = false;
        if (!mbLoginReconnect)
        {
            if (!string.IsNullOrEmpty(mstrValidateString))
            {
                bSend = game.mGameSender.LoginByString(mstrUser, mstrValidateString, strMacAddress);
            }
            else
            {
                bSend = game.mGameSender.Login(mstrUser, mstrPsd, strMacAddress);
            }

            if (!bSend)
            {
                mbConnected = false ;
                game.mbNetInit = false;
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(mstrValidateString) && !string.IsNullOrEmpty(mstrRoleName))
            {
                bSend = game.mGameSender.LoginReconnect(mstrUser, mstrPsd, mstrValidateString, strMacAddress, false, mstrRoleName, GetLastRoleInfo());
                if (bSend)
                {
                    mbLoginReconnect = false;
                }
                else
                {
                    mbConnected = false;
                }
            }
            else
            {
                bSend = game.mGameSender.Login(mstrUser, mstrPsd, strMacAddress);
                if (!bSend)
                {
                    game.mbNetInit = false;
                    mbConnected = false;
                }
            }
        }
    }

    /* @brief:连接服务器失败回调
       @param: args 回调参数列表
      @return void
*/
    public static void on_connect_fail(VarList args)
    {
       
        mbConnected = false;
        LogSystem.LogWarning("on_connect_fail");
        if (mbNeedReConnect)
        {
            gConnectCount++;
            LogSystem.LogWarning("on_connect_fail : " + gConnectCount);
            if (gConnectCount > 4)
            {
                mbNeedReConnect = false;
                TimerManager.Destroy("OnNetBlockTimer");
                ///显示网络已经断线
                string strHeader = TextManager.Instance.GetString("UI00031");
                string strContext = TextManager.Instance.GetString("NetClose");
                if (!string.IsNullOrEmpty(strHeader) && !string.IsNullOrEmpty(strContext))
                {
//                     if (GUIManager.HasView<SystemPrompt>())
//                     {
//                         GUIManager.HideView<SystemPrompt>("SystemPrompt");
//                     }
//                     PromptType pt = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);
//                     pt.layer = 5;
//                     pt.title = strHeader;
//                     pt.content = strContext;
//                     pt.style = PromptType.Style.OK;
//                     pt.callback_ok = OnConnectSecondTime;
//                     GUIManager.ShowView<SystemPrompt>(pt);
                }
            }
            return;
        }
        else if (mbSecondTimeConnect)
        {
            //连接失败，启动游戏
            GameReLogin();
        }
        else
        {
            //此方法是在登录界面登录时回调
            //弹出对话框
            string strHeader = TextManager.Instance.GetString("UI00031");
            string strContext = TextManager.Instance.GetString("ConnectFail");
            if (!string.IsNullOrEmpty(strHeader) && !string.IsNullOrEmpty(strContext))
            {
//                 PromptType pt = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);
//                 pt.layer = 5;
//                 pt.title = strHeader;
//                 pt.content = strContext;
//                 pt.style = PromptType.Style.OK;
//                 GUIManager.ShowView<SystemPrompt>(pt);
            }
        }
    }
    public static bool mbSecondTimeConnect = false;
    public static void OnConnectSecondTime()
    {
        mbNeedReConnect = false;
        mbSecondTimeConnect = false;
        
        GameReLogin();
        ////启动断线重连,重新发送登录消息
        //Game game = Game.Instance;
        //if (game != null)
        //{
        //    //game.mGameSock.Disconnect();
        //    game.mGameSock.InitUserSock();

        //    ConnectStage.RegistCallback(false);
        //    LoginStage.RegistCallback(false);
        //    WorldStage.RegistCallback(false);
        //    ///第一次连接失败，添加心跳开始持续一分钟连接12次，依然没有连上认为断开
        //    bool bCon = ConnectStage.ConnecToMember(mstrServer, miPort, mstrUser, mstrPsd, mstrValidateString, 2);
        //    if (!bCon)
        //    {
               
        //    }
        //}
    }
    /// <summary>
    /// 网络阻色
    /// </summary>
    /// <param name="args"></param>
    public static void on_connect_block(VarList args)
    {
        try
        {
            Game game = Game.Instance;
            if (game == null)
            {
                NGUIDebug.Log("on_connect_block: game is null");
                return;
            }
            if (!game.mbEnterGame)
            {
                NGUIDebug.Log("不在游戏中不进行断线重连");
                return;
            }
            //测试用，看看是否是因为这里面没有断线重连
            if (!mbConnected || isChangeRole)
            {
                NGUIDebug.Log("----------------- mbConnected =" + mbConnected);
                NGUIDebug.Log("----------------- isChangeRole =" + isChangeRole);
                //return;
            }

            ///被顶下线不重连
            if (LoginStage.miErrorCode == 21006)
            {
                NGUIDebug.Log("被顶下线不重连");
                return;
            }
            if (mbTryingConnect)
            {
                NGUIDebug.Log("尝试重连中。这里不重连");
                return;
            }

            mbTryingConnect = true;

            if (mbLoginSuccess && ObjectManager.mRole != null)
            {
                mbNeedReConnect = true;
                if (game.mGameSock == null)
                {
                    NGUIDebug.Log("Error!! ConnectState::on_connect_fail (game or game.mGameSock) is null");
                    LogSystem.LogError("Error!! ConnectState::on_connect_fail (game or game.mGameSock) is null");
                    return;
                }

                //网络标志连接中

                if (!game.mbPaused)
                {
                    OnNetBlockOrClose();
                }
            }
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError("Game::on_connect_block:", ex.ToString());
        } 


        
    }
    static string GetLastRoleInfo()
    {
        string mstrRoleInfo = string.Empty;

        if (ObjectManager.mRole != null)
        {
            float x = 0, y = 0, z = 0, orient = 0;

            int iRegionID = GameSceneManager.mintRegionID;
            UnityEngine.Transform transform = ObjectManager.mRole.mGameObject.transform;
            
            if (transform != null)
            {
                x = transform.position.x;
                y = transform.position.y;
                z = transform.position.z;
                //orient = ObjectManager.mRole.Rotation.eulerAngles.y / UnityEngine.Mathf.Rad2Deg;
            }
            else
            {
                CRoleObject roleObject = ObjectManager.mRole;
                x = roleObject.mGameSceneObj.GetPosiX();
                y = roleObject.mGameSceneObj.GetPosiY();
                z = roleObject.mGameSceneObj.GetPosiZ();
                orient = roleObject.mGameSceneObj.GetOrient();
            }

            mstrRoleInfo = iRegionID + "," + x + "," + y + "," + z + "," + orient;
        }

        return mstrRoleInfo;
    }
    /* @brief:网络中断回调
       @param: args 回调参数列表
      @return void
    */
    public static void on_close(VarList args)
    {
        NGUIDebug.Log("on_close" + mbConnected + " " + isChangeRole);
        if (!mbConnected || isChangeRole)
            return;

        ///被顶下线不重连
        if (LoginStage.miErrorCode == 21006)
        {
            return;
        }

        if (mbTryingConnect)
            return;

        mbTryingConnect = true;

        NGUIDebug.Log("on_close " + mbLoginSuccess);
        if (mbLoginSuccess && ObjectManager.mRole != null)
        {
            mbNeedReConnect = true;
            Game game = Game.Instance;
            if (game == null || game.mGameSock == null)
            {
                LogSystem.Log("Error!! ConnectState::on_connect_fail (game or game.mGameSock) is null");
                return;
            }
            NGUIDebug.Log("on_close game.mbPaused" + game.mbPaused);
            if (!game.mbPaused)
            {
                OnNetBlockOrClose();
            }
        }
        else if (mbLoginSuccess)
        {
            ///显示网络已经断线,普通情况
            string strHeader = TextManager.Instance.GetString("UI00031");
            string strContext = TextManager.Instance.GetString("NetClose");
            if (!string.IsNullOrEmpty(strHeader) && !string.IsNullOrEmpty(strContext))
            {
//                 if (GUIManager.HasView<SystemPrompt>())
//                 {
//                     GUIManager.HideView<SystemPrompt>("SystemPrompt");
//                 }
//                 PromptType pt = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);
//                 pt.layer = 5;
//                 pt.title = strHeader;
//                 pt.content = strContext;
//                 pt.style = PromptType.Style.OK;
//                 pt.callback_ok = OnClosePromptOk;
//                 GUIManager.ShowView<SystemPrompt>(pt);
            }
        }
    }
    static void GameReLogin()
    {
        mbConnected = false ;
        UnityEngine.GameObject oGameEntry = new UnityEngine.GameObject("GameEntry");
        if (oGameEntry != null)
        {
//             GameEntry gameEntry = oGameEntry.AddComponent<GameEntry>();
//             if (gameEntry != null)
//             {
//                 ///重启游戏
//                 gameEntry.ReRun(mstrServer, miPort, mstrUser, mstrPsd, mstrValidateString, ObjectManager.mRole != null ? 2 : 1);
//             }
        }
    }

    static int gConnectCount = 0;
    static void OnNetBlockOrClose()
    {
        try
        {
            NGUIDebug.Log("OnNetBlockOrClose");
            gConnectCount = 0;
            WorldStage.mbReConnected = true;
            //启动断线重连,重新发送登录消息
            Game game = Game.Instance;
            if (game != null)
            {
                game.mGameSock.Disconnect();
                game.mGameSock.InitUserSock();

                ConnectStage.RegistCallback(false);
                LoginStage.RegistCallback(false);
                WorldStage.RegistCallback(false);
                ///第一次连接失败，添加心跳开始持续一分钟连接12次，依然没有连上认为断开
                ConnectStage.ConnecToMember(mstrServer, miPort, mstrUser, mstrPsd, mstrValidateString, 2);
                NGUIDebug.Log("第一次重连");
                TimerManager.AddTimerRepeat("OnNetBlockTimer", 5.0f, OnNetBlockTimer);
                LogSystem.LogWarning("First block connect try");
                //SystemWaitPanel.Start(99999999);
            }
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError("Game::OnNetBlockOrClose:", ex.ToString());
        }
    }
    static void OnNetBlockTimer()
    {
        //启动断线重连,重新发送登录消息
        Game game = Game.Instance;
        if (game != null)
        {
            game.mGameSock.Disconnect();
            game.mGameSock.InitUserSock();

            ConnectStage.RegistCallback(false);
            LoginStage.RegistCallback(false);
            WorldStage.RegistCallback(false);
            NGUIDebug.Log("第二次重连");
            ConnectStage.ConnecToMember(mstrServer, miPort, mstrUser, mstrPsd, mstrValidateString, 2);
            
            gConnectCount++;
            LogSystem.LogWarning(gConnectCount+": block connect try");
            if (gConnectCount > 4)
            {
                mbNeedReConnect = false;
                TimerManager.Destroy("OnNetBlockTimer");
                ///显示网络已经断线
                string strHeader = TextManager.Instance.GetString("UI00031");
                string strContext = TextManager.Instance.GetString("NetClose");
                if (!string.IsNullOrEmpty(strHeader) && !string.IsNullOrEmpty(strContext))
                {
//                     if (GUIManager.HasView<SystemPrompt>())
//                     {
//                         GUIManager.HideView<SystemPrompt>("SystemPrompt");
//                     }
//                     PromptType pt = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);
//                     pt.layer = 5;
//                     pt.title = strHeader;
//                     pt.content = strContext;
//                     pt.style = PromptType.Style.OK;
//                     pt.callback_ok = OnConnectSecondTime;
//                     GUIManager.ShowView<SystemPrompt>(pt);
                }
            }
        }
    }
}
