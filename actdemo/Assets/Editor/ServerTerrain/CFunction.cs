using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Demo_CSharp
{
    class CFunction
    {
        // FxTerrain.dll路径
        public const System.String DLL_PATH = @"./FxTerrain.dll";
        // 场景路径
        public static System.String FILE_NAME = @"./map/ter/111/";
        // FxCore.dll目录
        public const System.String DLL_DIR = @"./";
      	public static TraceDetailCallback tdc;
        public static TraceSphereCallback tsc;
        public static TraceHittedCallback thc;
        public static GetGroundHeightCallback gghc;
        public static GetWaterExistsCallback gwec;
        public static GetWaterBaseHeightCallback gwbhc;

        // 创建中间件
        [DllImport(DLL_PATH, EntryPoint = "Create", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Create(System.String path);

        // 销毁中间件
        [DllImport(DLL_PATH, EntryPoint = "Destroy", CharSet = CharSet.Ansi)]
        public static extern void Destroy();

        // 初始化中间件
        [DllImport(DLL_PATH, EntryPoint = "Init", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Init(System.String file_name);

        // 设置碰撞测试详细信息回调
        public delegate bool TraceDetailCallback(ref D3DXVECTOR3 src, ref D3DXVECTOR3 dst, ref DxTraceInfo result);
        [DllImport(DLL_PATH, EntryPoint = "SetTraceDetailCallback", CharSet = CharSet.Ansi)]
        public static extern void SetTraceDetailCallback(TraceDetailCallback tdc);

        // 设置球体范围碰撞回调
        public delegate bool TraceSphereCallback(ref D3DXVECTOR3 center, float radius);
        [DllImport(DLL_PATH, EntryPoint = "SetTraceSphereCallback", CharSet = CharSet.Ansi)]
        public static extern void SetTraceSphereCallback(TraceSphereCallback tsc);

        // 设置碰撞测试是否有阻挡回调
        public delegate bool TraceHittedCallback(ref D3DXVECTOR3 src, ref D3DXVECTOR3 dst);
        [DllImport(DLL_PATH, EntryPoint = "SetTraceHittedCallback", CharSet = CharSet.Ansi)]
        public static extern void SetTraceHittedCallback(TraceHittedCallback thc);

        // 设置获得精确的地面高度回调
        public delegate float GetGroundHeightCallback(float x, float z);
        [DllImport(DLL_PATH, EntryPoint = "SetGetGroundHeightCallback", CharSet = CharSet.Ansi)]
        public static extern void SetGetGroundHeightCallback(GetGroundHeightCallback gghc);

        // 设置水面是否存在回调
        public delegate bool GetWaterExistsCallback(float x, float z);
        [DllImport(DLL_PATH, EntryPoint = "SetGetWaterExistsCallback", CharSet = CharSet.Ansi)]
        public static extern void SetGetWaterExistsCallback(GetWaterExistsCallback gwec);

        // 设置获得水面基本高度回调
        public delegate float GetWaterBaseHeightCallback(float x, float z);
        [DllImport(DLL_PATH, EntryPoint = "SetGetWaterBaseHeightCallback", CharSet = CharSet.Ansi)]
        public static extern void SetGetWaterBaseHeightCallback(GetWaterBaseHeightCallback gwbhc);

        // 取地形Left
        [DllImport(DLL_PATH, EntryPoint = "GetTerrainLeft", CharSet = CharSet.Ansi)]
        public static extern float GetTerrainLeft();

        // 取地形Top
        [DllImport(DLL_PATH, EntryPoint = "GetTerrainTop", CharSet = CharSet.Ansi)]
        public static extern float GetTerrainTop();
		
		// 取区域行列数
        [DllImport(DLL_PATH, EntryPoint = "GetZoneScale", CharSet = CharSet.Ansi)]
        public static extern uint GetZoneScale();

        // 取区域Left
        [DllImport(DLL_PATH, EntryPoint = "GetZoneLeft", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetZoneLeft(int zone_index, ref float left);

        // 取区域Top
        [DllImport(DLL_PATH, EntryPoint = "GetZoneTop", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetZoneTop(int zone_index, ref float top);

        // 建立区域数据
        [DllImport(DLL_PATH, EntryPoint = "BuildZone", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool BuildZone(int zone_index, uint rows, uint cols, float min_y, float max_y, IntPtr heights, IntPtr normals);

        // 获得区域的名称
        [DllImport(DLL_PATH, EntryPoint = "GetZoneName", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetZoneName(int zone_index);

        // 获得区域及其中所有物体的平面范围
        [DllImport(DLL_PATH, EntryPoint = "GetZoneRect", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetZoneRect(int zone_index, ref float min_x, ref float min_z, ref float max_x, ref float max_z);

        // 添加允许生成碰撞行走层点
        [DllImport(DLL_PATH, EntryPoint = "GenWalkAddPermit", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GenWalkAddPermit(int zone_index, float x, float y, float z);

        // 添加禁止生成碰撞行走层的范围
        [DllImport(DLL_PATH, EntryPoint = "GenWalkAddForbid", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GenWalkAddForbid(int zone_index, float x, float y, float z, float radius);

        // 添加场景有效范围栅栏点
        [DllImport(DLL_PATH, EntryPoint = "GenWalkAddFence", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GenWalkAddFence(int fence_index, float x, float z);

        // 开始生成碰撞信息
        [DllImport(DLL_PATH, EntryPoint = "GenWalkBegin", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GenWalkBegin(int zone_index, int precision, 
            float role_height, float role_radius, float step_height, 
            float trace_high, float trace_low, 
            float ground_normal_y, float floor_normal_y, float wall_angle);

        // 生成地表高度
        [DllImport(DLL_PATH, EntryPoint = "GenWalkGround", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GenWalkGround(int zone_index);

        // 生成障碍物高度
        [DllImport(DLL_PATH, EntryPoint = "GenWalkBalk", CharSet = CharSet.Ansi)]
        [return:MarshalAs(UnmanagedType.I1)]
        public static extern bool GenWalkBalk(int zone_index, int row);

        // 生成可行走数据
        [DllImport(DLL_PATH, EntryPoint = "GenWalkable", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GenWalkable(int zone_index, 
            float start_x, float start_y, float start_z);

        // 生成可行走层高度
        [DllImport(DLL_PATH, EntryPoint = "GenWalkFloor", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GenWalkFloor(int zone_index);

        // 生成墙面标记
        [DllImport(DLL_PATH, EntryPoint = "GenWalkWall", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GenWalkWall(int zone_index);

        // 生成水面信息
        [DllImport(DLL_PATH, EntryPoint = "GenWalkWater", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GenWalkWater(int zone_index);

        // 结束生成碰撞信息
        [DllImport(DLL_PATH, EntryPoint = "GenWalkEnd", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GenWalkEnd(int zone_index);

        // 保存指定区域的客户端行走信息
        [DllImport(DLL_PATH, EntryPoint = "SaveWalk", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SaveWalk(int zone_index, System.String file_name);

        // 保存指定区域的顶点高度数据
        [DllImport(DLL_PATH, EntryPoint = "SaveHeight", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SaveHeight(int zone_index, System.String file_name);

        // 保存压缩过的指定区域的顶点高度数据
        [DllImport(DLL_PATH, EntryPoint = "SaveHeightCompress", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SaveHeightCompress(int zone_index, System.String file_name);

        // 输出日志
        [DllImport(DLL_PATH, EntryPoint = "CoreTrace", CharSet = CharSet.Ansi)]
        public static extern void CoreTrace(System.String info);
		
		public static void SetTerrainPath(System.String path)
		{
			CFunction.FILE_NAME = path;
		}
    }
}
