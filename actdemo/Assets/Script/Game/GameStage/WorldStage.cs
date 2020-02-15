using System;
using Fm_ClientNet.Interface;
using SysUtils;

public class WorldStage
{
    public static bool mbReConnected = false;
    /* @brief:系统启动注册事件
         @return void
    */
    public static void RegistCallback(bool bClear = true)
    {
        IGameReceiver iGameRecv = Game.Instance.mGameRecv;
        if (iGameRecv == null)
        {
            LogSystem.Log("Error!! WorldStage::RegistCallback iGameRecv is null");
            return;
        }

        GameSceneManager.RegistCallback(iGameRecv, bClear);
        ObjectManager.RegistCallback(iGameRecv, bClear);
        RecordSystem.RegistCallback(iGameRecv, bClear);
        CustomSystem.RegistCallback(iGameRecv, bClear);
        ViewSystem.RegistCallback(iGameRecv, bClear);

        TableSystem.RegistCallback();
        ServerCustom.RegistCallBack();
  
        iGameRecv.RegistCallBack("on_msg_tracert", on_msg_tracert);
        iGameRecv.RegistCallBack("on_terminate", on_terminate);
    }

    public static void on_msg_tracert(VarList args)
    {
        long fTimes = args.GetInt64(0);
        Game.Instance.mfDisplayTime_Ping = fTimes;
#if DEV
        int iCount = args.GetCount() - 1;
        string strInfo = string.Empty;
        int hoptypen = 0;
        long hopstampn = 0;
        for (int i = 1; i < iCount; i += 2)
        {
            hoptypen = args.GetInt(i);
            hopstampn = args.GetInt64(i + 1);
            strInfo += UtilTools.StringBuilder(hoptypen, "=", hopstampn, "\n");
        }
        Game.Instance.mstrServerNetInfo = strInfo;
#endif
        ///设置类型 + 时间戳为
        //LogSystem.Log("on_msg_tracert:",strInfo);
    }
    /* @brief:服务器强制停机
           @param: args 回调参数
           @return void
  */
    public static void on_terminate(VarList args)
    {

    }
    /// <summary>
    /// 帧更新处理
    /// </summary>
    public static void OnUpdate()
    {
//         if (GameSceneManager.scene == null)
//             return;
// 
//         CRoleObject RoleObject = ObjectManager.mRole;
//         if (RoleObject == null)
//             return;
// 
//         if (RoleObject.mDynamicUnit == null)
//             return;
// 
//         //如果CG动画处于播放状态、那么场景的更新交付给CGMovie来处理
//         if (CGManager.isMoviePlaying)
//             return;
// 
//         if (Instance.Get<SequenceManager>().isPlaying)
//             return;
// 
//         try
//         {
//             GameSceneManager.scene.UpdateView(RoleObject.mDynamicUnit.position);
//         }
//         catch (System.Exception ex)
//         {
//             LogSystem.LogError("GameScene->UpdateView catch error", ex.ToString());
// 
//         }
    }
}
