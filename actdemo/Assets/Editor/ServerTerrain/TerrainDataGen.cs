using UnityEngine;
using UnityEditor;
using System.Collections;

using Demo_CSharp;

public class TerrainDataGen : EditorWindow
{
    private static TerrainDataGen window;

    [MenuItem("KPlaneTools/生成当前地形网格")]
    private static void OpenWindow()
    {
        window = (TerrainDataGen)EditorWindow.GetWindow(typeof(TerrainDataGen));
    }

    public GameObject water;

    void OnGUI()
    {        
        if (GUILayout.Button("生成地形网格信息"))
        {
            Demo_CSharp.CSharpFunction.Main(this);
        }
    }

    bool IsValidPoint(ref Vector3 pt)
    {
        if (IsValidX(ref pt.x) && IsValidY(ref pt.y) && IsValidZ(ref pt.z))
            return true;
        return false;
    }
    bool IsValidX(ref float x)
    {
        if (x < 0 || x >= Terrain.activeTerrain.terrainData.size.x)
            return false;
        return true;
    }
    bool IsValidY(ref float y)
    {
        if (y < 0 || y >= Terrain.activeTerrain.terrainData.size.y)
            return false;
        return true;
    }
    bool IsValidZ(ref float z)
    {
        if (z < 0 || z >= Terrain.activeTerrain.terrainData.size.z)
            return false;
        return true;
    }

    // 球体范围碰撞实现
    public bool TraceSphere(Vector3 _center, float radius)
    {
        Vector3 center = _center;

        center.x -= CFunction.GetTerrainLeft();
        center.z -= CFunction.GetTerrainTop();

        return InnerTraceSphere(center, radius);

    }

    private bool InnerTraceSphere(Vector3 center, float radius)
    {
        if (!IsValidPoint(ref center))
        {
            //Debug.LogWarning("InnerTraceSphere out of range!!!");
            return false;
        }

        RaycastHit hit;
        if (Physics.SphereCast(center, radius, Vector3.one, out hit, radius))
        {
            //Debug.Log (hit.collider.gameObject);
            return true;
        }

        return false;
    }

    // 碰撞测试是否有阻挡实现
    public bool TraceHitted(Vector3 _src, Vector3 _dst)
    {
        Vector3 src = _src;
        Vector3 dst = _dst;

        src.x -= CFunction.GetTerrainLeft();
        src.z -= CFunction.GetTerrainTop();

        dst.x -= CFunction.GetTerrainLeft();
        dst.z -= CFunction.GetTerrainTop();
        
        return InnerTraceHitted(src, dst);
    }
    private bool InnerTraceHitted(Vector3 src, Vector3 dst)
    {
        if (!IsValidPoint(ref src) || !IsValidPoint(ref dst))
        {
            //Debug.LogWarning("InnerTraceHitted out of range!!!");
            return false;
        }

        //Debug.DrawLine(src, dst,Color.red);

        RaycastHit hit;
        if (Physics.Linecast(src, dst, out hit))
        {
            //Debug.Log (hit.collider.gameObject);
            return true;
        }

        return false;
    }

    public bool TraceDetail(Vector3 _src, Vector3 _dst, ref RaycastHit[] hits, ref int hit_count)
    {
        Vector3 src = _src;
        Vector3 dst = _dst;

        src.x -= CFunction.GetTerrainLeft();
        src.z -= CFunction.GetTerrainTop();

        dst.x -= CFunction.GetTerrainLeft();
        dst.z -= CFunction.GetTerrainTop();

        return InnerTraceDetail(src, dst, ref hits, ref hit_count);
    }

    private bool InnerTraceDetail(Vector3 src, Vector3 dst, ref RaycastHit[] hits, ref int hit_count)
    {
        if (!IsValidPoint(ref src) || !IsValidPoint(ref dst))
        {
            //Debug.LogWarning("InnerTraceDetail out of range!!!");
            return false;
        }

        float dist = Vector3.Distance(src, dst);
        Vector3 origin = dst - src;

        //Debug.DrawRay(src, origin, Color.blue,dist);

        RaycastHit[] hit = Physics.RaycastAll(src, origin, dist);

        hit_count = 0;
        while ((hit_count < hits.Length) && (hit_count < hit.Length))
        {
            hits[hit_count] = hit[hit_count];
            //Debug.Log(hit);
            hit_count++;
        }

        if (hit_count > 0)
            return true;

        return false;
    }


    // 获得精确的地面高度实现
    public float GetGroundHeight(float x, float z)
    {
        x -= CFunction.GetTerrainLeft();
        z -= CFunction.GetTerrainTop();

        return InnerGetGroundHeight(x, z);
    }

    private float InnerGetGroundHeight(float x, float z)
    {
        if (!IsValidX(ref x) || !IsValidZ(ref z))
        {
            //Debug.LogWarning("InnerGetGroundHeight out of range!!!");
            return 0;
        }

        return Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
    }

    // 水面是否存在实现
    public bool GetWaterExists(float x, float z)
    {
        x -= CFunction.GetTerrainLeft();
        z -= CFunction.GetTerrainTop();

        return InnerGetWaterExists(x, z);
    }

    private bool InnerGetWaterExists(float x, float z)
    {
        if (!IsValidX(ref x) || !IsValidZ(ref z))
        {
            //Debug.LogWarning("InnerGetWaterExists out of range!!!");
            return false;
        }

        if (!water)
            return false;

        return water.transform.position.y > InnerGetGroundHeight(x, z);
    }

    // 获得水面基本高度实现
    public float GetWaterBaseHeight(float x, float z)
    {
        x -= CFunction.GetTerrainLeft();
        z -= CFunction.GetTerrainTop();

        return InnerGetWaterBaseHeight(x, z);
    }

    private float InnerGetWaterBaseHeight(float x, float z)
    {
        if (!IsValidX(ref x) || !IsValidZ(ref z))
        {
            //Debug.LogWarning("InnerGetWaterBaseHeight out of range!!!");
            return 0;
        }
        
        if (!water) return 0;

        float offset = water.transform.position.y - InnerGetGroundHeight(x, z);
        return (offset > 0 ? offset : 0);
    }
}
