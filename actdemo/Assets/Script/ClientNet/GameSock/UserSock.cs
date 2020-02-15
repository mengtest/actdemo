//--------------------------------------------------------------------
// 文件名	:	UserSock.cs
// 内  容	:
// 说  明	:
// 创建日期	:	2006年11月30日
// 创建人	:	陆利民
// 版权所有	:	苏州蜗牛电子有限公司
//--------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Fm_ClientNet
{
    /// <summary>
    /// 连接状态
    /// </summary>
    public enum SockState
    {
        /// <summary>
        /// 初始化状态
        /// </summary>
        Idle,
        /// <summary>
        /// 正在连接
        /// </summary>
        Connecting,
        /// <summary>
        /// 已连上
        /// </summary>
        Connected,
        /// <summary>
        /// 正在监听
        /// </summary>
        Listening,
        /// <summary>
        /// 连接失败、监听失败、发送数据失败或接收数据失败
        /// </summary>
        Failed
    };

    /// <summary>
    /// 事件类型
    /// </summary>
    public enum SockEventType
    {
        /// <summary>
        /// 接收
        /// </summary>
        Accept,
        /// <summary>
        /// 连接成功
        /// </summary>
        Connected,
        /// <summary>
        /// 连接失败
        /// </summary>
        ConnectFail = 10061,
        /// <summary>
        /// 异常错误
        /// </summary>
        Error,
        /// <summary>
        /// 连接被关闭
        /// </summary>
        Closed,
        /// <summary>
        /// 发送消息
        /// </summary>
        Send,
        /// <summary>
        /// 接收消息
        /// </summary>
        Receive,
        /// <summary>
        /// 输出日志
        /// </summary>
        PutInfo,
        /// <summary>
        /// 服务器主动断开
        /// </summary>
        ServerDisconnect = 10054,
        /// <summary>
        /// 保存二进制日志
        /// </summary>
        SaveByteLog,
    };

    /// <summary>
    /// 连接事件
    /// </summary>
    public class SockEvent
    {
        public SockEventType m_Type;
        public Object m_Sock;
        public Object m_Data;

        public SockEvent(SockEventType type, Object sock)
        {
            m_Type = type;
            m_Sock = sock;
            m_Data = null;
        }

        public SockEvent(SockEventType type, Object sock, Object data)
        {
            m_Type = type;
            m_Sock = sock;
            m_Data = data;
        }

        protected SockEvent()
        {
        }

        public SockEventType GetEventType()
        {
            return m_Type;
        }

        public Object GetEventSock()
        {
            return m_Sock;
        }

        public Object GetEventData()
        {
            return m_Data;
        }
    }

    /// <summary>
    /// 通讯错误
    /// </summary>
    public class SockError
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        private int m_nType;
        /// <summary>
        /// 消息
        /// </summary>
        private string m_sMsg;

        public SockError(int type, string msg)
        {
            m_nType = type;
            m_sMsg = msg;
        }

        protected SockError()
        {
        }

        public int GetErrorType()
        {
            return m_nType;
        }

        public string GetErrorMsg()
        {
            return m_sMsg;
        }
    }

    /// <summary>
    /// 消息数据
    /// </summary>
    public class SockMsg
    {
        private byte[] m_Data;
        private int m_nSize;

        public SockMsg(byte[] data, int size)
        {
            m_Data = data;
            m_nSize = size;
        }

        protected SockMsg()
        {
        }

        public int GetMsgSize()
        {
            return m_nSize;
        }

        public byte[] GetMsgData()
        {
            return m_Data;
        }
    }

    /// <summary>
    /// 事件回调接口
    /// </summary>
    public interface IUserSockCallee
    {
        int OnSockConnected(UserSock sock, string addr, int port);
        int OnSockConnectFail(UserSock sock, string addr, int port);
        int OnSockReceive(UserSock sock, byte[] data, int size);
        int OnSockSend(UserSock sock, int size);
        int OnSockClose(UserSock sock);
        int OnSockLog(UserSock sock, string msg);
        int OnSockError(UserSock sock, int error_type, string error_msg);
    }

    /// <summary>
    /// 通讯客户端
    /// </summary>
    public class UserSock
    {
        /// <summary>
        /// 异步辅助类
        /// </summary>
        private class SockAsync
        {
            /// <summary>
            /// 连接
            /// </summary>
            /// <param name="ar"></param>
            public static void Connect(IAsyncResult ar)
            {
                UserSock self = (UserSock)ar.AsyncState;
                if (self.m_State == SockState.Idle)
                {
                    LogSystem.Log("Idle");
                    return;
                }
                try
                {
                    self.m_Socket.EndConnect(ar);
                }
                catch (SocketException e)
                {
                    LogSystem.Log("SocketException e");
                    self.SetSockError(e.ErrorCode, e.ToString());
                    self.ConnectFail();
                    return;
                }
                catch (Exception e)
                {
                    LogSystem.Log("Exception e" , e.ToString());
                    return;
                }
                self.StopConnect();
            }

            /// <summary>
            /// 断开
            /// </summary>
            /// <param name="ar"></param>
            public static void Disconnect(IAsyncResult ar)
            {
                UserSock self = (UserSock)ar.AsyncState;
                if (self.m_State == SockState.Idle)
                {
                    return;
                }
                try
                {
                    self.m_Socket.EndDisconnect(ar);
                }
                catch (SocketException e)
                {
                    self.SetSockError(e.ErrorCode, e.ToString());
                    return;
                }
                catch (Exception)
                {
                    return;
                }
                self.StopDisconnect();
            }

            /// <summary>
            /// 接收
            /// </summary>
            /// <param name="ar"></param>
            public static void Receive(IAsyncResult ar)
            {
                RecvContext recvContext = (RecvContext)ar.AsyncState;
                UserSock self = recvContext.uUserSock;
                if (self.m_State == SockState.Idle)
                {
                    return;
                }
                int size = 0;
                try
                {
                    size = self.m_Socket.EndReceive(ar);
                    if (size == 0)
                    {
                        size = -1;
                    }
                }
                catch (SocketException e)
                {
                    if (self.m_Socket.Connected)
                    {
                        if (e.ErrorCode == 10035)
                        {
                            size = 0;
                        }
                        else
                        {
                            size = -1;
                            self.SetSockError(e.ErrorCode, e.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    size = -1;
                    if (self.m_Socket.Connected)
                    {
                        self.SetSockError(0, e.ToString());
                    }
                }
                finally
                {
                }

                self.StopReceive(recvContext.RecvBuff, size);
                self.PushRecvContext(recvContext);
            }

            /// <summary>
            /// 发送
            /// </summary>
            /// <param name="ar"></param>
            public static void Send(IAsyncResult ar)
            {
                SendContext self = (SendContext)ar.AsyncState;
                UserSock uSock = self.uUserSock;
                if (uSock.m_State == SockState.Idle)
                {
                    return;
                }
                int size = 0;
                try
                {
                    size = uSock.m_Socket.EndSend(ar);
                }
                catch (SocketException e)
                {
                    uSock.SetSockError(e.ErrorCode, e.ToString());
                    LogSystem.LogError(e.ToString());
                }
                catch (Exception ex)
                {
                    LogSystem.LogError(ex.ToString());
                }
                finally
                {

                }
                uSock.StopSend(self.sendBuff, size);
                uSock.PushSendContext(self);
            }
        }
        /// <summary>
        /// 接口
        /// </summary>
        private IUserSockCallee m_Callee;
        /// <summary>
        /// socket
        /// </summary>
        private Socket m_Socket = null;
        /// <summary>
        /// 服务器地址
        /// </summary>
        private string m_sAddr;
        /// <summary>
        /// 服务器端口号
        /// </summary>
        private int m_nPort;
        /// <summary>
        /// 错误类型
        /// </summary>
        private int m_nError = 0;
        /// <summary>
        /// 接受个数
        /// </summary>
        private int m_nRecvCount = 0;
        /// <summary>
        /// 前一个接收的byte
        /// </summary>
        private byte m_nRecvPrior = 0;
        /// <summary>
        /// 接收的BYTE大小
        /// </summary>
        private byte[] m_RecvBuf = new byte[0x2800];
        /// <summary>
        /// 异步接收数据接口
        /// </summary>
        private AsyncEvent m_AsyncEvent = new AsyncEvent();

        /// <summary>
        /// 消息发送队列
        /// </summary>
        private Queue m_SendQueue = new Queue();
        /// <summary>
        /// 状态
        /// </summary>
        private SockState m_State = SockState.Idle;
        private bool mbConnected = false;
        /// <summary>
        /// 消息发送锁
        /// </summary>
        private static Object s_SendinLock = new Object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callee">接口</param>
        /// <param name="addr">地址</param>
        /// <param name="port">端口号</param>
        public UserSock(IUserSockCallee callee, string addr, int port)
        {
            m_Callee = callee;
            m_sAddr = addr;
            m_nPort = port;

            InitSock(null);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callee">地址</param>
        /// <param name="sock">端口号</param>
        public UserSock(IUserSockCallee callee, Socket sock)
        {
            m_Callee = callee;
            m_sAddr = string.Empty;
            m_nPort = 0;
            InitSock(sock);
        }

        protected UserSock()
        {
        }

        /// <summary>
        /// 初始化SOCKET
        /// </summary>
        /// <param name="sock"></param>
        private void InitSock(Socket sock)
        {
            if (sock == null)
            {
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                m_Socket = sock;
            }
            //m_Socket.SendBufferSize = 64*1024;
            m_Socket.ReceiveBufferSize = 64 * 1024;
            m_Socket.Blocking = true;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            m_SendQueue.Clear();
            m_nRecvCount = 0;
            m_nRecvPrior = 0;
            SetSockState(SockState.Idle);
        }

        /// <summary>
        /// 获得socket
        /// </summary>
        /// <returns></returns>
        public Socket GetSocket()
        {
            return m_Socket;
        }

        /// <summary>
        /// 目标地址
        /// </summary>
        /// <returns></returns>
        public string GetDestAddr()
        {
            return m_sAddr;
        }

        /// <summary>
        /// 目标端口
        /// </summary>
        /// <returns></returns>
        public int GetDestPort()
        {
            return m_nPort;
        }

        /// <summary>
        /// 获得本地地址
        /// </summary>
        /// <returns></returns>
        public string GetLocalAddr()
        {
            if (m_Socket == null)
                return string.Empty;

            IPEndPoint ep = (IPEndPoint)m_Socket.LocalEndPoint;
            if (null == ep)
            {
                return string.Empty;
            }
            return ep.Address.ToString();
        }

        /// <summary>
        /// 获得本地端口号
        /// </summary>
        /// <returns></returns>
        public int GetLocalPort()
        {
            if (m_Socket == null)
                return 0;

            IPEndPoint ep = (IPEndPoint)m_Socket.LocalEndPoint;
            if (null == ep)
            {
                return 0;
            }
            return ep.Port;
        }

        /// <summary>
        /// 获得目标地址
        /// </summary>
        /// <returns></returns>
        public string GetRemoteAddr()
        {
            if (m_Socket == null)
                return string.Empty;

            IPEndPoint ep = (IPEndPoint)m_Socket.RemoteEndPoint;
            if (null == ep)
            {
                return string.Empty;
            }
            return ep.Address.ToString();
        }

        /// <summary>
        /// 获得目标端口号
        /// </summary>
        /// <returns></returns>
        public int GetRemotePort()
        {
            if (m_Socket == null)
                return 0;

            IPEndPoint ep = (IPEndPoint)m_Socket.RemoteEndPoint;
            if (null == ep)
            {
                return 0;
            }
            return ep.Port;
        }

        /// <summary>
        /// 是否连接成功
        /// </summary>
        /// <returns></returns>
        public bool Connected()
        {
            if (m_Socket == null)
                return false;

            return m_Socket.Connected;
        }

        /// <summary>
        /// 验证连接状态是否正确
        /// </summary>
        /// <returns></returns>
        public bool VerifyConnect()
        {
            if (m_Socket == null)
                return false;

            return (m_State == SockState.Connected) && m_Socket.Connected;
        }

        /// <summary>
        /// 回调接口
        /// </summary>
        /// <returns></returns>
        public IUserSockCallee GetSockCallee()
        {
            return m_Callee;
        }

        /// <summary>
        /// 获得当前状态
        /// </summary>
        /// <returns></returns>
        public SockState GetSockState()
        {
            return m_State;
        }

        /// <summary>
        /// 获得错误码
        /// </summary>
        /// <returns></returns>
        public int GetSockError()
        {
            return m_nError;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="state"></param>
        private void SetSockState(SockState state)
        {
            //lock (m_StateLock)
            {
                m_State = state;
                if (state == SockState.Connected || state == SockState.Connecting)
                {
                    mbConnected = true;
                }
                else if (state == SockState.Failed || state == SockState.Idle)
                {
                    mbConnected = false;
                }
                //LogSystem.Log("state " , m_State.ToString());
            }
        }

        /// <summary>
        /// 设置错误
        /// </summary>
        /// <param name="error"></param>
        /// <param name="msg"></param>
        private void SetSockError(int error, string msg)
        {
            m_nError = error;
            m_Callee.OnSockError(this, error, msg);
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="msg">消息</param>
        private void SetSockLog(string msg)
        {
            m_Callee.OnSockLog(this, msg);
        }

        /// <summary>
        /// 连接失败
        /// </summary>
        /// <returns></returns>
        private bool ConnectFail()
        {
            SetSockState(SockState.Failed);
            m_AsyncEvent.PushEvent(AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CONNECTFAILED, m_sAddr, m_nPort);
            //m_Callee.OnSockConnectFail(this, m_sAddr, m_nPort);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool StopConnect()
        {
            if (m_Socket.Connected)
            {
                LogSystem.Log("OnSockConnected(this, m_sAddr, m_nPort)");
                SetSockState(SockState.Connected);
                m_AsyncEvent.PushEvent(AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CONNECTED, m_sAddr, m_nPort);
                //m_Callee.OnSockConnected(this, m_sAddr, m_nPort);
                byte[] abySendMsg = null;
                lock (s_SendinLock)
                {
                    if (m_SendQueue.Count > 0)
                    {
                        abySendMsg = (byte[])m_SendQueue.Dequeue();
                    }
                }

                if (abySendMsg != null)
                {
                    SendData(abySendMsg, abySendMsg.Length);
                }
                return StartReceive();
            }
            else
            {
                LogSystem.Log("ConnectFail()");
                return ConnectFail();
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        private bool StopDisconnect()
        {
            //m_Callee.OnSockClose(this);
            //m_AsyncEvent.PushEvent(AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CLOSED);
            /*Close();
            return true;*/
            return Close();
        }
        private Queue<RecvContext> m_queueRecvContext = new Queue<RecvContext>();
        private Object moRecvQueue = new Object();
        public RecvContext GetRecvContext(UserSock uSock, AsyncEvent.RecvBufferData recvBuff)
        {
            RecvContext recvContext = null;
            lock (moRecvQueue)
            {
                if (m_queueRecvContext.Count > 0)
                {
                    recvContext = m_queueRecvContext.Dequeue();
                }
            }
            if (recvContext == null)
            {
                recvContext = new RecvContext(uSock, recvBuff);
            }
            else
            {
                recvContext.uUserSock = uSock;
                recvContext.RecvBuff = recvBuff;
            }
            return recvContext;
        }
        public void PushRecvContext(RecvContext recvContext)
        {
            lock (moRecvQueue)
            {
                recvContext.RecvBuff = null;
                recvContext.uUserSock = null;
                m_queueRecvContext.Enqueue(recvContext);
            }
        }
        private Queue<SendContext> m_queueSendContext = new Queue<SendContext>();
        private Object moSendQueue = new Object();
        public SendContext GetSendContext(UserSock uSock, SendBuffer sendBuff)
        {
            SendContext sendContext = null;
            lock (moSendQueue)
            {
                if (m_queueSendContext.Count > 0)
                {
                    sendContext = m_queueSendContext.Dequeue();
                }
            }
            if (sendContext == null)
            {
                sendContext = new SendContext(uSock, sendBuff);
            }
            else
            {
                sendContext.uUserSock = uSock;
                sendContext.sendBuff = sendBuff;
            }
            return sendContext;
        }
        public void PushSendContext(SendContext sendContext)
        {
            lock (moSendQueue)
            {
                sendContext.sendBuff = null;
                sendContext.uUserSock = null;
                m_queueSendContext.Enqueue(sendContext);
            }
        }
        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <returns></returns>
        private bool StartReceive()
        {
            if (m_Socket != null)
            {
                try
                {
                    RecvContext recvContext = GetRecvContext(this, m_AsyncEvent.GetBufferData());
                    AsyncCallback asynccallback = SockAsync.Receive;
                    m_Socket.BeginReceive(recvContext.RecvBuff.m_abyRecvBuffer, 0, AsyncEvent.ASYNCRECVBUF_LENGTH,
                               SocketFlags.None, asynccallback, recvContext);
                }
                catch (SocketException e)
                {
                    ///接收中出错，关闭
                    SetSockError(e.ErrorCode, e.ToString());
                    SetSockState(SockState.Failed);
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 接受数据
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool StopReceive(AsyncEvent.RecvBufferData recvBuff, int size)
        {
            recvBuff.m_iRecvSize = size;
            if (size > 0)
            {
                m_AsyncEvent.PushRecvData(recvBuff);
            }
            else
            {
                m_AsyncEvent.PushFreeData(recvBuff);
                ///接收不到数据了，可以断开完了，通知逻辑层
                if (size < 0)
                {
                    return Close();
                }
            }

            return StartReceive();
        }

        public void ProcessMessage()
        {
            AsyncEvent.RecvBufferData Data = null;
            AsyncEvent.EventData Event = null;

            while (m_AsyncEvent.PeekData(ref Data, ref Event))
            {
                ///处理内部网络事件
                if (Event != null)
                {
                    switch (Event.m_eEvent)
                    {
                        case AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CONNECTED:
                            m_Callee.OnSockConnected(this, Event.m_strIP, Event.m_iPort);
                            break;
                        case AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CONNECTFAILED:
                            m_Callee.OnSockConnectFail(this, Event.m_strIP, Event.m_iPort);
                            break;

                        case AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CLOSED:
                            m_Callee.OnSockClose(this);
                            break;

                        default:
                            break;
                    }
                    return;
                }

                ///处理数据接收
                if (Data != null)
                {
                    ReceiveData(Data.m_abyRecvBuffer, Data.m_iRecvSize);
                    m_AsyncEvent.PushFreeData(Data);
                    Data = null;
                }
            }
        }

        /// <summary>
        /// 发送下一个消息
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool StopSend(SendBuffer sendBuff, int size)
        {
            m_Callee.OnSockSend(this, size);
            lock (s_AsyncLockFree)
            {
                m_queueSenderBufferFree.Enqueue(sendBuff);
            }
            byte[] abySendMsg = null;
            lock (s_SendinLock)
            {
                if (m_SendQueue.Count > 0)
                {
                    abySendMsg = (byte[])m_SendQueue.Dequeue();
                }
            }

            if (abySendMsg != null)
            {
                return SendData(abySendMsg, abySendMsg.Length);
            }

            return true;
        }

        /// <summary>
        ///  接受连接后(Accept)初始化连接
        /// </summary>
        /// <returns></returns>
        public bool Attach()
        {
            return StopConnect();
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            return Open(string.Empty, 0);
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        public bool Open(string addr, int port)
        {
            LogSystem.LogWarning(addr, port);
            if (m_Socket == null || m_State == SockState.Failed)
            {
                InitSock(null);
                Init();
            }
            if (m_State != SockState.Idle)
            {
                return false;
            }
            m_nError = 0;
            if (addr.Length > 0)
            {
                m_sAddr = addr;
                m_nPort = port;
            }
            try
            {
                IPAddress ipad = IPAddress.Parse(m_sAddr);
                IPEndPoint ipe = new IPEndPoint(ipad, m_nPort);
                SetSockState(SockState.Connecting);
                m_Socket.BeginConnect(ipe, new AsyncCallback(SockAsync.Connect), this);
            }
            catch (SocketException e)
            {
                SetSockError(e.ErrorCode, e.ToString());
                SetSockState(SockState.Failed);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (m_State == SockState.Idle)
            {
                return true;
            }
            if (m_Socket == null)
                return true;

            try
            {
                if (m_Socket != null)
                {
                    if (m_Socket.Connected)
                    {
                        m_Socket.Shutdown(SocketShutdown.Both);
                        //m_Socket.Close();
                    }
                    
                    m_Socket = null;
                }

                //直接给逻辑回调
                //m_Callee.OnSockClose(this);

                //InitSock(null);此处造成套接字泄漏
            }
            catch (SocketException e)
            {
                m_State = SockState.Idle;
                SetSockError(e.ErrorCode, e.ToString());
            }

            m_AsyncEvent.PushEvent(AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CLOSED);

            Init();
            return true;
        }

        /// <summary>
        /// 停止连接
        /// </summary>
        /// <returns></returns>
        public bool Shut()
        {
            try
            {
                if (m_Socket == null)
                    return true;

                m_Socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException e)
            {
                SetSockError(e.ErrorCode, e.ToString());
                SetSockState(SockState.Failed);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public bool Send(byte[] data, int size)
        {
            // 正在连接
            if (m_State == SockState.Connecting)
            {
                lock (s_SendinLock)
                {
                    AddSendQueue(data, size);
                }
                return true;
            }

            if (m_State != SockState.Connected)
            {
                return false;
            }
            if (m_Socket == null)
                return false;

            if (!m_Socket.Connected  )
            {
                if (mbConnected)
                {
                    //设置网络状态
                    //SetSockState(SockState.Idle);
                    //断开网络
                    Close();
                }
                return false;
            }

            lock (s_SendinLock)
            {
                if (m_SendQueue.Count > 0)
                {
                    AddSendQueue(data, size);
                    return true;
                }
            }
            // 保证发送的顺序
            return SendData(data, size);
        }

        /// <summary>
        /// 添加消息到队列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private void AddSendQueue(byte[] data, int size)
        {
            byte[] msg = new byte[size];
            Array.Copy(data, msg, size);
            m_SendQueue.Enqueue(msg);
        }

        public class SendBuffer
        {
            public byte[] oData = null;
            public int iBuffSize = 0;
            public SendBuffer(int iSize)
            {
                oData = new byte[iSize];
                iBuffSize = 0;
            }
        }
        private Queue<SendBuffer> m_queueSenderBufferFree = new Queue<SendBuffer>();
        private static Object s_AsyncLockFree = new Object();

        SendBuffer GetSendBuffer()
        {
            SendBuffer sendBuff = null;

            lock (s_AsyncLockFree)
            {
                if (m_queueSenderBufferFree.Count > 0)
                {
                    sendBuff = m_queueSenderBufferFree.Dequeue();
                }
                else
                {
                    sendBuff = new SendBuffer(1024);
                }
            }

            return sendBuff;
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        private bool SendData(byte[] data, int size)
        {
            if (data == null)
                return false;

            SendBuffer sendBuff = null;
            int iBuffSize = size * 2 + 2;
            if (iBuffSize >= 1024)
            {
                sendBuff = new SendBuffer(iBuffSize);
        
            }
            else
            {
                sendBuff = GetSendBuffer();
            }

            if (sendBuff == null)
                return false;

            byte[] oData = sendBuff.oData;
            if (oData == null)
                return false;

            int nIndex = 0;
            for (int i = 0; i < size; i++)
            {
                oData[nIndex++] = data[i];
                if (data[i] == 0xEE)
                {
                    oData[nIndex++] = 0;
                }
            }
            oData[nIndex++] = 0xEE;
            oData[nIndex++] = 0xEE;
            sendBuff.iBuffSize = nIndex;

            if (m_Socket != null)
            {
                try
                {
                    AsyncCallback asyncallback = SockAsync.Send;
                    SendContext sContext = GetSendContext(this, sendBuff);
                    m_Socket.BeginSend(oData, 0, nIndex, SocketFlags.None, asyncallback, sContext);
                }
                catch (SocketException e)
                {
                    SetSockError(e.ErrorCode, e.ToString());
                    SetSockState(SockState.Failed);
                    return false;
                }
                return true;
            }

            return false;
        }

        public class RecvContext
        {
            public UserSock uUserSock;
            public AsyncEvent.RecvBufferData RecvBuff;
            public RecvContext(UserSock uSock, AsyncEvent.RecvBufferData RecvBuff)
            {
                this.uUserSock = uSock;
                this.RecvBuff = RecvBuff;
            }
        }
        public class SendContext
        {
            public UserSock uUserSock;
            public SendBuffer sendBuff;
            public SendContext(UserSock uSock, SendBuffer sendBuff)
            {
                this.uUserSock = uSock;
                this.sendBuff = sendBuff;
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        private bool ReceiveData(byte[] data, int size)
        {
            for (int i = 0; i < size; i++)
            {
                // IsEnd
                if ((0xEE == data[i]) && (0xEE == m_nRecvPrior))
                {
                    m_nRecvCount--;
                    if (m_nRecvCount > 0)
                    {
                        m_Callee.OnSockReceive(this, m_RecvBuf, m_nRecvCount);
                    }
                    m_nRecvPrior = 0;
                    m_nRecvCount = 0;
                    continue;
                }
                else if ((0 == data[i]) && (0xEE == m_nRecvPrior))
                {
                }
                else
                {
                    if (m_nRecvCount < m_RecvBuf.Length)
                    {
                        m_RecvBuf[m_nRecvCount++] = data[i];
                    }
                    else
                    {
                        //m_nRecvCount = 0;
                        byte[] recvBuf = new byte[m_RecvBuf.Length + 1024];
                        Array.Copy(m_RecvBuf, recvBuf, m_RecvBuf.Length);
                        m_RecvBuf = null;
                        m_RecvBuf = recvBuf;
                        m_RecvBuf[m_nRecvCount++] = data[i];
                        recvBuf = null;
                    }
                }
                m_nRecvPrior = data[i];
            }
            return true;
        }

        private bool ReceiveDataByLen(byte[] data, int size)
        {

            if (m_nRecvCount + size > m_RecvBuf.Length)
            {
                byte[] mRecvBuf = new byte[m_RecvBuf.Length + size];
                string info = string.Format(" - ReceiveDataByLen : Add Receive Buff Length ,Src = {0},Des = {1}", m_RecvBuf.Length, mRecvBuf.Length);
                SetSockLog(info);
                Array.Copy(m_RecvBuf, mRecvBuf, m_RecvBuf.Length);
                m_RecvBuf = mRecvBuf;
                mRecvBuf = null;
            }

            Array.Copy(data, m_RecvBuf, size);
            m_nRecvCount += size;

            if (m_nRecvCount <= 4)
                return true;

            int len = BitConverter.ToInt32(m_RecvBuf, 0);

            while (m_nRecvCount > 4 && (m_nRecvCount - 4 > len))
            {
                byte[] temp = new byte[len];
                Array.Copy(m_RecvBuf, 4, temp, 0, len);
                m_Callee.OnSockReceive(this, temp, len);
                Array.Copy(m_RecvBuf, len + 4, m_RecvBuf, 0, m_nRecvCount - (len + 4));
                m_nRecvCount -= (len + 4);
                len = BitConverter.ToInt32(m_RecvBuf, 0);
            }


            return true;
        }
    }
}
