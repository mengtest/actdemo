///作用：数据表格基类
///作者：zhangrj
///编写日期：20141225
using System.Collections.Generic;
using Fm_ClientNet.Interface;
using Fm_ClientNet;

/// <summary>
/// 分发消息标识
/// </summary>
public enum RecordMode
{
    INIT,       //初始化
    ADD,        //添加
    REMOVE,     //移除
    UPDATE,     //更新
    CLEAR,      //清除
}

/// <summary>
/// 事件委托方法
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
/// <param name="mode">标识</param>
/// <param name="t">数值</param>
/// <param name="row">行号</param>
public delegate void DelegateFunction<T>(RecordMode mode, Record<T> record, T item, int row);

/// <summary>
/// 数据表基类
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class Record<T> : ARecord
{

    /// <summary>
    /// 表格数据
    /// </summary>
    protected List<T> m_data = new List<T>();
    public List<T> Data
    {
        get
        {
            return m_data;
        }
    }

    /// <summary>
    /// 表数据是否挂人物身上
    /// </summary>
    protected bool role = true;

    /// <summary>
    /// 回调
    /// </summary>
    //protected DelegateFunction<T> m_callback;
    protected List<DelegateFunction<T>> m_callbacks = new List<DelegateFunction<T>>();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="recordName">表名</param>
    /// <param name="result">true.表绑在人上，false.表绑在场景上</param>
    public Record(string recordName, bool result = true)
        : base(recordName)
    {
        role = result;
        RecordSystem.RegistRecordDataCallBack(recordName, this);
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <returns></returns>
    public int QueryData()
    {
        m_data.Clear();

        Game game = Game.Instance;

        if (game == null || game.mGameClient == null) return 0;

        IGameObj obj = null;
        if (role)
        {
            obj = game.mGameClient.GetCurrentPlayer();
        }
        else
        {
            obj = GameSceneManager.mScene;

        }
        if (obj == null) return 0;

        GameRecord gRecord = obj.GetGameRecordByName(m_recordName);

        if (gRecord == null) return 0;

        int rows = gRecord.GetRowCount();

        on_record_add_row(m_recordName, 0, rows);

        return rows;
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="callback"></param>
    public void RegistCallback(DelegateFunction<T> callback)
    {
        if (!m_callbacks.Contains(callback))
        {
            m_callbacks.Add(callback);
        }
    }

    /// <summary>
    /// 取消注册
    /// </summary>
    /// <param name="callback"></param>
    public void UnRegistCallback(DelegateFunction<T> callback)
    {
        if (m_callbacks.Contains(callback))
        {
            m_callbacks.Remove(callback);
        }
    }

    /// <summary>
    /// 清空缓存数据  在切换场景使用，如果是完全释放，请调用Destory()
    /// </summary>
    public override void Clear()
    {
        m_data.Clear();
    }

    /// <summary>
    /// 移除
    /// </summary>
    public override void Destory()
    {
        m_callbacks.Clear();
        m_data.Clear();

        RecordSystem.UnRegistRecordDataCallBack(m_recordName);
    }

    /// <summary>
    /// 可视数据表添加行入口
    /// </summary>
    /// <param name="strRecordName">表名</param>
    /// <param name="nRow">添加的起始行</param>
    /// <param name="nRows">添加的行数</param>
    public override void on_record_add_row(string strRecordName, int nRow, int nRows)
    {
        Game game = Game.Instance;
        if (game == null || game.mGameClient == null) return;
        IGameObj record = null;
        if (role)
        {
            record = game.mGameClient.GetCurrentPlayer();
        }
        else
        {
            record = GameSceneManager.mScene;

        }
        if (record == null) return;
        int rows = record.GetRecordRows(m_recordName);
        for (int i = 0; i < nRows; i++)
        {
            int iRow = nRow + i;

            if (iRow < rows)
            {
                T item = System.Activator.CreateInstance<T>();
                ReadItemData(iRow, item, record);
                m_data.Add(item);
                SendCallFunc(RecordMode.ADD, this, item, iRow);
            }
        }
    }

    /// <summary>
    /// 可视数据表删除行入口
    /// </summary>
    /// <param name="strRecordName">表名</param>
    /// <param name="nRow">行号</param>
    public override void on_record_remove_row(string strRecordName, int nRow)
    {
        Game game = Game.Instance;
        if (game == null || game.mGameClient == null) return;
        IGameObj record = null;
        if (role)
        {
            record = game.mGameClient.GetCurrentPlayer();
        }
        else
        {
            record = GameSceneManager.mScene;
        }
        if (record == null) return;
        int iRows = record.GetRecordRows(strRecordName);
        if (nRow >= 0 && nRow <= iRows)
        {
            int len = m_data.Count;
            if (nRow >= len)
            {
                LogSystem.LogError("on_record_remove_row：", strRecordName, " Data.Count is ", len, " but Server send RemoveAt ", nRow);
            }
            else
            {
                T item = m_data[nRow];
                m_data.RemoveAt(nRow);
                SendCallFunc(RecordMode.REMOVE, this, item, nRow);
            }
        }
    }


    /// <summary>
    /// 可视数据表改变一行中的分列入口
    /// </summary>
    /// <param name="strRecordName">表名</param>
    /// <param name="nRow">行号</param>
    /// <param name="nCol">列号</param>
    public override void on_record_single_grid(string strRecordName, int nRow, int nCol)
    {
        Game game = Game.Instance;
        if (game == null || game.mGameClient == null) return;
        IGameObj record = null;
        if (role)
        {
            record = game.mGameClient.GetCurrentPlayer();
        }
        else
        {
            record = GameSceneManager.mScene;

        }
        if (record == null) return;
        int iRows = record.GetRecordRows(m_recordName);

        if (nRow >= 0 && nRow <= iRows)
        {
            int len = m_data.Count;
            if (nRow >= len)
            {
                LogSystem.LogError("on_record_single_grid：", strRecordName, " Data.Count is ", len, " but Server send Grid ", nRow);
            }
            else
            {
                ReadItemData(nRow, m_data[nRow], record);
                SendCallFunc(RecordMode.UPDATE, this, m_data[nRow], nRow);
            }
        }
    }

    /// <summary>
    /// 可视数据表删除入口
    /// </summary>
    /// <param name="strRecordName">表名</param>
    public override void on_record_clear(string strRecordName)
    {
        m_data.Clear();

        SendCallFunc(RecordMode.CLEAR, this, default(T), -1);
    }

    /// <summary>
    /// 数据读取模板，子类具体实现
    /// </summary>
    /// <param name="row">要读取的行号</param>
    /// <param name="t">接收的变量</param>
    /// <param name="record">数据表</param>
    protected virtual void ReadItemData(int row, T item, IGameObj record) { }

    /// <summary>
    /// 发送回调
    /// </summary>
    protected void SendCallFunc(RecordMode mode, Record<T> record, T item, int row)
    {
        if (m_callbacks != null)
        {
            for (int i = 0; i < m_callbacks.Count; i++)
            {
                if (m_callbacks[i] != null)
                {
                    m_callbacks[i](mode, record, item, row);
                }
            }
        }
    }
}
