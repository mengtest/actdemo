using UnityEngine;
using SysUtils;
using Fm_ClientNet.Interface;
using System.Collections.Generic;

public class ServerCustom
{
    enum TIPSTYPE
    {
        //GM调试信息;
        TIPSTYPE_GMINFO_MESSAGE = 0,

        // 普通聊天信息;
        // 玩家聊天频道简略版，显示2条聊天信息，该处显示世界，国家，队伍，帮会，私聊频道及系统消息;
        // 单条信息最多保留3s,多条消息直接顶掉;
        TIPSTYPE_NORMAL_CHAT_MESSAGE = 1,

        // 大喇叭聊天;
        // 玩家付费聊天频道从右往左滚动，只显示一次，并且界面层级为最高级，不会被任何界面挡住;
        TIPSTYPE_TRUMPET_CHAT_MESSAGE = 2,

        // 事件公告;
        // 包括运营发布的消息，及一些玩家的在游戏中获得的特殊成就，比如:natsu获得了神器屠龙刀，xxx玩家将装备强化到了xx等事件;
        // 表现形式：从右往左滚动;
        TIPSTYPE_EVENT_BROAD_MESSAGE = 3,

        // 世界事件;
        // 主要显示一些国战信息，比如，xxxx击杀了xxxx国的国王，xxxx在xxxx连续击杀了20人，xxxx在（21,12）击杀了本国人民，国战开始等信息;
        // 直接显示主界面上，停留2s消失;
        TIPSTYPE_WORLD_EVENT_MESSAGE = 4,

        // 属性提示;
        // 主要显示游戏中获得属性：如装备装备后战力提升，获得金币、经验等等;
        // 表现效果：出现在屏幕中间，之后向上移动大概3个提示的高度，之后消失;
        TIPSTYPE_PROPERTY_PROMPT_MESSAGE = 5,

        // 系统功能提示;
        // 游戏中一些系统功能的提示，如添加好友提示，删除好友好友提示，组队/离队提示，帮会提示等;
        // 表现效果为直接显示，单条停留2s，多条直接顶掉;
        TIPSTYPE_SYSFUNCTION_PROMPT_MESSAGE = 6,

        // GMCC提示消息;
        TIPSTYPE_GMCC_MESSAGE = 7,

        /// <summary>
        /// 成就完成
        /// </summary>
        TIPSTYPE_ACHIEVE_MESSAGE = 8,

        /// <summary>
        /// 红包消息;
        /// </summary>
        TIPSTYPE_REDPACK_MESSAGE = 9,

        // 高亮提示(特殊事件公告)
        // 神装激活、国王产生等
        // 表现形式：从右往左滚动
        TIPSTYPE_SPEC_EVENT_BROAD_MESSAGE = 10,
        /// <summary>
        /// 二阶神装
        /// </summary>
        TIPSTYPE_SPEC_EVENT_BROAD2_MESSAGE = 11,

        /// <summary>
        /// 弹框式提示
        /// </summary>
        TIPSTYPE_SHOW_WINDOW_MESSAGE = 12,

        /// <summary>
        /// 特殊信息提示（滚动条+聊天面板显示）
        /// </summary>
        TIPSTYPE_EVENT_CHAT_MESSAGE = 13,

        /// <summary>
        /// 滚动三遍
        /// </summary>
        TIPSTYPE_EVENT_LOOP_MESSAGE = 14,
    }

    /*  @brief:获取选中对象的Ident
     *  @prama:customSys 自定义消息系统
     *  @return void
     */
    public static void RegistCallBack(bool bClear = true)
    {

    }

    /// <summary>
    /// 快速使用装备
    /// enum
    ///{
    ///	TOOL_BOX_SUBMSG_SHOW	 = 1,	// 展示新获得物品 int 数量 {string configid, string UniqueID, int 物品数量}
    ///	TOOL_BOX_SUBMSG_NEW_USE	 = 2,	// 新获得物品提示使用 string configid, string UniqueID, int 物品数量
    ///}
    /// </summary>
    /// <param name="args"></param>
    private static void on_fast_use_item(VarList args)
    {
        int p = 2;
        int type = args.GetInt(p++);
        switch (type)
        {
            //             case 1://展示新获得物品
            //                 {
            //                     if (GUIManager.HasView<NewItemsPrompt>())
            //                     {
            //                         GUIManager.DestroyView<NewItemsPrompt>();
            //                     }
            //                     GUIManager.ShowView<NewItemsPrompt>(args);
            //                 }
            //                 break;
            //             case 2: //新道具提醒
            //                 {
            //                     if (!GUIManager.HasView<FastUseItemPanel>())
            //                     {
            //                         GUIManager.ShowView<FastUseItemPanel>(args);
            //                     }
            //                     GUIManager.CallViewFunc<FastUseItemPanel>((IView view) =>
            //                     {
            //                         FastUseItemPanel pPanel = view as FastUseItemPanel;
            //                         if (pPanel != null)
            //                         {
            //                             pPanel.AddItems(args);
            //                         }
            //                     });
            //                 }
            //                 break;
        }

    }

    /// <summary>
    /// 服务器自动将玩家踢出游戏
    /// </summary>
    /// <param name="args"></param>
    public static void on_game_out(VarList args)
    {
        int p = 2;
        string content = args.GetString(p++);
        ConnectStage.isGameOut = true;
        //         if (!GUIManager.HasView<SystemPrompt>())
        //         {
        //             GUIManager.ShowView<SystemPrompt>();
        //         }
        //         GUIManager.CallViewFunc<SystemPrompt>((IView view) =>
        //         {
        //             SystemPrompt pPanel = view as SystemPrompt;
        //             if (pPanel != null)
        //             {
        //                 PromptType prompt = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);
        //                 prompt.layer = 3;
        //                 prompt.title = TextManager.GetUIString("UI30108");
        //                 prompt.content = TextManager.GetString(content);
        //                 prompt.style = PromptType.Style.OK;
        //                 prompt.callback_ok = OnSureGameOutClick;
        //                 pPanel.UpdateContent(prompt);
        //             }
        //         });
    }

    /// <summary>
    /// 确认退出游戏
    /// </summary>
    /// <param name="type"></param>
    private static void OnSureGameOutClick()
    {
        ConnectStage.isGameOut = false;
        //回到登陆界面
        LoginStage.GoBackLogin();
    }

    /// <summary>
    /// 系统购买提示
    /// </summary>
    /// <param name="args"></param>
    private static void on_system_buy_prompt(VarList args)
    {
        //         if (GUIManager.HasView<SystemShopPrompt>())
        //         {
        //             GUIManager.DestroyView<SystemShopPrompt>();
        //         }
        //         VarList arg2 = VarList.GetVarList();
        //         arg2.Copy(args);
        //         GUIManager.ShowView<SystemShopPrompt>(arg2);
    }

    private static void on_rank(VarList args)
    {
        //RankManager.on_rank(args);
    }

    //红包消息处理;
    private static void on_redPacketCallBack(VarList args)
    {
        //RedPackPanel.OnRecvRedPackMsg(args);
    }
    //世界boss
    private static void on_world_boss_callback(VarList args)
    {
        //WorldBossManager.RespondWorldBossListen(args);
    }

    //世界抽奖
    private static void on_world_award_callback(VarList args)
    {
        //WorldBossManager.RespondBossLotteryListen(args);
    }

    //断线重连
    private static void on_rest_net_callback(VarList args)
    {
        NGUIDebug.Log("收到连接消息，判断是否掉线");
        Game game = Game.Instance;
        if (game != null)
        {
            game.RestTime();
            //if (Time.realtimeSinceStartup - game.mfSendCheckTime < Game.mfIntervalTime)
            //{
            //    NGUIDebug.Log("收到连接消息，判断是否掉线 RestTime");
            //    game.RestTime();
            //}
        }
    }

    //喝酒相关消息;
    private static void on_drinkCallBack(VarList args)
    {
        //PartnerExecutor.OnRecvDrinkMsg(args);
    }

    // 换装操作执行后的服务器回调;
    private static void on_switchSuitCallBack(VarList args)
    {

    }
}