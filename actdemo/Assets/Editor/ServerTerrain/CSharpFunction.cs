using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using UnityEngine;

namespace Demo_CSharp
{
	class CSharpFunction
    {	
		public static TerrainDataGen game_obj;
        // 碰撞测试详细信息实现
        public bool TraceDetail(ref D3DXVECTOR3 src, ref D3DXVECTOR3 dst, ref DxTraceInfo result)
        {
			Vector3 _src;
			_src.x = src.x;
			_src.y = src.y;
			_src.z = src.z;
            
			Vector3 _dst;
			_dst.x = dst.x;
			_dst.y = dst.y;
			_dst.z = dst.z;
			
			float dis = Vector3.Distance(_src, _dst);
			if(dis<=0)
			{
				return false;
			}
			
			RaycastHit[] hits = new RaycastHit[result.nHitMax];
			
 			if(!game_obj.TraceDetail(_src,_dst, ref hits, ref result.nHitCount))
			{
				return false;
			}
			
			Vector3 origin = _dst - _src;
			
			//RaycastHit 转换成 DxTraceInfo
			result.fDistance = 1e8f; 
			for( int i = 0; i < result.nHitCount; ++i)
			{
				RaycastHit hit = hits[i];
		
				result.Hits[i].fDistance = hit.distance / dis;
				if(result.fDistance > result.Hits[i].fDistance)
					result.fDistance = result.Hits[i].fDistance;
				
				if((hit.collider && hit.collider.gameObject) && (Terrain.activeTerrain.gameObject == hit.collider.gameObject))
					result.Hits[i].TraceID = new PERSISTID(0);
				else
					result.Hits[i].TraceID = new PERSISTID(1);
				
				result.Hits[i].nMaterialIndex = 0;
				result.Hits[i].strTexName = "";
				
				if (hit.normal.y != 0.0f)
				{
					Vector3 p = hit.point + hit.normal;
					
					float y = (Vector3.Dot(hit.normal, hit.point) - hit.normal.x * p.x - hit.normal.z * p.z) / hit.normal.y;
					Vector3 p1 = new Vector3(p.x, y, p.z);
					Vector3 n = Vector3.Cross(hit.normal, p1);
					Vector3 p2 = n + hit.point;
					
					result.Hits[i].Vertex[0] = hit.point;
					result.Hits[i].Vertex[1] = p1;
					result.Hits[i].Vertex[2] = p2;
					
					result.Hits[i].fTraceU = 0.0f;
					result.Hits[i].fTraceV = 0.0f;
					
					continue;
				}
				
				if (hit.normal.x != 0.0f)
				{
					Vector3 p = hit.point + hit.normal;
					
					float x = (Vector3.Dot(hit.normal, hit.point) - hit.normal.y * p.y - hit.normal.z * p.z) / hit.normal.x;
					Vector3 p1 = new Vector3(x, p.y, p.z);
					Vector3 n = Vector3.Cross(hit.normal, p1);
					Vector3 p2 = n + hit.point;
					
					result.Hits[i].Vertex[0] = hit.point;
					result.Hits[i].Vertex[1] = p1;
					result.Hits[i].Vertex[2] = p2;
					
					result.Hits[i].fTraceU = 0.0f;
					result.Hits[i].fTraceV = 0.0f;
					
					continue;
				}
				
				if (hit.normal.z != 0.0f)
				{
					Vector3 p = hit.point + hit.normal;
					
					float z = (Vector3.Dot(hit.normal, hit.point) - hit.normal.x * p.x - hit.normal.y * p.y) / hit.normal.z;
					Vector3 p1 = new Vector3(p.x, p.y, z);
					Vector3 n = Vector3.Cross(hit.normal, p1);
					Vector3 p2 = n + hit.point;
					
					result.Hits[i].Vertex[0] = hit.point;
					result.Hits[i].Vertex[1] = p1;
					result.Hits[i].Vertex[2] = p2;
					
					result.Hits[i].fTraceU = 0.0f;
					result.Hits[i].fTraceV = 0.0f;
					
					continue;
				}
			}
			
            return true;
        }

        public void SetTraceDetail()
        {
            CFunction.tdc = new CFunction.TraceDetailCallback(TraceDetail);
            CFunction.SetTraceDetailCallback(CFunction.tdc);
        }

        // 球体范围碰撞实现
        public bool TraceSphere(ref D3DXVECTOR3 center, float radius)
        {
			Vector3 _center ;
			_center.x = center.x;
			_center.y = center.y;
			_center.z = center.z;
			return game_obj.TraceSphere( _center,radius);
        }

        public void SetTraceSphere()
        {
            CFunction.tsc = new CFunction.TraceSphereCallback(TraceSphere);
            CFunction.SetTraceSphereCallback(CFunction.tsc);
        }

        // 碰撞测试是否有阻挡实现
        public bool TraceHitted(ref D3DXVECTOR3 src, ref D3DXVECTOR3 dst)
        {
			Vector3 _src;
			_src.x = src.x;
			_src.y = src.y;
			_src.z = src.z;
            
			Vector3 _dst;
			_dst.x = dst.x;
			_dst.y = dst.y;
			_dst.z = dst.z;
			
			return game_obj.TraceHitted( _src, _dst);
        }

        public void SetTraceHitted()
        {
            CFunction.thc = new CFunction.TraceHittedCallback(TraceHitted);
            CFunction.SetTraceHittedCallback(CFunction.thc);
        }

		
        // 获得精确的地面高度实现
        public float GetGroundHeight(float x, float z)
        {
            return game_obj.GetGroundHeight(x,z);
        }
        

        public void SetGetGroundHeight()
        {
            CFunction.gghc = new CFunction.GetGroundHeightCallback(GetGroundHeight);
            CFunction.SetGetGroundHeightCallback(CFunction.gghc);
        }

        // 水面是否存在实现
        public bool GetWaterExists(float x, float z)
        {
            return game_obj.GetWaterExists(x,z);
        }

        public void SetGetWaterExists()
        {
            CFunction.gwec = new CFunction.GetWaterExistsCallback(GetWaterExists);
            CFunction.SetGetWaterExistsCallback(CFunction.gwec);
        }

        // 获得水面基本高度实现
        public float GetWaterBaseHeight(float x, float z)
        {
            return game_obj.GetWaterBaseHeight(x,z);
        }

        public void SetGetWaterBaseHeight()
        {
            CFunction.gwbhc = new CFunction.GetWaterBaseHeightCallback(GetWaterBaseHeight);
            CFunction.SetGetWaterBaseHeightCallback(CFunction.gwbhc);
        }

       public static void Main(TerrainDataGen obj)
        {
			game_obj = obj;
            {
                CSharpFunction cSharpF = new CSharpFunction();

                // 创建中间件
                bool sucess = CFunction.Create(CFunction.DLL_DIR);
                if (!sucess)
                {
                    CFunction.CoreTrace("Create failed");
                    return;
                }
				CFunction.CoreTrace("Create sucess");
				
				// 设置地形路径
				CFunction.SetTerrainPath(@"./map/ter/111/");
				
                // 初始化中间件
                sucess = CFunction.Init(CFunction.FILE_NAME);
                if (!sucess)
                {
                    CFunction.CoreTrace("Init failed");
                    return;
                }
				CFunction.CoreTrace("Init sucess");
				
				cSharpF.SetTraceDetail();
                cSharpF.SetTraceSphere();
                cSharpF.SetTraceHitted();
                cSharpF.SetGetGroundHeight();
                cSharpF.SetGetWaterExists();
                cSharpF.SetGetWaterBaseHeight();
				
				int terrain_width = (int)Terrain.activeTerrain.terrainData.size.x;
				int terrain_height = (int)Terrain.activeTerrain.terrainData.size.z;
				
				uint zone_scale = CFunction.GetZoneScale();
				int zone_rows = (int)(terrain_height / zone_scale);
				int zone_cols = (int)(terrain_width / zone_scale);
				
				// 高度数据
		        uint rows = zone_scale + 1;
		        uint cols = zone_scale + 1;
		        float[] hv = new float[rows * cols];
				
				for (int r = 0; r < zone_rows; ++r)
				{
					for (int c = 0; c < zone_cols; ++c)
					{
						int zone_index = r * zone_cols + c;
						// 区域索引
		                System.String zone_name = "";
		
		                IntPtr intPtr = CFunction.GetZoneName(zone_index);
		                if (intPtr == (IntPtr)0)
		                {
		                    CFunction.CoreTrace("GetZoneName failed");
		                    continue;
		                }
		
		                zone_name =  Marshal.PtrToStringAnsi(intPtr);
		
		
		                sucess = (zone_name != "");
		                if (!sucess)
		                {
		                    CFunction.CoreTrace("zone_name error");
		                    continue;
		                }
						CFunction.CoreTrace("GetZoneName sucess");
						
		                float min_y = 1e8f;
		                float max_y = -1e8f;
						int zone_row_offset = (int)(r * zone_scale);
						int zone_col_offset = (int)(c * zone_scale);
						
						float zone_left = 0.0f;
						float zone_top = 0.0f;
						CFunction.GetZoneLeft(zone_index, ref zone_left);
						CFunction.GetZoneTop(zone_index, ref zone_top);
		                for (uint i = 0; i < rows; ++i)
		                {
		                    for (uint j = 0; j < cols; ++j)
		                    {
		                        hv[i * cols + j] = game_obj.GetGroundHeight(zone_left + j, zone_top + i);
		                        if (hv[i * cols + j] < min_y)
		                        {
		                            min_y = hv[i * cols + j];
		                        }
		
		                        if (hv[i * cols + j] > max_y)
		                        {
		                            max_y = hv[i * cols + j];
		                        }
		                    }
		                }
		                IntPtr heighs = Marshal.AllocHGlobal((int)(sizeof(float) * hv.Length));
		                Marshal.Copy(hv, 0, heighs, hv.Length);
		                IntPtr normals = new IntPtr();
		
		                sucess = CFunction.BuildZone(zone_index, rows, cols, min_y, max_y, heighs, normals);
		                if (!sucess)
		                {
		                    CFunction.CoreTrace("BuildZone failed");
		                    continue;
		                }

						CFunction.CoreTrace("BuildZone sucess");
		
		                sucess = CFunction.GenWalkBegin(zone_index, 1, 1.8f, 0.2f, 0.5f, 100.0f, 0.0f, 0.7f, 0.34f, 15.0f);
		                if (!sucess)
		                {
		                    CFunction.CoreTrace("GenWalkBegin failed");
		                    continue;
		                }
						CFunction.CoreTrace("GenWalkBegin sucess");
		
		                //sucess = CFunction.GenWalkAddPermit(zone_index, 0, 10.0f, 0);
		                //if (!sucess)
		                //{
		                //    CFunction.CoreTrace("GenWalkAddPermit failed");
		                //    continue;
		                //}
						//CFunction.CoreTrace("GenWalkAddPermit sucess");
		
		                //sucess = CFunction.GenWalkAddForbid(zone_index, 0, 0, 0, 1);
		                //if (!sucess)
		                //{
		                //    CFunction.CoreTrace("GenWalkAddForbid failed");
		                //    continue;
		                //}
						//CFunction.CoreTrace("GenWalkAddForbid sucess");
		
		                sucess = CFunction.GenWalkGround(zone_index);
		                if (!sucess)
		                {
		                    CFunction.CoreTrace("GenWalkGround failed");
		                    continue;
		                }
						CFunction.CoreTrace("GenWalkGround sucess");
		
		                int row = 0;
		
		                while (sucess)
		                {
		                    sucess = CFunction.GenWalkBalk(zone_index, row);
		
		                    row = row + 1;
		                }
		                
						float left = 0;
						float top = 0;
						CFunction.GetZoneLeft(zone_index, ref left);
						CFunction.GetZoneTop(zone_index, ref top);
						
						float zone_center_x = zone_scale * (r + 0.5f);
						float zone_center_z = zone_scale * (c + 0.5f);
						float zone_center_y = game_obj.GetGroundHeight(zone_center_x, zone_center_z);
				
		                sucess = CFunction.GenWalkable(zone_index, zone_center_x + left , zone_center_y, zone_center_z + top);
		                //if (!sucess)
		                //{
		                //    CFunction.CoreTrace("GenWalkable failed");
		                //    continue;
		                //}
						CFunction.CoreTrace("GenWalkable sucess");
		
		                sucess = CFunction.GenWalkFloor(zone_index);
		                //if (!sucess)
		                //{
		                //    CFunction.CoreTrace("GenWalkFloor failed");
		                //    continue;
		                //}
						CFunction.CoreTrace("GenWalkFloor sucess");
		
		                sucess = CFunction.GenWalkWall(zone_index);
		                //if (!sucess)
		                //{
		                //    CFunction.CoreTrace("GenWalkWall failed");
		                //    continue;
		                //}
						CFunction.CoreTrace("GenWalkWall sucess");
		
		                sucess = CFunction.GenWalkWater(zone_index);
		                //if (!sucess)
		                //{
		                //    CFunction.CoreTrace("GenWalkWater failed");
		                //   continue;
		                //}
						CFunction.CoreTrace("GenWalkWater sucess");
		
		                sucess = CFunction.GenWalkEnd(zone_index);
		                if (!sucess)
		                {
		                    CFunction.CoreTrace("GenWalkEnd failed");
		                    continue;
		                }
						CFunction.CoreTrace("GenWalkEnd sucess");
		
		                sucess = CFunction.SaveWalk(zone_index, CFunction.FILE_NAME + "walk/" + zone_name + ".walk");
		                if (!sucess)
		                {
		                    CFunction.CoreTrace("SaveWalk failed");
		                    continue;
		                }
						CFunction.CoreTrace("SaveWalk sucess");
		
		                sucess = CFunction.SaveHeight(zone_index, CFunction.FILE_NAME + zone_name + ".height");
		                if (!sucess)
		                {
		                    CFunction.CoreTrace("SaveHeight failed");
		                    continue;
		                }
						CFunction.CoreTrace("SaveHeight sucess");
		
		                sucess = CFunction.SaveHeightCompress(zone_index, CFunction.FILE_NAME + zone_name + ".ground");
		                if (!sucess)
		                {
		                    CFunction.CoreTrace("SaveHeightCompress failed");
		                    continue;
		                }
						CFunction.CoreTrace("SaveHeightCompress sucess");
					}
				}
				
                // 销毁中间件
                CFunction.Destroy();
            }

        }
		
		
		public  bool trace_triangle( ref Vector3 v0, 
			ref Vector3 v1, ref Vector3 v2, ref Vector3 src, 
			ref Vector3 dir, ref DxTraceInfo result, ref bool hitted)
		{
			float t = 0;
			float u = 0;
			float v = 0;
		
			// 射线与三角形相交
			if (VisUtil_IntersectTri(ref v0, ref v1, ref v2, ref src, ref dir, ref u, ref v, ref t))
			{
				if ((t >= 0.0f) && (t <= 1.0f))
				{
					if (result.nHitMax > 1)
					{
						if (result.nHitCount >= result.nHitMax)
						{
							return false;
						}
		
						hitted = true;
		
						if (t < result.fDistance)
						{
							// 距离最近的碰撞点
							result.fDistance = t;
						}
		
						DxTraceInfo.hit_t pHit = result.Hits[result.nHitCount];
		
						pHit.fDistance = t;
						pHit.fTraceU = u;
						pHit.fTraceV = v;
						// 设置为地形对象
						pHit.TraceID = new PERSISTID(0);
						pHit.nMaterialIndex = 0;
						pHit.Vertex[0] = v0;
						pHit.Vertex[1] = v1;
						pHit.Vertex[2] = v2;
						pHit.strTexName = "";
		
						if (++result.nHitCount >= result.nHitMax)
						{
							return false;
						}
					}
					else if (t < result.fDistance)
					{
						hitted = true;
		
						// 距离最近的碰撞点
						result.fDistance = t;
		
						DxTraceInfo.hit_t pHit = result.Hits[0];
		
						pHit.fDistance = t;
						pHit.fTraceU = u;
						pHit.fTraceV = v;
						// 设置为地形对象
						pHit.TraceID = new PERSISTID(0);
						pHit.nMaterialIndex = 0;
						pHit.Vertex[0] = v0;
						pHit.Vertex[1] = v1;
						pHit.Vertex[2] = v2;
						pHit.strTexName = "";
		
						result.nHitCount = 1;
					}
				}
			}
		
			return true;
		}

		// 内联三角形射线计算
		public bool VisUtil_IntersectTri(ref Vector3 v0,
			ref Vector3 v1, ref Vector3 v2, ref Vector3 orig,
			ref Vector3 dir, ref float u, ref float v, ref float t)
		{   
			// Find vectors for two edges sharing vert0  
			Vector3 edge1 = v1 - v0;   
			Vector3 edge2 = v2 - v0;    
		
			// Begin calculating determinant - also used to calculate U parameter   
			Vector3 pvec;   
		
			pvec = Vector3.Cross(dir, edge2); 
		
			// If determinant is near zero, ray lies in plane of triangle  
			float det = Vector3.Dot( edge1,  pvec);   
			
			Vector3 tvec; 
		
			if (det > 0)   
			{    
				tvec = orig - v0;   
			}  
			else   
			{      
				tvec = v0 - orig;  
				det = -det; 
			}
		
			if (det < 0.0001F) 
			{
				return false; 
			}
		
			// Calculate U parameter and test bounds  
			u = Vector3.Dot( tvec,  pvec);  
		
			if ((u < 0.0F) || (u > det))
			{
				return false;  
			}
		
			// Prepare to test V parameter    
			Vector3 qvec; 
		
			qvec = Vector3.Cross( tvec, edge1); 
		
			// Calculate V parameter and test bounds  
			v = Vector3.Dot(dir,  qvec); 
		
			if ((v < 0.0F) || ((u + v) > det))  
			{
				return false;   
			}
		
			// Calculate t, scale parameters, ray intersects triangle  
			t = Vector3.Dot( edge2,  qvec);  
		
			float fInvDet = 1.0F / det;  
		
			t *= fInvDet;  
			u *= fInvDet;   
			v *= fInvDet;   
		
			return true;
		}
		
		
		
    }
}
