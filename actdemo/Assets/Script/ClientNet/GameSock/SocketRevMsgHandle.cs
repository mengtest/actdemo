//--------------------------------------------------------------------
// 文件名	:	SocketRevMsgHandle.cs
// 内  容	:   消息分发器
// 说  明	:
// 创建日期	:	2014年06月10日
// 创建人	:	丁有进
// 版权所有	:	苏州蜗牛电子有限公司
//--------------------------------------------------------------------


using System;
using System.Collections.Generic;
using SysUtils;

namespace Fm_ClientNet
{
    class SocketRevMsgHandle
    {
        public delegate void MsgEevent(GlobalServerMsg id, object args,int iLen);
        private Dictionary<GlobalServerMsg, MsgEevent> m_handlers = new Dictionary<GlobalServerMsg, MsgEevent>();
        public bool m_bSimpleProtocal = false;

        private Byte[] m_pBuffer_SP = new Byte[262144];
        private Byte[] m_pBuffer_Batch = new Byte[2621440];

        public SocketRevMsgHandle()
        {
            //add event

            //SERVER_SET_VERIFY (1)
            AddEvent(GlobalServerMsg.SERVER_SET_VERIFY, RecvPacket.recvSetVerify);
            //SERVER_SET_ENCODE (2)
            AddEvent(GlobalServerMsg.SERVER_SET_ENCODE, RecvPacket.setEncode);
            //SERVER_ERROR_CODE (3)
            AddEvent(GlobalServerMsg.SERVER_ERROR_CODE, RecvPacket.recvErrorCode);
            AddEvent(GlobalServerMsg.SERVER_IDLE, RecvPacket.recvIdle);
            //SERVER_LOGIN_SUCCEED(4)
            AddEvent(GlobalServerMsg.SERVER_LOGIN_SUCCEED, RecvPacket.recvLoginSuccess);
            //SERVER_WORLD_INFO (5)
            AddEvent(GlobalServerMsg.SERVER_WORLD_INFO, RecvPacket.recvWorldInfo);
            //SERVER_IDLE (6)
            AddEvent(GlobalServerMsg.SERVER_IDLE, RecvPacket.recvIdle);
            //SERVER_QUEUE (7)
            AddEvent(GlobalServerMsg.SERVER_QUEUE, RecvPacket.recvQueue);
            //SERVER_TERMINATE (8)
            AddEvent(GlobalServerMsg.SERVER_TERMINATE, RecvPacket.recvTerminate);
            //SERVER_PROPERTY_TABLE(9)
            AddEvent(GlobalServerMsg.SERVER_PROPERTY_TABLE, RecvPacket.recvPropertyTable);
            //SERVER_RECORD_TABLE (10)
            AddEvent(GlobalServerMsg.SERVER_RECORD_TABLE, RecvPacket.recvRecordTable);
            //SERVER_ENTRY_SCENE (11)
            AddEvent(GlobalServerMsg.SERVER_ENTRY_SCENE, RecvPacket.recvEntryScene);
            //SERVER_EXIT_SCENE (12)
            AddEvent(GlobalServerMsg.SERVER_EXIT_SCENE, RecvPacket.recvExitScene);
            //SERVER_ADD_OBJECT (13)
            AddEvent(GlobalServerMsg.SERVER_ADD_OBJECT, RecvPacket.recvAddObj);
            //SERVER_REMOVE_OBJECT (14)
            AddEvent(GlobalServerMsg.SERVER_REMOVE_OBJECT, RecvPacket.recvRemoveObj);
            //SERVER_SCENE_PROPERTY (15)
            AddEvent(GlobalServerMsg.SERVER_SCENE_PROPERTY, RecvPacket.recvSceneProperty);
            //SERVER_OBJECT_PROPERTY (16)
            AddEvent(GlobalServerMsg.SERVER_OBJECT_PROPERTY, RecvPacket.recvObjectProperty);
            //SERVER_RECORD_ADDROW (17)
            AddEvent(GlobalServerMsg.SERVER_RECORD_ADDROW, RecvPacket.recvRecordAddRow);
            //SERVER_RECORD_DELROW (18)
            AddEvent(GlobalServerMsg.SERVER_RECORD_DELROW, RecvPacket.recvRecordDelRow);
            //SERVER_RECORD_GRID(19)
            AddEvent(GlobalServerMsg.SERVER_RECORD_GRID, RecvPacket.recvRecordGrid);
            //SERVER_RECORD_CLEAR (20)
            AddEvent(GlobalServerMsg.SERVER_RECORD_CLEAR, RecvPacket.recvRecordClear);
            //SERVER_CREATE_VIEW (21)
            AddEvent(GlobalServerMsg.SERVER_CREATE_VIEW, RecvPacket.recvCreateView);
            //SERVER_DELETE_VIEW (22)
            AddEvent(GlobalServerMsg.SERVER_DELETE_VIEW, RecvPacket.recvDeleteView);
            //SERVER_VIEW_PROPERTY (23)
            AddEvent(GlobalServerMsg.SERVER_VIEW_PROPERTY, RecvPacket.recvViewProperty);
            //SERVER_VIEW_ADD (24)
            AddEvent(GlobalServerMsg.SERVER_VIEW_ADD, RecvPacket.recvViewAdd);
            //SERVER_VIEW_REMOVE (25)
            AddEvent(GlobalServerMsg.SERVER_VIEW_REMOVE, RecvPacket.recvViewRemove);
            //SERVER_SPEECH (26)
            AddEvent(GlobalServerMsg.SERVER_SPEECH, RecvPacket.recvSpeech);
            //SERVER_SYSTEM_INFO (27)
            AddEvent(GlobalServerMsg.SERVER_SYSTEM_INFO, RecvPacket.recvSystemInfo);
            //SERVER_MENU (28)
            AddEvent(GlobalServerMsg.SERVER_MENU, RecvPacket.recvMenu);
            //SERVER_CLEAR_MENU (29)
            AddEvent(GlobalServerMsg.SERVER_CLEAR_MENU, RecvPacket.recvClearMenu);
            //SERVER_CUSTOM (30) (OK)
            AddEvent(GlobalServerMsg.SERVER_CUSTOM, RecvPacket.recvCustom);
            //SERVER_LOCATION (31) (OK)
            AddEvent(GlobalServerMsg.SERVER_LOCATION, RecvPacket.recvOnLocation);
            //SERVER_MOVING (32)
            AddEvent(GlobalServerMsg.SERVER_MOVING, RecvPacket.recvOnMoving);
            //SERVER_ALL_DEST (33) (OK)
            AddEvent(GlobalServerMsg.SERVER_ALL_DEST, RecvPacket.recvAllDest);
            //SERVER_WARNING (34)
            AddEvent(GlobalServerMsg.SERVER_WARNING, RecvPacket.recvWarning);
            //SERVER_FROM_GMCC (35)
            AddEvent(GlobalServerMsg.SERVER_FROM_GMCC, RecvPacket.recvFromGmcc);
            //SERVER_LINK_TO (36)
            AddEvent(GlobalServerMsg.SERVER_LINK_TO, RecvPacket.recvLinkTo);
            //SERVER_UNLINK(37) 
            AddEvent(GlobalServerMsg.SERVER_UNLINK, RecvPacket.recvUnLink);
            //SERVER_LINK_MOVE (38)
            AddEvent(GlobalServerMsg.SERVER_LINK_MOVE, RecvPacket.recvLinkMove);
            //SERVER_CP_CUSTOM (39)
      
        
            //SERVER_VIEW_CHANGE (43)
            AddEvent(GlobalServerMsg.SERVER_VIEW_CHANGE, RecvPacket.recvViewChange);
 
            //SERVER_ALL_PROP (45)
            AddEvent(GlobalServerMsg.SERVER_ALL_PROP, RecvPacket.recvAllProp);

            //SERVER_ADD_MORE_OBJECT (47)
            AddEvent(GlobalServerMsg.SERVER_ADD_MORE_OBJECT, RecvPacket.recvAddMoreObject);

            //SERVER_REMOVE_MORE_OBJECT (49)
            AddEvent(GlobalServerMsg.SERVER_REMOVE_MORE_OBJECT, RecvPacket.recvRemoveMoreObject);
            //SERVER_CHARGE_VALIDSTRING (50)

            AddEvent(GlobalServerMsg.SERVER_CHARGE_VALIDSTRING, RecvPacket.recvChargeValidString);

            //AddEvent(GlobalServerMsg.SERVER_MSG_ENTRY_STUB, RecvPacket.recvEntryStub);
            //AddEvent(GlobalServerMsg.SERVER_MSG_EXIT_STUB, RecvPacket.recvExitStub);
            //AddEvent(GlobalServerMsg.SERVER_MSG_STUB_EXITED_STUB, RecvPacket.recvStubExitedStub);

            //SERVER_MSG_VERSION(66)
            AddEvent(GlobalServerMsg.SERVER_MSG_VERSION, RecvPacket.recvServerMsgVersion);
            //SERVER_MSG_VERSION(68)
            AddEvent(GlobalServerMsg.SERVER_MSG_TRACERT, RecvPacket.recvServerMsgTraceRT);

            //SERVER_LOCATION_GRID (72)
            AddEvent(GlobalServerMsg.SERVER_LOCATION_GRID, RecvPacket.recvServerLocationGrid);
            //SERVER_MOVING_GRID (73)
            AddEvent(GlobalServerMsg.SERVER_MOVING_GRID, RecvPacket.recvServerMovingGrid);
            //SERVER_ALL_DEST_GRID(74)
            AddEvent(GlobalServerMsg.SERVER_ALL_DEST_GRID, RecvPacket.recvServerAllDestGrid);

        }

        private bool AddEvent(GlobalServerMsg id, MsgEevent msg)
        {
            if (m_handlers.ContainsKey(id))
            {
                m_handlers.Remove(id);
            }
            m_handlers.Add(id, msg);
            return true;
        }

        public void RemoveEvent(GlobalServerMsg id)
        {
            if (m_handlers.ContainsKey(id))
            {
                m_handlers.Remove(id);
            }
        }

        public void ExcuteEvent(GlobalServerMsg id, object args,int ilen)
        {
            if (!m_handlers.ContainsKey(id))
            {
                LogSystem.Log("DispatchNow: has not id matched for:" , id);
                return;
            }

            MsgEevent handler = m_handlers[id];
            if (handler != null)
            {
                try
                {
                    handler(id, args, ilen);
                }
                catch (System.Exception ex)
                {
                    LogSystem.LogError("ExcuteEvent: handler exception:", id, " ", ex.ToString());
                }
            }
        }

        public void HandleMessage(byte[] data, int size)
        {
            if (data.Length == 0 || 0 == size)
            {
                LogSystem.LogError("Error, HandleMessage data is 0 ");
                return;
            }
            GlobalServerMsg protocolId = (GlobalServerMsg)data[0];
            bool is_compressed = false;
            switch (protocolId)
            {
                case GlobalServerMsg.SERVER_MSG_BATCH_COMPRESS:
                    {
                        protocolId = GlobalServerMsg.SERVER_MSG_BATCH_COMPRESS;
                        is_compressed = true;
                    }
                    break;
                case GlobalServerMsg.SERVER_IDLE:
                    {
                        return;  //不处理
                    }
                case GlobalServerMsg.SERVER_CP_LOGIN_SUCCEED:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_LOGIN_SUCCEED;
                    break;
                case GlobalServerMsg.SERVER_CP_PROPERTY_TABLE:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_PROPERTY_TABLE;
                    break;
                case GlobalServerMsg.SERVER_CP_RECORD_TABLE:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_RECORD_TABLE;
                    break;
                case GlobalServerMsg.SERVER_CP_CUSTOM:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_CUSTOM;
                    break;
                case GlobalServerMsg.SERVER_CP_ADD_OBJECT:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_ADD_OBJECT;
                    break;
                case GlobalServerMsg.SERVER_CP_RECORD_ADDROW:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_RECORD_ADDROW;
                    break;
                case GlobalServerMsg.SERVER_CP_VIEW_ADD:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_VIEW_ADD;
                    break;
                case GlobalServerMsg.SERVER_CP_ALL_DEST:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_ALL_DEST;
                    break;
                case GlobalServerMsg.SERVER_CP_ALL_DEST_EX:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_ALL_DEST_EX;
                    break;
                case GlobalServerMsg.SERVER_CP_ALL_PROP:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_ALL_PROP;
                    break;
                case GlobalServerMsg.SERVER_CP_ADD_MORE_OBJECT:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_ADD_MORE_OBJECT;
                    break;
                case GlobalServerMsg.SERVER_CP_LOCATION_GRID:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_LOCATION_GRID;
                    break;
                case GlobalServerMsg.SERVER_CP_MOVING_GRID:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_MOVING_GRID;
                    break;
                case GlobalServerMsg.SERVER_CP_ALL_DEST_GRID:
                    is_compressed = true;
                    protocolId = GlobalServerMsg.SERVER_ALL_DEST_GRID;
                    break;
                default:
                    is_compressed = false;
                    break;
            }

            if (is_compressed)
            {
                int origin_size = QuickLZ.sizeDecompressed(data);
                if (origin_size > (65535 - 1))
                {
                    LogSystem.Log("(GameReceiver::ProcessMessage)decompress size error");
                    return;
                }

                data = QuickLZ.decompress(data);
                data[0] = System.Convert.ToByte(protocolId);
                size = origin_size + 1;
            }
            if (GlobalServerMsg.SERVER_MSG_BATCH_COMPRESS == (GlobalServerMsg)data[0])
            {
                byte nRecvPrior = 0;
                int nRecvCount = 0;

                for (int i = 1; i < size; ++i)
                {
                    if ((0xEE == data[i]) && (0xEE == nRecvPrior))
                    {
                        nRecvCount--;
                        if (nRecvCount > 0)
                        {
                            HandleMessage(m_pBuffer_Batch, nRecvCount);
                        }
                        nRecvPrior = 0;
                        nRecvCount = 0;
                        continue;
                    }
                    else if ((0 == data[i]) && (0xEE == nRecvPrior))
                    {
                    }
                    else
                    {
                        if (nRecvCount < m_pBuffer_Batch.Length)
                        {
                            m_pBuffer_Batch[nRecvCount++] = data[i];
                        }
                        else
                        {
                            //     出错了
                        }
                    }
                    nRecvPrior = data[i];

                }
                return;
            }
            if (m_bSimpleProtocal)
            {
                InnerSimpleProtocal(ref data, size);
            }
            //LogSystem.Log( "Receive msg id = " , protocolId );
            //根据不同的协议ID进行消息分发
            ExcuteEvent(protocolId, data, size);
        }
        /// <summary>
        /// 先按简单协议还原数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public int InnerSimpleProtocal(ref Byte[] data, int len)
        {
            if (data.Length == 0 || 0 == len)
            {
                //长度错误
                return len;
            }
            GlobalServerMsg nMsgType = (GlobalServerMsg)data[0];

            switch (nMsgType)
            {
                case GlobalServerMsg.SERVER_OBJECT_PROPERTY:
                    {
                        len = proc_server_object_property(ref data, len);
                    }
                    break;
                case GlobalServerMsg.SERVER_RECORD_GRID:
                    {
                        len = proc_server_record_grid(ref data, len);
                    }
                    break;
                case GlobalServerMsg.SERVER_RECORD_ADDROW:
                    {
                        len = proc_server_record_addrow(ref data, len);
                    }
                    break;
                case GlobalServerMsg.SERVER_RECORD_DELROW:
                    {
                        len = proc_server_record_delrow(ref data, len);
                    }
                    break;
                case GlobalServerMsg.SERVER_LOCATION:
                    {
                        len = proc_server_location(ref data, len);
                    }
                    break;
                case GlobalServerMsg.SERVER_MOVING:
                    {
                        len = proc_server_moving(ref data, len);
                    }
                    break;
                case GlobalServerMsg.SERVER_ALL_DEST:
                    {
                        len = proc_server_all_dest(ref data, len);
                    }
                    break;
            }
            return len;
        }
        /// <summary>
        /// 服务器对象属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private int proc_server_object_property(ref Byte[] data, int len)
        {
            byte[] buffer = null;
            if (len >= m_pBuffer_SP.Length)
            {
                buffer = new byte[len];
            }
            else
            {
                buffer = m_pBuffer_SP;
            }
            //就省一个字节
            buffer[0] = data[0];
            //Array.Copy(data, new_data, 1);
            Array.Copy(data, 1, buffer, 2, len - 1);
            ushort new_nCount = BitConverter.ToUInt16(buffer, 10);
            ushort nIsViewObj = BitUtils.GetValue(ref new_nCount, 14, 15);
            buffer[1] = (byte)nIsViewObj;

            byte[] nCount = BitConverter.GetBytes(new_nCount);
            buffer[10] = nCount[0];
            buffer[11] = nCount[1];

            data = buffer;

            return len + 1;
        }
        /// <summary>
        /// 服务器表数据行改变
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private int proc_server_record_grid(ref Byte[] data, int len)
        {

            // Byte[] new_data = new Byte[len + 3];
            byte[] buffer = null;
            if (len >= m_pBuffer_SP.Length)
            {
                buffer = new byte[len];
            }
            else
            {
                buffer = m_pBuffer_SP;
            }
            buffer[0] = data[0];
            //Array.Copy(data, new_data, 1);
            Array.Copy(data, 1, buffer, 2, 8);        //nIsViewObj
            Array.Copy(data, 9, buffer, 12, len - 9); //nIndex

            ushort new_nCount = BitConverter.ToUInt16(buffer, 12);
            ushort nIsViewObj = BitUtils.GetValue(ref new_nCount, 14, 15);
            ushort nIndex = BitUtils.GetValue(ref new_nCount, 6, 13);

            buffer[1] = (byte)nIsViewObj;
            buffer[10] = (byte)nIndex;
            buffer[11] = 0;

            byte[] nCount = BitConverter.GetBytes(new_nCount);

            buffer[12] = nCount[0];
            buffer[13] = nCount[1];

            data = buffer;

            return len + 3;
        }

        /// <summary>
        /// 服务器表添加行
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private int proc_server_record_addrow(ref Byte[] data, int len)
        {
            //就省一个字节
            byte[] buffer = null;
            if (len >= m_pBuffer_SP.Length )
            {
                buffer = new byte[len];
            }
            else
            {
                buffer = m_pBuffer_SP;
            }
            // Byte[] new_data = new Byte[len + 1];
            buffer[0] = data[0];
            //Array.Copy(data, new_data, 1);
            Array.Copy(data, 1, buffer, 2, len - 1);
            ushort new_nIndex = BitConverter.ToUInt16(buffer, 10);
            ushort nIsViewObj = BitUtils.GetValue(ref new_nIndex, 12, 15);
            buffer[1] = (byte)nIsViewObj;

            byte[] nIndex = BitConverter.GetBytes(new_nIndex);
            buffer[10] = nIndex[0];
            buffer[11] = nIndex[1];

            data = buffer;

            return len + 1;
        }
        /// <summary>
        /// 服务器删掉表消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private int proc_server_record_delrow(ref Byte[] data, int len)
        {
            byte[] buffer = null;
            if (len >= m_pBuffer_SP.Length)
            {
                buffer = new byte[len];
            }
            else
            {
                buffer = m_pBuffer_SP;
            }
            //就省一个字节
            // Byte[] new_data = new Byte[len + 1];
            buffer[0] = data[0];
            //Array.Copy(data, new_data, 1);
            Array.Copy(data, 1, buffer, 2, len - 1);
            ushort new_nIndex = BitConverter.ToUInt16(buffer, 10);
            ushort nIsViewObj = BitUtils.GetValue(ref new_nIndex, 12, 15);
            buffer[1] = (byte)nIsViewObj;

            byte[] nIndex = BitConverter.GetBytes(new_nIndex);
            buffer[10] = nIndex[0];
            buffer[11] = nIndex[1];

            data = buffer;

            return len + 1;
        }
        /// <summary>
        /// 服务器定位消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private int proc_server_location(ref Byte[] data, int len)
        {
            if (len < 16)
            {
                return len;
            }

            LoadArchive arLoad = LoadArchive.Load(data, len);
            // Byte[] buffer = new Byte[1024];
            StoreArchive arStore = StoreArchive.Load(m_pBuffer_SP, 4096);

            uint nMsgType = 0;
            arLoad.ReadUInt8(ref nMsgType);
            arStore.WriteUInt8(nMsgType);

            ObjectID objectId = ObjectID.zero;
            arLoad.ReadObject(ref objectId);
            arStore.WriteObject(objectId);

            int x = 0;
            arLoad.ReadInt16(ref x);
            arStore.WriteFloat(x / 100.0f);

            int y = 0;
            arLoad.ReadInt16(ref y);
            arStore.WriteFloat(y / 100.0f);

            int z = 0;
            arLoad.ReadInt16(ref z);
            arStore.WriteFloat(z / 100.0f);

            int orient = 0;
            arLoad.ReadInt16(ref orient);
            arStore.WriteFloat(orient / 100.0f);

            data = arStore.GetData();
            return arStore.GetLength();

        }
        /// <summary>
        /// 服务器移动消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private int proc_server_moving(ref Byte[] data, int len)
        {
            if (len < 28)
            {
                return len;
            }

            LoadArchive arLoad = LoadArchive.Load(data, len);
            // Byte[] buffer = new Byte[1024];
            StoreArchive arStore = StoreArchive.Load(m_pBuffer_SP, 4096);

            uint nMsgType = 0;
            arLoad.ReadUInt8(ref nMsgType);
            arStore.WriteUInt8(nMsgType);

            ObjectID objectId = ObjectID.zero;
            arLoad.ReadObject(ref objectId);
            arStore.WriteObject(objectId);

            int x = 0;
            arLoad.ReadInt16(ref x);
            arStore.WriteFloat(x / 100.0f);

            int y = 0;
            arLoad.ReadInt16(ref y);
            arStore.WriteFloat(y / 100.0f);

            int z = 0;
            arLoad.ReadInt16(ref z);
            arStore.WriteFloat(z / 100.0f);

            int orient = 0;
            arLoad.ReadInt16(ref orient);
            arStore.WriteFloat(orient / 100.0f);

            int moveSpeed = 0;
            arLoad.ReadInt16(ref moveSpeed);
            arStore.WriteFloat(moveSpeed / 100.0f);

            int rotateSpeed = 0;
            arLoad.ReadInt16(ref rotateSpeed);
            arStore.WriteFloat(rotateSpeed / 100.0f);

            int jumpSpeed = 0;
            arLoad.ReadInt16(ref jumpSpeed);
            arStore.WriteFloat(jumpSpeed / 100.0f);

            int gravity = 0;
            arLoad.ReadInt16(ref gravity);
            arStore.WriteFloat(gravity / 100.0f);

            int mode = 0;
            arLoad.ReadInt32(ref mode);
            arStore.WriteInt32(mode);

            data = arStore.GetData();
            return arStore.GetLength();

        }
        /// <summary>
        /// 服务器所有目标点移动
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private int proc_server_all_dest(ref Byte[] data, int len)
        {
            LoadArchive arLoad = LoadArchive.Load(data, len);
            // Byte[] buffer = new Byte[1024];
            StoreArchive arStore = StoreArchive.Load(m_pBuffer_SP, 4096);

            uint nMsgType = 0;
            arLoad.ReadUInt8(ref nMsgType);
            arStore.WriteUInt8(nMsgType);

            uint nCount = 0;
            arLoad.ReadUInt16(ref nCount);
            arStore.WriteUInt16(nCount);

            if (nCount > 50)
            {
                LogSystem.Log("proc_server_all_dest");
                return len;
            }
            for (int i = 0; i < nCount; ++i)
            {
                ObjectID objectId = ObjectID.zero;
                arLoad.ReadObject(ref objectId);
                arStore.WriteObject(objectId);

                int x = 0;
                arLoad.ReadInt16(ref x);
                arStore.WriteFloat(x / 100.0f);

                int y = 0;
                arLoad.ReadInt16(ref y);
                arStore.WriteFloat(y / 100.0f);

                int z = 0;
                arLoad.ReadInt16(ref z);
                arStore.WriteFloat(z / 100.0f);

                int orient = 0;
                arLoad.ReadInt16(ref orient);
                arStore.WriteFloat(orient / 100.0f);

                int moveSpeed = 0;
                arLoad.ReadInt16(ref moveSpeed);
                arStore.WriteFloat(moveSpeed / 100.0f);

                int rotateSpeed = 0;
                arLoad.ReadInt16(ref rotateSpeed);
                arStore.WriteFloat(rotateSpeed / 100.0f);

                int jumpSpeed = 0;
                arLoad.ReadInt16(ref jumpSpeed);
                arStore.WriteFloat(jumpSpeed / 100.0f);

                int gravity = 0;
                arLoad.ReadInt16(ref gravity);
                arStore.WriteFloat(gravity / 100.0f);

                int mode = 0;
                arLoad.ReadInt32(ref mode);
                arStore.WriteInt32(mode);
            }


            data = arStore.GetData();
            return arStore.GetLength();

        }
    }
}
