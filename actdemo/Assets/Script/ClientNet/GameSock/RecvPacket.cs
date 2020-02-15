
using SysUtils;
using System;
using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
    public class RecvPacket
    {
        private static GameReceiver m_receiver = null;
        private static IGameMsgHander m_gameMsgHandle = null;
        private static ICustomHandler m_cusMsgHandle = null;
        private static GameClient m_gameClient = null;
        private static VarList m_argList = VarList.GetVarList();

        public static void SetGameReceiver(GameReceiver receiver)
        {
            m_receiver = receiver;
        }

        public static void SetGameClient(GameClient client)
        {
            m_gameClient = client;
        }

        public static void SetMsgHandle(IGameMsgHander msgHandle)
        {
            m_gameMsgHandle = msgHandle;
        }

        public static IGameMsgHander GetMsgHandle()
        {
            return m_gameMsgHandle;
        }

        public static void SetCustomHandle(ICustomHandler customHandle)
        {
            m_cusMsgHandle = customHandle;
        }

        public static ICustomHandler GetCustomHandle()
        {
            return m_cusMsgHandle;
        }

        //SERVER_SET_ENCODE(3)
        static public void setEncode(GlobalServerMsg id, object args,int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, iLen);


            if (!loadAr.Seek(1))
            {
                return;
            }

            try
            {
                int nDynamicKey = 0;
                int nEncodeId = 0;
                int nAddress = 0;
                byte[] btinfo = null;//System.Text.Encoding.Default.GetBytes(strInfo);
                ///nCondition 设置进来
                byte[] btcond = null;//System.Text.Encoding.Default.GetBytes(strCondition);
                ///pData 设置进来
                //byte[] btdata = null;//System.Text.Encoding.Default.GetBytes(strData);
                byte[] btData = null;
                if (!loadAr.ReadUserDataNoLen(ref btinfo, 32))
                    return;
                if (!loadAr.ReadInt32(ref nDynamicKey))
                    return;
                if (!loadAr.ReadInt32(ref nEncodeId))
                    return;
                if (!loadAr.ReadInt32(ref nAddress))
                    return;
                if (!loadAr.ReadUserDataNoLen(ref btcond, 16))
                    return;
				UInt32 size = BitConverter.ToUInt32(loadAr.GetData(), loadAr.GetLength());

				if (!loadAr.ReadUserDataNoLen(ref btData,(int)size))
                    return;

                /// byte[] btData = System.Text.Encoding.Default.GetBytes(strData);
                fxVerify.SetEncode(btinfo, nDynamicKey, nEncodeId, nAddress, btcond, btData);

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnSetEncode();
                }
                else
                {
                    m_argList.Clear();
                    m_receiver.Excute_CallBack("on_set_encode", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.Log("RecvPacket.setEncode() Exception:" , ex.ToString());
            }

        }
        //SERVER_SET_VERIFY(1)
        static public void recvSetVerify(GlobalServerMsg id, object args,int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, iLen);


            if (!loadAr.Seek(1))
            {
                return;
            }
            try
            {

                int nWidth = 0;
                int nHeight = 0;
                if (!loadAr.ReadInt32(ref nWidth))
                {
                    return;
                }

                if (!loadAr.ReadInt32(ref nHeight))
                {
                    return;
                }


                if (nHeight == 1)
                {

                }
                else
                {

                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnSetVerify(nWidth, nHeight, string.Empty, null);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddInt(nWidth);
                    m_argList.AddInt(nHeight);
                    m_receiver.Excute_CallBack("on_set_verify", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.Log("RecvPacket.recvSetVerify() Exception:" , ex.ToString());
            }
        }

        //读取登录成功后玩家获取到的玩家角色信息
        static private bool readLoginRoleData(int nRoleNum, ref LoadArchive loadAr)
        {
            m_gameClient.ClearAll();
            for (int i = 0; i < nRoleNum; i++)
            {
                uint nArg_num = 0;
                RoleData roleData = new RoleData();
                if (!loadAr.ReadUInt16(ref nArg_num))
                {
                    return false;
                }

                for (uint index = 0; index < nArg_num; index++)
                {
                    int type = 0;
                    if (!loadAr.ReadInt8(ref type))
                    {
                        return false;
                    }

                    switch (type)
                    {
                        case VarType.Bool:
                            {
                                int value = 0;
                                if (!loadAr.ReadInt8(ref value))
                                {
                                    return false;
                                }

                                if (value > 0)
                                {
                                    roleData.paraList.AddBool(true);
                                }
                                else
                                {
                                    roleData.paraList.AddBool(false);
                                }

                            }
                            break;
                        case VarType.Int:
                            {
                                int value = 0;
                                if (!loadAr.ReadInt32(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddInt(value);
                            }
                            break;
                        case VarType.Int64:
                            {
                                long value = 0;
                                if (!loadAr.ReadInt64(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddInt64(value);
                            }
                            break;
                        case VarType.Float:
                            {
                                float value = 0.0f;
                                if (!loadAr.ReadFloat(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddFloat(value);
                            }
                            break;
                        case VarType.Double:
                            {
                                double value = 0.0f;
                                if (!loadAr.ReadDouble(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddDouble(value);
                            }
                            break;
                        case VarType.String:
                            {
                                string value = string.Empty;
                                if (!loadAr.ReadString(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddString(value);
                            }
                            break;
                        case VarType.WideStr:
                            {
                                string value = string.Empty;
                                if (!loadAr.ReadWideStr(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddWideStr(value);
                            }
                            break;
                        case VarType.Object:
                            {
                                ObjectID value = ObjectID.zero;
                                if (!loadAr.ReadObject(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddObject(value);
                            }
                            break;
                        default:
                            LogSystem.Log("readLoginRoleData role type error");
                            return false;
                    }
                }

                if (nArg_num >= 4)
                {
                    if (roleData.paraList.GetType(0) == VarType.Int)
                    {
                        roleData.RoleIndex = roleData.paraList.GetInt(0);
                    }

                    if (roleData.paraList.GetType(1) == VarType.Int)
                    {
                        roleData.SysFlags = roleData.paraList.GetInt(1);
                    }

                    if (roleData.paraList.GetType(2) == VarType.WideStr)
                    {
                        roleData.Name = roleData.paraList.GetWideStr(2);
                    }

                    if (roleData.paraList.GetType(3) == VarType.WideStr)
                    {
                        roleData.Para = roleData.paraList.GetWideStr(3);
                    }

                }

                if (nArg_num >= 6)
                {
                    if (roleData.paraList.GetType(4) == VarType.Int)
                    {
                        int nTest = roleData.paraList.GetInt(4);
                        roleData.SetDeleted(nTest);
                    }

                    if (roleData.paraList.GetType(5) == VarType.Double)
                    {
                        roleData.DeleteTime = roleData.paraList.GetDouble(5);
                    }
                }
                else
                {
                    roleData.Deleted = 0;
                    roleData.DeleteTime = 0.0;
                }
                m_receiver.AddRoleData(i, ref roleData);
            }

            return true;
        }

        //SERVER_LOGIN_SUCCEED(4) OK
        static public void recvLoginSuccess(GlobalServerMsg id, object args, int iLen)
        {
            m_receiver.ClearRoles();
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);


            //跳过8位的协议Id
            //9个int 分别是指如下9个整型字段
            int nIsFree = 0;		// 是否免费
            int nPoints = 0;		// 剩余点数
            int nYear = 0;			// 包月截止日期
            int nMonth = 0;
            int nDay = 0;
            int nHour = 0;
            int nMinute = 0;
            int nSecond = 0;
            int nDynamicKey = 0;	// 动态密匙

            int roleNum = 0;  //此账号的现在角色数
            if (!loadAr.Seek(1))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nIsFree))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nPoints))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nYear))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nMonth))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nDay))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nHour))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nMinute))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nSecond))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nDynamicKey))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref roleNum))
            {
                return;
            }

            if (!readLoginRoleData(roleNum, ref loadAr))
            {
                LogSystem.Log("RecvPacket recvLoginSuccess readLoginRoleData failed");
                return;
            }


            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnLoginSucceed(nIsFree, nPoints, nYear, nMonth, nDay, nHour, nMinute, nSecond, roleNum);
            }
            else
            {

                m_argList.Clear();
                if (0 == nIsFree)
                {
                    m_argList.AddBool(false);
                }
                else
                {
                    m_argList.AddBool(true);
                }

                m_argList.AddInt(nPoints);
                m_argList.AddInt(nYear);
                m_argList.AddInt(nMonth);
                m_argList.AddInt(nDay);
                m_argList.AddInt(nHour);
                m_argList.AddInt(nMinute);
                m_argList.AddInt(nSecond);
                m_argList.AddInt(roleNum);
                m_receiver.Excute_CallBack("on_login_succeed", m_argList);
            }

        }

        //SERVER_WORLD_INFO (5)
        static public void recvWorldInfo(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);


            if (!loadAr.Seek(1))
            {
                return;
            }

            int nInfoType = 0;
            if (!loadAr.ReadInt16(ref nInfoType))
            {
                return;
            }

            string info = string.Empty;
            if (!loadAr.ReadWideStrNoLen(ref info))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnWorldInfo(nInfoType, info);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nInfoType);
                m_argList.AddWideStr(info);
                m_receiver.Excute_CallBack("on_world_info", m_argList);
            }
        }

        //SERVER_ERROR_CODE (3) OK
        static public void recvErrorCode(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);

            int nErrorCode = 0;

            if (!loadAr.Seek(1))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nErrorCode))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnErrorCode(nErrorCode);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nErrorCode);
                m_receiver.Excute_CallBack("on_error_code", m_argList);
            }
        }

        //SERVER_ADD_OBJECT(13) OK
        static public void recvAddObj(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);


            loadAr.Seek(1);
            ObjectID objId = ObjectID.zero;

            if (!loadAr.ReadObject(ref objId))
            {
                LogSystem.Log("read object failed");
                return;
            }

            OuterPost outerPost = OuterPost.zero;

            if (!outerPost.initOuterPost(ref loadAr))
            {
                LogSystem.Log("initOuterPost failed");
                return;
            }

            OuterDest outerDest = OuterDest.zero;

            if (!outerDest.initOuterDest(ref loadAr))
            {
                LogSystem.Log("initOuterDest failed");
                return;
            }

            int nPropCount = 0;
            if (!loadAr.ReadInt16(ref nPropCount))
            {
                LogSystem.Log("read nPropCount failed");
                return;
            }

            string ident = Tools.GetIdent(objId);
            GameClient client = m_gameClient;
            if (null == client)
            {
                LogSystem.Log("client is null");
                return;
            }

            GameScene scene = (GameScene)client.GetCurrentScene();
            if (scene == null)
            {
                LogSystem.Log("current scene is null");
                return;
            }

            GameSceneObj sceneObj = ((GameScene)client.GetCurrentScene()).AddSceneObj(ident);
            if (sceneObj == null)
            {
                return;
            }
            sceneObj.SetObjId(objId);

            if (nPropCount > 0)
            {
                if (!m_receiver.RecvProperty(ref sceneObj, ref loadAr, nPropCount, false))
                {
                    LogSystem.Log("RecvProperty failed");
                    return;
                }
            }

            sceneObj.SetLocation(outerPost.x, outerPost.y, outerPost.z, outerPost.orient);
            sceneObj.SetDestination(outerDest.x, outerDest.y, outerDest.z, outerDest.orient,
                outerDest.MoveSpeed, outerDest.RotateSpeed, outerDest.JumpSpeed, outerDest.Gravity);
            sceneObj.SetMode(outerDest.Mode);

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnAddObject(ident, nPropCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddString(ident);
                m_argList.AddInt(nPropCount);
                m_receiver.Excute_CallBack("on_add_object", m_argList);
            }

            return;
        }
      
        //SERVER_CHARGE_VALIDSTRING (50)(不用做)
        static public void recvChargeValidString(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);


            loadAr.Seek(1);

            //byte[] validStr = new byte[1];
            //validStr[0] = 0x0;
            string strValid = string.Empty;
            if (!loadAr.ReadStringNoLen(ref strValid))
            {
                return;
            }

            ///string strValid = System.Text.Encoding.Default.GetString(validStr);
            ///Stream stream = new MemoryStream();
            ///stream.Write(validStr, 0, validStr.Length);
            ///string strValid = stream.ToString();
            ///stream.Close();

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnServerChargeValidstring(strValid);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddString(strValid);
                m_receiver.Excute_CallBack("on_charge_validstring", m_argList);
            }
        }
        /// 接收服务器消息版本信息
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="args">参数</param>
        static public void recvServerLocationGrid(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);

            loadAr.Seek(1);
            ObjectID objId = ObjectID.zero;
            if (!loadAr.ReadObject(ref objId))
            {
                LogSystem.Log("read object failed");
                return;
            }

            uint posigrid_t = 0;
            if (!loadAr.ReadUInt32(ref posigrid_t))
            {
                LogSystem.Log("read posigrid_t failed");
                return;
            }

            string ident = Tools.GetIdent(objId);
            GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
            if (null == sceneObj)
            {
                LogSystem.Log("(GameReceiver::ServerLocationGrid)no object");
            }
            else
            {
                int iindex = (int)posigrid_t >> 10;
                float fOrient = (float)(posigrid_t & 0x3FF) / 100.0f;
                sceneObj.SetLocation(iindex, fOrient);
                sceneObj.SetDestination(iindex, fOrient, 0.0F);
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnLocationGrid(ident);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddString(ident);
                m_receiver.Excute_CallBack("on_location_grid", m_argList);
            }
        }
        /// 接收服务器消息版本信息
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="args">参数</param>
        static public void recvServerMovingGrid(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, iLen);

            loadAr.Seek(1);

            ObjectID objId = ObjectID.zero;
            if (!loadAr.ReadObject(ref objId))
            {
                LogSystem.Log("read object failed");
                return;
            }

            uint destgrid_t = 0;
            if (!loadAr.ReadUInt32(ref destgrid_t))
            {
                LogSystem.Log("read destgrid_t failed");
                return;
            }

            int MoveSpeed = 0;
            if( !loadAr.ReadInt8(ref MoveSpeed))
            {
                LogSystem.Log("read MoveSpeed failed");
                return;
            }

            int Mode = 0;
            if( !loadAr.ReadInt32(ref Mode))
            {
                LogSystem.Log("read Mode failed");
                return;
            }

            string ident = Tools.GetIdent(objId);
            GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
            if (null == sceneObj)
            {
                LogSystem.Log("(GameReceiver::recvServerMovingGrid)no object");
            }
            else
            {
                int iindex = (int)destgrid_t >> 10;
                float fOrient = (float)(destgrid_t & 0x3FF) / 100.0f;
                float fMoveSpeed = (float)MoveSpeed;
                sceneObj.SetDestination(iindex, fOrient, fMoveSpeed / 10.0f);
                sceneObj.SetMode(Mode);
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnMovingGrid(ident);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddString(ident);
                m_receiver.Excute_CallBack("on_moving_grid", m_argList);
            }
        }
        /// 接收服务器消息版本信息
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="args">参数</param>
        static public void recvServerAllDestGrid(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, iLen);

            loadAr.Seek(1);
            uint nCount = 0;
            if (!loadAr.ReadUInt32(ref nCount))
            {
                LogSystem.Log("read destgrid_t failed");
                return;
            }

            for (uint i = 0; i < nCount; ++i)
            {
                ObjectID objId = ObjectID.zero;
                if (!loadAr.ReadObject(ref objId))
                {
                    LogSystem.Log("read object failed");
                    return;
                }

                uint destgrid_t = 0;
                if (!loadAr.ReadUInt32(ref destgrid_t))
                {
                    LogSystem.Log("read destgrid_t failed");
                    return;
                }

                int MoveSpeed = 0;
                if (!loadAr.ReadInt8(ref MoveSpeed))
                {
                    LogSystem.Log("read MoveSpeed failed");
                    return;
                }

                int Mode = 0;
                if (!loadAr.ReadInt32(ref Mode))
                {
                    LogSystem.Log("read Mode failed");
                    return;
                }

                string ident = Tools.GetIdent(objId);
                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (null == sceneObj)
                {
                    LogSystem.Log("(GameReceiver::recvServerMovingGrid)no object");
                }
                else
                {
                    int iindex = (int)destgrid_t >> 10;
                    float fOrient = (float)(destgrid_t & 0x3FF) / 100.0f;
                    float fMoveSpeed = (float)MoveSpeed;
                    sceneObj.SetDestination(iindex, fOrient, fMoveSpeed / (10.0f));
                    sceneObj.SetMode(Mode);
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnMovingGrid(ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(ident);
                    m_receiver.Excute_CallBack("on_moving_grid", m_argList);
                }
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnAllDestinationGrid((int)nCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt((int)nCount);
                m_receiver.Excute_CallBack("on_all_dest_grid", m_argList);
            }
        }

        /// <summary>
        /// 接收服务器消息版本信息
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="args">参数</param>
        static public void recvServerMsgVersion(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);
            uint nUseSimpleProtocal = 0;		// 精简协议
            uint nBeatInterval = 0;
            uint nCheckInterval = 0;
            uint byMsgEncoder = 0;
            uint nBeatType = 0;
            if (!loadAr.ReadUInt8(ref nUseSimpleProtocal))
            {
                return;
            }
            if (!loadAr.ReadUInt8(ref nBeatInterval))
            {
                return;
            }
            if (!loadAr.ReadUInt8(ref nCheckInterval))
            {
                return;
            }
            if (!loadAr.ReadUInt8(ref byMsgEncoder))
            {
                return;
            }
            //心跳类型 0:不使用心跳检测,1:断线检测心跳. 1:消息延迟检测类心跳
            if (!loadAr.ReadUInt8(ref nBeatType))
            {
                return;
            }
            m_receiver.SetBeatType(nBeatType);
            m_receiver.SetBeatInterval(nBeatInterval);
            if (nCheckInterval <= 0 || nCheckInterval == nBeatInterval)
            {
                nCheckInterval = nBeatInterval + 5;
            }
            m_receiver.SetCheckInterval(nCheckInterval);
            m_receiver.SetSimpleProtocal(nUseSimpleProtocal != 0);
            fxVerify.SetVerifyUse(byMsgEncoder > 0);
            if (null != m_gameMsgHandle)
            {
                ///m_gameMsgHandle.onserver
            }
            else
            {
                m_argList.Clear();
                m_receiver.Excute_CallBack("on_server_msg_version", m_argList);
            }
        }
        static public void recvServerMsgTraceRT(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);
            /// 客户端的时间戳，精确到毫秒
            uint clietstamp = 0;
            if (!loadAr.ReadUInt32(ref clietstamp))
            {
                return;
            }
            /// 测试类型，默认为0(场景延时)，以后可以完善(登录、存档等)
            uint type = 0;
            if (!loadAr.ReadUInt8(ref type))
            {
                return;
            }

            /// 客户端测试消息序列串号
            uint serial = 0;
            if (!loadAr.ReadUInt32(ref serial))
            {
                return;
            }

            /// 参数数量
            uint nArgNum = 0;
            if (!loadAr.ReadUInt16(ref nArgNum))
            {
                return;
            }

            long uiRecvTime = DateTime.Now.Ticks / 10000 & 0xfffffff;
            m_argList.Clear();
            m_argList.AddInt64(uiRecvTime - clietstamp);
            uint hoptypen = 0;
            /// 客户端测试消息序列串号
            uint hopstampn = 0;
            for (int i = 0; i < nArgNum; i++)
            {
                if (loadAr.ReadUInt8(ref hoptypen) && loadAr.ReadUInt32(ref hopstampn))
                {
                    m_argList.AddInt((int)hoptypen);
                    m_argList.AddInt64((long)hopstampn);
                }
            }
            m_receiver.Excute_CallBack("on_msg_tracert", m_argList);
        }

        //SERVER_PROPERTY_TABLE (9) OK
        static public void recvPropertyTable(GlobalServerMsg id, object args, int iLen)
        {

            m_receiver.ClearPropertyTable();

            byte[] data = (byte[])args;
            m_receiver.SetPropertyTableMd5(data);

            LoadArchive loadAr = LoadArchive.Load(data, data.Length);

            //属性数量
            int nPropertyNum = 0;
            loadAr.Seek(1);

            if (!loadAr.ReadInt16(ref nPropertyNum))
            {
                return;
            }

            for (int i = 0, count = 0; i < nPropertyNum * 2; i = i + 2, count++)
            {
                string strProp = string.Empty;
                if (!loadAr.ReadStringNoLen(ref strProp))
                {
                    return;
                }

                int ntype = 0;
                if (!loadAr.ReadInt8(ref ntype))
                {
                    return;
                }

                PropData propData =PropData.zero;
                propData.nCount = 0;
                propData.strName = strProp;
                propData.nType = ntype;
                //Propetry prop = new Propetry(strProp, ntype);
                m_receiver.AddPropData(count, ref propData);
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnPropertyTable(nPropertyNum);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nPropertyNum);
                m_receiver.Excute_CallBack("on_property_table", m_argList);
            }

        }

        //SERVER_RECORD_TABLE (10)OK
        static public void recvRecordTable(GlobalServerMsg id, object args, int iLen)
        {

            m_receiver.ClearRecordTable();

            byte[] data = (byte[])args;
            m_receiver.SetRecordTableMd5(data);
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            int nRecordCount = 0;
            if (!loadAr.ReadInt16(ref nRecordCount))
            {
                return;
            }

            for (int i = 0; i < nRecordCount; i++)
            {
                string recordName = string.Empty;
                if (!loadAr.ReadStringNoLen(ref recordName))
                {
                    return;
                }

                //LogSystem.Log("recordName" , recordName);
                int nCols = 0;
                if (!loadAr.ReadInt16(ref nCols))
                {
                    return;
                }

                RecData rec = new RecData();
                rec.strName = recordName;
                rec.nCols = nCols;

                //获取表格详情
                for (int index = 0; index < nCols; index++)
                {
                    int nColType = 0;
                    if (!loadAr.ReadInt8(ref nColType))
                    {
                        return;
                    }

                    if (!rec.AddRecColType(index, nColType))
                    {
                        return;
                    }
                }

                m_receiver.AddRecData(i, ref rec);
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnRecordTable(nRecordCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nRecordCount);
                m_receiver.Excute_CallBack("on_record_table", m_argList);
            }
        }

        //SERVER_ENTRY_SCENE (11) OK
        static public void recvEntryScene(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            //动态密匙
            int nDynamicKey = 0;
            if (!loadAr.ReadInt32(ref nDynamicKey))
            {
                return;
            }

            //主角玩家的对象标识
            ObjectID objId = ObjectID.zero;
            if (!loadAr.ReadObject(ref objId))
            {
                return;
            }

            //后续场景属性数量
            int nPropCount = 0;
            if (!loadAr.ReadInt16(ref nPropCount))
            {
                return;
            }

            string ident = Tools.GetIdent(objId);
            if (m_gameClient == null)
            {
                LogSystem.Log("GameClient is null");
                return;
            }

            GameScene scene = m_gameClient.CreateScene(ident);
            if (scene == null)
            {
                LogSystem.Log("CreateScene is null");
                return;
            }

            if (!m_receiver.RecvProperty(ref scene, ref loadAr, nPropCount, false))
            {
                LogSystem.Log("RecvProperty Failed");
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnEntryScene(ident, nPropCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddString(ident);
                m_argList.AddInt(nPropCount);
                m_receiver.Excute_CallBack("on_entry_scene", m_argList);
            }
        }

        //SERVER_EXIT_SCENE (12)
        static public void recvExitScene(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);

            if (!loadAr.Seek(1))
            {
                return;
            }

            if (m_gameClient == null)
            {
                LogSystem.Log("GameClient is null");
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnExitScene();
            }
            else
            {
                m_argList.Clear();
                m_receiver.Excute_CallBack("on_exit_scene", m_argList);
            }
        }

        //SERVER_RECORD_CLEAR (20) OK(obj =0,3,1,2)
        static public void recvRecordClear(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            int nIsViewObj = -1;
            if (!loadAr.ReadInt8(ref nIsViewObj))
            {
                return;
            }

            ObjectID objId = ObjectID.zero;
            if (!loadAr.ReadObject(ref objId))
            {
                return;
            }

            int nIndex = 0;
            if (!loadAr.ReadInt16(ref nIndex))
            {
                return;
            }

            if (null == m_receiver)
            {
                return;
            }

            string recName = m_receiver.GetRecordName(nIndex);
            if (recName == null || recName.Length == 0)
            {
                LogSystem.Log("recName is empty or is null");
                return;
            }

            if (m_gameClient == null)
            {
                LogSystem.Log("GameClient is null");
                return;
            }

            if (m_receiver == null)
            {
                LogSystem.Log("GameReceiver is null");
                return;
            }

            if (nIsViewObj == 0)
            {
                string ident = Tools.GetIdent(objId);
                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (null == sceneObj)
                {
                    LogSystem.Log("scene obj is null");
                    return;
                }
                GameRecord record = sceneObj.GetGameRecordByName(recName);
                if (record != null)
                {
                    record.ClearRow();
                }
                else
                {
                    if (!m_receiver.AddRecord(ref sceneObj, nIndex))
                    {
                        LogSystem.Log("add failed record" , recName);
                    }
                }


                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnRecordClear(ident, recName);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(ident);
                    m_argList.AddString(recName);
                    m_receiver.Excute_CallBack("on_record_clear", m_argList);
                }


            }//end nIsViewObj == 0
            else if (nIsViewObj == 3)
            {
                string view_ident = objId.m_nIdent.ToString();
                if (m_gameClient == null)
                {
                    LogSystem.Log("GameClient is null");
                    return;
                }

                GameView viewObj = m_receiver.GetView(view_ident);
                if (viewObj == null)
                {
                    LogSystem.Log("no object record name " , recName);
                }
                else
                {
                    GameRecord record = viewObj.GetGameRecordByName(recName);
                    if (record != null)
                    {
                        record.ClearRow();

                    }
                    else
                    {
                        if (!m_receiver.AddRecord(ref viewObj, nIndex))
                        {
                            LogSystem.Log("add failed record name " , recName);
                        }
                    }
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnViewRecordClear(view_ident, recName);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(view_ident);
                    m_argList.AddString(recName);
                    m_receiver.Excute_CallBack("on_view_record_clear", m_argList);
                }

            }//end nIsViewObj == 3
            else if (nIsViewObj == 1)
            {
                string view_ident = objId.m_nIdent.ToString();
                string item_ident = objId.m_nSerial.ToString();
                GameViewObj viewObj = m_receiver.GetViewObj(view_ident, item_ident);
                if (viewObj != null)
                {
                    GameRecord record = viewObj.GetGameRecordByName(recName);
                    if (record != null)
                    {
                        record.ClearRow();
                    }
                    else
                    {
                        if (!m_receiver.AddRecord(ref viewObj, nIndex))
                        {
                            LogSystem.Log("add failed record name " , recName);
                        }
                    }

                }
                else
                {
                    LogSystem.Log("no object record name " , recName);
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnViewObjRecordClear(view_ident, item_ident, recName);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(view_ident);
                    m_argList.AddString(item_ident);
                    m_argList.AddString(recName);
                    m_receiver.Excute_CallBack("on_viewobj_record_clear", m_argList);
                }

            }//end nIsViewObj == 1
            else if (nIsViewObj == 2)
            {
                GameScene sceneObj = (GameScene)m_gameClient.GetCurrentScene();
                if (sceneObj != null)
                {
                    GameRecord record = sceneObj.GetGameRecordByName(recName);
                    if (record != null)
                    {
                        record.ClearRow();
                    }
                    else
                    {
                        if (!m_receiver.AddRecord(ref sceneObj, nIndex))
                        {
                            LogSystem.Log("add failed record name " , recName);
                        }
                    }

                }
                else
                {
                    LogSystem.Log("no object record name = " , recName);
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnSceneRecordClear(recName);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(recName);
                    m_receiver.Excute_CallBack("on_scene_record_clear", m_argList);
                }
            }//end nIsViewObj == 2
        }

        //SERVER_CREATE_VIEW (21) OK
        static public void recvCreateView(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            //视图编号
            //容量
            //后续属性数量
            uint nViewId = 0;
            int nCapacity = 0;
            int nCount = 0;
            if (!loadAr.ReadUInt16(ref nViewId))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nCapacity))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nCount))
            {
                return;
            }

            string view_ident = string.Empty;
            view_ident = nViewId.ToString();
            if (m_gameClient == null)
            {
                LogSystem.Log("GameClient is null");
                return;
            }

            try
            {
                GameView view = m_gameClient.CreateView(view_ident, nCapacity);

                if (view != null)
                {
                    if (false == m_receiver.RecvProperty(ref view, ref loadAr, nCount, false))
                    {
                        LogSystem.Log("RecvProperty failed");
                    }
                }
                else
                {
                    LogSystem.Log("Create View is null");
                    return;
                }

            }
            catch (Exception ex)
            {
                LogSystem.Log("Exception:" , ex.ToString());
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnCreateView(view_ident, nCapacity, nCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddString(view_ident);
                m_argList.AddInt(nCapacity);
                m_argList.AddInt(nCount);
                m_receiver.Excute_CallBack("on_create_view", m_argList);
            }
        }

        // SERVER_OBJECT_PROPERTY(16) (ok)
        static public void recvObjectProperty(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            int nIsViewObj = 0;
            if (!loadAr.ReadInt8(ref nIsViewObj))
            {
                return;
            }

            ObjectID objId = ObjectID.zero;
            if (!loadAr.ReadObject(ref objId))
            {
                return;
            }

            int nCount = 0;
            if (!loadAr.ReadInt16(ref nCount))
            {
                return;
            }

            if (m_gameClient == null)
            {
                LogSystem.Log("GameClient is null");
                return;
            }

            if (m_receiver == null)
            {
                LogSystem.Log("GameReceiver is null");
                return;
            }

            try
            {
                if (nIsViewObj == 0)
                {
                    string ident = Tools.GetIdent(objId);
                    GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                    if (sceneObj == null)
                    {
                        LogSystem.Log("scene obj is null");
                        return;
                    }

                    if (!m_receiver.RecvProperty(ref sceneObj,
                        ref loadAr, nCount, true))
                    {
                        LogSystem.Log("property error");
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnObjectProperty(ident, nCount);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(ident);
                        m_argList.AddInt(nCount);
                        m_receiver.Excute_CallBack("on_object_property", m_argList);
                    }

                }
                else
                {
                    string view_ident = objId.m_nIdent.ToString();
                    string item_ident = objId.m_nSerial.ToString();
                    GameViewObj viewObj = m_receiver.GetViewObj(view_ident, item_ident);
                    if (viewObj != null)
                    {
                        if (!m_receiver.RecvViewProperty(view_ident, ref viewObj,
                            ref loadAr, nCount, true))
                        {
                            LogSystem.Log("property error");
                        }

                    }
                    else
                    {
                        LogSystem.Log("view obj is null");
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewObjProperty(view_ident, item_ident, nCount);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(view_ident);
                        m_argList.AddString(item_ident);
                        m_argList.AddInt(nCount);
                        m_receiver.Excute_CallBack("on_view_object_property", m_argList);
                    }
                }//end if (nIsViewObj == 0)

            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }//end try catch
        }

        //SERVER_RECORD_ADDROW (17) OK (viewobj =0,3,1,2)
        static public void recvRecordAddRow(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            int nIsViewObj = 0;
            if (!loadAr.ReadInt8(ref nIsViewObj))
            {
                return;
            }

            ObjectID objId = ObjectID.zero;
            if (!loadAr.ReadObject(ref objId))
            {
                return;
            }


            int nIndex = 0;
            if (!loadAr.ReadInt16(ref nIndex))
            {
                return;
            }

            int nRow = 0;
            if (!loadAr.ReadInt16(ref nRow))
            {
                return;
            }

            int nRows = 0;
            if (!loadAr.ReadInt16(ref nRows))
            {
                return;
            }

            try
            {
                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                string recName = m_receiver.GetRecordName(nIndex);
                if (nIsViewObj == 0)
                {
                    string ident = Tools.GetIdent(objId);
                    if (null == m_gameClient)
                    {
                        LogSystem.Log("GameClient is null");
                        return;
                    }

                    GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                    if (sceneObj == null)
                    {
                        return;
                    }
                    GameRecord record = sceneObj.GetGameRecordByName(recName);
                    if (record == null)
                    {
                        if (!m_receiver.AddRecord(ref sceneObj, nIndex))
                        {
                            LogSystem.Log("add failed record name:", recName);
                        }

                        record = sceneObj.GetGameRecordByName(recName);
                    }

                    if (record != null)
                    {
                        if (!m_receiver.RecvRecordRow(ref record, nIndex,
                            ref loadAr, nRow, nRows))
                        {
                            LogSystem.Log("RecvRecordRow failed");
                            return;
                        }
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnRecordAddRow(ident, recName, nRow, nRows);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_argList.AddInt(nRows);
                        m_receiver.Excute_CallBack("on_record_add_row", m_argList);
                    }

                }//end nIsViewObj == 0
                else if (nIsViewObj == 3)
                {
                    string ident = objId.m_nIdent.ToString();
                    if (null == m_gameClient)
                    {
                        LogSystem.Log("GameClient is null");
                        return;
                    }

                    GameView viewObj = m_receiver.GetView(ident);
                    if (viewObj == null)
                    {
                        LogSystem.Log("view obj is null");
                        return;
                    }

                    GameRecord record = viewObj.GetGameRecordByName(recName);
                    if (record == null)
                    {
                        if (!m_receiver.AddRecord(ref viewObj, nIndex))
                        {
                            LogSystem.Log("AddRecord failed recrod" , record.GetName());
                        }

                        record = viewObj.GetGameRecordByName(recName);
                    }

                    if (record != null)
                    {
                        if (!m_receiver.RecvRecordRow(ref record, nIndex,
                            ref loadAr, nRow, nRows))
                        {
                            LogSystem.Log("obj 3 RecvRecordRow record" , record.GetName());
                        }
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewRecordAddRow(ident, recName, nRow, nRows);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_argList.AddInt(nRows);
                        m_receiver.Excute_CallBack("on_view_record_add_row", m_argList);
                    }

                }//end nIsViewObj == 3 
                else if (nIsViewObj == 1)
                {
                    string view_ident = objId.m_nIdent.ToString();
                    string item_ident = objId.m_nSerial.ToString();
                    if (m_gameClient == null)
                    {
                        LogSystem.Log("GameClient is null");
                        return;
                    }

                    GameViewObj viewObj = m_receiver.GetViewObj(view_ident, item_ident);
                    if (viewObj == null)
                    {
                        LogSystem.Log("obj 1 view obj is null ");
                        return;
                    }

                    GameRecord record = viewObj.GetGameRecordByName(recName);
                    if (null == record)
                    {
                        if (!m_receiver.AddRecord(ref viewObj, nIndex))
                        {
                            LogSystem.Log("obj 1 AddRecord failed record" , record.GetName());
                        }

                        record = viewObj.GetGameRecordByName(recName);
                    }

                    if (record != null)
                    {
                        if (!m_receiver.RecvRecordRow(ref record,
                            nIndex, ref loadAr, nRow, nRows))
                        {
                            LogSystem.Log("obj 1 RecvRecordRow failed record :" , record.GetName());
                        }
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewObjRecordAddRow(view_ident, item_ident, recName, nRow, nRows);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(view_ident);
                        m_argList.AddString(item_ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_argList.AddInt(nRows);
                        m_receiver.Excute_CallBack("on_viewobj_record_add_row", m_argList);
                    }

                }//end nIsViewObj == 1
                else if (nIsViewObj == 2)
                {
                    if (m_gameClient == null)
                    {
                        LogSystem.Log("obj 2 GameClient is null");
                        return;
                    }

                    GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                    if (scene == null)
                    {
                        LogSystem.Log("obj 2 scene is null");
                        return;
                    }

                    GameRecord record = scene.GetGameRecordByName(recName);
                    if (record == null)
                    {
                        if (!m_receiver.AddRecord(ref scene, nIndex))
                        {
                            LogSystem.Log("obj 2 AddRecord failed record:" , recName);
                        }
                        record = scene.GetGameRecordByName(recName);
                    }

                    if (record != null)
                    {
                        if (!m_receiver.RecvRecordRow(ref record,
                            nIndex, ref loadAr, nRow, nRows))
                        {
                            LogSystem.Log("obj 2 RecvRecordRow failed");
                        }
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnSceneRecordAddRow(recName, nRow, nRows);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_argList.AddInt(nRows);
                        m_receiver.Excute_CallBack("on_scene_record_add_row", m_argList);
                    }
                }//end nIsViewObj == 2

            }
            catch (Exception ex)
            {
                LogSystem.Log("Exception:" , ex.ToString());
            }//end try catch

        }


        

        //SERVER_VIEW_CHANGE (43)
        static public void recvViewChange(GlobalServerMsg id, object args, int iLen)
        {

            try
            {
                byte[] data = (byte[])args;
                LoadArchive loadAr = LoadArchive.Load(data, data.Length);
                if (!loadAr.Seek(1))
                {
                    return;
                }


                if (m_gameClient == null)
                {
                    LogSystem.Log("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                int nViewId = 0;
                int nOldObjectId = 0;
                int nNewOjbectId = 0;
                if (!loadAr.ReadInt16(ref nViewId))
                {
                    return;
                }

                if (!loadAr.ReadInt16(ref nOldObjectId))
                {
                    return;
                }

                if (!loadAr.ReadInt16(ref nNewOjbectId))
                {
                    return;
                }

                string view_ident = nViewId.ToString();
                string old_item_ident = nOldObjectId.ToString();
                string new_item_ident = nNewOjbectId.ToString();

                GameView view = m_gameClient.GetViewByIdent(view_ident);
                if (view != null)
                {
                    if (!view.ChangeViewObj(old_item_ident, new_item_ident))
                    {
                        LogSystem.Log("change failed");
                    }

                }
                else
                {
                    LogSystem.Log("no view");
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnViewChange(view_ident, old_item_ident, new_item_ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(view_ident);
                    m_argList.AddString(old_item_ident);
                    m_argList.AddString(new_item_ident);
                    m_receiver.Excute_CallBack("on_view_change", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }
        }

        // SERVER_ALL_PROP(44)
        static public void recvAllProp(GlobalServerMsg id, object args, int iLen)
        {

            try
            {
                byte[] data = (byte[])args;
                LoadArchive loadAr = LoadArchive.Load(data, data.Length);
                if (!loadAr.Seek(1))
                {
                    return;
                }


                if (m_gameClient == null)
                {
                    LogSystem.Log("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                GameScene scene = (GameScene)(GameScene)m_gameClient.GetCurrentScene();
                if (scene == null)
                {
                    LogSystem.Log("no scene");
                    return;
                }

                for (int i = 0; i < nCount; i++)
                {
                    ObjectID obj = ObjectID.zero;
                    if (!loadAr.ReadObject(ref obj))
                    {
                        return;
                    }

                    int prop_num = 0;
                    if (!loadAr.ReadInt16(ref prop_num))
                    {
                        return;
                    }

                    string ident = Tools.GetIdent(obj);
                    GameSceneObj sceneObj = scene.GetSceneObjByIdent(ident);
                    if (sceneObj == null)
                        return;

                    if (!m_receiver.RecvProperty(ref sceneObj, ref loadAr,
                        prop_num, true))
                    {
                        LogSystem.Log("property error");
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnObjectProperty(ident, prop_num);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(ident);
                        m_argList.AddInt(prop_num);
                        m_receiver.Excute_CallBack("on_object_property", m_argList);
                    }
                }//end for
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnAllProperty(nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_all_prop", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }
        }

        

        //SERVER_ADD_MORE_OBJECT (47)
        static public void recvAddMoreObject(GlobalServerMsg id, object args, int iLen)
        {

            try
            {
                byte[] data = (byte[])args;
                LoadArchive loadAr = LoadArchive.Load(data, data.Length);

                loadAr.Seek(1);

                if (m_gameClient == null)
                {
                    LogSystem.Log("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                if (scene == null)
                {
                    LogSystem.Log("no scene");
                    return;
                }

                for (int i = 0; i < nCount; i++)
                {
                    ObjectID objId = ObjectID.zero;
                    if (!loadAr.ReadObject(ref objId))
                    {
                        return;
                    }

                    OuterPost outerPost = OuterPost.zero;
                    if (!outerPost.initOuterPost(ref loadAr))
                    {
                        return;
                    }

                    OuterDest outerDest = OuterDest.zero;
                    if (!outerDest.initOuterDest(ref loadAr))
                    {
                        return;
                    }
                    int prop_count = 0;
                    if (!loadAr.ReadInt16(ref prop_count))
                    {
                        return;
                    }

                    string ident = Tools.GetIdent(objId);
                    GameSceneObj sceneObj = scene.AddSceneObj(ident);
                    if (prop_count > 0)
                    {
                        if (!m_receiver.RecvProperty(ref sceneObj,
                            ref loadAr, prop_count, false))
                        {
                            LogSystem.Log("property error");
                        }
                    }

                    sceneObj.SetLocation(outerPost.x, outerPost.y, outerPost.z, outerPost.orient);
                    sceneObj.SetDestination(outerDest.x, outerDest.y, outerDest.z,
                        outerDest.orient, outerDest.MoveSpeed, outerDest.RotateSpeed,
                        outerDest.JumpSpeed, outerDest.Gravity);
                    sceneObj.SetMode(outerDest.Mode);

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnAddObject(ident, prop_count);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(ident);
                        m_argList.AddInt(prop_count);
                        m_receiver.Excute_CallBack("on_add_object", m_argList);
                    }

                }//end for
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnAddMoreObject(nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_add_more_object", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }
        }

      

        //SERVER_REMOVE_MORE_OBJECT (49) (OK)
        static public void recvRemoveMoreObject(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);

            loadAr.Seek(1);

            try
            {
                if (m_gameClient == null)
                {
                    LogSystem.Log("GameCLient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                if (scene == null)
                {
                    LogSystem.Log("no scene");
                    return;
                }

                for (int i = 0; i < nCount; i++)
                {
                    ObjectID objId = ObjectID.zero;
                    if (!loadAr.ReadObject(ref objId))
                    {
                        return;
                    }

                    string ident = Tools.GetIdent(objId);

                    GameSceneObj sceneObj = scene.GetSceneObjByIdent(ident);
                    if (sceneObj == null)
                    {
                        continue;
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnBeforeRemoveObject(ident);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(ident);
                        m_receiver.Excute_CallBack("on_before_remove_object", m_argList);
                    }
                
                    scene.RemoveSceneObj(ident);
                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnRemoveObject(ident);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(ident);
                        m_receiver.Excute_CallBack("on_remove_object", m_argList);
                    }
                }//end for

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnRemoveMoreObject(nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_remove_more_object", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }
        }

       

        //CLIENT_SPEECH (26) (OK)
        static public void recvSpeech(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);

            loadAr.Seek(1);

            ObjectID obj = ObjectID.zero;
            if (!loadAr.ReadObject(ref obj))
            {
                return;
            }

            string content = string.Empty;
            if (!loadAr.ReadWideStrNoLen(ref content))
            {
                return;
            }

            string ident = Tools.GetIdent(obj);
            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnSpeech(ident, content);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddString(ident);
                m_argList.AddWideStr(content);
                m_receiver.Excute_CallBack("on_speech", m_argList);
            }
        }

        //SERVER_SYSTEM_INFO (27) (OK)
        static public void recvSystemInfo(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            int nInfoType = 0;
            if (!loadAr.ReadInt16(ref nInfoType))
            {
                return;
            }

            string info = string.Empty;
            if (!loadAr.ReadWideStrNoLen(ref info))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnSystemInfo(nInfoType, info);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nInfoType);
                m_argList.AddWideStr(info);
                m_receiver.Excute_CallBack("on_system_info", m_argList);
            }

        }

        //SERVER_MENU (28) 
        static public void recvMenu(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);
            try
            {
                ObjectID objId = ObjectID.zero;
                int nCount = 0;
                if (!loadAr.ReadObject(ref objId))
                {
                    return;
                }

                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                m_receiver.ClearMunus();

                for (int i = 0; i < nCount; i++)
                {
                    MenuData menuData = new MenuData();
                    int nType = 0;
                    if (!loadAr.ReadInt8(ref nType))
                    {
                        return;
                    }

                    int nMark = 0;
                    if (!loadAr.ReadInt16(ref nMark))
                    {
                        return;
                    }

                    string info = string.Empty;
                    if (!loadAr.ReadWideStr(ref info))
                    {
                        return;
                    }

                    if (!m_receiver.AddMenuData(i, ref menuData))
                    {
                        LogSystem.Log("AddMenuData failed");
                        return;
                    }

                }
                string ident = Tools.GetIdent(objId);
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnMenu(ident, nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(ident);
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_menu", m_argList);
                }

            }//end try
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }//end catch
        }

        //SERVER_CLEAR_MENU (29)
        static public void recvClearMenu(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            try
            {
                m_receiver.ClearMunus();
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnClearMenu();
                }
                else
                {
                    m_argList.Clear();
                    m_receiver.Excute_CallBack("on_clear_menu", m_argList);
                }

            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
                return;
            }
        }

        //SERVER_CUSTOM
        static public void recvCustom(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            int nArgNum = 0;
            if (!loadAr.ReadInt16(ref nArgNum))
            {
                return;
            }

            VarList argList = VarList.GetVarList();
            argList.AddInt(nArgNum);
            try
            {
                for (int i = 0; i < nArgNum; i++)
                {
                    int nType = 0;
                    if (!loadAr.ReadInt8(ref nType))
                    {
                        argList.Collect();
                        return;
                    }

                    switch (nType)
                    {
                        case VarType.Int:
                            {
                                int value = 0;
                                if (!loadAr.ReadInt32(ref value))
                                {
                                    argList.Collect();
                                    return;
                                }
                                argList.AddInt(value);
                            }
                            break;
                        case VarType.Int64:
                            {
                                long value = 0;
                                if (!loadAr.ReadInt64(ref value))
                                {
                                    argList.Collect();
                                    return;
                                }
                                argList.AddInt64(value);
                            }
                            break;
                        case VarType.Float:
                            {
                                float value = 0.0f;
                                if (!loadAr.ReadFloat(ref value))
                                {
                                    return;
                                }
                                argList.AddFloat(value);
                            }
                            break;
                        case VarType.Double:
                            {
                                double value = 0.0;
                                if (!loadAr.ReadDouble(ref value))
                                {
                                    argList.Collect();
                                    return;
                                }
                                argList.AddDouble(value);
                            }
                            break;
                        case VarType.String:
                            {
                                string value = string.Empty;
                                if (!loadAr.ReadString(ref value))
                                {
                                    argList.Collect();
                                    return;
                                }
                                argList.AddString(value);
                            }
                            break;
                        case VarType.WideStr:
                            {
                                string value = string.Empty;
                                if (!loadAr.ReadWideStr(ref value))
                                {
                                    argList.Collect();
                                    return;
                                }
                                argList.AddWideStr(value);
                            }
                            break;
                        case VarType.Object:
                            {
                                ObjectID value = ObjectID.zero;
                                if (!loadAr.ReadObject(ref value))
                                {
                                    argList.Collect();
                                    return;
                                }
                                argList.AddObject(value);
                            }
                            break;
                        default:
                            LogSystem.Log("error arg type");
                            break;
                    }

                }//end for

                if (null != m_cusMsgHandle)
                {
                    m_cusMsgHandle.Process(argList);
                }
                else
                {
                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnCustom(ref argList);
                    }
                    else
                    {
                        m_receiver.Excute_CallBack("on_custom", argList);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }
            argList.Collect();
        }

        //SERVER_VIEW_PROPERTY(ok)
        static public void recvViewProperty(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            int nViewId = 0;
            int nPropCount = 0;
            if (!loadAr.ReadInt16(ref nViewId))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nPropCount))
            {
                return;
            }

            try
            {
                if (m_gameClient == null)
                {
                    LogSystem.Log("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                string view_ident = nViewId.ToString();
                GameView view = m_gameClient.GetViewByIdent(view_ident);
                if (view != null)
                {
                    if (!m_receiver.RecvProperty(ref view,
                        ref loadAr, nPropCount, true))
                    {
                        LogSystem.Log("property error");
                    }
                }
                else
                {
                    LogSystem.Log("no view");
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnViewProperty(view_ident, nPropCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(view_ident);
                    m_argList.AddInt(nPropCount);
                    m_receiver.Excute_CallBack("on_view_property", m_argList);
                }
            }//end try
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }

        }

        //SERVER_VIEW_ADD (24) (ok)
        static public void recvViewAdd(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            int nViewId = 0;
            int objId = 0;
            int nPropCount = 0;

            if (!loadAr.ReadInt16(ref nViewId))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref objId))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nPropCount))
            {
                return;
            }

            if (m_gameClient == null)
            {
                LogSystem.Log("GameClient is null");
                return;
            }

            if (m_receiver == null)
            {
                LogSystem.Log("GameReceiver is null");
                return;
            }

            string view_ident = nViewId.ToString();
            string item_ident = objId.ToString();

            GameView view = m_gameClient.GetViewByIdent(view_ident);
            if (view != null)
            {
                GameViewObj viewObj = view.AddViewObj(item_ident);

                if (viewObj != null)
                {
                    if (!m_receiver.RecvViewProperty(view_ident, ref viewObj, ref loadAr, nPropCount, false))
                    {
                        LogSystem.Log("property error");
                    }
                }
                else
                {
                    LogSystem.Log("view obj is null");
                }
            }
            else
            {
                LogSystem.Log("no view ");
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnViewAdd(view_ident, item_ident, nPropCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddString(view_ident);
                m_argList.AddString(item_ident);
                m_argList.AddInt(nPropCount);
                m_receiver.Excute_CallBack("on_view_add", m_argList);
            }
        }

        //SERVER_VIEW_REMOVE (25) (OK)
        static public void recvViewRemove(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            int nViewId = 0;
            int objId = 0;


            if (!loadAr.ReadInt16(ref nViewId))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref objId))
            {
                return;
            }

            if (m_gameClient == null)
            {
                LogSystem.Log("GameClient is null");
                return;
            }

            if (m_receiver == null)
            {
                LogSystem.Log("GameReceiver is null");
                return;
            }

            string view_ident = nViewId.ToString();
            string item_ident = objId.ToString();

            GameView view = m_gameClient.GetViewByIdent(view_ident);
            if (view != null)
            {
                if (!view.RemoveViewObj(item_ident))
                {
                    LogSystem.Log("remove failed");
                }

            }
            else
            {
                LogSystem.Log("no view");
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnViewRemove(view_ident, item_ident);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddString(view_ident);
                m_argList.AddString(item_ident);
                m_receiver.Excute_CallBack("on_view_remove", m_argList);
            }
        }

        //SERVER_REMOVE_OBJECT (14)
        static public void recvRemoveObj(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);


            ObjectID obj = ObjectID.zero;

            if (!loadAr.ReadObject(ref obj))
            {
                return;
            }

            try
            {
                if (m_gameClient == null)
                {
                    LogSystem.Log("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                string ident = Tools.GetIdent(obj);
                GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                if (scene != null)
                {
                    scene.RemoveSceneObj(ident);
                }
                else
                {
                    LogSystem.Log("no scene");
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnBeforeRemoveObject(ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(ident);
                    m_receiver.Excute_CallBack("on_before_remove_object", m_argList);
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnRemoveObject(ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(ident);
                    m_receiver.Excute_CallBack("on_remove_object", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }
        }

        //SERVER_SCENE_PROPERTY (15) (OK)
        static public void recvSceneProperty(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);

            if (!loadAr.Seek(1))
            {
                return;
            }

            try
            {
                if (m_gameClient == null)
                {
                    LogSystem.Log("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                if (scene != null)
                {
                    if (!m_receiver.RecvProperty(ref scene,
                        ref loadAr, nCount, true))
                    {
                        LogSystem.Log("property error");
                    }
                }
                else
                {
                    LogSystem.Log("no scene");
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnSceneProperty(nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_scene_property", m_argList);
                }
            }//end try
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }//end catch
        }

        //SERVER_RECORD_DELROW (ok viewobj=0 obj=3,obj=1,obj=2)
        static public void recvRecordDelRow(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            int nViewObj = 0;
            ObjectID obj = ObjectID.zero;

            int nIndex = 0;
            int nRow = 0;



            if (!loadAr.ReadInt8(ref nViewObj))
            {
                return;
            }

            if (!loadAr.ReadObject(ref obj))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nIndex))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nRow))
            {
                return;
            }

            if (null == m_receiver)
            {
                LogSystem.Log("GameReceiver is null");
                return;
            }

            if (null == m_gameClient)
            {
                LogSystem.Log("GameClient is null");
                return;
            }

            string recName = m_receiver.GetRecordName(nIndex);
            if (recName == null || recName.Length == 0)
            {
                LogSystem.Log("recName is null");
                return;
            }

            try
            {
                if (nViewObj == 0)
                {
                    string ident = Tools.GetIdent(obj);
                    if (ident == null || ident.Length == 0)
                    {
                        LogSystem.Log("viewObj is 0 ident is null ");
                        return;
                    }

                    if (m_gameClient != null)
                    {
                        GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                        if (sceneObj == null)
                        {
                            return;
                        }

                        GameRecord record = sceneObj.GetGameRecordByName(recName);
                        if (record != null)
                        {
                            bool bRet = record.DeleteRow(nRow);
                            if (!bRet)
                            {
                                LogSystem.Log("delete row failed record name" , recName.ToString());
                            }
                        }
                        else
                        {
                            LogSystem.Log("no record record name =" , recName.ToString());
                        }

                        if (null != m_gameMsgHandle)
                        {
                            m_gameMsgHandle.OnRecordRemoveRow(ident, recName, nRow);
                        }
                        else
                        {
                            m_argList.Clear();
                            m_argList.AddString(ident);
                            m_argList.AddString(recName);
                            m_argList.AddInt(nRow);
                            m_receiver.Excute_CallBack("on_record_remove_row", m_argList);
                        }
                    }

                }//end  if (nViewObj == 0)
                else if (nViewObj == 3)
                {
                    string view_ident = obj.m_nIdent.ToString();

                    if (m_gameClient != null)
                    {
                        GameView viewObj = m_receiver.GetView(view_ident);
                        if (viewObj != null)
                        {
                            GameRecord record = viewObj.GetGameRecordByName(recName);
                            if (record != null)
                            {
                                if (!record.DeleteRow(nRow))
                                {
                                    LogSystem.Log("delete row failed record name" , recName);
                                }
                            }
                            else
                            {
                                LogSystem.Log("no record record name =" , recName);
                            }
                        }
                    }
                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewRecordRemoveRow(view_ident, recName, nRow);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(view_ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_receiver.Excute_CallBack("on_view_record_remove_row", m_argList);
                    }

                }//end if (nViewObj == 3)
                else if (nViewObj == 1)
                {
                    string view_ident = obj.m_nIdent.ToString();
                    string item_idnet = obj.m_nSerial.ToString();
                    if (m_gameClient != null)
                    {
                        GameViewObj viewObj = m_receiver.GetViewObj(view_ident, item_idnet);
                        if (viewObj != null)
                        {
                            GameRecord record = viewObj.GetGameRecordByName(recName);
                            if (record != null)
                            {
                                if (!record.DeleteRow(nRow))
                                {
                                    LogSystem.Log("delete row failed record name =" , recName);
                                }

                            }
                            else
                            {
                                LogSystem.Log("no record record name " , recName);
                            }

                        }
                        else
                        {
                            LogSystem.Log("no object record name " , recName);
                        }

                    }
                    //end m_gameClient != null
                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewObjRecordRemoveRow(view_ident, item_idnet, recName, nRow);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(view_ident);
                        m_argList.AddString(item_idnet);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_receiver.Excute_CallBack("on_viewobj_record_remove_row", m_argList);
                    }

                }//end if (nViewObj == 1)
                else if (nViewObj == 2)
                {
                    if (m_gameClient != null)
                    {
                        GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                        if (scene != null)
                        {
                            GameRecord record = scene.GetGameRecordByName(recName);
                            if (record != null)
                            {
                                if (!record.DeleteRow(nRow))
                                {
                                    LogSystem.Log("delete row failed record name "
                                        + recName);
                                }

                            }
                            else
                            {
                                LogSystem.Log("no record record name " , recName);
                            }

                        }
                        else
                        {
                            LogSystem.Log("no object record name " , recName);
                        }


                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnSceneRecordRemoveRow(recName, nRow);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_receiver.Excute_CallBack("on_scene_record_remove_row", m_argList);
                    }
                }//end if (nViewObj == 2)
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
                return;
            }
        }

        //SERVER_DELETE_VIEW OK
        static public void recvDeleteView(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);

            loadAr.Seek(1);

            uint nViewId = 0;

            if (!loadAr.ReadUInt16(ref nViewId))
            {
                return;
            }

            if (m_gameClient == null)
            {
                LogSystem.Log("GameClient is null");
                return;
            }

            string view_ident = nViewId.ToString();
            if (!m_gameClient.DeleteView(view_ident))
            {
                LogSystem.Log("delete failed view id = " , nViewId.ToString());
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnDeleteView(view_ident);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddString(view_ident);
                m_receiver.Excute_CallBack("on_delete_view", m_argList);
            }
        }

        //SERVER_QUEUE
        static public void recvQueue(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);

            loadAr.Seek(1);

            int nQueue = 0;//队列编号
            int nPosition = 0;//在队列中的位置（为0 表示结束排队）
            int nPriorCount = 0;//绿色通道人数

            if (!loadAr.ReadInt32(ref nQueue))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nPosition))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nPriorCount))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnQueue(nQueue, nPosition, nPriorCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nQueue);
                m_argList.AddInt(nPosition);
                m_argList.AddInt(nPriorCount);
                m_receiver.Excute_CallBack("on_queue", m_argList);
            }
        }

        //SERVER_TERMINATE
        static public void recvTerminate(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);

            loadAr.Seek(1);

            int nResult = 0;

            if (!loadAr.ReadInt32(ref nResult))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnTerminate(nResult);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nResult);
                m_receiver.Excute_CallBack("on_terminate", m_argList);
            }
        }
        //SERVER_FROM_GMCC
        static public void recvFromGmcc(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);
            int nId = 0;//GM编号
            string name = string.Empty;//GM名称
            string info = string.Empty;//信息内容

            if (!loadAr.ReadInt32(ref nId))
            {
                return;
            }

            if (!loadAr.ReadUnicodeLen(ref name, (Const.OUTER_OBJNAME_LENGTH + 1) * 2))
            {
                return;
            }

            if (!loadAr.ReadWideStrNoLen(ref info))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnFromGmcc(nId, name, info);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nId);
                m_argList.AddWideStr(name);
                m_argList.AddWideStr(info);
                m_receiver.Excute_CallBack("on_from_gmcc", m_argList);
            }
        }

        //SERVER_LINK_TO
        static public void recvLinkTo(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            try
            {
                ObjectID objId = ObjectID.zero;
                ObjectID linkId = ObjectID.zero;
                if (!loadAr.ReadObject(ref objId))
                {
                    return;
                }

                if (!loadAr.ReadObject(ref linkId))
                {
                    return;
                }

                OuterPost outerPost =OuterPost.zero;
                if (!outerPost.initOuterPost(ref loadAr))
                {
                    return;
                }

                string ident = Tools.GetIdent(objId);
                string link_itent = Tools.GetIdent(linkId);
                if (null == m_gameClient)
                {
                    LogSystem.Log("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (sceneObj == null)
                {
                    return;
                }

                sceneObj.SetLinkIdent(link_itent);
                sceneObj.SetLinkPos(outerPost.x, outerPost.y, outerPost.z, outerPost.orient);

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnLinkTo(ident, link_itent, outerPost.x, outerPost.y, outerPost.z, outerPost.orient);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(ident);
                    m_argList.AddString(link_itent);
                    m_argList.AddFloat(outerPost.x);
                    m_argList.AddFloat(outerPost.y);
                    m_argList.AddFloat(outerPost.z);
                    m_argList.AddFloat(outerPost.orient);
                    m_receiver.Excute_CallBack("on_link_to", m_argList);
                }

            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }
        }

        //SERVER_UNLINK
        static public void recvUnLink(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            try
            {
                ObjectID objId = ObjectID.zero;

                if (!loadAr.ReadObject(ref objId))
                {
                    return;
                }

                string ident = Tools.GetIdent(objId);
                if (null == m_gameClient)
                {
                    LogSystem.Log("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (sceneObj == null)
                {
                    return;
                }

                sceneObj.SetLinkIdent(string.Empty);

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnUnlink(ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(ident);
                    m_receiver.Excute_CallBack("on_un_link", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }
        }

        //SERVER_LINK_MOVE
        static public void recvLinkMove(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            try
            {
                if (null == m_gameClient)
                {
                    LogSystem.Log("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.Log("GameReceiver is null");
                    return;
                }

                ObjectID objId = ObjectID.zero;
                ObjectID linkId = ObjectID.zero;
                if (!loadAr.ReadObject(ref objId))
                {
                    return;
                }
                if (!loadAr.ReadObject(ref linkId))
                {
                    return;
                }

                OuterPost outerPost = OuterPost.zero;

                if (!outerPost.initOuterPost(ref loadAr))
                {
                    return;
                }

                string ident = Tools.GetIdent(objId);
                string link_ident = Tools.GetIdent(linkId);


                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (sceneObj == null)
                {
                    return;
                }

                sceneObj.SetLinkIdent(link_ident);
                sceneObj.SetLinkPos(outerPost.x, outerPost.y, outerPost.z, outerPost.orient);
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnLinkMove(ident, link_ident, outerPost.x,
                        outerPost.y, outerPost.z, outerPost.orient);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(ident);
                    m_argList.AddString(link_ident);
                    m_argList.AddFloat(outerPost.x);
                    m_argList.AddFloat(outerPost.y);
                    m_argList.AddFloat(outerPost.z);
                    m_argList.AddFloat(outerPost.orient);
                    m_receiver.Excute_CallBack("on_link_move", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }
        }

      

        //SERVER_WARNING (34)
        static public void recvWarning(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);


            int nType = 0;//告警类型
            string info = string.Empty;//告警内容

            if (!loadAr.ReadInt16(ref nType))
            {
                return;
            }

            if (!loadAr.ReadWideStrNoLen(ref info))
            {
                return;
            }
            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnWarning(nType, info);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nType);
                m_argList.AddWideStr(info);
                m_receiver.Excute_CallBack("on_warning", m_argList);
            }
        }

        //SERVER_IDLE
        static public void recvIdle(GlobalServerMsg id, object args, int iLen)
        {

            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);
            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnIdle();
            }
            else
            {
                m_argList.Clear();
                m_receiver.Excute_CallBack("on_idle", m_argList);
            }
        }

        //SERVER_ALL_DEST (OK)
        static public void recvAllDest(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            try
            {
                if (m_gameClient == null)
                {
                    LogSystem.LogWarning("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.LogWarning("GameReceiver is null");
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    LogSystem.LogWarning("loadAr.ReadInt16 error");
                    return;
                }

                //VarList arg = VarList.GetVarList();
                GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                if (scene != null)
                {
                    for (int i = 0; i < nCount; i++)
                    {
                        ObjectID obj = ObjectID.zero;
                        if (!loadAr.ReadObject(ref obj))
                        {
                            LogSystem.LogWarning("read object failed");
                            return;
                        }

                        OuterDest dest = OuterDest.zero;
                        if (!dest.initOuterDest(ref loadAr))
                        {
                            LogSystem.LogWarning("initOuterDest failed");
                            return;
                        }

                        string ident = Tools.GetIdent(obj);
                        GameSceneObj sceneObj = scene.GetSceneObjByIdent(ident);
                        if (sceneObj == null)
                        {
                            LogSystem.LogWarning("sceneObj is null");
                            continue;
                        }

                        sceneObj.SetDestination(dest.x, dest.y,
                            dest.z, dest.orient, dest.MoveSpeed, dest.RotateSpeed,
                            dest.JumpSpeed, dest.Gravity);

                        sceneObj.SetMode(dest.Mode);

                        if (null != m_gameMsgHandle)
                        {
                            m_gameMsgHandle.OnMoving(ident);
                        }
                        else
                        {
                            m_argList.Clear();
                            m_argList.AddString(ident);
                            m_receiver.Excute_CallBack("on_moving", m_argList);
                        }
                    }//end for(int i= 0)

                }
                else
                {
                    LogSystem.LogWarning("no scene");
                }
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnAllDestination(nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_all_dest", m_argList);
                }

            }//end try
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }//end catch


        }

        //SERVER_MOVING
        static public void recvOnMoving(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);

            try
            {
                if (m_gameClient == null)
                {
                    LogSystem.LogWarning("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    LogSystem.LogWarning("GameReceiver is null");
                    return;
                }

                ObjectID obj = ObjectID.zero;
                OuterDest dest = OuterDest.zero;
                if (!loadAr.ReadObject(ref obj))
                {
                    LogSystem.LogWarning("loadAr.ReadObject error");
                    return;
                }

                if (!dest.initOuterDest(ref loadAr))
                {
                    LogSystem.LogWarning("initOuterDest failed");
                    return;
                }

                string ident = Tools.GetIdent(obj);
                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (sceneObj == null)
                {
                    LogSystem.LogWarning("sceneObj is null");
                    return;
                }
                sceneObj.SetDestination(dest.x, dest.y, dest.z,
                    dest.orient, dest.MoveSpeed, dest.RotateSpeed,
                    dest.JumpSpeed, dest.Gravity);
                sceneObj.SetMode(dest.Mode);

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnMoving(ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(ident);
                    m_receiver.Excute_CallBack("on_moving", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }



        }

        //SERVER_LOCATION (31) (OK)
        static public void recvOnLocation(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);
            try
            {
                if (m_gameClient == null)
                {
                    return;
                }

                if (m_receiver == null)
                {
                    return;
                }

                ObjectID obj = ObjectID.zero;
                OuterPost post = OuterPost.zero;

                if (!loadAr.ReadObject(ref obj))
                {
                    LogSystem.Log("read object failed");
                    return;
                }

                if (!post.initOuterPost(ref loadAr))
                {
                    LogSystem.Log("init outer post failed");
                    return;
                }

                string ident = Tools.GetIdent(obj);
                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (sceneObj == null)
                {
                    return;
                }

                sceneObj.SetLocation(post.x, post.y, post.z, post.orient);
                sceneObj.SetDestination(post.x, post.y, post.z, post.orient, 0.0f, 0.0f, 0.0f, 0.0f);

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnLocation(ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(ident);
                    m_receiver.Excute_CallBack("on_location", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }



        }

        //SERVER_RECORD_GRID
        static public void recvRecordGrid(GlobalServerMsg id, object args, int iLen)
        {
            byte[] data = (byte[])args;
            LoadArchive loadAr = LoadArchive.Load(data, data.Length);
            loadAr.Seek(1);
            try
            {
                if (m_gameClient == null)
                {
                    return;
                }

                if (m_receiver == null)
                {
                    return;
                }

                int nIsViewObj = 0;
                if (!loadAr.ReadInt8(ref nIsViewObj))
                {
                    return;
                }

                ObjectID objId = ObjectID.zero;
                if (!loadAr.ReadObject(ref objId))
                {
                    return;
                }

                int nIndex = 0;
                if (!loadAr.ReadInt16(ref nIndex))
                {
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                string recName = m_receiver.GetRecordName(nIndex);
                if (nIsViewObj == 0)
                {
                    string ident = Tools.GetIdent(objId);
                    GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                    if (null == sceneObj)
                    {
                        return;
                    }

                    GameRecord record = sceneObj.GetGameRecordByName(recName);
                    if (record != null)
                    {
                        if (!m_receiver.RecvRecordGrid(ref record,
                            nIsViewObj, (int)objId.m_nIdent, (int)objId.m_nSerial, nIndex,
                            ref loadAr, nCount))
                        {
                            LogSystem.Log("recv failed");
                        }

                        if (null != m_gameMsgHandle)
                        {
                            m_gameMsgHandle.OnRecordGrid(ident, recName, nCount);
                        }
                        else
                        {
                            m_argList.Clear();
                            m_argList.AddString(ident);
                            m_argList.AddString(recName);
                            m_argList.AddInt(nCount);
                            m_receiver.Excute_CallBack("on_record_grid", m_argList);
                        }
                    }
                    else
                    {
                        LogSystem.Log("no record record name " , recName);
                    }
                }//end if (nIsViewObj == 0)
                else if (nIsViewObj == 3)
                {
                    string view_ident = objId.m_nIdent.ToString();
                    GameView viewObj = m_receiver.GetView(view_ident);
                    if (viewObj != null)
                    {
                        GameRecord record = viewObj.GetGameRecordByName(recName);
                        if (record != null)
                        {
                            if (!m_receiver.RecvRecordGrid(ref record,
                                nIsViewObj, (int)objId.m_nIdent, (int)objId.m_nSerial,
                                nIndex, ref loadAr, nCount))
                            {
                                LogSystem.Log("recv failed record name " , recName);
                            }
                        }
                        else
                        {
                            LogSystem.Log("no record record name " , recName);
                        }

                    }
                    else
                    {
                        LogSystem.Log("no object record name " , recName);
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewRecordGrid(view_ident, recName, nCount);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(view_ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nCount);
                        m_receiver.Excute_CallBack("on_view_record_grid", m_argList);
                    }

                }//end if (nIsViewObj == 3)
                else if (nIsViewObj == 1)
                {
                    string view_ident = objId.m_nIdent.ToString();
                    string item_ident = objId.m_nSerial.ToString();

                    GameViewObj viewObj = m_receiver.GetViewObj(view_ident, item_ident);
                    if (viewObj != null)
                    {
                        GameRecord record = viewObj.GetGameRecordByName(recName);
                        if (record != null)
                        {
                            if (!m_receiver.RecvRecordGrid(ref record,
                                nIsViewObj, (int)objId.m_nIdent, (int)objId.m_nSerial,
                                nIndex, ref loadAr, nCount))
                            {
                                LogSystem.Log("recv failed record name " , recName);
                            }
                        }
                        else
                        {
                            LogSystem.Log("no record record name " , recName);
                        }
                    }
                    else
                    {
                        LogSystem.Log("no object record name " , recName);
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewObjRecordGrid(view_ident, item_ident, recName, nCount);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(view_ident);
                        m_argList.AddString(item_ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nCount);
                        m_receiver.Excute_CallBack("on_viewobj_record_grid", m_argList);
                    }

                }//end if (nIsViewObj == 1)
                else if (nIsViewObj == 2)
                {
                    GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                    if (scene != null)
                    {
                        GameRecord record = scene.GetGameRecordByName(recName);
                        if (record != null)
                        {
                            if (!m_receiver.RecvRecordGrid(ref record, nIsViewObj, (int)objId.m_nIdent,
                                (int)objId.m_nSerial, nIndex, ref loadAr, nCount))
                            {
                                LogSystem.Log("recv failed record name " , recName);
                            }

                        }
                        else
                        {
                            LogSystem.Log("no reocrd record name ", recName);
                        }

                    }
                    else
                    {
                        LogSystem.Log("no obejct record name " , recName);
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnSceneRecordGrid(recName, nCount);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(recName);
                        m_argList.AddInt(nCount);
                        m_receiver.Excute_CallBack("on_scene_record_grid", m_argList);
                    }

                }//end if (nIsViewObj == 2)
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
            }
        }

    }
}
