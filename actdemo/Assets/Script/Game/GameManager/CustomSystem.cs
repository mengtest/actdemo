using Fm_ClientNet.Interface;
using SysUtils;
using System.Collections.Generic;


public class CustomSystem
{
    //自定义消息单件
    public delegate void OnCustom(VarList args);
    static Dictionary<int, List<OnCustom>> mCustoms = new Dictionary<int, List<OnCustom>>();

    /// <summary>
    /// 清空
    /// </summary>
    public static void Clear()
    {
        mCustoms.Clear();
    }

    /* @brief: 启动注册自定义消息监听
         @param: iGameRecv 消息注册接口
         @return void
    */
    public static void RegistCallback(IGameReceiver iGameRecv, bool bClear = true)
    {
        if (bClear)
            mCustoms.Clear();

        iGameRecv.RegistCallBack("on_custom", CustomSystem.on_custom);
    }
    /* @brief: 接收服务器自定义消息，并按注册代理分发
         @param: args 服务器自定义消息参数列表
         @return void
    */
    public static void on_custom(VarList args)
    {
        int iCustomCmd = args.GetInt(1);

        if (mCustoms.ContainsKey(iCustomCmd))
        {
            List<OnCustom> list = mCustoms[iCustomCmd];
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        try
                        {
                            list[i](args);
                        }
                        catch (System.Exception e)
                        {
                            LogSystem.LogError(e.ToString());
                        }
                    }
                }
            }
        }
    }
    /* @brief: 逻辑功能注册自定义消息接口
       @param: args 服务器自定义消息参数列表
       @return void
     */
    public static void RegistCustomCallback(int iCustomCmd, OnCustom fnCallback)
    {
        if (mCustoms.ContainsKey(iCustomCmd))
        {
            if (!mCustoms[iCustomCmd].Contains(fnCallback))
            {
                mCustoms[iCustomCmd].Add(fnCallback);
            }
        }
        else
        {
            mCustoms[iCustomCmd] = new List<OnCustom>();
            mCustoms[iCustomCmd].Add(fnCallback);
        }
    }
    /* @brief: 逻辑功能取消注册自定义消息接口
         @param1: iCustomCmd 自定义消息编号
         @param2: callback  逻辑自定义消息回调接口
         @return void
       */
    public static void UnRegistCustomCallback(int iCustomCmd, OnCustom fnCallback)
    {
        if (mCustoms.ContainsKey(iCustomCmd))
        {
            if (mCustoms[iCustomCmd].Contains(fnCallback))
            {
                mCustoms[iCustomCmd].Remove(fnCallback);
                if (mCustoms[iCustomCmd].Count == 0)
                {
                    mCustoms.Remove(iCustomCmd);
                }
            }
        }
    }

}
