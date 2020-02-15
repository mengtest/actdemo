using UnityEngine;
using SysUtils;
using Fm_ClientNet.Interface;
using System.Collections.Generic;

/// <summary>
/// 主角控制类
/// </summary>
public class CRoleObject : IObject
{
    /// <summary>
    /// 主角的CO对象的Ident;
    /// </summary>
    public string mstrSelectIdent = string.Empty;

    /// <summary>
    /// 在线时长
    /// </summary>
    public static int mStayTime = 0;

    /// <summary>
    /// AccountName
    /// </summary>
    public string RoleName { get { return mRoleName; } }
    private string mRoleName = string.Empty;

    /// <summary>
    /// 是否为第一次加入游戏
    /// </summary>
    public bool isFirstInGame { get { return true; } }

    /// <summary>
    /// 主角是否Ready
    /// </summary>
    public bool IsRoleReady { get { return mRoleReady; } }
    private bool mRoleReady = false;

    private int mComboCount = 0;
    public int ComboCount
    {
        get
        {
            return mComboCount;
        }
    }

    public void IncComboCount()
    {
        ++mComboCount;
    }

    public void ClearComboCount()
    {
        mComboCount = 0;
    }

    /// <summary>
    /// 资源加载完成
    /// </summary>
    public bool bRoleResReady
    {
        private set;
        get;
    }

    #region <>抽象方法<>
    /// <summary>
    /// 增加对象
    /// </summary>
    override public void OnAddObject()
    {
        ObjectManager.mRole = this;

        base.OnAddObject();

        OnRoleReady();
    }

    /// <summary>
    /// 人物被添加或删除
    /// </summary>
    /// <param name="Cobj"></param>
    /// <param name="Operate"></param>
    void ObjAddOrRemoveChange(params object[] arge)
    {

    }

    /// <summary>
    /// 显示对象创建or删除
    /// </summary>
    /// <param name="flag"></param>
    public override void CreateState(bool flag)
    {
        base.CreateState(flag);
    }

    /// <summary>
    /// 对象属性接入
    /// </summary>
    override public void OnObjectProperty()
    {
        base.OnObjectProperty();
    }

    public override void OnHorse()
    {
        base.OnHorse();
    }

    public override void DownHorse()
    {
        base.DownHorse();
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    public override void StopMove()
    {
        base.StopMove();
    }

    public override void OnActive()
    {
        base.OnActive();
    }

    /// <summary>
    /// 对象单个属性变化
    /// </summary>
    /// <param name="strPropName">变化的属性名</param>
    override public void OnObjectPropertyChange(string strPropName)
    {
        if (mGameSceneObj == null)
        {
            return;
        }

        base.OnObjectPropertyChange(strPropName);
    }

    /// <summary>
    /// 升级
    /// </summary>
    public override void Upgrade()
    {
        base.Upgrade();
    }

    /// <summary>
    /// 对象删除前回调
    /// </summary>
    override public void OnRemoveObjectBefore(bool bImmediatelyDelete = false)
    {
        base.OnRemoveObjectBefore(bImmediatelyDelete);
    }

    /// <summary>
    /// 对象删除回调
    /// </summary>
    override public void OnRemoveObject()
    {
        base.OnRemoveObject();
    }

    /// <summary>
    /// 主角移动处理
    /// </summary>
    /// <param name="flag"></param>
    public override void MoveState(bool flag)
    {
        base.MoveState(flag);
    }

    /// <summary>
    /// 移动处理
    /// </summary>
    override public void OnMoving()
    {
        base.OnMoving();
    }

    /// <summary>
    /// 收到对象位置信息
    /// </summary>
    override public void OnLocation()
    {
        if (mGameSceneObj == null)
        {
            return;
        }

        base.OnLocation();
    }

    #endregion

    void OnRoleReady()
    {
        GameCommand.ClientReady();
    }
}
