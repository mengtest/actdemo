
using System.Collections.Generic;
using SysUtils;
using System;

using Fm_ClientNet.Interface;
using System.Security.Cryptography;

namespace Fm_ClientNet
{
    public partial class GameReceiver : IGameReceiver
    {
        public delegate int on_event(ObjectID ent_ident, VarList args);

        private Dictionary<int, RoleData> mRoles = null;
        private Dictionary<int, PropData> mPropertyTable = null;
        private Dictionary<int, RecData> mRecordTable = null;
        private Dictionary<int, MenuData> mMenus = null;
        private Dictionary<string, CallBack> m_CallBacks = new Dictionary<string, CallBack>();


        private SocketRevMsgHandle m_RecvHandle = new SocketRevMsgHandle();
        private GameClient m_client = null;
        private uint mnBeatType = 0;
        private uint m_nBeatInterval = 10;
        private uint m_nCheckInterval = 15;

        public void SetBeatType(uint nBeatType)
        {
            mnBeatType = nBeatType;
        }
        public void SetBeatInterval(uint nBeatInterval)
        {
            m_nBeatInterval = nBeatInterval;
        }
        public void SetCheckInterval(uint nCheckInterval)
        {
            m_nCheckInterval = nCheckInterval;
        }

        public uint GetBeatType()
        {
            return mnBeatType;
        }
        public uint GetBeatInterval()
        {
            return m_nBeatInterval;
        }
        public uint GetCheckInterval()
        {
            return m_nCheckInterval;
        }
        private string m_strPropertyTableMd5 = string.Empty;
        private string m_strRecordTableMd5 = string.Empty;
        public void SetPropertyTableMd5(byte[] data)
        {
            MD5 md5 = MD5.Create();
            byte[] md5data = md5.ComputeHash(data);
            m_strPropertyTableMd5 = System.Text.Encoding.Default.GetString(md5data);
        }
        public string GetPropertyTableMd5()
        {
            return m_strPropertyTableMd5;
        }

        public void SetRecordTableMd5(byte[] data)
        {
            MD5 md5 = MD5.Create();
            byte[] md5data = md5.ComputeHash(data);
            m_strRecordTableMd5 = System.Text.Encoding.Default.GetString(md5data);
        }

        public string GetRecordTableMd5()
        {
            return m_strRecordTableMd5;
        }
        private int m_nMaxCustomArguments = 64;

        public GameReceiver()
        {
            mRoles = new Dictionary<int, RoleData>();
            mPropertyTable = new Dictionary<int, PropData>();
            mRecordTable = new Dictionary<int, RecData>();
            mMenus = new Dictionary<int, MenuData>();
            RecvPacket.SetGameReceiver(this);
        }

        public void SetGameClient(GameClient client)
        {
            m_client = client;
            RecvPacket.SetGameClient(client);
        }

        public void SetGameMsgHander(IGameMsgHander msgHandle)
        {
            RecvPacket.SetMsgHandle(msgHandle);
        }

        public IGameMsgHander GetGameMsgHandler()
        {
            return RecvPacket.GetMsgHandle();
        }

        // 自定义消息处理器对象
        public void SetCustomHandler(ICustomHandler msgHandle)
        {
            RecvPacket.SetCustomHandle(msgHandle);
        }

        public ICustomHandler GetCustomHandler()
        {
            return RecvPacket.GetCustomHandle();
        }

        // 自定义消息的最大参数数量
        public void SetMaxCustomArguments(int value)
        {
            m_nMaxCustomArguments = value;
        }

        public int GetMaxCustomArguments()
        {
            return m_nMaxCustomArguments;
        }

        public void ClearRoles()
        {
            if (mRoles != null)
            {
                mRoles.Clear();
            }
        }

        public void ClearPropertyTable()
        {
            if (mPropertyTable != null)
            {
                mPropertyTable.Clear();
            }
        }

        public void ClearRecordTable()
        {
            if (mRecordTable != null)
            {
                mRecordTable.Clear();
            }
        }

        public void AddRoleData(int index, ref RoleData roleData)
        {
            if (mRoles == null)
            {
                LogSystem.Log("GameReceiver.AddRoleData mRoles is null");
                return;
            }

            try
            {
                if (mRoles.ContainsKey(index))
                {
                    LogSystem.Log("GameReceiver.AddRoleData Has Same Index:", index);
                    return;
                }

                mRoles.Add(index, roleData);
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.AddRoleData Exception:", ex.ToString());
                return;
            }
        }

        public void AddPropData(int index, ref PropData propData)
        {
            if (mPropertyTable == null)
            {
                LogSystem.Log("GameReceiver.AddPropData mPropertyTable is null");
                return;
            }

            try
            {
                if (mPropertyTable.ContainsKey(index))
                {
                    LogSystem.Log("GameReceiver.AddPropData Has Same Index:", index);
                    return;
                }

                mPropertyTable.Add(index, propData);
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.AddPropData Exception:", ex.ToString());
                return;
            }
        }

        public void AddRecData(int index, ref RecData recData)
        {
            if (mRecordTable == null)
            {
                LogSystem.Log("GameReceiver.AddRecData mPropertyTable is null");
                return;
            }

            try
            {
                if (mRecordTable.ContainsKey(index))
                {
                    LogSystem.Log("GameReceiver.AddRecData Has Same Index:", index);
                    return;
                }

                mRecordTable.Add(index, recData);
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.AddRecData Exception:", ex.ToString());
                return;
            }
        }


        //回调事件注册
        public bool RegistCallBack(string funcName, CallBack callBack)
        {
            //
            if (string.IsNullOrEmpty(funcName) || (callBack == null))
            {
                return false;
            }

            try
            {
                /*if (m_CallBacks.ContainsKey(funcName))
                {
                    m_CallBacks.Remove(funcName);
                }*/

                m_CallBacks[funcName] = callBack;
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return false;
            }
            return true;
        }


        public bool RemoveCallBack(string funcName)
        {
            if (funcName == null || funcName.Length == 0)
            {
                return false;
            }

            try
            {
                m_CallBacks.Remove(funcName);
            }
            catch (Exception ex)
            {
                LogSystem.LogError("AgentEx RemoveCallBack exception =[", ex.ToString(), "]");
                return false;
            }

            return true;
        }

        //回调处理
        public bool Excute_CallBack(string fun_name, VarList args)
        {
            try
            {
                CallBack call;
                if (m_CallBacks.TryGetValue(fun_name, out call))
                {
                    call(args);
                    return true;
                }
                else
                {
                    //LogSystem.LogError("can not find call_function " , fun_name);
                }
            }
            catch (Exception ex)
            {
                LogSystem.LogError("AgentEx Excute_CallBack exception =[", ex.ToString(), "]");
            }
            return false;
        }

        public RecData GetRecDataByIndex(int index)
        {
            try
            {
                if (!mRecordTable.ContainsKey(index))
                {
                    return null;
                }

                RecData rec = mRecordTable[index];
                return rec;
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.GetRecDataByIndex Exception:", ex.ToString());
                return null;
            }
        }

        public PropData GetPropDataByIndex(int index)
        {
            try
            {
                if (!mPropertyTable.ContainsKey(index))
                {
                    return PropData.zero;
                }

                PropData prop = mPropertyTable[index];
                return prop;
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.GetPropDataByIndex Exception:", ex.ToString());
                return PropData.zero;
            }
        }

        public int GetRoleCount()
        {
            if (mRoles == null)
            {
                return 0;
            }
            return mRoles.Count;
        }

        public int GetRoleInfoCount()
        {
            if (mRoles == null || mRoles.Count == 0)
            {
                return 0;
            }

            int nCount = 0;
            try
            {
                nCount = mRoles[0].paraList.GetCount();
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.GetRoleInfoCount Exception:", ex.ToString());
                return nCount;
            }
            return nCount;
        }

        public int GetRoleIndex(int index)
        {
            int nRet = 0;
            if (mRoles == null)
            {
                return 0;
            }

            if (index >= mRoles.Count)
            {
                return 0;
            }

            try
            {
                nRet = mRoles[index].RoleIndex;
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.GetRoleIndex Exception:", ex.ToString());
                return nRet;
            }
            return nRet;
        }

        public int GetRoleSysFlags(int index)
        {
            int nRet = 0;
            if (mRoles == null)
            {
                return 0;
            }

            if (index >= mRoles.Count)
            {
                return 0;
            }

            try
            {
                nRet = mRoles[index].SysFlags;
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.GetRoleSysFlags Exception:", ex.ToString());
                return nRet;
            }
            return nRet;
        }

        public string GetRoleName(int index)
        {
            if (mRoles == null)
            {
                return string.Empty;
            }

            if (index >= mRoles.Count)
            {
                return string.Empty;
            }

            try
            {
                return mRoles[index].Name;
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.GetRoleName Exception:", ex.ToString());
                return string.Empty;
            }
        }

        public string GetRolePara(int index)
        {
            string strRet = string.Empty;
            if (mRoles == null)
            {
                return strRet;
            }

            if (index >= mRoles.Count)
            {
                return strRet;
            }

            try
            {
                strRet = mRoles[index].Para;
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.GetRolePara Exception:", ex.ToString());
                return strRet;
            }
            return strRet;
        }

        public int GetRoleDeleted(int index)
        {
            int nRet = 0;
            if (mRoles == null)
            {
                return 0;
            }

            if (index >= mRoles.Count)
            {
                return 0;
            }

            try
            {
                nRet = mRoles[index].Deleted;
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.GetRoleDeleted Exception:", ex.ToString());
                return nRet;
            }
            return nRet;
        }

        public double GetRoleDeleteTime(int index)
        {
            double dRet = 0.0;
            if (mRoles == null)
            {
                return dRet;
            }

            if (index >= mRoles.Count)
            {
                return dRet;
            }

            try
            {
                dRet = mRoles[index].DeleteTime;
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.GetRoleDeleteTime Exception:", ex.ToString());
                return dRet;
            }
            return dRet;
        }

        public void GetRoleInfo(ref VarList args, ref VarList ret)
        {
            try
            {
                if (mRoles == null || mRoles.Count == 0)
                {
                    return;
                }

                if (args.GetCount() == 0)
                {
                    return;
                }

                if (args.GetType(0) != VarType.Int)
                {
                    return;
                }

                int nIndex = args.GetInt(0);
                if (nIndex >= mRoles.Count)
                {
                    return;
                }

                VarList paraList = mRoles[nIndex].paraList;
                for (int i = 0; i < paraList.GetCount(); i++)
                {
                    switch (paraList.GetType(i))
                    {
                        case VarType.Bool:
                            {
                                ret.AddBool(paraList.GetBool(i));
                            }
                            break;
                        case VarType.Int:
                            {
                                ret.AddInt(paraList.GetInt(i));
                            }
                            break;
                        case VarType.Int64:
                            {
                                ret.AddInt64(paraList.GetInt64(i));
                            }
                            break;
                        case VarType.Float:
                            {
                                ret.AddFloat(paraList.GetFloat(i));
                            }
                            break;
                        case VarType.Double:
                            {
                                ret.AddDouble(paraList.GetDouble(i));
                            }
                            break;
                        case VarType.String:
                            {
                                ret.AddString(paraList.GetString(i));
                            }
                            break;
                        case VarType.WideStr:
                            {
                                ret.AddWideStr(paraList.GetWideStr(i));
                            }
                            break;
                        case VarType.Object:
                            {
                                ret.AddObject(paraList.GetObject(i));
                            }
                            break;
                        default:
                            return;
                    }
                }

            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.GetRoleInfo Exception:", ex.ToString());
            }
            return;
        }

        public string GerPropertyName(int index)
        {
            if (index < 0 || index >= mPropertyTable.Count)
            {
                return string.Empty;
            }

            try
            {
                if (!mPropertyTable.ContainsKey(index))
                {
                    return string.Empty;
                }

                PropData propData = mPropertyTable[index];
                return propData.strName;
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.GerPropertyName Exception:", ex.ToString());
                return string.Empty;
            }
            return string.Empty;
        }

        private bool InnerParsePropValue(LoadArchive loadAr, int type, ref Var key)
        {
            switch (type)
            {
                case OuterDataType.OUTER_TYPE_BYTE:
                    {
                        int value = 0;
                        if (!loadAr.ReadInt8(ref value))
                        {
                            return false;
                        }
                        key.SetInt(value);
                    }
                    break;
                case OuterDataType.OUTER_TYPE_WORD:
                    {
                        int value = 0;
                        if (!loadAr.ReadInt16(ref value))
                        {
                            return false;
                        }
                        key.SetInt(value);
                    }
                    break;
                case OuterDataType.OUTER_TYPE_DWORD:
                    {
                        int value = 0;
                        if (!loadAr.ReadInt32(ref value))
                        {
                            return false;
                        }
                        key.SetInt(value);
                    }
                    break;
                case OuterDataType.OUTER_TYPE_QWORD:
                    {
                        long value = 0;
                        if (!loadAr.ReadInt64(ref value))
                        {
                            return false;
                        }
                        key.SetInt64(value);
                    }
                    break;
                case OuterDataType.OUTER_TYPE_FLOAT:
                    {
                        float value = 0.0f;
                        if (!loadAr.ReadFloat(ref value))
                        {
                            return false;
                        }
                        key.SetFloat(value);
                    }
                    break;
                case OuterDataType.OUTER_TYPE_DOUBLE:
                    {
                        double value = 0.0;
                        if (!loadAr.ReadDouble(ref value))
                        {
                            return false;
                        }
                        key.SetDouble(value);
                    }
                    break;
                case OuterDataType.OUTER_TYPE_STRING:
                    {
                        string value = string.Empty;
                        if (!loadAr.ReadString(ref value))
                        {
                            return false;
                        }
                        key.SetString(value);
                    }
                    break;
                case OuterDataType.OUTER_TYPE_WIDESTR:
                    {
                        string value = string.Empty;
                        if (!loadAr.ReadWideStr(ref value))
                        {
                            return false;
                        }
                        key.SetWideStr(value);
                    }
                    break;
                case OuterDataType.OUTER_TYPE_OBJECT:
                    {
                        ObjectID value = ObjectID.zero;

                        if (!loadAr.ReadObject(ref value))
                        {
                            return false;
                        }
                        key.SetObject(value);
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }

        private bool InnerRecvProperty(GameObj obj, LoadArchive loadAr, int count, bool change)
        {
            if (obj == null)
            {
                return false;
            }

            try
            {
                Var key = Var.zero;

                for (int i = 0; i < count; i++)
                {
                    int index = 0;
                    if (!loadAr.ReadInt16(ref index))
                    {
                        return false;
                    }

                    if (index >= mPropertyTable.Count)
                    {
                        return false;
                    }

                    PropData propData = GetPropDataByIndex(index);

                    propData.nCount = propData.nCount + 1;

                    if (!InnerParsePropValue(loadAr, propData.nType, ref key))
                    {
                        return false;
                    }

                    if (!obj.UpdateProperty(ref propData.strName, key))
                    {
                        return false;
                    }

                    if (change)
                    {
                        VarList argList = VarList.GetVarList();
                        argList.AddString(obj.GetIdent());
                        argList.AddString(propData.strName);
                        Excute_CallBack("on_object_property_change", argList);
                        argList.Collect();
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.Log("GameReceiver.InnerRecvViewProperty() Exception:", ex.ToString());
            }
            return true;
        }
        private bool InnerRecvViewProperty(string strViewIdent, GameObj obj, LoadArchive loadAr, int count, bool change)
        {
            try
            {
                Var key = Var.zero;

                for (int i = 0; i < count; i++)
                {
                    int index = 0;
                    if (!loadAr.ReadInt16(ref index))
                    {
                        return false;
                    }

                    if (index >= mPropertyTable.Count)
                    {
                        return false;
                    }

                    PropData propData = GetPropDataByIndex(index);
 

                    propData.nCount = propData.nCount + 1;

                    if (!InnerParsePropValue(loadAr, propData.nType, ref key))
                    {
                        return false;
                    }

                    if (!obj.UpdateProperty(ref propData.strName, key))
                    {
                        return false;
                    }

                    if (change)
                    {
                        VarList argList = VarList.GetVarList();
                        argList.AddString(strViewIdent);
                        argList.AddString(obj.GetIdent());
                        argList.AddString(propData.strName);
                        Excute_CallBack("on_view_object_property_change", argList);
                        argList.Collect();
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.Log("GameReceiver.InnerRecvViewProperty() Exception:", ex.ToString());
            }

            return true;
        }
        public bool RecvProperty(ref GameObj obj, ref LoadArchive loadAr,
            int count, bool change)
        {
            try
            {
                return InnerRecvProperty(obj, loadAr, count, change);
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.RecvProperty(GameObj) Exception:", ex.ToString());
                return false;
            }
        }

        public bool RecvProperty(ref GameView obj, ref LoadArchive loadAr,
            int count, bool change)
        {
            try
            {
                return InnerRecvProperty(obj, loadAr, count, change);
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.RecvProperty(GameView) Exception:", ex.ToString());
                return false;
            }
        }

        public bool RecvViewProperty(string strViewIdent, ref GameViewObj obj, ref LoadArchive loadAr,
            int count, bool change)
        {
            try
            {
                return InnerRecvViewProperty(strViewIdent, obj, loadAr, count, change);
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.RecvProperty(GameViewObj) Exception:", ex.ToString());
                return false;
            }
        }

        public bool RecvProperty(ref GameScene obj, ref LoadArchive loadAr,
            int count, bool change)
        {
            try
            {
                return InnerRecvProperty(obj, loadAr, count, change);
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.RecvProperty(GameScene) Exception:", ex.ToString());
                return false;
            }
        }

        public bool RecvProperty(ref GameSceneObj obj, ref LoadArchive loadAr,
            int count, bool change)
        {
            try
            {
                return InnerRecvProperty(obj, loadAr, count, change);
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameReceiver.RecvProperty Exception:", ex.ToString());
                return false;
            }
        }

        public string GetRecordName(int index)
        {
            try
            {
                if (index < 0 || index >= mRecordTable.Count)
                {
                    LogSystem.Log("index is not right");
                    return string.Empty;
                }
                return mRecordTable[index].strName;
            }
            catch (Exception ex)
            {
                LogSystem.Log("Exception:", ex.ToString());
            }
            return string.Empty;
        }

        public GameSceneObj GetSceneObj(string ident)
        {
            try
            {
                if (null == ident || ident.Length == 0)
                {
                    return null;
                }

                if (m_client == null)
                {
                    return null;
                }

                GameScene scene = (GameScene)m_client.GetCurrentScene();
                if (scene == null)
                {
                    return null;
                }

                GameSceneObj sceneObj = scene.GetSceneObjByIdent(ident);
                return sceneObj;
            }
            catch (Exception ex)
            {
                LogSystem.Log("Exception:", ex.ToString());
                return null;
            }
        }

        public void ClearMunus()
        {
            mMenus.Clear();
        }

        public int GetMenuCount()
        {
            return mMenus.Count;
        }

        public int GetMenuType(int index)
        {
            if (index < 0 || index > mMenus.Count)
            {
                return 0;
            }

            try
            {
                if (!mMenus.ContainsKey(index))
                {
                    return 0;
                }

                return mMenus[index].nType;

            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
                return 0;
            }
        }

        public int GetMenuMark(int index)
        {
            if (index < 0 || index > mMenus.Count)
            {
                return 0;
            }

            try
            {
                if (!mMenus.ContainsKey(index))
                {
                    return 0;
                }

                return mMenus[index].nMark;

            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
                return 0;
            }
        }

        public string GetMenuContent(int index)
        {
            if (index < 0 || index > mMenus.Count)
            {
                return string.Empty;
            }

            try
            {
                if (!mMenus.ContainsKey(index))
                {
                    return string.Empty;
                }

                return mMenus[index].info;

            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
                return string.Empty;
            }
        }

        public bool AddMenuData(int index, ref MenuData menuData)
        {
            if (index < 0)
            {
                return false;
            }

            try
            {
                if (mMenus.ContainsKey(index))
                {
                    return false;
                }

                mMenus.Add(index, menuData);

            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
                return false;
            }
            return true;
        }

        public bool ClearAll()
        {
            if (m_client != null)
            {
                m_client.ClearAll();
            }

            ClearPropertyTable();
            ClearRecordTable();
            ClearRoles();
            ClearMunus();
            return true;
        }

        // 消息处理
        public void ProcessMsg(byte[] data, int size)
        {
            m_RecvHandle.HandleMessage(data, size);
        }

        // 暂时没有实现
        public bool DumpMsgStat(string file_name)
        {
            return false;
        }
    }
}

