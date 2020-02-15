using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
namespace Demo_CSharp
{
    public struct D3DXVECTOR2 
    {
        public float x, y;
    }

    public struct D3DXVECTOR3 
    {
        public float x, y, z;

        public D3DXVECTOR3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    // 碰撞结果信息
    public struct DxTraceInfo
    {
        // 碰撞数据
        public struct hit_t
        {
            public float fDistance;         // 碰撞距离
            public float fTraceU;           // 碰撞位置
            public float fTraceV;
            public PERSISTID TraceID;       // 碰撞对象
            public int nMaterialIndex;      // 材质索引
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Vector3[] Vertex;    // 三角形顶点
            public String strTexName;       // 贴图名
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public D3DXVECTOR2[] TexUV;     // 三角形顶点的贴图UV
        };

        public int nHitMax;                 // 最多碰撞次数
        public int nHitCount;               // 发生碰撞次数
        public float fDistance;             // 最近碰撞距离(0到1)
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public hit_t[] Hits;

        //public DxTraceInfo()
        //{
        //    nHitMax = 1;
        //    nHitCount = 0;
        //    fDistance = 1e8;
        //}
    };
}
