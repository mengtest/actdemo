using UnityEngine;
using Fm_ClientNet.Interface;
using SysUtils;

public class LoginStage
{
    public delegate void OnEvent(SysUtils.VarList args);

    /// <summary>
    /// 是否新创建角色
    /// </summary>
    public static bool isNewCreateRole = false;

    public static bool loginStatus = false;                      //是否登录成功/

    /* @brief:系统启动注册事件
        @return void
    */
    public static void RegistCallback(bool bClear = true)
    {
        IGameReceiver iGameRecv = Game.Instance.mGameRecv;
        if (iGameRecv == null)
        {
            LogSystem.Log("Error!! LoginStage::RegistCallback iGameRecv is null");
            return;
        }

        miErrorCode = 0;

        //登录账号后服务器爆满排队回调
        iGameRecv.RegistCallBack("on_queue", on_queue);

        //绑定计费串
        iGameRecv.RegistCallBack("on_charge_validstring", on_charger_validstring);

        //登录服务器失败，返回错误码回调
        iGameRecv.RegistCallBack("on_error_code", on_error_code);

        //登录成功，新建角色，删除角色, 恢复角色后回调, 如果是登录成功，不一定有，比如顶账号
        iGameRecv.RegistCallBack("on_login_succeed", on_login_succeed);

        ///收到验证消息
        iGameRecv.RegistCallBack("on_set_encode", on_set_encode);

        ///接收到服务器消息版本信息
        iGameRecv.RegistCallBack("on_server_msg_version", on_server_msg_version);
    }
    /// <summary>
    /// 响应接收到服务器消息版本信息
    /// </summary>
    /// <param name="args"></param>
    public static void on_server_msg_version(VarList args)
    {
        ConnectStage.mbRecvMsgVersion = true;
        ConnectStage.UserLogin();
    }
    /// <summary>
    /// 响应接收到服务器设置加密验证消息
    /// </summary>
    /// <param name="args"></param>
    public static void on_set_encode(VarList args)
    {
        ///通知服务器接收完成
        IGameSender iGameSender = Game.Instance.mGameSender;
        if (iGameSender == null)
        {
            LogSystem.Log("Error!! LoginStage::on_set_encode iGameSender is null");
            return;
        }
        iGameSender.ClientRetEncode();
    }
    /// <summary>
    /// 服务器发送计费串
    /// </summary>
    /// <param name="args">参数列表</param>
    public static void on_charger_validstring(VarList args)
    {
        string strValidateString = args.GetString(0);
        if (strValidateString == null)
        {
            LogSystem.Log("//Debug strValidateString is null");
        }
        else
        {
            //Game.Instance.SetValidateString(strValidateString);
        }
    }

    /* @brief:登录后排队回调
       @param: args 回调参数列表
      @return void
   */
    public static void on_queue(VarList args)
    {
//         SystemWaitPanel.End();
//         ///排队中，您的排位次序为：//在队列中的位置（为0 表示结束排队）
// 
//         int nPosition = args.GetInt(1);
// 
//         if (!GUIManager.HasView<QueuePrompt>())
//         {
//             GUIManager.ShowView<QueuePrompt>(nPosition);
//         }
//         else
//         {
//             QueuePrompt queuePrompt = GUIManager.GetView<QueuePrompt>("QueuePrompt");
//             if (queuePrompt != null)
//             {
//                 queuePrompt.SetInfo(nPosition);
//             }
//         }
    }

    public static int miErrorCode = 0;
    /* @brief:登录账号错误回调
       @param: args 回调参数列表
      @return void
   */
    public static void on_error_code(VarList args)
    {
        ConnectStage.mbConnected = false;
        ConnectStage.mbNeedReConnect = false;
        ConnectStage.mbSecondTimeConnect = false;
        loginStatus = false;
        if (TimerManager.IsHaveTimer("OnNetBlockTimer"))
        {
            TimerManager.Destroy("OnNetBlockTimer");
        }
        
        //SystemWaitPanel.End();
        UtilTools.PrintVarlist("on_error_code", args);
        int errorCode = args.GetInt(0);
        miErrorCode = errorCode;
        LogSystem.Log("LoginState ErrorCode::" , errorCode);

        if (ConnectStage.isGameOut)
        {
            //服务器自动踢出游戏后，不在更新提示框
            return;
        }

        PromptManager.Instance.ShowPromptUI(XmlManager.Instance.GetCommonText("Error" + errorCode));
    }

    /// <summary>
    /// 返回登录
    /// </summary>
    public static void GoBackLogin()
    {
        loginStatus = false;
        UnityEngine.GameObject oGameEntry = new UnityEngine.GameObject("GameEntry");
        if (oGameEntry != null)
        {
//             GameEntry gameEntry = oGameEntry.AddComponent<GameEntry>();
//             if (gameEntry != null)
//             {
//                 ///先关闭当前网络
//                 /// Game.Instance.UnInitNetWork();
//                 ///重启游戏
//                 gameEntry.ReRun(string.Empty, 0, string.Empty, string.Empty, string.Empty, 3);
//             }
        }
    }

    /* @brief:登录成功回调，返回角色列表
         @param: args 回调参数列表
        @return void
     */
    public static void on_login_succeed(VarList args)
    {
        ConnectStage.mbTryingConnect = false;
        WorldStage.mbReConnected = false;
        miErrorCode = 0;

        if (ConnectStage.isChangeRole)
        {
            Game.Instance.ReChooseRoleClear();
        }

        IGameReceiver iGameRecv = Game.Instance.mGameRecv;
        if (iGameRecv == null)
        {
            LogSystem.Log("Error!! LoginStage::on_login_succeed iGameRecv is null");
            return;
        }

        loginStatus = true;

        IGameSender iGameSender = Game.Instance.mGameSender;
        ///如果重连发来了角色列表，超过保护时间了，直接连吧
//         if( (ConnectStage.mbNeedReConnect || ConnectStage.mbSecondTimeConnect) && !string.IsNullOrEmpty(ConnectStage.mstrRoleName))
//         {
//             mstrAutoChooseName = ConnectStage.mstrRoleName;
//         }

        int roleCount = iGameRecv.GetRoleCount();

        //LoginUI ui = UIManager.Instance.GetUI<LoginUI>("LoginUI");
        //if (ui == null)
        //{
        //    return;
        //}

        //// 无角色，跳转创角界面
        //// 有角色，跳转选角界面
        //ui.SwitchUI(roleCount == 0 ? LoginUIState.LoginUIState_CreateRole : LoginUIState.LoginUIState_ChooseRole);
    }
}
