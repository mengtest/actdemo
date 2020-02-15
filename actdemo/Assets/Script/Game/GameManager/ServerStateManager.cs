using UnityEngine;
using System.Collections;
using Fm_ClientNet;
using SysUtils;
using System;
using System.Collections.Generic;

public class ByteArray
{
    const int maxDataBytes = 1024;

    public byte[] m_pData;
    public int m_nCurPosi;

    public ByteArray()
    {
        m_pData = new byte[maxDataBytes];
        m_nCurPosi = 0;
    }

    public void WriteInt(int iVal)
    {
        byte[] tempBytes = null;
        tempBytes = BitConverter.GetBytes(iVal);
        tempBytes.CopyTo(m_pData, m_nCurPosi);
        m_nCurPosi += 4;
    }

    public bool WriteEnd()
    {
        if (m_nCurPosi > maxDataBytes - 4)
        {
            return false;
        }

        byte[] tempBytes = null;
        tempBytes = BitConverter.GetBytes(m_nCurPosi);
        for (int i = m_nCurPosi - 1; i >= 0; i--)
        {
            m_pData[i + 4] = m_pData[i];
        }

        Array.Copy(tempBytes, 0, m_pData, 0, 4);
        m_nCurPosi += 4;

        return true;
    }
}

public class ServerStateManager
{
    /// <summary>
    /// 服务器类型/
    /// </summary>
    public enum ServerStyle
    {
        Normal = 0,                                             //普通服/
        Recommend = 1,                                          //推荐服/
        NewServer = 2,                                          //新服/
    }

    /// <summary>
    /// 服务器运行状态
    /// </summary>
    public enum ServerRunning
    {
        StopRun = 0,                                            //停止运行/
        IsRunning = 1,                                          //正在运行/
        NotOpen = 2,                                            //未开放/
    }

    /// <summary>
    /// 服务器状态
    /// </summary>
    public enum ServerState
    {
        Youliang = 0,                                           //优良/
        Fanmang = 1,                                            //繁忙/
        Yongji = 2,                                             //拥挤/
    }

    static ServerStateManager instance;
    public static ServerStateManager GetInstance()
    {
        if (instance == null)
        {
            instance = new ServerStateManager();
        }

        return instance;
    }

    ServerStateManager()
    {

    }

    public delegate void OnServerListOK(bool result);
    public OnServerListOK onServerListOK;

    // 服务器列表数据已经就绪
    public bool dataIsReady = false;

    public Dictionary<string, string> serverList = new Dictionary<string, string>();

    private ServerStateSock mSock;

    private string ip;
    private int port;
    private int issuerId;

    public void RequestServerState()
    {
        string ipInfo = "127.0.0.1";// Config.GetUpdaterConfig("StateServerPath", "Value");
        if (string.IsNullOrEmpty(ipInfo))
        {
            //状态服务器地址没有配置/
            LogSystem.LogError("RequestServerState --> ipinfo not in config!");
            return;
        }

        string[] ipinfoStrs = UtilTools.Split(ipInfo, ':');
        if (ipinfoStrs.Length < 3)
        {
            LogSystem.LogError("RequestServerState -->ipinfo config Error");
            return;
        }

        ip = ipinfoStrs[0];
        port = UtilTools.IntParse(ipinfoStrs[1]);
        issuerId = UtilTools.IntParse(ipinfoStrs[2]);

        ServerStateSock sock = new ServerStateSock(OnTextSockConnected, OnTextSockConnectFailed, OnTextSockRecieveData);
        if (sock != null)
        {
            sock.Connect(ip, port);
        }
        dataIsReady = false;

        TimerManager.AddTimer("StateReqOverTime", 10f, StateReqListener);
        //#endif
    }

    /// <summary>
    /// 服务器状态请求超时监听
    /// </summary>
    private void StateReqListener()
    {
        if (dataIsReady == false)
        {
            //请求不到数据/
            LogSystem.LogError("RequestServerState -->Can't get server list data!");
            if (instance.onServerListOK != null)
            {
                instance.onServerListOK(false);
            }
        }
    }

    /// <summary>
    /// 文本协议socket连接成功
    /// </summary>
    /// <param name="textSock"></param>
    static void OnTextSockConnected(ServerStateSock textSock)
    {
        LogSystem.Log("OnTextSockConnected");
        if (textSock != null)
        {
            ByteArray byteArray = new ByteArray();
            byteArray.WriteInt(0);
            byteArray.WriteInt(0);
            byteArray.WriteInt(1);
            byteArray.WriteInt(0);
            byteArray.WriteInt(44);
            byteArray.WriteInt(instance.issuerId);
            bool arrayRight = byteArray.WriteEnd();
            if (arrayRight)
            {
                textSock.Send(byteArray.m_pData, byteArray.m_nCurPosi);
            }
        }
    }

    /// <summary>
    /// 文本协议socket连接失败
    /// </summary>
    /// <param name="textSock"></param>
    static void OnTextSockConnectFailed(ServerStateSock textSock)
    {
        LogSystem.LogError("OnTextSockConnectFailed");
        textSock.Disconnect();
        instance.dataIsReady = false;

        if (instance.onServerListOK != null)
        {
            instance.onServerListOK(false);
        }

        LogSystem.LogError("RequestServerState -->Can't get server list data!");
    }

    /// <summary>
    /// 文本协议Socket消息接受，只用于获取服务器列表数据
    /// </summary>
    /// <param name="textSock"></param>
    /// <param name="args"></param>
    static void OnTextSockRecieveData(ServerStateSock textSock, ref VarList args)
    {
        LogSystem.Log("OnTextSockRecieveData");

        instance.serverList.Clear();

        int serverCount = args.GetInt(0);
        int index = 1;
        for (int i = 0; i < serverCount; i++)
        {
            string serverDataStr = args.GetString(index);
            index++;

            string[] dataArray = UtilTools.Split(serverDataStr, '/');
            LoginUI.ServerInfo info = new LoginUI.ServerInfo();
            info.mId = int.Parse(dataArray[0]);
            info.mServerName = dataArray[1];
            info.mIp = dataArray[2];
            info.mPort = dataArray[3];
            int state = UtilTools.IntParse(dataArray[6]);
            int service = UtilTools.IntParse(dataArray[5]);

//             if (instance.serverList.ContainsKey(info.Name))
//             {
//                 instance.serverList[info.Name] = info;
//             }
//             else
//             {
//                 instance.serverList.Add(info.Name, info);
//             }
        }

        instance.dataIsReady = true;
        if (instance.onServerListOK != null)
        {
            instance.onServerListOK(true);
        }
    }

    ///// <summary>
    ///// 获取本地服务器配置表信息/
    ///// </summary>
    //public void ReadLocalServerList()
    //{
    //    instance.serverList.Clear();

    //    int iServerNum = Config.mDictServerList.Count;
    //    for (int i = 0; i < iServerNum; i++)
    //    {
    //        Dictionary<string, string> Server = Config.mDictServerList[i];
    //        if (Config.mbVerifyVersion)
    //        {
    //            if (!Server.ContainsKey("Useage") || Server["Useage"] != Config.GetVersionUseage())
    //            {
    //                if (Server.ContainsKey("Useage"))
    //                {
    //                    LogSystem.Log(Server["Useage"], "-", Config.GetVersionUseage(), "-", Server["Useage"] == Config.GetVersionUseage());
    //                }
    //                else
    //                {
    //                    LogSystem.Log("n h Useage");
    //                }
    //                continue;
    //            }
    //        }

    //        LoginPanel.ServerInfo info = new LoginPanel.ServerInfo();
    //        info.No = Convert.ToInt32(Server["No"]);
    //        info.ID = Server["ID"];
    //        info.Name = Server["Name"];
    //        info.IP = Server["IP"];
    //        info.Port = Convert.ToInt32(Server["Port"]);
    //        info.State = Convert.ToInt32(Server["State"]);
    //        info.Recommend = Convert.ToInt32(Server["Recommended"]);
    //        info.NewService = Convert.ToInt32(Server["New"]);

    //        if (instance.serverList.ContainsKey(info.Name))
    //        {
    //            instance.serverList[info.Name] = info;
    //        }
    //        else
    //        {
    //            instance.serverList.Add(info.Name, info);
    //        }
    //    }

    //    instance.dataIsReady = true;
    //}
}
