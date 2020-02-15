using System.Collections.Generic;
using Fm_ClientNet.Interface;
using SysUtils;
/// <summary>
/// 可视表
/// </summary>
public class RecordSystem
{
    static Dictionary<string, List<IRecord>> mRecords = new Dictionary<string, List<IRecord>>();
    static Dictionary<string, Dictionary<string, List<IRecord>>> mViewRecords = new Dictionary<string, Dictionary<string, List<IRecord>>>();
    static Dictionary<string, Dictionary<string, Dictionary<string, List<IRecord>>>> mViewObjRecords = new Dictionary<string, Dictionary<string, Dictionary<string, List<IRecord>>>>();
    static Dictionary<string, List<IRecord>> mSceneRecords = new Dictionary<string, List<IRecord>>();

    /// <summary>
    /// 数据类表格
    /// </summary>
    static Dictionary<string, ARecord> mDataRecords = new Dictionary<string, ARecord>();

    /// <summary>
    /// 清空
    /// </summary>
    public static void Clear()
    {
        mRecords.Clear();
        mViewRecords.Clear();
        mViewObjRecords.Clear();
        mSceneRecords.Clear();
        //UnInit();
    }


    /* @brief: 注册表数据消息监听
          @param: iGameRecv 消息注册接口
          @return void
     */
    public static void RegistCallback(IGameReceiver iGameRecv, bool bClear = true)
    {
        ///清空所有之前监听的对象
        if (bClear)
            Clear();

        Init();

        iGameRecv.RegistCallBack("on_record_table", on_record_table);
        ///主角可视表回调
        iGameRecv.RegistCallBack("on_record_add_row", on_record_add_row);
        iGameRecv.RegistCallBack("on_record_remove_row", on_record_remove_row);
        iGameRecv.RegistCallBack("on_record_grid", on_record_grid);
        iGameRecv.RegistCallBack("on_record_single_grid", on_record_single_grid);
        iGameRecv.RegistCallBack("on_record_clear", on_record_clear);

        ///视窗回调
        iGameRecv.RegistCallBack("on_view_record_add_row", on_view_record_add_row);
        iGameRecv.RegistCallBack("on_view_record_remove_row", on_view_record_remove_row);
        iGameRecv.RegistCallBack("on_view_record_grid", on_view_record_grid);
        iGameRecv.RegistCallBack("on_view_record_single_grid", on_view_record_single_grid);
        iGameRecv.RegistCallBack("on_view_record_clear", on_view_record_clear);

        ///视窗对象可视表回调
        iGameRecv.RegistCallBack("on_viewobj_record_add_row", on_viewobj_record_add_row);
        iGameRecv.RegistCallBack("on_viewobj_record_remove_row", on_viewobj_record_remove_row);
        iGameRecv.RegistCallBack("on_viewobj_record_grid", on_viewobj_record_grid);
        iGameRecv.RegistCallBack("on_viewobj_record_single_grid", on_viewobj_record_single_grid);
        iGameRecv.RegistCallBack("on_viewobj_record_clear", on_viewobj_record_clear);

        ///场景可视表回调
        iGameRecv.RegistCallBack("on_scene_record_add_row", on_scene_record_add_row);
        iGameRecv.RegistCallBack("on_scene_record_remove_row", on_scene_record_remove_row);
        iGameRecv.RegistCallBack("on_scene_record_grid", on_scene_record_grid);
        iGameRecv.RegistCallBack("on_scene_record_single_grid", on_scene_record_single_grid);
        iGameRecv.RegistCallBack("on_scene_record_clear", on_scene_record_clear);
    }

    /// <summary>
    /// 添加数据服务类监听
    /// </summary>
    /// <param name="strRecordName"></param>
    /// <param name="record"></param>
    public static void RegistRecordDataCallBack(string strRecordName, ARecord record)
    {
        mDataRecords[strRecordName] = record;
    }

    /// <summary>
    /// 移除数据服务类监听
    /// </summary>
    /// <param name="strRecordName"></param>
    /// <param name="record"></param>
    public static void UnRegistRecordDataCallBack(string strRecordName)
    {
        if (mDataRecords.ContainsKey(strRecordName))
        {
            mDataRecords.Remove(strRecordName);
        }
    }

    /* @brief: 注册主角表数据监听对象
         @param: record 监听对象
         @return void
    */
    public static void RegistRecordCallback(string strRecordName, IRecord record)
    {
        List<IRecord> iRecord;
        if (mRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            if (!iRecord.Contains(record))
            {
                iRecord.Add(record);
            }
        }
        else
        {
            iRecord = new List<IRecord>();
            iRecord.Add(record);
            mRecords[strRecordName] = iRecord;
        }
    }
    /* @brief:取消主角注册表数据监听对象
         @param: record 监听对象
         @return void
    */
    public static void UnRegistRecordCallback(string strRecordName, IRecord record)
    {
        List<IRecord> iRecord;
        if (mRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            if (iRecord.Contains(record))
            {
                iRecord.Remove(record);
            }
        }
    }
    /* @brief: 注册场景表数据监听对象
        @param: record 监听对象
        @return void
   */
    public static void RegistSceneRecordCallback(string strRecordName, IRecord record)
    {
        List<IRecord> iRecord;
        if (mSceneRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            if (!iRecord.Contains(record))
            {
                iRecord.Add(record);
            }
        }
        else
        {
            iRecord = new List<IRecord>();
            iRecord.Add(record);
            mSceneRecords[strRecordName] = iRecord;
        }
    }
    /* @brief:取消注册场景表数据监听对象
         @param: record 监听对象
         @return void
    */
    public static void UnRegistSceneRecordCallback(string strRecordName, IRecord record)
    {
        List<IRecord> iRecord;
        if (mSceneRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            if (iRecord.Contains(record))
            {
                iRecord.Remove(record);
            }
        }
    }
    /* @brief: 注册视窗表数据监听对象
        @param: record 监听对象
        @return void
   */
    public static void RegistViewRecordCallback(string strViewID, string strRecordName, IRecord record)
    {
        Dictionary<string, List<IRecord>> ViewDict = null;
        if (!mViewRecords.ContainsKey(strViewID))
        {
            ViewDict = new Dictionary<string, List<IRecord>>();
            mViewRecords[strViewID] = ViewDict;
        }
        else
        {
            ViewDict = mViewRecords[strViewID];
        }

        List<IRecord> records = null;
        if (!ViewDict.ContainsKey(strRecordName))
        {
            records = new List<IRecord>();
            ViewDict[strRecordName] = records;
        }
        else
        {
            records = ViewDict[strRecordName];
        }

        if (!records.Contains(record))
        {
            records.Add(record);
        }
    }
    /* @brief:取消视窗注册表数据监听对象
         @param: record 监听对象
         @return void
    */
    public static void UnRegistViewRecordCallback(string strViewID, string strRecordName, IRecord record)
    {
        Dictionary<string, List<IRecord>> dicRecord;
        if (mViewRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            List<IRecord> iRecord;
            if (dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
            {
                if (iRecord.Contains(record))
                {
                    iRecord.Remove(record);
                }
            }
        }
    }

    /* @brief: 注册视窗道具表数据监听对象
       @param: record 监听对象
       @return void
  */
    public static void RegistViewObjRecordCallback(string strViewID, string strItemIndex, string strRecordName, IRecord record)
    {
        Dictionary<string, Dictionary<string, List<IRecord>>> ViewDict = null;
        if (!mViewObjRecords.ContainsKey(strViewID))
        {
            ViewDict = new Dictionary<string, Dictionary<string, List<IRecord>>>();
            mViewObjRecords[strViewID] = ViewDict;
        }
        else
        {
            ViewDict = mViewObjRecords[strViewID];
        }

        Dictionary<string, List<IRecord>> ItemsDict = null;
        if (!ViewDict.ContainsKey(strItemIndex))
        {
            ItemsDict = new Dictionary<string, List<IRecord>>();
            ViewDict[strItemIndex] = ItemsDict;
        }
        else
        {
            ItemsDict = ViewDict[strItemIndex];
        }

        List<IRecord> records = null;
        if (!ItemsDict.ContainsKey(strRecordName))
        {
            records = new List<IRecord>();
            ItemsDict[strRecordName] = records;
        }
        else
        {
            records = ItemsDict[strRecordName];
        }

        if (!records.Contains(record))
        {
            records.Add(record);
        }
    }
    /* @brief:取消 注册视窗道具表数据监听对象
         @param: record 监听对象
         @return void
    */
    public static void UnRegistViewObjRecordCallback(string strViewID, string strItemIndex, string strRecordName, IRecord record)
    {
        Dictionary<string, Dictionary<string, List<IRecord>>> dicRecord;
        if (mViewObjRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            Dictionary<string, List<IRecord>> _dicRecord;
            if (dicRecord.TryGetValue(strItemIndex, out _dicRecord) && _dicRecord != null)
            {
                List<IRecord> iRecord;
                if (_dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
                {
                    if (iRecord.Contains(record))
                    {
                        iRecord.Remove(record);
                    }
                }
            }
        }
    }
    /* @brief:表结构信息消息入口点
         @param: args 该事件参数列表
         @return void
    */
    public static void on_record_table(VarList args)
    {
        //int nRecordCount = args.GetInt(2);
        //return;
    }
    /* @brief:表数据单行添加消息入口点
             @param: args 该事件参数列表
             @return void
    */
    public static void on_record_add_row(VarList args)
    {
        string strRecordName = args.GetString(1);
        int iRow = args.GetInt(2);
        int iRows = args.GetInt(3);
        List<IRecord> iRecord;
        if (mRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            for (int i = 0; i < iRecord.Count; i++)
            {
                if (iRecord[i] != null)
                {
                    try
                    {
                        iRecord[i].on_record_add_row(strRecordName, iRow, iRows);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_add_row(strRecordName, iRow, iRows);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据单行删除消息入口点
              @param: args 该事件参数列表
              @return void
    */
    public static void on_record_remove_row(VarList args)
    {
        string strRecordName = args.GetString(1);
        int iRow = args.GetInt(2);

        List<IRecord> iRecord;
        if (mRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            for (int i = 0; i < iRecord.Count; i++)
            {
                if (iRecord[i] != null)
                {
                    try
                    {
                        iRecord[i].on_record_remove_row(strRecordName, iRow);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_remove_row(strRecordName, iRow);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据单行数据改变消息入口点
            @param: args 该事件参数列表
            @return void
     */
    public static void on_record_grid(VarList args)
    {
        string strRecordName = args.GetString(1);
        int iCount = args.GetInt(2);

        List<IRecord> iRecord;
        if (mRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            for (int i = 0; i < iRecord.Count; i++)
            {
                if (iRecord[i] != null)
                {
                    try
                    {
                        iRecord[i].on_record_grid(strRecordName, iCount);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }
    /* @brief:表数据单行中列数据改变消息入口点
        @param: args 该事件参数列表
        @return void
   */
    public static void on_record_single_grid(VarList args)
    {
        string strRecordName = args.GetString(1);
        int iRow = args.GetInt(2);
        int iCol = args.GetInt(3);

        List<IRecord> iRecord;
        if (mRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            for (int i = 0; i < iRecord.Count; i++)
            {
                if (iRecord[i] != null)
                {
                    try
                    {
                        iRecord[i].on_record_single_grid(strRecordName, iRow, iCol);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_single_grid(strRecordName, iRow, iCol);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据清空消息入口点
        @param: args 该事件参数列表
        @return void
   */
    public static void on_record_clear(VarList args)
    {
        string strRecordName = args.GetString(1);

        List<IRecord> iRecord;
        if (mRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            for (int i = 0; i < iRecord.Count; i++)
            {
                if (iRecord[i] != null)
                {
                    try
                    {
                        iRecord[i].on_record_clear(strRecordName);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_clear(strRecordName);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }

    /* @brief:表数据单行添加消息入口点
             @param: args 该事件参数列表
             @return void
    */
    public static void on_view_record_add_row(VarList args)
    {
        string strViewID = args.GetString(0);
        string strRecordName = args.GetString(1);
        int iRow = args.GetInt(2);
        int iRows = args.GetInt(3);
        //LogSystem.Log(strViewID, strRecordName, iRow, iRows);
        if (!mViewRecords.ContainsKey(strViewID))
            return;

        Dictionary<string, List<IRecord>> dicRecord;
        if (mViewRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            List<IRecord> iRecord;
            if (dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
            {
                for (int i = 0; i < iRecord.Count; i++)
                {
                    if (iRecord[i] != null)
                    {
                        try
                        {
                            iRecord[i].on_record_add_row(strRecordName, iRow, iRows);
                        }
                        catch (System.Exception e)
                        {
                            LogSystem.LogError(e.ToString());
                        }
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_add_row(strRecordName, iRow, iRows);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据单行删除消息入口点
              @param: args 该事件参数列表
              @return void
    */
    public static void on_view_record_remove_row(VarList args)
    {
        string strViewID = args.GetString(0);
        string strRecordName = args.GetString(1);
        int iRow = args.GetInt(2);
        if (!mViewRecords.ContainsKey(strViewID))
            return;


        Dictionary<string, List<IRecord>> dicRecord;
        if (mViewRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            List<IRecord> iRecord;
            if (dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
            {
                for (int i = 0; i < iRecord.Count; i++)
                {
                    if (iRecord[i] != null)
                    {
                        try
                        {
                            iRecord[i].on_record_remove_row(strRecordName, iRow);
                        }
                        catch (System.Exception e)
                        {
                            LogSystem.LogError(e.ToString());
                        }
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_remove_row(strRecordName, iRow);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据单行数据改变消息入口点
            @param: args 该事件参数列表
            @return void
     */
    public static void on_view_record_grid(VarList args)
    {
        string strViewID = args.GetString(0);
        string strRecordName = args.GetString(1);
        int iCount = args.GetInt(2);
        LogSystem.Log(strViewID, strRecordName);
        if (!mViewRecords.ContainsKey(strViewID))
            return;

        Dictionary<string, List<IRecord>> dicRecord;
        if (mViewRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            List<IRecord> iRecord;
            if (dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
            {
                for (int i = 0; i < iRecord.Count; i++)
                {
                    if (iRecord[i] != null)
                    {
                        try
                        {
                            iRecord[i].on_record_grid(strRecordName, iCount);
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
    /* @brief:表数据单行中列数据改变消息入口点
        @param: args 该事件参数列表
        @return void
   */
    public static void on_view_record_single_grid(VarList args)
    {
        string strViewID = args.GetString(0);
        string strRecordName = args.GetString(1);
        LogSystem.Log(strViewID, strRecordName);
        int iRow = args.GetInt(2);
        int iCol = args.GetInt(3);

        if (!mViewRecords.ContainsKey(strViewID))
            return;


        Dictionary<string, List<IRecord>> dicRecord;
        if (mViewRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            List<IRecord> iRecord;
            if (dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
            {
                for (int i = 0; i < iRecord.Count; i++)
                {
                    if (iRecord[i] != null)
                    {
                        try
                        {
                            iRecord[i].on_record_single_grid(strRecordName, iRow, iCol);
                        }
                        catch (System.Exception e)
                        {
                            LogSystem.LogError(e.ToString());
                        }
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_single_grid(strRecordName, iRow, iCol);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据清空消息入口点
        @param: args 该事件参数列表
        @return void
   */
    public static void on_view_record_clear(VarList args)
    {
        string strViewID = args.GetString(0);
        string strRecordName = args.GetString(1);

        if (!mViewRecords.ContainsKey(strViewID))
            return;

        Dictionary<string, List<IRecord>> dicRecord;
        if (mViewRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            List<IRecord> iRecord;
            if (dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
            {
                for (int i = 0; i < iRecord.Count; i++)
                {
                    if (iRecord[i] != null)
                    {
                        try
                        {
                            iRecord[i].on_record_clear(strRecordName);
                        }
                        catch (System.Exception e)
                        {
                            LogSystem.LogError(e.ToString());
                        }
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_clear(strRecordName);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }

    /* @brief:表数据单行添加消息入口点
             @param: args 该事件参数列表
             @return void
    */
    public static void on_viewobj_record_add_row(VarList args)
    {
        string strViewID = args.GetString(0);
        string strViewIndex = args.GetString(1);
        string strRecordName = args.GetString(2);
        int iRow = args.GetInt(3);
        int iRows = args.GetInt(4);

        if (!mViewObjRecords.ContainsKey(strViewID))
            return;

        Dictionary<string, Dictionary<string, List<IRecord>>> dicRecord;
        if (mViewObjRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            Dictionary<string, List<IRecord>> _dicRecord;
            if (dicRecord.TryGetValue(strViewIndex, out _dicRecord) && _dicRecord != null)
            {
                List<IRecord> iRecord;
                if (_dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
                {
                    for (int i = 0; i < iRecord.Count; i++)
                    {
                        if (iRecord[i] != null)
                        {
                            try
                            {
                                iRecord[i].on_record_add_row(strRecordName, iRow, iRows);
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

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_add_row(strRecordName, iRow, iRows);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据单行删除消息入口点
              @param: args 该事件参数列表
              @return void
    */
    public static void on_viewobj_record_remove_row(VarList args)
    {
        string strViewID = args.GetString(0);
        string strViewIndex = args.GetString(1);
        string strRecordName = args.GetString(2);
        int iRow = args.GetInt(3);
        if (!mViewObjRecords.ContainsKey(strViewID))
            return;

        Dictionary<string, Dictionary<string, List<IRecord>>> dicRecord;
        if (mViewObjRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            Dictionary<string, List<IRecord>> _dicRecord;
            if (dicRecord.TryGetValue(strViewIndex, out _dicRecord) && _dicRecord != null)
            {
                List<IRecord> iRecord;
                if (_dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
                {
                    for (int i = 0; i < iRecord.Count; i++)
                    {
                        if (iRecord[i] != null)
                        {
                            try
                            {
                                iRecord[i].on_record_remove_row(strRecordName, iRow);
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

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_remove_row(strRecordName, iRow);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据单行数据改变消息入口点
            @param: args 该事件参数列表
            @return void
     */
    public static void on_viewobj_record_grid(VarList args)
    {
        string strViewID = args.GetString(0);
        string strViewIndex = args.GetString(1);
        string strRecordName = args.GetString(2);
        int iCount = args.GetInt(3);

        if (!mViewObjRecords.ContainsKey(strViewID))
            return;

        Dictionary<string, Dictionary<string, List<IRecord>>> dicRecord;
        if (mViewObjRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            Dictionary<string, List<IRecord>> _dicRecord;
            if (dicRecord.TryGetValue(strViewIndex, out _dicRecord) && _dicRecord != null)
            {
                List<IRecord> iRecord;
                if (_dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
                {
                    for (int i = 0; i < iRecord.Count; i++)
                    {
                        if (iRecord[i] != null)
                        {
                            try
                            {
                                iRecord[i].on_record_grid(strRecordName, iCount);
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
    }
    /* @brief:表数据单行中列数据改变消息入口点
        @param: args 该事件参数列表
        @return void
   */
    public static void on_viewobj_record_single_grid(VarList args)
    {
        string strViewID = args.GetString(0);
        string strViewIndex = args.GetString(1);
        string strRecordName = args.GetString(2);
        int iRow = args.GetInt(3);
        int iCol = args.GetInt(4);

        if (!mViewObjRecords.ContainsKey(strViewID))
            return;

        Dictionary<string, Dictionary<string, List<IRecord>>> dicRecord;
        if (mViewObjRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            Dictionary<string, List<IRecord>> _dicRecord;
            if (dicRecord.TryGetValue(strViewIndex, out _dicRecord) && _dicRecord != null)
            {
                List<IRecord> iRecord;
                if (_dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
                {
                    for (int i = 0; i < iRecord.Count; i++)
                    {
                        if (iRecord[i] != null)
                        {
                            try
                            {
                                iRecord[i].on_record_single_grid(strRecordName, iRow, iCol);
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

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_single_grid(strRecordName, iRow, iCol);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据清空消息入口点
        @param: args 该事件参数列表
        @return void
   */
    public static void on_viewobj_record_clear(VarList args)
    {
        string strViewID = args.GetString(0);
        string strViewIndex = args.GetString(1);
        string strRecordName = args.GetString(2);

        if (!mViewObjRecords.ContainsKey(strViewID))
            return;

        Dictionary<string, Dictionary<string, List<IRecord>>> dicRecord;
        if (mViewObjRecords.TryGetValue(strViewID, out dicRecord) && dicRecord != null)
        {
            Dictionary<string, List<IRecord>> _dicRecord;
            if (dicRecord.TryGetValue(strViewIndex, out _dicRecord) && _dicRecord != null)
            {
                List<IRecord> iRecord;
                if (_dicRecord.TryGetValue(strRecordName, out iRecord) && iRecord != null)
                {
                    for (int i = 0; i < iRecord.Count; i++)
                    {
                        if (iRecord[i] != null)
                        {
                            try
                            {
                                iRecord[i].on_record_clear(strRecordName);
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

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_clear(strRecordName);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }

    /* @brief:表数据单行添加消息入口点
             @param: args 该事件参数列表
             @return void
    */
    public static void on_scene_record_add_row(VarList args)
    {
        string strRecordName = args.GetString(0);
        int iRow = args.GetInt(1);
        int iRows = args.GetInt(2);

        List<IRecord> iRecord;
        if (mSceneRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            for (int i = 0; i < iRecord.Count; i++)
            {
                if (iRecord[i] != null)
                {
                    try
                    {
                        iRecord[i].on_record_add_row(strRecordName, iRow, iRows);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_add_row(strRecordName, iRow, iRows);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据单行删除消息入口点
              @param: args 该事件参数列表
              @return void
    */
    public static void on_scene_record_remove_row(VarList args)
    {
        string strRecordName = args.GetString(0);
        int iRow = args.GetInt(1);

        List<IRecord> iRecord;
        if (mSceneRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            for (int i = 0; i < iRecord.Count; i++)
            {
                if (iRecord[i] != null)
                {
                    try
                    {
                        iRecord[i].on_record_remove_row(strRecordName, iRow);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_remove_row(strRecordName, iRow);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据单行数据改变消息入口点
            @param: args 该事件参数列表
            @return void
     */
    public static void on_scene_record_grid(VarList args)
    {
        string strRecordName = args.GetString(0);
        int iCount = args.GetInt(1);

        List<IRecord> iRecord;
        if (mSceneRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            for (int i = 0; i < iRecord.Count; i++)
            {
                if (iRecord[i] != null)
                {
                    try
                    {
                        iRecord[i].on_record_grid(strRecordName, iCount);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }
    /* @brief:表数据单行中列数据改变消息入口点
        @param: args 该事件参数列表
        @return void
   */
    public static void on_scene_record_single_grid(VarList args)
    {
        string strRecordName = args.GetString(0);
        int iRow = args.GetInt(1);
        int iCol = args.GetInt(2);

        List<IRecord> iRecord;
        if (mSceneRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            for (int i = 0; i < iRecord.Count; i++)
            {
                if (iRecord[i] != null)
                {
                    try
                    {
                        iRecord[i].on_record_single_grid(strRecordName, iRow, iCol);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_single_grid(strRecordName, iRow, iCol);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }
    /* @brief:表数据清空消息入口点
        @param: args 该事件参数列表
        @return void
   */
    public static void on_scene_record_clear(VarList args)
    {
        string strRecordName = args.GetString(0);


        List<IRecord> iRecord;
        if (mSceneRecords.TryGetValue(strRecordName, out iRecord) && iRecord != null)
        {
            for (int i = 0; i < iRecord.Count; i++)
            {
                if (iRecord[i] != null)
                {
                    try
                    {
                        iRecord[i].on_record_clear(strRecordName);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }

        ///分发数据类信息
        ARecord aRecord;
        if (mDataRecords.TryGetValue(strRecordName, out aRecord) && aRecord != null)
        {
            try
            {
                aRecord.on_record_clear(strRecordName);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
    }

    /******************************表格数据类*******************************/
    ///example   public MoneyData data = new MoneyData();
    ///          void Init()
    ///          {
    ///                 mDataRecords.Add(data.RecordName,data);
    ///          }

    /// <summary>
    /// 钱币数据
    /// </summary>
    //public MoneyData moneyData = new MoneyData();

    ///// <summary>
    ///// 任务数据服务类
    ///// </summary>
    //public TaskRecord TaskData = new TaskRecord();

    ///// <summary>
    ///// 好友黑名单
    ///// </summary>
    //public BlackFriendRecord BlackFriend = new BlackFriendRecord();

    /// <summary>
    /// 数据类表格初始化
    /// </summary>
    static void Init()
    {
        //mDataRecords.Add(moneyData.RecordName, moneyData);
        //mDataRecords.Add(TaskData.RecordName, TaskData);
        //mDataRecords.Add(BlackFriend.RecordName, BlackFriend);
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    public static void UnInit()
    {
        if (mDataRecords != null)
        {
            foreach (ARecord record in mDataRecords.Values)
            {
                try
                {
                    record.Destory();
                }
                catch (System.Exception e)
                {
                    LogSystem.LogError(e.ToString());
                }
            }
        }
    }
}
