using System.Collections.Generic;
using Fm_ClientNet.Interface;

/// <summary>
/// 数据解析基类
/// Author : YangDan
/// </summary>
public abstract class RecordBase : IRecord
{
    public delegate void DelegateFunction(RecordBase value);

    protected string m_recordName = string.Empty;
    public string RecordName { get { return m_recordName; } }

    protected List<object> m_data;
    public List<object> Data
    {
        get
        {
            return m_data;
        }
    }

    protected DelegateFunction m_callFunction;
    /// <summary>
    /// 清除数据
    /// </summary>
    public void Clear()
    {
        m_data.Clear();
    }
    /// <summary>
    /// 数据解析类构造方法
    /// </summary>
    /// <param name="name">数据表名称</param>
    /// <param name="callFunction">数据发送改变回调</param>

    public RecordBase(string name, DelegateFunction callFunction)
    {
        m_recordName = name;

        m_callFunction = callFunction;

        m_data = new List<object>();

        RecordSystem.RegistRecordCallback(name, this);

    }

    /// <summary>
    /// 读取数据
    /// </summary>

    public int QueryData()
    {
        m_data.Clear();
        IGameSceneObj roleobj = ObjectManager.GetRoleDataObj();
        if (roleobj == null)
            return 0;

        int rows = roleobj.GetRecordRows(m_recordName);

        for (int i = 0; i < rows; i++)
            on_record_add_row(m_recordName, i, 1);

        return rows;
    }

    /// <summary>
    /// 注销数据
    /// </summary>

    virtual public void Destroy()
    {
        RecordSystem.UnRegistRecordCallback(m_recordName, this);

        m_recordName = string.Empty;
        m_data.Clear();
        m_data = null;
        m_callFunction = null;
    }

    /// <summary>
    /// 可视数据表添加行入口
    /// </summary>
    /// <param name="args"></param>

    virtual public void on_record_add_row(string strRecordName, int nRow, int nRows)
    {
        IGameSceneObj roleObj = ObjectManager.GetRoleDataObj();
        if (roleObj == null)
            return;

        for (int i = 0; i < nRows; i++)
        {
            int iRow = nRow + i;
            int rows = roleObj.GetRecordRows(m_recordName);
            if (iRow < rows)
            {
                object data = GetDataItem();
                ReadItemData(iRow, data, roleObj);
                m_data.Add(data);
            }
        }

        if (m_callFunction != null)
        {
            m_callFunction(this);
        }
    }

    /// <summary>
    /// 可视数据表改变一行数据入口
    /// </summary>
    /// <param name="args"></param>

    virtual public void on_record_grid(string strRecordName, int nCount)
    {
        //int iRows = m_record.GetRecordRows(m_recordName);

        //if (nCount >= 0 && nCount <= iRows)
        //{

        //    ReadItemData(nCount, m_data[nCount]);

        //    if (m_callFunction != null)
        //        m_callFunction(this);
        //}
    }

    /// <summary>
    /// 可视数据表结构入口(一般在游戏刚进入场景时就处理过了)
    /// </summary>
    /// <param name="args"></param>

    virtual public void on_record_table(int nRecordCount)
    {
        LogSystem.Log("on_record_table");
    }

    /// <summary>
    /// 可视数据表删除行入口
    /// </summary>
    /// <param name="args"></param>

    virtual public void on_record_remove_row(string strRecordName, int nRow)
    {
        IGameSceneObj roleObj = ObjectManager.GetRoleDataObj();
        if (roleObj == null)
            return;

        int iRows = roleObj.GetRecordRows(strRecordName);

        if (nRow >= 0 && nRow <= iRows)
        {
            m_data.RemoveAt(nRow);
        }

        m_callFunction(this);
    }

    /// <summary>
    /// 可视数据表改变一行中的分列入口
    /// </summary>
    /// <param name="args"></param>

    virtual public void on_record_single_grid(string strRecordName, int nRow, int nCol)
    {
        IGameSceneObj roleObj = ObjectManager.GetRoleDataObj();
        if (roleObj == null)
            return;

        int iRows = roleObj.GetRecordRows(m_recordName);

        if (nRow >= 0 && nRow <= iRows)
        {
            ReadItemData(nRow, m_data[nRow], roleObj);

            if (m_callFunction != null)
                m_callFunction(this);
        }
    }

    /// <summary>
    /// 可视数据表删除入口
    /// </summary>
    /// <param name="args"></param>

    virtual public void on_record_clear(string strRecordName)
    {
        m_data.Clear();

        m_callFunction(this);
    }

    /// <summary>
    /// 获取 DataItem
    /// </summary>
    /// <returns></returns>
    virtual protected object GetDataItem() { return null; }

    /// <summary>
    /// 读取指定行数据
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    virtual protected void ReadItemData(int row, object item, IGameSceneObj record) { }
}