using Fm_ClientNet.Interface;
using SysUtils;
using System.Collections.Generic;
using UnityEngine;
using Fm_ClientNet;

/// <summary>
/// 对象管理器
/// </summary>
public class ObjectManager
{
    /// <summary>
    /// 场景中增删对象回调
    /// </summary>
    public static UserDelegate ObjAddOrRemoveChange = new UserDelegate();

    /// <summary>
    /// 对象添加等待队列
    /// </summary>
    static public Dictionary<string, IObject> mObjectsDic = new Dictionary<string, IObject>();

    /// <summary>
    /// 
    /// </summary>
    public static List<IObject> mObjectsList = new List<IObject>();

    /// <summary>
    /// 场景对象主角
    /// </summary>
    public static CRoleObject mRole = null;

    /// <summary>
    /// 场景加载　true 加载完成 false 正在加载
    /// </summary>
    public static bool bSceneExist = true;

    /// <summary>
    /// 设置数据对象
    /// </summary>
    /// <returns>数据对象</returns>
    public static IGameSceneObj GetRoleDataObj()
    {
        if (mRole != null)
        {
            return mRole.mGameSceneObj;
        }

        return null;
    }

    static private GameObject ObjPool = new GameObject("ObjectPool");
    static private int miPoolID = 0;

    public static void InitPoolManager()
    {
//         if (mSpawnPool != null)
//             return;
//         mSpawnPool = CachePoolManager.Pools.Create("ObjectPool" + miPoolID++);
//         mSpawnPool.group.parent = ObjPool.transform;
    }

    public static void ClearPoolManager()
    {
//         if (mSpawnPool != null)
//             GameObject.DestroyObject(mSpawnPool.gameObject);
//         // CachePoolManager.Pools.DestroyAll();
//         mSpawnPool = CachePoolManager.Pools.Create("ObjectPool" + miPoolID++);
//         mSpawnPool.group.parent = ObjPool.transform;
    }

    /// <summary>
    /// 获取主角的数据表
    /// </summary>
    /// <param name="strRecordName"></param>
    /// <returns></returns>
    public static GameRecord GetRoleRecord(string strRecordName)
    {
        if (mRole != null && mRole.mGameSceneObj != null)
        {
            return mRole.mGameSceneObj.GetGameRecordByName(strRecordName);
        }

        return null;
    }

    /* 
        @brief:异步添加一个到列表
         @param: strIdent ,对象Ident
         @param:o ,对象
         @return void
    */
    public static void AddObject(string strIdent, IObject o)
    {
        if (o == null)
            return;

        mObjectsDic[strIdent] = o;

//         if (o.mDynamicUnit != null)
//         {
//             mObjects2[o.miCreateID] = o;
//         }
//         ObjAddOrRemoveChange.ExecuteCalls(o, true);
    }

    /* 
        @brief:查找对象
        @param: strIdent ,对象Ident
        @return IObject 找到的对象
    */
    public static IObject FindObject(string strIdent)
    {
        IObject cObj;
        if (mObjectsDic.TryGetValue(strIdent, out cObj))
        {
            return cObj;
        }

        return null;
    }
    
    /// <summary>
    /// 启动前清空
    /// </summary>
    public static void Clear()
    {
        ///清空引用
        mRole = null;

        ObjAddOrRemoveChange.ClearCalls();

        // 对象添加等待队列
        mObjectsDic.Clear();
    }

    public static void ClearObjects()
    {
        ///清空引用
        foreach (IObject obj in mObjectsDic.Values)
        {
            obj.OnRemoveObjectBefore(true);
            obj.OnRemoveObject();
        }

        mRole = null;

        // 对象添加等待队列
        mObjectsDic.Clear();
    }

    public static void DestroyObject(IObject obj)
    {
        if (obj == null)
        {
            return;
        }

        GameObject.Destroy(obj.mGameObject);
        obj = null;
    }

    /* 
        @brief: 启动注册自定义消息监听
        @param: iGameRecv 消息注册接口
        @return void
    */
    public static void RegistCallback(IGameReceiver iGameRecv, bool bClear = true)
    {
        if (bClear)
            Clear();

        iGameRecv.RegistCallBack("on_property_table", on_property_table);
        iGameRecv.RegistCallBack("on_add_object", on_add_object);
        iGameRecv.RegistCallBack("on_object_property", on_object_property);
        iGameRecv.RegistCallBack("on_all_prop", on_all_prop);
        iGameRecv.RegistCallBack("on_object_property_change", on_object_property_change);
        iGameRecv.RegistCallBack("on_remove_object", on_remove_object);
        iGameRecv.RegistCallBack("on_before_remove_object", on_before_remove_object);

        //移动管理
        iGameRecv.RegistCallBack("on_moving", on_moving);
        iGameRecv.RegistCallBack("on_location", on_location);
        iGameRecv.RegistCallBack("on_all_dest", on_all_dest);
    }
    /* @brief: 格子系统移动位置
     @param: args 该事件相应参数列表
     @return void
   */
    public static void on_location_grid(VarList args)
    {
        string strIdent = args.GetString(0);
        IObject cObject = ObjectManager.FindObject(strIdent);
        if (cObject != null)
        {
            //cObject.on_location_grid();
            //取格子位置，通知对象移动到制定的格子
        }
    }
    /* @brief: 格子系统移动位置
     @param: args 该事件相应参数列表
     @return void
   */
    public static void on_moving_grid(VarList args)
    {
        string strIdent = args.GetString(0);
        IObject cObject = ObjectManager.FindObject(strIdent);
        if (cObject != null)
        {

        }
    }
    /* @brief: 格子系统移动位置
     @param: args 该事件相应参数列表
     @return void
   */
    public static void on_all_dest_grid(VarList args)
    {
        //占时不适用
    }

    /* @brief: 属性表信息事件入口点
        @param: args 该事件相应参数列表
        @return void
*/
    public static void on_property_table(VarList args)
    {
        //导入属性表，不需处理
    }

    /* @brief: 对象添加事件入口点
          @param: args 该事件相应参数列表
          @return void
    */
    public static void on_add_object(VarList args)
    {
        string strIdent = args.GetString(0);
        if (string.IsNullOrEmpty(strIdent))
        {
            return;
        }

        CallAddObject(strIdent);
    }

    public static void CallAddObject(string strIdent)
    {
        try
        {
            Game game = Game.Instance;
            if (game == null || game.mGameClient == null)
            {
                return;
            }

            IGameSceneObj sceneObj = game.mGameClient.GetSceneObj(strIdent);
            if (sceneObj == null)
            {
                return;
            }

            IObject cObj = null;
            int type = 0;
            if (!sceneObj.QueryPropInt("ObjectType", ref type))
            {
                LogSystem.Log("could not find type : ", strIdent);

                return;
            }

            string strModel = string.Empty;
            if (!sceneObj.QueryPropString("ResourcePath", ref strModel))
            {
                LogSystem.Log("could not find ResourcePath : ", strIdent);

                return;
            }

            ObjectType ObjType = (ObjectType)type;

            switch (ObjType)
            {
                case ObjectType.ObjectType_Player:
                    if (game.mGameClient.IsPlayer(strIdent))
                    {
                        cObj = new CRoleObject();
                        ObjectManager.mRole = cObj as CRoleObject;
                    }
                    else
                    {
                        cObj = new CPlayerObject();
                    }
                    break;

                case ObjectType.ObjectType_Npc:
                    cObj = new CNpcObject();
                    break;

                case ObjectType.ObjectType_Monster:
                    cObj = new CMonsterObject();
                    break;

                case ObjectType.ObjectType_Soldier:
                case ObjectType.ObjectType_Creeps:

                    break;

                case ObjectType.ObjectType_Born:
                    break;
            }

            if (cObj != null)
            {
                cObj.mObjectType = ObjType;
                cObj.mGameSceneObj = sceneObj;
                cObj.mObjectRes = strModel;
                cObj.mStrIdent = strIdent;
                cObj.OnAddObject();
                ObjectManager.AddObject(strIdent, cObj);
                if (cObj is CRoleObject)
                {
                    //GUIManager.CacheView<CharacterPanel>();
                    //GUIManager.CacheView<PlayerMainViewPanel>();
                    //GUIManager.CacheView<PlayerControlPanel>();
                    //GUIManager.CacheView<PlayerStatePanel>();
                    //GUIManager.CacheView<PlayerMapControlPanel>();
                    //GUIManager.CacheView<MainChatPanel>();

                    // 添加到主角的AOI中
                }
                else
                {
                    RangeTools.MotifyObjectAoi(mRole, cObj);
                }
            }
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError("on_add_object catch error", ex.ToString());
        }
    }

    /* 
        @brief: 对象属性添加事件入口点
        @param: args 该事件相应参数列表
        @return void
    */
    public static void on_object_property(VarList args)
    {
        string strIdent = args.GetString(0);
        IObject cobj;
        if (mObjectsDic.TryGetValue(strIdent, out cobj) && cobj != null)
        {
            cobj.OnObjectProperty();
        }
    }

    /*
        @brief: 对象属性添加事件入口点
        @param: args 该事件相应参数列表
        @return void
   */
    public static void on_all_prop(VarList args)
    {
        //int iCount = args.GetInt(0);

    }

    /* 
        @brief: 对象属性变化事件入口点
        @param: args 该事件相应参数列表
        @return void
    */
    public static void on_object_property_change(VarList args)
    {
        string strIdent = args.GetString(0);
        string strPropName = args.GetString(1);
        IObject cObj;
        if (mObjectsDic.TryGetValue(strIdent, out cObj) && cObj != null)
        {
            cObj.OnObjectPropertyChange(strPropName);
            //BindPropManager.CallBackProp(cObj, strPropName);
        }
    }

    /* 
        @brief: 对象删除消息事件入口点
        @param: args 该事件相应参数列表
        @return void
    */
    public static void on_remove_object(VarList args)
    {
        string strIdent = args.GetString(0);
        IObject obj = FindObject(strIdent);
        if (obj != null)
        {
            mObjectsDic.Remove(strIdent);
            //mObjects2.Remove(obj.miCreateID);
            obj.OnRemoveObject();
            //if( mTalkTaskNpc == obj )
            //{
            //    TimerManager.Destroy("TalkTask");
            //    mTalkTaskNpc = null;
            //}
            ObjAddOrRemoveChange.ExecuteCalls(obj, false);
        }
    }

    /* 
        @brief: 对象删除前消息事件入口点
        @param: args 该事件相应参数列表
        @return void
    */
    public static void on_before_remove_object(VarList args)
    {
        string strIdent = args.GetString(0);
        IObject cobj;
        if (mObjectsDic.TryGetValue(strIdent, out cobj) && cobj != null)
        {
            //BindPropManager.UnBindAllProp(strIdent);
//             if (cobj.mVisualTrans != null)
//             {
//                 mObjTrans.Remove(cobj.mVisualTrans);
//             }
//             cobj.OnRemoveObjectBefore(false);
        }
    }

    /* @brief: 响应服务器移动事件入口
        @param: args 该事件相应参数列表
        @return void
   */
    public static void on_moving(VarList args)
    {
        string strIdent = args.GetString(0);
        IObject cobj;
        if (mObjectsDic.TryGetValue(strIdent, out cobj) && cobj != null)
        {
            cobj.OnMoving();

            // 更新玩家AOI
            RangeTools.MotifyObjectAoi(mRole, cobj);
        }
        else
        {
            LogSystem.LogWarning("Recive on_moving , but ", strIdent, " you can't find in mObjects");
        }
    }

    /* @brief: 响应服务器同步位置事件入口
       @param: args 该事件相应参数列表
       @return void
  */
    public static void on_location(VarList args)
    {
        string strIdent = args.GetString(0);
        IObject cobj;
        if (mObjectsDic.TryGetValue(strIdent, out cobj) && cobj != null)
        {
            cobj.OnLocation();

            // 更新玩家AOI
            RangeTools.MotifyObjectAoi(mRole, cobj);
        }
    }

    /* @brief: 响应服务器同步所有对象位置事件入口
      @param: args 该事件相应参数列表
      @return void
   */
    public static void on_all_dest(VarList args)
    {

    }

    /// <summary>
    /// 移动所有玩家
    /// </summary>
    /// <returns></returns>
    public static bool RemovePlayerAll()
    {
        foreach (IObject cObj in mObjectsDic.Values)
        {
            if (cObj == mRole)
            {
                continue;
            }
                
            try
            {
                cObj.OnRemoveObjectBefore(true);
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
            try
            {

                cObj.OnRemoveObject();
            }
            catch (System.Exception e)
            {
                LogSystem.LogError(e.ToString());
            }
        }
        if (mRole != null)
        {
            mRole.OnRemoveObjectBefore(true);
            mRole.OnRemoveObject();
        }

        mObjectsDic.Clear();

        return true;
    }
}
