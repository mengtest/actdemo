using Fm_ClientNet.Interface;
using SysUtils;
using System.Collections.Generic;
/// <summary>
/// 视窗
/// </summary>
public class ViewSystem
{
    static Dictionary<string, List<IDataView>> mViews = new Dictionary<string, List<IDataView>>();

    /* @brief: 启动注册自定义消息监听
        @param: iGameRecv 消息注册接口
        @return void
   */
    public static void RegistCallback(IGameReceiver iGameRecv, bool bClear = true)
    {
        if (bClear)
            Clear();

        iGameRecv.RegistCallBack("on_create_view", on_create_view);
        iGameRecv.RegistCallBack("on_delete_view", on_delete_view);
        iGameRecv.RegistCallBack("on_view_property", on_view_property);
        iGameRecv.RegistCallBack("on_view_add", on_view_add);
        iGameRecv.RegistCallBack("on_view_remove", on_view_remove);
        iGameRecv.RegistCallBack("on_view_object_property", on_view_object_property);
        iGameRecv.RegistCallBack("on_view_object_property_change", on_view_object_property_change);
    }
    /// <summary>
    /// 清空
    /// </summary>
    public static void Clear()
    {
        mViews.Clear();
    }

    /* @brief: 添加视窗对象注册监听
        @param: strViewID 视窗号
       @param: view 视窗对象
       @return void
   */
    public static void RegistViewCallback(string strViewID, IDataView view)
    {
        if (mViews.ContainsKey(strViewID))
        {
            if (!mViews[strViewID].Contains(view))
            {
                mViews[strViewID].Add(view);
            }
        }
        else
        {
            mViews[strViewID] = new List<IDataView>();
            mViews[strViewID].Add(view);
        }
    }
    /* @brief: 取消视窗对象注册监听
      @param: strViewID 视窗号
     @param: view 视窗对象
     @return void
    */
    public static void UnRegistViewCallback(string strViewID, IDataView view)
    {
        if (mViews.ContainsKey(strViewID))
        {
            if (mViews[strViewID].Contains(view))
            {
                mViews[strViewID].Remove(view);
            }
        }
    }
    /* @brief: 视窗创建回调
        @param: args 回调参数
        @return void
    */
    public static void on_create_view(VarList args)
    {
        string strViewID = args.GetString(0);
        int capacity = args.GetInt(1);
        List<IDataView> iDataView;
        if (mViews.TryGetValue(strViewID, out iDataView) && iDataView != null)
        {
            for (int i = 0; i < iDataView.Count; i++)
            {
                if (iDataView[i] != null)
                {
                    try
                    {
                        iDataView[i].on_create_view(strViewID, capacity);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }
    /* @brief: 视窗删除回调
     @param: args 回调参数
     @return void
 */
    public static void on_delete_view(VarList args)
    {
        string strViewID = args.GetString(0);

        List<IDataView> iDataView;
        if (mViews.TryGetValue(strViewID, out iDataView) && iDataView != null)
        {
            for (int i = 0; i < iDataView.Count; i++)
            {
                if (iDataView[i] != null)
                {
                    try
                    {
                        iDataView[i].on_delete_view(strViewID);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }
    /* @brief: 视窗属性变化回调
    @param: args 回调参数
    @return void
*/
    public static void on_view_property(VarList args)
    {
        string strViewID = args.GetString(0);
        int count = args.GetInt(1);
        List<IDataView> iDataView;
        if (mViews.TryGetValue(strViewID, out iDataView) && iDataView != null)
        {
            for (int i = 0; i < iDataView.Count; i++)
            {
                if (iDataView[i] != null)
                {
                    try
                    {
                        iDataView[i].on_view_property(strViewID, count);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }
    /* @brief: 视窗对象添加回调
    @param: args 回调参数
    @return void
*/
    public static void on_view_add(VarList args)
    {
        string strViewID = args.GetString(0);
        string strObjID = args.GetString(1);

        List<IDataView> iDataView;
        if (mViews.TryGetValue(strViewID, out iDataView) && iDataView != null)
        {
            for (int i = 0; i < iDataView.Count; i++)
            {
                if (iDataView[i] != null)
                {
                    try
                    {
                        iDataView[i].on_view_add(strViewID, strObjID);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }
    /* @brief: 视窗对象删除回调
   @param: args 回调参数
   @return void
*/
    public static void on_view_remove(VarList args)
    {
        string strViewID = args.GetString(0);
        string strObjID = args.GetString(1);

        List<IDataView> iDataView;
        if (mViews.TryGetValue(strViewID, out iDataView) && iDataView != null)
        {
            for (int i = 0; i < iDataView.Count; i++)
            {
                if (iDataView[i] != null)
                {
                    try
                    {
                        iDataView[i].on_view_remove(strViewID, strObjID);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }
    /* @brief: 视窗对象属性添加回调
        @param: args 回调参数
        @return void
*/
    public static void on_view_object_property(VarList args)
    {
        string strViewID = args.GetString(0);
        string strObjID = args.GetString(1);
        int count = args.GetInt(2);
        List<IDataView> iDataView;
        if (mViews.TryGetValue(strViewID, out iDataView) && iDataView != null)
        {
            for (int i = 0; i < iDataView.Count; i++)
            {
                if (iDataView[i] != null)
                {
                    try
                    {
                        iDataView[i].on_view_object_property(strViewID, strObjID, count);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }
    /* @brief: 视窗对象属性变化回调
       @param: args 回调参数
       @return void
*/
    public static void on_view_object_property_change(VarList args)
    {
        string strViewID = args.GetString(0);
        string strObjID = args.GetString(1);
        string strName = args.GetString(2);

        List<IDataView> iDataView;
        if (mViews.TryGetValue(strViewID, out iDataView) && iDataView != null)
        {
            for (int i = 0; i < iDataView.Count; i++)
            {
                if (iDataView[i] != null)
                {
                    try
                    {
                        iDataView[i].on_view_object_property_change(strViewID, strObjID, strName);
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
