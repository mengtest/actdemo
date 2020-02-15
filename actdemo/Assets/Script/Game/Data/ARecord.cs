///作用：数据表格抽象类
///作者：zhangrj
///编写日期：20141225

public abstract class ARecord
{
    /// <summary>
    /// 表名
    /// </summary>
    protected string m_recordName;
    public string RecordName
    {
        get { return m_recordName; }
    }

    public ARecord(string recordName)
    {
        this.m_recordName = recordName;
    }

    public abstract void Clear();

    /// <summary>
    /// 抽象方法::删除调用
    /// </summary>
    public abstract void Destory();


    //可视数据表添加行入口
    public virtual void on_record_add_row(string strRecordName, int nRow, int nRows)
    {

    }
    //可视数据表删除行入口
    public virtual void on_record_remove_row(string strRecordName, int nRow)
    {

    }
    //可视数据表改变一行中的分列入口
    public virtual void on_record_single_grid(string strRecordName, int nRow, int nCol)
    {

    }
    //可视数据表删除入口
    public virtual void on_record_clear(string strRecordName)
    {

    }
}
