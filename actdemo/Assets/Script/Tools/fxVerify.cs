using System;

namespace Fm_ClientNet
{
    public class fxVerify
    {
        public const int ROLENAME_MAX_LENGTH = 32;
        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns>类型</returns>
        public delegate void FxVerify_GetType();
        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns>类型</returns>
        public delegate void FxVerify_GetInterface();
        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns>版本号</returns>
        public delegate int FxVerify_GetVersion();
        /// <summary>
        /// 设置客户端条件参数
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="cond"></param>
        public delegate void FxVerify_SetClientCond(Byte[] cond);

        /// <summary>
        /// 设置客户端代码参数
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="cond"></param>
        public delegate void FxVerify_SetClientCode(Byte[] cond, int nLen);

        /// <summary>
        /// 设置验证返回码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strIp"></param>
        /// <param name="nDynamicKey"></param>
        /// <param name="nPort"></param>
        /// <param name="strInfo"></param>
        /// <param name="nStrInfoLen"></param>
        /// <param name="strDecID"></param>
        /// <param name="nAddress"></param>
        /// <param name="output"></param>
        public delegate void FxVerify_GetRetEncodeVerify(string strIp, int nDynamicKey, int nPort,
            Byte[] strInfo, int nStrInfoLen, string strDecID, int nAddress, ref Byte[] output);

        /// <summary>
        /// 获取登录验证码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strInfo"></param>
        /// <param name="nStrInfoLen"></param>
        /// <param name="nPort"></param>
        /// <param name="strIp"></param>
        /// <param name="strDecID"></param>
        /// <param name="nAddress"></param>
        /// <param name="nDynamicKey"></param>
        /// <param name="output"></param>
        public delegate void FxVerify_GetLoginVerify(Byte[] strInfo, int nStrInfoLen, int nPort,
                                                          string strIp, string strDecID, int nAddress, int nDynamicKey, ref Byte[] output);

        /// <summary>
        /// 获取选择较色验证码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strInfo"></param>
        /// <param name="nStrInfoLen"></param>
        /// <param name="strIp"></param>
        /// <param name="nPort"></param>
        /// <param name="strDecID"></param>
        /// <param name="nDynamicKey"></param>
        /// <param name="szRoleName"></param>
        /// <param name="nRoleNameLen"></param>
        /// <param name="nAddress"></param>
        /// <param name="output"></param>
        public delegate void FxVerify_GetChooseRoleVerify(Byte[] strInfo, int nStrInfoLen,
                                                               string strIp, int nPort, string strDecID, int nDynamicKey, byte[] szRoleName, int nRoleNameLen, int nAddress, ref Byte[] output);

        /// <summary>
        /// 获取选择对象验证码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="nDynamicKey"></param>
        /// <param name="nFunctionId"></param>
        /// <param name="strIp"></param>
        /// <param name="nPort"></param>
        /// <param name="strDecID"></param>
        /// <param name="nPersistidSerial"></param>
        /// <param name="nSerial"></param>
        /// <param name="nPersistidIdent"></param>
        /// <param name="output"></param>
        public delegate void FxVerify_GetSelectVerify(int nDynamicKey, int nFunctionId,
                                                           string strIp, int nPort, string strDecID, int nPersistidSerial, int nSerial, int nPersistidIdent, ref Byte[] output);

        /// <summary>
        /// 获取自定义消息验证码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strIp"></param>
        /// <param name="nDynamicKey"></param>
        /// <param name="nPort"></param>
        /// <param name="nSerial"></param>
        /// <param name="nArgNum"></param>
        /// <param name="strDecID"></param>
        /// <param name="pArgData"></param>
        /// <param name="nArgLen"></param>
        /// <param name="output"></param>
        public delegate void FxVerify_GetCustomVerify(string strIp, int nDynamicKey, int nPort, int nSerial,
                                                           int nArgNum, string strDecID, Byte[] pArgData, int nArgLen, ref Byte[] output);

        /// <summary>
        /// 获取加密账号码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strAccount"></param>
        /// <param name="OutBuffer"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        public delegate bool FxVerify_EncodeAccount(string strAccount, ref Byte[] OutBuffer, ref int nSize);

        /// <summary>
        /// 获取加密密码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strPassword"></param>
        /// <param name="OutBuffer"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        public delegate bool FxVerify_EncodePassword(string strPassword, ref Byte[] OutBuffer, ref int nSize);

        /// <summary>
        /// 释放验证库
        /// </summary>
        /// <param name="pInstance"></param>
        /// <returns></returns>
        public delegate void FxVerify_Release();

        /// <summary>
        /// 验证库函数验证码库接口表
        /// </summary>

        private static FxVerify_GetInterface fn_FxVerify_GetInterface = null;
        private static FxVerify_GetVersion fn_FxVerify_GetVersion = null;
        private static FxVerify_SetClientCond fn_FxVerify_fnSetClientCond = null;
        private static FxVerify_SetClientCode fn_FxVerify_fnSetClientCode = null;
        private static FxVerify_GetRetEncodeVerify fn_FxVerify_GetRetEncodeVerify = null;
        private static FxVerify_GetLoginVerify fn_FxVerify_GetLoginVerify = null;
        private static FxVerify_GetChooseRoleVerify fn_FxVerify_GetChooseRoleVerify = null;
        private static FxVerify_GetSelectVerify fn_FxVerify_GetSelectVerify = null;
        private static FxVerify_GetCustomVerify fn_FxVerify_GetCustomVerify = null;
        private static FxVerify_EncodeAccount fn_FxVerify_EncodeAccount = null;
        private static FxVerify_EncodePassword fn_FxVerify_EncodePassword = null;
        private static FxVerify_Release fn_FxVerify_Release = null;


        /// <summary>
        /// 设置验证库接口之获取接口
        /// </summary>
        /// <returns>类型</returns>
        public static void SetFxVerify_GetInterface(FxVerify_GetInterface fnFxVerify_GetInterface)
        {
            fn_FxVerify_GetInterface = fnFxVerify_GetInterface;
        }
        /// <summary>
        /// 设置验证库接口之获取类型
        /// </summary>
        /// <returns>版本号</returns>
        public static void SetFxVerify_GetVersion(FxVerify_GetVersion fnFxVerify_GetVersion)
        {
            fn_FxVerify_GetVersion = fnFxVerify_GetVersion;
        }
        /// <summary>
        /// 设置验证库接口之设置客户端条件参数
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="cond"></param>
        public static void SetFxVerify_fnSetClientCond(FxVerify_SetClientCond fnFxVerify_SetClientCond)
        {
            fn_FxVerify_fnSetClientCond = fnFxVerify_SetClientCond;
        }
        /// <summary>
        /// 设置验证库接口之设置客户端代码参数
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="cond"></param>
        public static void SetFxVerify_fnSetClientCode(FxVerify_SetClientCode fnFxVerify_SetClientCode)
        {
            fn_FxVerify_fnSetClientCode = fnFxVerify_SetClientCode;
        }
        /// <summary>
        /// 设置验证库接口之设置验证返回码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strIp"></param>
        /// <param name="nDynamicKey"></param>
        /// <param name="nPort"></param>
        /// <param name="strInfo"></param>
        /// <param name="nStrInfoLen"></param>
        /// <param name="strDecID"></param>
        /// <param name="nAddress"></param>
        /// <param name="output"></param>
        public static void SetFxVerify_GetRetEncodeVerify(FxVerify_GetRetEncodeVerify fnFxVerify_GetRetEncodeVerify)
        {
            fn_FxVerify_GetRetEncodeVerify = fnFxVerify_GetRetEncodeVerify;
        }
        /// <summary>
        /// 设置验证库接口之获取登录验证码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strInfo"></param>
        /// <param name="nStrInfoLen"></param>
        /// <param name="nPort"></param>
        /// <param name="strIp"></param>
        /// <param name="strDecID"></param>
        /// <param name="nAddress"></param>
        /// <param name="nDynamicKey"></param>
        /// <param name="output"></param>
        public static void SetFxVerify_GetLoginVerify(FxVerify_GetLoginVerify fnFxVerify_GetLoginVerify)
        {
            fn_FxVerify_GetLoginVerify = fnFxVerify_GetLoginVerify;
        }
        /// <summary>
        /// 设置验证库接口之获取选择较色验证码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strInfo"></param>
        /// <param name="nStrInfoLen"></param>
        /// <param name="strIp"></param>
        /// <param name="nPort"></param>
        /// <param name="strDecID"></param>
        /// <param name="nDynamicKey"></param>
        /// <param name="szRoleName"></param>
        /// <param name="nRoleNameLen"></param>
        /// <param name="nAddress"></param>
        /// <param name="output"></param>
        public static void SetFxVerify_GetChooseRoleVerify(FxVerify_GetChooseRoleVerify fnFxVerify_GetChooseRoleVerify)
        {
            fn_FxVerify_GetChooseRoleVerify = fnFxVerify_GetChooseRoleVerify;
        }
        /// <summary>
        /// 设置验证库接口之获取选择对象验证码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="nDynamicKey"></param>
        /// <param name="nFunctionId"></param>
        /// <param name="strIp"></param>
        /// <param name="nPort"></param>
        /// <param name="strDecID"></param>
        /// <param name="nPersistidSerial"></param>
        /// <param name="nSerial"></param>
        /// <param name="nPersistidIdent"></param>
        /// <param name="output"></param>
        public static void SetFxVerify_GetSelectVerify(FxVerify_GetSelectVerify fnFxVerify_GetSelectVerify)
        {
            fn_FxVerify_GetSelectVerify = fnFxVerify_GetSelectVerify;
        }
        /// <summary>
        /// 设置验证库接口之获取自定义消息验证码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strIp"></param>
        /// <param name="nDynamicKey"></param>
        /// <param name="nPort"></param>
        /// <param name="nSerial"></param>
        /// <param name="nArgNum"></param>
        /// <param name="strDecID"></param>
        /// <param name="pArgData"></param>
        /// <param name="nArgLen"></param>
        /// <param name="output"></param>
        public static void SetFxVerify_GetCustomVerify(FxVerify_GetCustomVerify fnFxVerify_GetCustomVerify)
        {
            fn_FxVerify_GetCustomVerify = fnFxVerify_GetCustomVerify;
        }
        /// <summary>
        /// 设置验证库接口之获取加密账号码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strAccount"></param>
        /// <param name="OutBuffer"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        public static void SetFxVerify_EncodeAccount(FxVerify_EncodeAccount fnFxVerify_EncodeAccount)
        {
            fn_FxVerify_EncodeAccount = fnFxVerify_EncodeAccount;
        }
        /// <summary>
        /// 设置验证库接口之获取加密密码
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="strPassword"></param>
        /// <param name="OutBuffer"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        public static void SetFxVerify_EncodePassword(FxVerify_EncodePassword fnFxVerify_EncodePassword)
        {
            fn_FxVerify_EncodePassword = fnFxVerify_EncodePassword;
        }
        /// <summary>
        /// 设置验证库接口之释放验证库
        /// </summary>
        /// <param name="pInstance"></param>
        /// <returns></returns>
        public static void SetFxVerify_Release(FxVerify_Release fnFxVerify_Release)
        {
            fn_FxVerify_Release = fnFxVerify_Release;
        }

        /// <summary>
        /// 加密模块实例
        /// </summary>
        public static byte[] mbtInfo = null;
        public static int miDynamicKey = -1;
        public static int miEncodeId = -1;
        public static int miAddress = -1;
        public static byte[] mbtCondition = null;
        public static byte[] mbtData = null;
        public static string mstrDeviceUid = string.Empty;
        public static bool mbUseVerify = false;
        /// <summary>
        /// 初始化加密模块
        /// </summary>
        public static void Initalize(string strDeviceUid)
        {
            mstrDeviceUid = strDeviceUid;
            try
            {
                if (fn_FxVerify_GetInterface != null)
                {
                    fn_FxVerify_GetInterface();
                }
            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex.ToString());
            }
        }
        /// <summary>
        /// 设置加密是否使用
        /// </summary>
        /// <param name="bUse">是否使用</param>
        public static void SetVerifyUse(bool bUse)
        {
            mbUseVerify = bUse;
        }
        /// <summary>
        /// 卸载初始化模块
        /// </summary>
        public static void UnInitalize()
        {
            if (fn_FxVerify_Release != null)
            {
                fn_FxVerify_Release();
            }
        }
        /// <summary>
        /// 加密参数设置
        /// </summary>
        /// <param name="iDynamicKey">动态码</param>
        /// <param name="iEncodeId">加密编号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="btCondition">条件</param>
        /// <param name="btData">数据</param>
        public static void SetEncode(byte[] btInfo, int iDynamicKey, int iEncodeId, int iAddress, byte[] btCondition, byte[] btData)
        {
            mbtInfo = btInfo;
            miDynamicKey = iDynamicKey;
            miEncodeId = iEncodeId;
            miAddress = iAddress;
            mbtCondition = btCondition;
            mbtData = btData;

            ///设置加密参数
            SetClientCond(mbtCondition);
            SetClientCode(mbtData);
        }
        /// <summary>
        /// 当前登录的服务器
        /// </summary>
        private static string mstrServer;
        /// <summary>
        /// 服务器端口
        /// </summary>
        private static int miPort;
        public static void SetLoginInfo(string strServer, int iPort)
        {
            mstrServer = strServer;
            miPort = iPort;
        }
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        public static int GetVersion()
        {
            if (mbUseVerify)
            {
                if (fn_FxVerify_GetVersion != null)
                {
                    return fn_FxVerify_GetVersion();
                }
            }

            return 0x300;
        }
        // 收到服务端 SERVER_MSG_SET_ENCODE 把 nCondition 设置进来
        public static void SetClientCond(Byte[] cond)
        {
            if (cond != null)
            {
                if (fn_FxVerify_fnSetClientCond != null)
                {
                    fn_FxVerify_fnSetClientCond(cond);
                }
            }
        }
        // 收到服务端 SERVER_MSG_SET_ENCODE 把 pData 设置进来
        public static void SetClientCode(Byte[] code)
        {
            if (code != null)
            {
                try
                {
                    if (fn_FxVerify_fnSetClientCode != null)
                    {
                        fn_FxVerify_fnSetClientCode(code, code.Length);
                    }
                }
                catch (System.Exception ex)
                {
                    LogSystem.LogError(ex.ToString());
                }
            }
        }

        // 发送CLIENT_RET_ENCODE 时参数 nVerify 这边获得
        // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
        public static bool GetRetEncodeVerify(ref Byte[] output)
        {
            if (mbUseVerify)
            {
                if (mbtInfo == null)
                    return false;

                if (fn_FxVerify_GetRetEncodeVerify != null)
                {
                    fn_FxVerify_GetRetEncodeVerify(mstrServer, miDynamicKey, miPort, mbtInfo, mbtInfo.Length, mstrDeviceUid, miAddress, ref output);
                    return true;
                }
            }
            return false;
        }

        // 发送CLIENT_LOGIN 时参数 nVerify 这边获得
        // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
        public static bool GetLoginVerify(ref Byte[] output)
        {
            if (mbUseVerify)
            {
                if (fn_FxVerify_GetLoginVerify != null)
                {
                    fn_FxVerify_GetLoginVerify(null, 0, miPort, mstrServer, mstrDeviceUid, miAddress, miDynamicKey, ref output);

                    return true;
                }
            }
            return false;
        }

        // 发送CLIENT_CHOOSE_ROLE 时参数 nVerify 这边获得
        // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
        public static bool GetChooseRoleVerify(string strRoleName, ref Byte[] output)
        {
            if (mbUseVerify)
            {
                if (mbtInfo == null)
                    return false;

                byte[] roleNameDst = new byte[(ROLENAME_MAX_LENGTH + 1) * 2 + 1];
                // 截断
                int val_len = strRoleName.Length;
                int iLen = (ROLENAME_MAX_LENGTH + 1) * 2;
                if (val_len >= (iLen / 2))
                {
                    val_len = (iLen / 2) - 1;
                }
                int iCurPosi = 0;
                for (int c = 0; c < iLen; c++)
                {
                    roleNameDst[iCurPosi + c] = 0;
                }

                for (int c = 0; c < val_len; c++)
                {
                    BitConverter.GetBytes(strRoleName[c]).CopyTo(roleNameDst,
                                                                 iCurPosi + c * 2);
                }

                if (fn_FxVerify_GetChooseRoleVerify != null)
                {
                    fn_FxVerify_GetChooseRoleVerify(mbtInfo, mbtInfo.Length, mstrServer, miPort, mstrDeviceUid, miDynamicKey, roleNameDst, 66, miAddress, ref output);
                    return true;
                }
            }
            return false;
        }

        // 发送CLIENT_SELECT 时参数 nVerify 这边获得
        // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
        public static bool GetSelectVerify(int nFunctionId, int nPersistidSerial, int iCustomIndex, int nPersistidIdent, ref Byte[] output)
        {
            if (mbUseVerify)
            {
                if (mbtInfo == null)
                    return false;

                if (fn_FxVerify_GetSelectVerify != null)
                {

                    fn_FxVerify_GetSelectVerify(miDynamicKey, nFunctionId, mstrServer, miPort, mstrDeviceUid, nPersistidSerial, iCustomIndex, nPersistidIdent, ref output);

                    return true;
                }
            }
            return false;
        }

        // 发送CLIENT_CUSTOM 时参数 nVerify 这边获得
        // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
        public static bool GetCustomVerify(int iCustomIndex, int nArgNum, Byte[] pArgData, int nArgLen, ref Byte[] output)
        {
            if (mbUseVerify)
            {
                if (mbtInfo == null)
                    return false;

                if (fn_FxVerify_GetCustomVerify != null)
                {
                    fn_FxVerify_GetCustomVerify(mstrServer, miDynamicKey, miPort, iCustomIndex, nArgNum, mstrDeviceUid, pArgData, nArgLen, ref output);
                    return true;
                }
            }
            return false;
        }

        // 发送CLIENT_LOGIN 时参数 账户加密
        public static bool EncodeAccount(string strAccount, ref Byte[] pOutBuffer, ref int nSize)
        {
            /*
            if (mbUseVerify)
            {
                if (fn_FxVerify_EncodeAccount != null)
                {
                    return fn_FxVerify_EncodeAccount(strAccount, ref pOutBuffer, ref nSize); ;
                }
            }*/
            return false;
        }
        // 发送CLIENT_LOGIN 时参数 密码加密
        public static bool EncodePassword(string strPassword, ref Byte[] pOutBuffer, ref int nSize)
        {   /*
            if (mbUseVerify)
            {
                if (fn_FxVerify_EncodePassword != null)
                {
                    return fn_FxVerify_EncodePassword(strPassword, ref pOutBuffer, ref  nSize);
                }
            }*/
            return false;
        }
    }
}
