﻿//--------------------------------------------------------------------
// 文件名	:	SerrverStateSock.cs
// 内  容	:
// 说  明	:
// 创建日期	:	2015-09-29
// 创建人	:	zhugq
// 版权所有	:	苏州蜗牛电子有限公司
//--------------------------------------------------------------------
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SysUtils;

namespace Fm_ClientNet
{

    /// <summary>
    /// 文本sock，专门用于与listener服务器通信，获取服务器列表(IP，端口，负载等等)
    /// </summary>
    public class ServerStateSock
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
                ServerStateSock self = (ServerStateSock)ar.AsyncState;
                if (self.m_State == SockState.Idle)
                {
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
                    LogSystem.Log("Exception e", e.ToString());
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
                ServerStateSock self = (ServerStateSock)ar.AsyncState;
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
                self.Disconnect();
            }

            /// <summary>
            /// 接收
            /// </summary>
            /// <param name="ar"></param>
            public static void Receive(IAsyncResult ar)
            {
                ServerStateSock self = (ServerStateSock)ar.AsyncState;
                if (self.m_State == SockState.Idle)
                {
                    return;
                }
                int size = 0;
                try
                {
                    size = self.m_Socket.EndReceive(ar);
                }
                catch (SocketException e)
                {
                    if (self.m_Socket.Connected)
                    {
                        self.SetSockError(e.ErrorCode, e.ToString());
                    }
                }
                catch (Exception e)
                {
                    if (self.m_Socket.Connected)
                    {
                        self.SetSockError(0, e.ToString());
                    }
                }
                finally
                {
                }

                self.StopReceive(size);
            }

            /// <summary>
            /// 发送
            /// </summary>
            /// <param name="ar"></param>
            public static void Send(IAsyncResult ar)
            {
                ServerStateSock self = (ServerStateSock)ar.AsyncState;
                if (self.m_State == SockState.Idle)
                {
                    return;
                }
                int size;
                try
                {
                    size = self.m_Socket.EndSend(ar);
                    LogSystem.Log("SerrverStateSock EndSend", size);
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
            }
        }

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

        private const int maxBufLength = 40960;

        /// <summary>
        /// 接收的BYTE大小
        /// </summary>
        private byte[] m_RecvBuf = new byte[maxBufLength];


        /// <summary>
        /// 状态
        /// </summary>
        private SockState m_State = SockState.Idle;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callee">接口</param>
        /// <param name="addr">地址</param>
        /// <param name="port">端口号</param>
        public ServerStateSock(OnConnected onConnected, OnConnectFailed onConnectFailed, OnRecieveData onRecieveData)
        {
            monConnected = onConnected;
            monConnectFailed = onConnectFailed;
            monRecieve = onRecieveData;
            InitSock(null);

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

            m_Socket.ReceiveBufferSize = 64 * 1024;
            m_Socket.Blocking = true;

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
            m_State = state;
        }

        /// <summary>
        /// 设置错误
        /// </summary>
        /// <param name="error"></param>
        /// <param name="msg"></param>
        private void SetSockError(int error, string msg)
        {
            m_nError = error;
        }

        /// <summary>
        /// 连接失败
        /// </summary>
        /// <returns></returns>
        private bool ConnectFail()
        {
            if (monConnectFailed != null)
            {
                monConnectFailed(this);
            }
            SetSockState(SockState.Failed);
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
                SetSockState(SockState.Connected);
                if (monConnected != null)
                {
                    monConnected(this);
                }
                return StartReceive();
            }
            else
            {
                return ConnectFail();
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            return Close();
        }
        public delegate void OnRecieveData(ServerStateSock stateSock, ref VarList args);
        public delegate void OnConnected(ServerStateSock stateSock);
        public delegate void OnConnectFailed(ServerStateSock stateSock);
        OnRecieveData monRecieve = null;
        OnConnected monConnected = null;
        OnConnectFailed monConnectFailed = null;
        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <returns></returns>
        public bool StartReceive()
        {
            try
            {
                if (m_Socket == null)
                    return false;
                AsyncCallback asynccallback = SockAsync.Receive;
                m_Socket.BeginReceive(m_RecvBuf, 0, 0x2800, SocketFlags.None, asynccallback, this);
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
        /// 接受数据
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool StopReceive(int size)
        {
            if (size > 0)
            {
                RecieveData(m_RecvBuf, size);
            }
            return StartReceive();
        }
        int miIndex = 0;
        byte[] mBuffer = new byte[maxBufLength];
        int messageLength = 0;
        public bool RecieveData(byte[] data, int iSize)
        {
            if (iSize > maxBufLength - miIndex)
            {
                return false;
            }

            Array.Copy(data, 0, mBuffer, miIndex, iSize);
            if (miIndex >= 4)
            {
                //消息头已经收到，解析出消息头/
                messageLength = BitConverter.ToInt32(mBuffer, 0);
            }
            else
            {
                //头还没有接接收完/
                messageLength = -1;
            }

            miIndex += iSize;
            if (miIndex >= messageLength)
            {
                int compressFlag = BitConverter.ToInt32(mBuffer, 4);
                byte[] bufferBody = new byte[mBuffer.Length - 8];
                Array.Copy(mBuffer, 8, bufferBody, 0, bufferBody.Length);
                byte[] bodys = null;
                if (compressFlag > 0)
                {
                    bodys = ServerStateQuickLZ.decompress(bufferBody);
                }
                else
                {
                    bodys = new byte[mBuffer.Length - 8];
                    try
                    {
                        Array.Copy(mBuffer, 8, bodys, 0, bodys.Length);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e.ToString());
                    }
                }

                if (bodys.Length > 0)
                {
                    //解析消息
                    int m_nCurPosi = 0;
                    int Head1_0 = 0;
                    int Head1_1 = 0;

                    int Head2_0 = 0;
                    int Head2_1 = 0;

                    m_nCurPosi = 0;

                    Head1_0 = BitConverter.ToInt32(bodys, m_nCurPosi);
                    m_nCurPosi += 4;

                    Head1_1 = BitConverter.ToInt32(bodys, m_nCurPosi);
                    m_nCurPosi += 4;

                    Head2_0 = BitConverter.ToInt32(bodys, m_nCurPosi);
                    m_nCurPosi += 4;

                    Head2_1 = BitConverter.ToInt32(bodys, m_nCurPosi);
                    m_nCurPosi += 4;

                    int count = BitConverter.ToInt32(bodys, m_nCurPosi);
                    m_nCurPosi += 4;

                    VarList args = VarList.GetVarList();
                    args.AddInt(count);
                    int byteSize = bodys.Length;
                    for (int i = 0; i < count; i++)
                    {
                        int len = 0;
                        for (int c = 0; c < byteSize - m_nCurPosi - 1; c++)
                        {
                            try
                            {
                                char v = '\0';
                                if (m_nCurPosi + c < bodys.Length)
                                {
                                    v = BitConverter.ToChar(bodys, m_nCurPosi + c);
                                }

                                if (v == '\0' || bodys[m_nCurPosi + c] == 0)
                                {
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                UnityEngine.Debug.LogError(e.ToString());
                            }

                            len++;
                        }

                        if (len > 0)
                        {
                            byte[] dddd = new byte[len];
                            Array.Copy(bodys, m_nCurPosi, dddd, 0, len);

                            //byte[] des = Encoding.Convert(Encoding.UTF8, System.Text.Encoding.Default, dddd);
                            string str = System.Text.Encoding.UTF8.GetString(dddd);
                            args.AddString(str);
                        }

                        m_nCurPosi += len + 1;//1表示'\0'
                    }

                    if (monRecieve != null)
                    {
                        monRecieve(this, ref args);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        public bool Connect(string addr, int port)
        {
            if (m_State == SockState.Failed)
            {
                InitSock(null);
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
                if (m_Socket == null)
                    return false;

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
                    }
                    m_Socket.Close();
                    m_Socket = null;
                }
            }
            catch (SocketException e)
            {
                m_State = SockState.Idle;
                SetSockError(e.ErrorCode, e.ToString());
            }

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
        public bool Send(byte[] data, int dataLength)
        {
            // 正在连接
            if (m_Socket == null)
                return false;

            if (!m_Socket.Connected)
            {
                return false;
            }

            // 保证发送的顺序
            return SendData(data, dataLength);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        private bool SendData(byte[] data, int dataLength)
        {
            byte[] buf = new byte[dataLength];
            Array.Copy(data, buf, dataLength);

            try
            {
                if (m_Socket == null)
                    return false;
                AsyncCallback asyncCallback = SockAsync.Send;
                m_Socket.BeginSend(buf, 0, buf.Length, SocketFlags.None, asyncCallback, this);
            }
            catch (SocketException e)
            {
                SetSockError(e.ErrorCode, e.ToString());
                SetSockState(SockState.Failed);
                return false;
            }
            buf = null;
            return true;
        }
    }
}
