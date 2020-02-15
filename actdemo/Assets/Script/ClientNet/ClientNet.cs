//--------------------------------------------------------------------
// 文件名	:	GameSock.cs
// 内  容	:
// 说  明	:   C# 客户端入口
// 创建日期	:	2014年06月10日
// 创建人	:	丁有进
// 版权所有	:	苏州蜗牛电子有限公司
//--------------------------------------------------------------------

using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
    public class ClientNet //: IClientNet
    {
        private IGameSock m_gameSock = null;
        private IGameClient m_gameClient = null;

        //private static ClientNet _Instance = null;

        //初始化句柄
        /*public static IClientNet Instance()
        {
            if ( null == _Instance )
            {
                _Instance = new ClientNet();
            }

            return _Instance;
        }*/

        //初始化
        public bool Init(string strDeviceUid)
        {
            return Init(strDeviceUid, null, null);
        }

        public bool Init(string strDeviceUid, Log.on_log on_log, ISockCallee callee)
        {
            fxVerify.Initalize(strDeviceUid);
            Log.SetLogCallBack(on_log);
            m_gameClient = new GameClient();
            if (m_gameSock != null)
            {
                m_gameSock.Disconnect();
                m_gameSock = null;
            }
            m_gameSock = new GameSock();
            if (!m_gameSock.Init(callee))
            {
                return false;
            }

            ((GameReceiver)m_gameSock.GetGameReceiver()).SetGameClient((GameClient)m_gameClient);
            return true;
        }
        /// <summary>
        /// 关闭网络
        /// </summary>
        /// <param name="bClearAll">是否清空数据</param>
        /// <returns>关闭状态</returns>
        public bool ShutDown(bool bClearAll = true)
        {
            bool bDisconnectok = true;
            bool bClearAllok = true;
            if (m_gameSock != null)
            {
                m_gameSock.Disconnect();
                if (bClearAll && m_gameClient != null)
                {
                    bClearAllok = m_gameClient.ClearAll();
                }
            }

            return bDisconnectok && bClearAllok;
        }
        //结束
        public bool UnInit()
        {
            //_Instance = null;
            //关闭文件句柄
            fxVerify.UnInitalize();
            Log.CloseFile();
            return true;
        }

        public IGameClient GetGameClient()
        {
            return m_gameClient;
        }

        public IGameSock GetGameSock()
        {
            return m_gameSock;
        }

        //帧循环
        public void Excutue()
        {
            if (m_gameSock != null)
            {
                ((GameSock)m_gameSock).ProcessMessage();
            }
        }
    }
}
