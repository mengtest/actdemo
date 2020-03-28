//--------------------------------------------------------------------
// 文件名	:	GameSock.cs
// 内  容	:
// 说  明	:
// 创建日期	:	2014年06月10日
// 创建人	:	丁有进
// 版权所有	:	苏州蜗牛电子有限公司
//--------------------------------------------------------------------

using System;
using System.Collections.Generic;

using System.Net.Sockets;

using SysUtils;
using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
    public class GameSock : IGameSock
    {
        public GameSock()
        {

        }
        //sock 接口回调相关
        public class SockCall : IUserSockCallee
        {
            private GameSock m_gamesock = null;
            //private bool m_bCallfun;    //是否使用回调函数 true 回调函数 false 接口方法

            public void SetGameSock(GameSock sock)
            {
                m_gamesock = sock;
            }

            public int OnSockConnected(UserSock sock, string addr, int port)
            {
                LogSystem.Log("OnSockConnected");
                m_gamesock.SetConnected(true);
                if (null != m_gamesock.m_lgsockcall)
                {
                    m_gamesock.m_lgsockcall.on_connected(addr, port);
                    return 1;
                }
                else
                { 
                    if (m_gamesock != null)
                    {
                        VarList args = VarList.GetVarList();
                        args.AddString(addr);
                        args.AddInt(port);
                        m_gamesock.Excute_CallBack("on_connected", args);
                        args.Collect();
                        return 1;
                    }
                    else
                    {
                        LogSystem.LogError("Error, OnSockConnected gamesock is null");
                    }
                
                    return 0;
                }
            }


            public int OnSockConnectFail(UserSock sock, string addr, int port)
            {
                LogSystem.Log("OnSockConnectFail");

                PromptManager.Instance.ShowPromptUI(XmlManager.Instance.GetCommonText("System0001"));

                m_gamesock.SetConnected(false);
                if (null != m_gamesock.m_lgsockcall)
                {
                    m_gamesock.m_lgsockcall.on_connect_failed(addr, port);
                    return 1;
                }
                else
                {
                    if (m_gamesock != null)
                    {
                        VarList args = VarList.GetVarList();
                        args.AddString(addr);
                        args.AddInt(port);
                        m_gamesock.Excute_CallBack("on_connect_fail", args);
                        args.Collect();
                        return 1;
                    }
                    else
                    {
                        LogSystem.LogError("Error, OnSockConnectFail gamesock is null");
                    }
                    return 0;
                }
            }


            public int OnSockClose(UserSock sock)
            {
                m_gamesock.SetConnected(false);
                if (null != m_gamesock.m_lgsockcall)
                {
                    m_gamesock.m_lgsockcall.on_close();
                    return 1;
                }
                else
                {
                    if (m_gamesock != null)
                    {
                        VarList varlist = VarList.GetVarList();
                        m_gamesock.Excute_CallBack("on_close", varlist);
                        varlist.Collect();
                        return 1;

                    }
                    else
                    {
                        LogSystem.LogError("Error, OnSockClose gamesock is null");
                    }
                    return 0;
                }
            }

            public int OnSockReceive(UserSock sock, byte[] data, int size)
            {

                m_gamesock.ProcessMsg(data, size);
                return 1;
            }

            public int OnSockSend(UserSock sock, int size)
            {
                return 1;
            }

            public int OnSockLog(UserSock sock, string msg)
            {
                LogSystem.Log("OnSockLog ", msg);
                return 1;
            }

            public int OnSockError(UserSock sock, int error_type, string error_msg)
            {
                LogSystem.LogError("Error, OnSockError error_type: ", error_type.ToString(), " error_msg:", error_msg);
                return 1;
            }
        }


        //消息事件回调
        //public delegate void CallBack(VarList args);
        private Dictionary<string, CallBack> m_CallBacks = new Dictionary<string, CallBack>();

        private GameReceiver m_gameReceiver = null;
        private GameSender m_gameSender = null;

        private UserSock m_scoket = null;

        private SockCall m_sockcall = null;

        private ISockCallee m_lgsockcall = null;
        private Socket m_sock = null;

        private bool mbConnected = false;
        private long miRecvLastMsgTime = 0;
        private long miLastSendBeatTime = 0;
        private long miCheckTime = 0;
        private bool mbSocketBlock = false;

        public void SetConnected(bool bConnected)
        {
            if (bConnected)
            {
                mbSocketBlock = false;
                miRecvLastMsgTime = DateTime.Now.Ticks / 10000000;
                miCheckTime = miRecvLastMsgTime;
            }
            mbConnected = bConnected;
        }

        public bool IsNetBlock()
        {
            return mbSocketBlock;
        }

        public bool Init(ISockCallee sockcall)
        {
            m_lgsockcall = sockcall;
            m_sockcall = new SockCall();

            m_sockcall.SetGameSock(this);

            m_gameSender = new GameSender();
            m_gameReceiver = new GameReceiver();
            InitUserSock();

            m_gameSender.SetReceiver(m_gameReceiver);

            return true;
        }

        public void InitUserSock()
        {
            if (m_sock != null)
            {
                try
                {
                    if (m_sock.Connected)
                    {
                        m_sock.Disconnect(false);
                        //m_sock.Close();
                    }
                    
                }
                catch (System.Exception ex)
                {
                    LogSystem.LogError(ex.ToString());
                } 
            }

            m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_scoket = new UserSock(m_sockcall, m_sock);
            if (m_gameSender == null)
            {
                m_gameSender = new GameSender();
            }
            m_gameSender.SetSocket(ref m_scoket);
        }

        //获取发送对象
        public IGameSender GetGameSender()
        {
            return m_gameSender;
        }


        //获取接受对象
        public IGameReceiver GetGameReceiver()
        {
            return m_gameReceiver;
        }


        //连接服务器
        public bool Connect(string addr, int port)
        {
            return m_scoket.Open(addr, port);
        }

        //是否连接成功 
        public bool Connected()
        {
            return m_scoket.Connected();
        }

        public bool Disconnect()
        {
            return m_scoket.Close();
        }

        //连接状态 
        public SockState GetState()
        {
            return m_scoket.GetSockState();
        }

        //回调事件注册
        public bool RegistCallBack(string funcName, CallBack callBack)
        {
            //
            if (string.IsNullOrEmpty(funcName) || callBack == null)
            {
                return false;
            }

            try
            {
                m_CallBacks[funcName] = callBack;
            }
            catch (Exception ex)
            {
                LogSystem.LogError("AgentEx RegistCallBack exception =[", ex.ToString(), "]");
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
                if (m_CallBacks.ContainsKey(fun_name))
                {
                    m_CallBacks[fun_name](args);
                }
                else
                {
                    LogSystem.LogError("can find call_function ", fun_name);
                }
            }
            catch (Exception ex)
            {
                LogSystem.LogError("AgentEx Excute_CallBack exception =[", ex.ToString(), "]");
                return false;
            }
            return false;
        }

        public void ProcessMsg(byte[] data, int size)
        {
            miRecvLastMsgTime = DateTime.Now.Ticks / 10000000;
            m_gameReceiver.ProcessMsg(data, size);
        }

        public void ProcessMessage()
        {
            if (m_scoket != null)
            {
                uint uBeatType = m_gameReceiver.GetBeatType();
                if (uBeatType > 0 && mbConnected)
                { //发送心跳
                    long iSceneds = DateTime.Now.Ticks / 10000000;
                    long iMilisceneds = DateTime.Now.Ticks / 10000;
                    long iBeatInterval = (long)m_gameReceiver.GetBeatInterval();
                    long nSend;
                    if (uBeatType == 1)
                    {
                        long iCheckInterval = (long)m_gameReceiver.GetCheckInterval();
                        long nRecv;
                        nRecv = (iSceneds - miRecvLastMsgTime);
                        nSend = (iSceneds - miLastSendBeatTime);
                        if (nRecv >= iBeatInterval && nSend >= iBeatInterval)
                        {
                            miLastSendBeatTime = iSceneds;
                            m_gameSender.SendBeat();
                        }
                        iSceneds = DateTime.Now.Ticks / 10000000;
                        nRecv = (iSceneds - miRecvLastMsgTime);
                        nSend = (iSceneds - miCheckTime);
                        if (nRecv > iCheckInterval && nSend > iCheckInterval)
                        {
                            miCheckTime = iSceneds;
                            mbConnected = false;
                            mbSocketBlock = true;
                            ///通知逻辑层检查网络状态，Failed重连网络[1]
                            VarList varlist = VarList.GetVarList();
                            Excute_CallBack("on_connect_block", varlist);
                            varlist.Collect();
                        }
                    }
                    else if (uBeatType == 2)
                    {
                        long iCheckInterval = (long)m_gameReceiver.GetCheckInterval();
                        long nRecv;
                        nRecv = (iSceneds - miRecvLastMsgTime) * 3;
                        nSend = (iSceneds - miLastSendBeatTime) * 3;
                     //   if (nRecv >= iBeatInterval && nSend >= iBeatInterval)
                        if (iSceneds - miLastSendBeatTime >= iBeatInterval)
                        {
                            miLastSendBeatTime = iSceneds;
                            m_gameSender.SendTracert(0);
                        }
                        iSceneds = DateTime.Now.Ticks / 10000000;
                        nRecv = (iSceneds - miRecvLastMsgTime);
                        nSend = (iSceneds - miCheckTime);
                        if (nRecv > iCheckInterval && nSend > iCheckInterval)
                        {
                            miCheckTime = iSceneds;
                            mbConnected = false;
                            mbSocketBlock = true;
                            ///通知逻辑层检查网络状态，Failed重连网络[1]
                            VarList varlist = VarList.GetVarList();
                            Excute_CallBack("on_connect_block", varlist);
                            varlist.Collect();
                        }
                    }
                }
                m_scoket.ProcessMessage();
            }
        }
    }
}
