/// 作者 wanglc
/// 修改人 zhangrj
/// 修改日期 20141209
using SysUtils;
using System.Collections.Generic;
/// <summary>
/// 虚拟表单件
/// </summary>
public class TableSystem
{
    static Dictionary<string, List<ITable>> mRecords = new Dictionary<string, List<ITable>>();
    static Dictionary<string, int> mRecordCols = new Dictionary<string, int>();

    /// <summary>
    /// 清空
    /// </summary>
    public static void Clear()
    {
        //mRecordCols.Clear();
        mRecords.Clear();
    }
    /* @brief: 注册表数据消息监听
          @param: iGameRecv 消息注册接口
          @return void
     */
    public static void RegistCallback(bool bClear = true)
    {
        if (bClear)
            Clear();

        //注册所有表
        CreateTables();

        //注册表自定义消息入口
        CustomSystem.RegistCustomCallback(CallbackDefine.SERVER_CUSTOMMSG_VIRTUALREC_ADD, onTableAdd);
        CustomSystem.RegistCustomCallback(CallbackDefine.SERVER_CUSTOMMSG_VIRTUALREC_CLEAR, onTableClear);
        CustomSystem.RegistCustomCallback(CallbackDefine.SERVER_CUSTOMMSG_VIRTUALREC_UPDATE, onTableChangeSingle);
        CustomSystem.RegistCustomCallback(CallbackDefine.SERVER_CUSTOMMSG_VIRTUALREC_UPDATE_ROW, onTableChange);
        CustomSystem.RegistCustomCallback(CallbackDefine.SERVER_CUSTOMMSG_VIRTUALREC_REMOVE_ROW, onTableDelete);
    }

    /* @brief: 添加所有表
          @return void
     */
    public static void CreateTables()
    {
        AppendTables(TableDefine.GUILDLIST, 8);
    }

    /* @brief: 附加一张表信息
        @param: 表号 ，
        @param: 表列数 ，
        @return void
   */
    public static void AppendTables(string tableName, int cols)
    {
        mRecordCols[tableName] = cols;
    }

    /* @brief: 添加逻辑注册表数据监听对象
       @param:表号 ，
       @param:table ,监听对象
       @return void
  */
    public static void RegistTableCallback(string tableName, ITable table)
    {
        List<ITable> iTabel;
        if (mRecords.TryGetValue(tableName, out iTabel)
            && iTabel != null)
        {
            if (!iTabel.Contains(table))
            {
                iTabel.Add(table);
            }
        }
        else
        {
            iTabel = new List<ITable>();
            iTabel.Add(table);
            mRecords[tableName] = iTabel;
        }
    }

    /* @brief: 取消逻辑注册表数据监听对象
         @param:表号 ，
         @param:table ,监听对象
         @return void
   */
    public static void UnRegistTableCallback(string tableName, ITable table)
    {
        List<ITable> iTabel;
        if (mRecords.TryGetValue(tableName, out iTabel)
            && iTabel != null)
        {
            if (iTabel.Contains(table))
            {
                iTabel.Remove(table);
            }
        }
    }
    /* @brief: 表格创建时加载数据
         @param:回调参数列表
         @return void
   */
    public static void onTableAdd(VarList args)
    {
        int index = 2;
        string tableName = args.GetString(index++);
        List<ITable> iTabel;
        if (mRecords.TryGetValue(tableName, out iTabel)
            && iTabel != null
            && mRecordCols.ContainsKey(tableName))
        {
            int iCols = mRecordCols[tableName];
            for (int i = 0; i < iTabel.Count; i++)
            {
                if (iTabel[i] != null)
                {
                    try
                    {
                        iTabel[i].on_table_add(tableName, args, (args.GetCount() - index) / iCols, iCols, index);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }

    /* @brief: 分发表格更新指定内容消息
        @param:回调参数列表
        @return void
  */
    public static void onTableChangeSingle(VarList args)
    {
        int index = 2;
        string tableName = args.GetString(index++);
        int row = args.GetInt(index++);
        int col = args.GetInt(index++);

        List<ITable> iTabel;
        if (mRecords.TryGetValue(tableName, out iTabel)
            && iTabel != null
            && mRecordCols.ContainsKey(tableName))
        {
            int iCols = mRecordCols[tableName];
            for (int i = 0; i < iTabel.Count; i++)
            {
                if (iTabel[i] != null)
                {
                    try
                    {
                        iTabel[i].on_table_change_single(tableName, args, row, col, (args.GetCount() - index) / iCols, iCols, index);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }

    /* @brief: 分发表格更新指定内容消息
         @param:回调参数列表
         @return void
   */
    public static void onTableChange(VarList args)
    {
        int index = 2;
        string tableName = args.GetString(index++);
        int row = args.GetInt(index++);

        List<ITable> iTabel;
        if (mRecords.TryGetValue(tableName, out iTabel)
            && iTabel != null
            && mRecordCols.ContainsKey(tableName))
        {
            int iCols = mRecordCols[tableName];
            for (int i = 0; i < iTabel.Count; i++)
            {
                if (iTabel[i] != null)
                {
                    try
                    {
                        iTabel[i].on_table_change(tableName, args, row, (args.GetCount() - index) / iCols, iCols, index);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }

    /* @brief: 分发表格删除指定内容消息
         @param:回调参数列表
         @return void
    */
    public static void onTableDelete(VarList args)
    {
        int index = 2;
        string tableName = args.GetString(index++);
        int row = args.GetInt(index++);

        List<ITable> iTabel;
        if (mRecords.TryGetValue(tableName, out iTabel)
            && iTabel != null
            && mRecordCols.ContainsKey(tableName))
        {
            int iCols = mRecordCols[tableName];
            for (int i = 0; i < iTabel.Count; i++)
            {
                if (iTabel[i] != null)
                {
                    try
                    {
                        iTabel[i].on_table_delete(tableName, args, row, (args.GetCount() - index) / iCols, iCols, index);
                    }
                    catch (System.Exception e)
                    {
                        LogSystem.LogError(e.ToString());
                    }
                }
            }
        }
    }

    /* @brief: 分发表格清除消息
        @param:回调参数列表
        @return void
   */
    public static void onTableClear(VarList args)
    {
        int index = 2;
        string tableName = args.GetString(index++);

        List<ITable> iTabel;
        if (mRecords.TryGetValue(tableName, out iTabel)
            && iTabel != null
            && mRecordCols.ContainsKey(tableName))
        {
            for (int i = 0; i < iTabel.Count; i++)
            {
                if (iTabel[i] != null)
                {
                    try
                    {
                        iTabel[i].on_table_clear(tableName, args, index);
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

