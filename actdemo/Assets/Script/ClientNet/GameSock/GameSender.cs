//--------------------------------------------------------------------
// 文件名	:	GameSender.cs
// 内  容	:
// 说  明	:   消息发送器
// 创建日期	:	2014年06月12日
// 创建人	:	丁有进
// 版权所有	:	苏州蜗牛电子有限公司
//--------------------------------------------------------------------

using System;
using SysUtils;

using Fm_ClientNet.Interface;
using System.Text;

namespace Fm_ClientNet
{
    public class GameSender : IGameSender
    {
        public static int OUTER_PROTOCOL_VERSION = 0x300;
        private UserSock m_sender = null;
        private byte[] m_buffer = new byte[512];
        private IGameReceiver mRecver = null;

        public void SetSocket(ref UserSock sender)
        {
            m_sender = sender;
        }

        public void SetReceiver(IGameReceiver gamerecver)
        {
            mRecver = gamerecver;
        }
        public bool ClientReady()
        {
            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_READY);//消息ID
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }
        /// <summary>
        /// 发送加密确认消息
        /// </summary>
        /// <returns>发送是否成</returns>
        public bool ClientRetEncode()
        {
            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_RET_ENCODE);//消息ID
            ar.WriteUserDataNoLen(fxVerify.mbtInfo);
            ar.WriteInt32(fxVerify.miAddress);
            byte[] btVerify = new byte[16];
            //如果下面没再使用output 就要保证对象不被垃圾回收掉

            fxVerify.GetRetEncodeVerify(ref btVerify);
            ar.WriteUserDataNoLen(btVerify);

            return m_sender.Send(ar.GetData(), ar.GetLength());
        }
        public bool GetVerify()
        {
            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_GET_VERIFY);//消息ID
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        public bool Speech(string info)
        {
            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_SPEECH);//消息ID
            ar.WriteWideStrNoLen(info);
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        public bool GetWorldInfo(int type)
        {
            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_WORLD_INFO);//消息ID
            ar.WriteInt32(type);
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }


        public bool ChooseRole(string role_name)
        {
            if (role_name == null)
            {
                LogSystem.LogError("Role Name Is Empty!");
                return false;
            }

            if (0 == role_name.Length)
            {
                LogSystem.Log("sendChooseRole packet role name is empty!");
                return false;
            }

            byte[] name = new byte[ServerInfo.ROLENAME_MAX_LENGTH * 2 + 1];
            Array.Copy(System.Text.Encoding.Default.GetBytes(role_name), name, role_name.Length);

            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_CHOOSE_ROLE);//消息ID

            //unsigned short wsName[OUTER_OBJNAME_LENGTH + 1];	// 名称
            //unsigned char nVerify[16];							// 校验码
            //char strInfo[1];	

            ar.WriteUnicodeLen(role_name, (ServerInfo.ROLENAME_MAX_LENGTH + 1) * 2);//玩家名
            byte[] verify = new byte[16];

            fxVerify.GetChooseRoleVerify(role_name, ref verify);
            ar.WriteUserDataNoLen(verify);//检验码
            ar.WriteInt8(0);//附加信息
            return m_sender.Send(ar.GetData(), ar.GetLength());

        }
        public bool AddMsgVarList(StoreArchive ar, VarList args, int beg, int end)
        {
            try
            {
                for (int i = beg; i < end; ++i)
                {
                    switch (args.GetType(i))
                    {
                        case VarType.Int:
                            ar.WriteInt8(VarType.Int);
                            ar.WriteInt32(args.GetInt(i));
                            break;
                        case VarType.Int64:
                            ar.WriteInt8(VarType.Int64);
                            ar.WriteInt64(args.GetInt64(i));
                            break;
                        case VarType.Float:
                            ar.WriteInt8(VarType.Float);
                            ar.WriteFloat(args.GetFloat(i));
                            break;
                        case VarType.Double:
                            ar.WriteInt8(VarType.Double);
                            ar.WriteDouble(args.GetDouble(i));
                            break;
                        case VarType.String:
                            ar.WriteInt8(VarType.String);
                            ar.WriteString(args.GetString(i));
                            break;
                        case VarType.WideStr:
                            ar.WriteInt8(VarType.WideStr);
                            ar.WriteWideStr(args.GetWideStr(i));
                            break;
                        case VarType.Object:
                            ar.WriteInt8(VarType.Object);
                            ar.WriteObject(args.GetObject(i));
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex.ToString());
                return false;
            }

            return true;
        }
        public bool ReviveRole(string role_name)
        {
            if (role_name == null)
            {
                LogSystem.LogError("Role Name Is Empty!");
                return false;
            }

            if (0 == role_name.Length)
            {
                LogSystem.Log("ReviveRole  role name is empty!");
                return false;
            }


            byte[] name = new byte[ServerInfo.ROLENAME_MAX_LENGTH * 2 + 1];
            Array.Copy(System.Text.Encoding.Default.GetBytes(role_name), name, role_name.Length);

            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_REVIVE_ROLE);//消息ID

            //unsigned short wsName[OUTER_OBJNAME_LENGTH + 1];	// 名称

            ar.WriteUnicodeLen(role_name, (ServerInfo.ROLENAME_MAX_LENGTH + 1) * 2);//玩家名
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        public bool DeleteRole(string role_name)
        {
            if (role_name == null)
            {
                LogSystem.LogError("Role Name Is Empty!");
                return false;
            }

            if (0 == role_name.Length)
            {
                LogSystem.Log("DeleteRole  role name is empty!");
                return false;
            }


            byte[] name = new byte[ServerInfo.ROLENAME_MAX_LENGTH * 2 + 1];
            Array.Copy(System.Text.Encoding.Default.GetBytes(role_name), name, role_name.Length);

            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_DELETE_ROLE);//消息ID

            //unsigned short wsName[OUTER_OBJNAME_LENGTH + 1];	// 名称

            ar.WriteUnicodeLen(role_name, (ServerInfo.ROLENAME_MAX_LENGTH + 1) * 2);//玩家名
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }
        static int mCustomIndex = 0;
        public bool Select(string object_ident, int func_id)
        {
            if (object_ident == null)
            {
                LogSystem.LogError("GameSender Select object_ident is null");
                return false;
            }

            if (0 == object_ident.Length)
            {
                LogSystem.LogError("GameSender Select object_ident is empty");
                return false;
            }


            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_SELECT);//消息ID
            //unsigned char nVerify[16];	// 校验码
            //int nSerial;				// 消息序列号	
            //outer_object_t ObjectId;	// 对象标识
            //int nFunctionId;			// 功能号为0表示执行缺省功能
            //byte[] message = TypeConvert.encode(m_buffer,ar.GetLength());
            ObjectID obj = ObjectID.FromString(object_ident);
            byte[] verify = new byte[16];

            fxVerify.GetSelectVerify(func_id, (int)(obj.m_nSerial), mCustomIndex, (int)(obj.m_nIdent), ref verify);
            ar.WriteUserDataNoLen(verify);//检验码

            ar.WriteInt32(mCustomIndex++);//消息序列号
            ar.WriteObject(obj);//对象标识
            ar.WriteInt32(func_id);

            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        public bool GetWorldInfo2(int type, string info)
        {
            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           

            ar.WriteInt8(GlobalClineMsgId.CLIENT_WORLD_INFO);//消息ID
            ar.WriteInt32(type);
            ar.WriteWideStr(info);
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }
        private byte[] varifyByString = new byte[16];
        public bool LoginByString(string account, string login_string, string device_uid)
        {
            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_LOGIN);//消息ID
            int nVersion = fxVerify.GetVersion();
            ar.WriteInt32(nVersion);//版本号

            Array.Clear(varifyByString, 0, varifyByString.Length);
            fxVerify.GetLoginVerify(ref varifyByString);
            ar.WriteUserDataNoLen(varifyByString);

           
            byte[] btAccount = null;
            byte[] btPassword = null;
			int iAccountSize = 0;
			int iPasswordSize = 0;
            bool bVerifyok1 = fxVerify.EncodeAccount(account, ref btAccount, ref iAccountSize);
            bool bVerifyok2 = fxVerify.EncodePassword("", ref btPassword, ref iPasswordSize);
            if (bVerifyok1 && bVerifyok2)

            { 
                ar.WriteUserData(btAccount);//账号
                ar.WriteUserData(btPassword);//密码
            }
            else
            {
                ar.WriteString(account);               
				ar.WriteString("");
            }

            ar.WriteString(login_string);//登录串
            ar.WriteInt32(2);//登录类型
            ar.WriteInt8(0); ///是否是本地副本
            ar.WriteString(device_uid);

            string strPropMd5 = string.Empty;
            string strRecdMd5 = string.Empty;
            if (mRecver != null)
            {
                strPropMd5 = mRecver.GetPropertyTableMd5();
                strRecdMd5 = mRecver.GetRecordTableMd5();
            }

            if (strPropMd5 != null)
            {
                ar.WriteString(strPropMd5);
            }
            else
            {
                ar.WriteString(string.Empty);
            }

            if (strRecdMd5 != null)
            {
                ar.WriteString(strRecdMd5);
            }
            else
            {
                ar.WriteString(string.Empty);
            }
            ar.WriteInt8(0);
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }
        private byte[] varifyLogin = new byte[16];
        public bool Login(string account, string password, string device_uid)
        {
            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_LOGIN);//消息ID
            int nVersion = fxVerify.GetVersion();
            ar.WriteInt32(nVersion);//版本号
            Array.Clear(varifyLogin, 0, varifyLogin.Length);
            fxVerify.GetLoginVerify(ref varifyLogin);
            ar.WriteUserDataNoLen(varifyLogin);

            byte[] btAccount = null;
            byte[] btPassword = null;
            int iAccountSize = 0;
            int iPasswordSize = 0;
            bool bVerifyok1 = fxVerify.EncodeAccount(account, ref btAccount, ref iAccountSize);
            bool bVerifyok2 = fxVerify.EncodePassword(password, ref btPassword, ref iPasswordSize);
            if (bVerifyok1 && bVerifyok2)
            {
                ar.WriteUserData(btAccount);//账号
                ar.WriteUserData(btPassword);//密码
            }
            else
            {
                ar.WriteString(account);
                ar.WriteString(password);
            }
            ar.WriteString("0");//登录串
            ar.WriteInt32(1);//登录类型
            ar.WriteInt8(0);
            ar.WriteString(device_uid);
            string strPropMd5 = string.Empty;
            string strRecdMd5 = string.Empty;
            if (mRecver != null)
            {
                strPropMd5 = mRecver.GetPropertyTableMd5();
                strRecdMd5 = mRecver.GetRecordTableMd5();
            }

            if (strPropMd5 != null)
            {
                ar.WriteString(strPropMd5);
            }
            else
            {
                ar.WriteString(string.Empty);
            }

            if (strRecdMd5 != null)
            {
                ar.WriteString(strRecdMd5);
            }
            else
            {
                ar.WriteString(string.Empty);
            }

            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        /// <summary>
        /// 断线重连接
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="pswd">密码</param>
        /// <param name="validate_string">计费串</param>
        /// <param name="device_uid">设备号</param>
        /// <param name="in_stub">是否是本地副本（false）</param>
        /// <returns></returns>
        private byte[] varifyLoginReconnect = new byte[16];
        public bool LoginReconnect(string account, string pswd, string validate_string, string device_uid, bool in_stub = false, string strRoleName = "", string strRoleInfo = "")
        {
            if (account == null || pswd == null || validate_string == null || device_uid == null)
                return false;

            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_LOGIN);//消息ID
            int iVerson = fxVerify.GetVersion();
            ar.WriteInt32(iVerson);//版本号
            Array.Clear(varifyLoginReconnect, 0, varifyLoginReconnect.Length);
            fxVerify.GetLoginVerify(ref varifyLoginReconnect);
            ar.WriteUserDataNoLen(varifyLoginReconnect);

            byte[] btAccount = null;
            byte[] btPassword = null;
            int iAccountSize = 0;
            int iPasswordSize = 0;
            bool bVerifyok1 = fxVerify.EncodeAccount(account, ref btAccount, ref iAccountSize);
            bool bVerifyok2 = fxVerify.EncodePassword(pswd, ref btPassword, ref iPasswordSize);
            if (bVerifyok1 && bVerifyok2)
            {
                ar.WriteUserData(btAccount);//账号
                ar.WriteUserData(btPassword);//密码
            }
            else
            {
                ar.WriteString(account);
                ar.WriteString(pswd);
            }
            ar.WriteString(validate_string); //登录串
            ar.WriteInt32(100);                     //登录类型 LOGIN_TYPE_RECONNECT 100
            if (in_stub)
            {
                ar.WriteInt8(1);
            }
            else
            {
                ar.WriteInt8(0);
            }
            ar.WriteString(device_uid);
            string strPropMd5 = string.Empty;
            string strRecdMd5 = string.Empty;
            if (mRecver != null)
            {
                strPropMd5 = mRecver.GetPropertyTableMd5();
                strRecdMd5 = mRecver.GetRecordTableMd5();
            }

            if (strPropMd5 != null)
            {
                ar.WriteString(strPropMd5);
            }
            else
            {
                ar.WriteString(string.Empty);
            }

            if (strRecdMd5 != null)
            {
                ar.WriteString(strRecdMd5);
            }
            else
            {
                ar.WriteString(string.Empty);
            }
            //发送角色名称
            if (strRoleName != null)
            {
                ar.WriteWideStr(strRoleName);
            }
            else
            {
                ar.WriteWideStr(string.Empty);
            }
            //发送角色名称
            if (strRoleInfo != null)
            {
                ar.WriteString(strRoleInfo);
            }
            else
            {
                ar.WriteString(string.Empty);
            }
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }
        public bool CreateRole(ref VarList args)
        {
            try
            {
                if ((args.GetCount() < 2) || (args.GetType(1) != VarType.WideStr))
                {
                    LogSystem.Log("arguments error");
                    return false;
                }

                string roleName = args.GetWideStr(1);
                //role name
                byte[] name = new byte[ServerInfo.ROLENAME_MAX_LENGTH * 2 + 1];
        
                Array.Copy(System.Text.Encoding.Default.GetBytes(roleName), name, roleName.Length);

                //verify
                // byte[] verify = new byte[16];

                StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
               
                ar.WriteInt8(GlobalClineMsgId.CLIENT_CREATE_ROLE);
                ar.WriteInt32(args.GetInt(0));
                ar.WriteUnicodeLen(roleName, (ServerInfo.ROLENAME_MAX_LENGTH + 1) * 2);
                ///ar.WriteUserDataNoLen(verify);
                ar.WriteUInt16((uint)(args.GetCount() - 2));
                ///ar.WriteInt32();

                if (!AddMsgVarList(ar, args, 2, args.GetCount()))
                    return false;

                return m_sender.Send(ar.GetData(), ar.GetLength());
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
                return false;
            }
        }
        private byte[] varifyByShield = new byte[16];
        public bool LoginByShield(string account, string password, string login_string, string device_uid)
        {
            if (account == null || password == null || login_string == null || device_uid == null)
                return false;

            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_LOGIN);//消息ID
            ar.WriteInt32(OUTER_PROTOCOL_VERSION);//版本号
            Array.Clear(varifyByShield, 0, varifyByShield.Length);
            fxVerify.GetLoginVerify(ref varifyByShield);
            ar.WriteUserDataNoLen(varifyByShield);

            byte[] btAccount = null;
            byte[] btPassword = null;
            int iAccountSize = 0;
            int iPasswordSize = 0;
            bool bVerifyok1 = fxVerify.EncodeAccount(account, ref btAccount, ref iAccountSize);
            bool bVerifyok2 = fxVerify.EncodePassword(password, ref btPassword, ref iPasswordSize);
            if (bVerifyok1 && bVerifyok2)
            {
                ar.WriteUserData(btAccount);//账号
                ar.WriteUserData(btPassword);//密码
            }
            else
            {
                ar.WriteString(account);
                ar.WriteString(password);
            }
            ar.WriteString(login_string); //登录串
            ar.WriteInt32(4);                     //登录类型 LOGIN_TYPE_RECONNECT 100
            ar.WriteInt8(0); ///是否是本地副本
            ar.WriteString(device_uid);

            string strPropMd5 = string.Empty;
            string strRecdMd5 = string.Empty;
            if (mRecver != null)
            {
                strPropMd5 = mRecver.GetPropertyTableMd5();
                strRecdMd5 = mRecver.GetRecordTableMd5();
            }
            if (strPropMd5 != null)
            {
                ar.WriteString(strPropMd5);
            }
            else
            {
                ar.WriteString(string.Empty);
            }
            if (strRecdMd5 != null)
            {
                ar.WriteString(strRecdMd5);
            }
            else
            {
                ar.WriteString(string.Empty);
            }

            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        //发送带验证码的登录消息
        public bool LoginVerify(string account, string password, string verify, string device_uid)
        {
            if (account == null || password == null || verify == null || device_uid == null)
                return false;

            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_LOGIN);//消息ID
            ar.WriteInt32(OUTER_PROTOCOL_VERSION);//版本号

            byte[] varify = Encoding.Default.GetBytes(verify);
            ar.WriteUserDataNoLen(varify);//验证码
            byte[] btAccount = null;
            byte[] btPassword = null;
            int iAccountSize = 0;
            int iPasswordSize = 0;
            bool bVerifyok1 = fxVerify.EncodeAccount(account, ref btAccount, ref iAccountSize);
            bool bVerifyok2 = fxVerify.EncodePassword(password, ref btPassword, ref iPasswordSize);
            if (bVerifyok1 && bVerifyok2)
            {
                ar.WriteUserData(btAccount);//账号
                ar.WriteUserData(btPassword);//密码
            }
            else
            {
                ar.WriteString(account);
                ar.WriteString(password);
            }
            ar.WriteString(string.Empty); //登录串
            ar.WriteInt32(1);                     //登录类型 LOGIN_TYPE_RECONNECT 100
            ar.WriteInt8(0); ///是否是本地副本
            ar.WriteString(device_uid);

            string strPropMd5 = string.Empty;
            string strRecdMd5 = string.Empty;
            if (mRecver != null)
            {
                strPropMd5 = mRecver.GetPropertyTableMd5();
                strRecdMd5 = mRecver.GetRecordTableMd5();
            }
            if (strPropMd5 != null)
            {
                ar.WriteString(strPropMd5);
            }
            else
            {
                ar.WriteString(string.Empty);
            }
            if (strRecdMd5 != null)
            {
                ar.WriteString(strRecdMd5);
            }
            else
            {
                ar.WriteString(string.Empty);
            }

            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        //发送请求移动消息
        //public bool RequestMove(int mode, int arg_num, float[] args, string info)
        public bool RequestMove(ref VarList args, ref VarList ret)
        {
            try
            {
                if (args.GetCount() < 1)
                {
                    ret.AddBool(false);
                    return false;
                }

                int mode = args.GetInt(0);
                int arg_num = args.GetCount() - 1;

                StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
                ar.WriteInt8(GlobalClineMsgId.CLIENT_REQUEST_MOVE);//消息ID
                ar.WriteInt8(mode);
                ar.WriteInt16(arg_num);

                for (int i = 0; i < arg_num; i++)
                {
                    float value = args.GetFloat(1 + i);
                    ar.WriteFloat(value);
                }

                return m_sender.Send(ar.GetData(), ar.GetLength());
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
                return false;
            }
        }

        private byte[] varifyCustom = new byte[16];
        public bool Custom(ref VarList args)
        {
            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           

            int iCount = args.GetCount();
            if (!AddMsgVarList(ref ar, ref args, 0, iCount))
            {
                return false;
            }
            Array.Clear(varifyCustom, 0, varifyCustom.Length);
            fxVerify.GetCustomVerify(mCustomIndex, iCount, ar.GetData(), ar.GetLength(), ref varifyCustom);

            ar = StoreArchive.Load(m_buffer, m_buffer.Length);

            ar.WriteInt8(GlobalClineMsgId.CLIENT_CUSTOM);//消息ID
            ar.WriteUserDataNoLen(varifyCustom);//校验码

            ar.WriteInt32(mCustomIndex++);
            ar.WriteInt16(iCount);

            if (!AddMsgVarList(ref ar, ref args, 0, iCount))
            {
                LogSystem.Log("add para error");
                return false;
            }
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        bool AddMsgVarList(ref StoreArchive storeAr, ref VarList args,
            int beg, int end)
        {
            try
            {
                for (int i = beg; i < end; i++)
                {
                    switch (args.GetType(i))
                    {
                        case VarType.Int:
                            {
                                storeAr.WriteInt8(VarType.Int);
                                storeAr.WriteInt32(args.GetInt(i));
                            }
                            break;
                        case VarType.Int64:
                            {
                                storeAr.WriteInt8(VarType.Int64);
                                storeAr.WriteInt64(args.GetInt64(i));
                            }
                            break;
                        case VarType.Float:
                            {
                                storeAr.WriteInt8(VarType.Float);
                                storeAr.WriteFloat(args.GetFloat(i));
                            }
                            break;
                        case VarType.Double:
                            {
                                storeAr.WriteInt8(VarType.Double);
                                storeAr.WriteDouble(args.GetDouble(i));
                            }
                            break;
                        case VarType.String:
                            {
                                storeAr.WriteInt8(VarType.String);
                                storeAr.WriteString(args.GetString(i));
                            }
                            break;
                        case VarType.WideStr:
                            {
                                storeAr.WriteInt8(VarType.WideStr);
                                storeAr.WriteWideStr(args.GetWideStr(i));
                            }
                            break;
                        case VarType.Object:
                            {
                                storeAr.WriteInt8(VarType.Object);
                                storeAr.WriteObject(args.GetObject(i));
                            }
                            break;
                        default:
                            LogSystem.Log("unkown data type");
                            break;
                    }//end switch
                }//end for

            }
            catch (Exception ex)
            {
                LogSystem.Log("Exception:", ex.ToString());
                return false;
            }//end try catch
            return true;
        }
        /// <summary>
        /// 发送心跳消息
        /// </summary>
        /// <returns></returns>
        public bool SendBeat()
        {
            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
           
            ar.WriteInt8(GlobalClineMsgId.CLIENT_BEAT);
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }
        /// <summary>
        /// 网络延时检测序号
        /// </summary>
        private uint uSerial = 0;
        /// <summary>
        /// 发送网络延时检测消息
        /// </summary>
        /// <param name="uType"></param>
        /// <returns></returns>
        public bool SendTracert(uint uType = 0)
        {
            StoreArchive ar = StoreArchive.Load(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_TRACERT);
            //毫秒数
            long clientsteamp = (long)DateTime.Now.Ticks / 10000 & 0xfffffff;
            // 客户端的时间戳，精确到毫秒数
            ar.WriteUInt32((uint)clientsteamp);
            ar.WriteUInt8(uType);
            uSerial = (++uSerial) % 1000;
            ar.WriteUInt32(uSerial);
            ar.WriteUInt16(0);

            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

    }
}

