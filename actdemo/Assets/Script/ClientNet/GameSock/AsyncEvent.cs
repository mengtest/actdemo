//USRecvBuff.cs
//接收缓冲类

using System;
using System.Collections.Generic;

namespace Fm_ClientNet
{
    public class AsyncEvent
    {
        #region 数据接收缓冲结构定义

        public const int ASYNCRECVBUF_COUNT = 0; //缓冲数量
        public const int ASYNCRECVBUF_LENGTH = 0x1000; //缓冲体大小

        //消息接收结构体
        public class RecvBufferData
        {
            public byte[] m_abyRecvBuffer = new byte[ASYNCRECVBUF_LENGTH];
            public int m_iRecvSize = 0;
        }

        public enum e_SocketEvent
        {
            E_SOCKETEVENT_CONNECTED,
            E_SOCKETEVENT_CONNECTFAILED,
            E_SOCKETEVENT_CLOSED,
        }

        //事件结构体
        public class EventData
        {
            public e_SocketEvent m_eEvent;
            public string m_strIP;
            public int m_iPort;

            public EventData(e_SocketEvent eEvent)
            {
                m_eEvent = eEvent;
            }

            public EventData(e_SocketEvent eEvent, string strIP, int iPort)
            {
                m_eEvent = eEvent;
                m_strIP = strIP;
                m_iPort = iPort;
            }
        }

        //空闲缓冲队列
        private Queue<RecvBufferData> m_queueRecvBufferFree = new Queue<RecvBufferData>();
        private Queue<RecvBufferData> m_queueRecvBufferRecv = new Queue<RecvBufferData>();
        private Queue<EventData> m_queueEvents = new Queue<EventData>();

        //互斥锁声明
        private static Object s_AsyncLockEvent = new Object();
        private static Object s_AsyncLockFree = new Object();
        private static Object s_AsyncLockRecv = new Object();
        #endregion

        public AsyncEvent()
        {
            //初始化缓冲队列
            for (int i = 0; i < ASYNCRECVBUF_COUNT; ++i)
            {
                m_queueRecvBufferFree.Enqueue(new RecvBufferData());
            }
        }

        //获得空闲数据缓冲
        public RecvBufferData GetBufferData()
        {
            RecvBufferData FreeData = null;
            lock (s_AsyncLockFree)
            {
                if (m_queueRecvBufferFree.Count > 0)
                {
                    FreeData = m_queueRecvBufferFree.Dequeue();
                }
            }
            if (FreeData == null)
            {
                FreeData = new RecvBufferData();
            }
            else
            {
                Array.Clear(FreeData.m_abyRecvBuffer,0,FreeData.m_abyRecvBuffer.Length);
            }

            return FreeData;
        }
        public void PushFreeData(RecvBufferData data)
        {
            lock (s_AsyncLockFree)
            {
                m_queueRecvBufferFree.Enqueue(data);
            }
        }
        //将网络数据压入队列
        public void PushRecvData(RecvBufferData Data)
        {
            lock (s_AsyncLockRecv)
            {
                m_queueRecvBufferRecv.Enqueue(Data);
            }
        }

        //将消息体压入链表
        public void PushEvent(e_SocketEvent eEvent, string strIP, int iPort)
        {
            lock (s_AsyncLockEvent)
            {
                m_queueEvents.Enqueue(new EventData(eEvent, strIP, iPort));
            }
        }
        public void PushEvent(e_SocketEvent eEvent)
        {
            lock (s_AsyncLockEvent)
            {
                m_queueEvents.Enqueue(new EventData(eEvent));
            }
        }

        bool InnerPeekData(ref RecvBufferData Data)
        {
            bool bHasData = false;
            lock (s_AsyncLockRecv)
            {
                if (m_queueRecvBufferRecv.Count > 0)
                {
                    Data = m_queueRecvBufferRecv.Dequeue();
                    bHasData = true;
                }
            }

            return bHasData;
        }

        bool InnerPeekEvent(ref EventData Event)
        {
            bool bHasData = false;
            lock (s_AsyncLockEvent)
            {
                if (m_queueEvents.Count > 0)
                {
                    Event = m_queueEvents.Dequeue();
                    bHasData = true;
                }
            }

            return bHasData;
        }

        //读取链表缓冲数据
        public bool PeekData(ref RecvBufferData Data, ref EventData Event)
        {
            if (InnerPeekEvent(ref Event))
            {               
                return true;
            }
            return InnerPeekData(ref Data);
        }
    }
}