using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ExportSceneToObj : EditorWindow
{
    [MenuItem("KPlaneTools/导出场景Obj")]
    private static void Execute()
    {
        EditorWindow.CreateInstance<ExportSceneToObj>().Show();
    }

    private int indexOffset = 0;

    private string saveToDir = new DirectoryInfo(Application.dataPath).Parent.FullName;
    //private string saveToDir = "Assets"; 

    private string saveName = "exportScene.obj";

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("将整个场景导出为一个Obj模型\n保存到" + saveToDir + "/" + saveName, MessageType.Info);

        EditorGUILayout.Space();

        if (GUILayout.Button("开始导出"))
        {
            indexOffset = 0;

            string filePath = saveToDir + "/" + saveName;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                AssetDatabase.Refresh();
            }

            StreamWriter sw = new StreamWriter(filePath);

            sw.WriteLine("# ExportSceneToObj File");

            ExportMeshes(sw);
            ExportTerrains(sw);

            sw.Close();

            Progress("生成完成，正在预处理");
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            EditorUtility.OpenWithDefaultApp(saveToDir);

            EditorUtility.ClearProgressBar();
        }
    }

    private void ExportMeshes(StreamWriter sw)
    {
        MeshFilter[] mfs = Object.FindObjectsOfType<MeshFilter>();
        int numMFs = mfs == null ? 0 : mfs.Length;
        for (int i = 0; i < numMFs; ++i)
        {
            MeshFilter mf = mfs[i];
            if (mf.sharedMesh != null)
            {
                sw.WriteLine("g " + mf.gameObject.name);

                Progress("生成模型" + mf.gameObject.name);
                Matrix4x4 o2w = mf.transform.localToWorldMatrix;
                ExportMesh(mf.sharedMesh, o2w, sw);
            }
        }
    }

    private void ExportMesh(Mesh mesh, Matrix4x4 o2w, StreamWriter sw)
    {
        Vector3[] vertices = mesh.vertices;
        int numVertices = vertices == null ? 0 : vertices.Length;
        for (int i = 0; i < numVertices; ++i)
        {
            vertices[i] = o2w.MultiplyPoint(vertices[i]);
            vertices[i].x *= -1;
        }

        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;

        Vector3[] normals = mesh.normals;
        int numNormals = normals == null ? 0 : normals.Length;
        for (int i = 0; i < numNormals; ++i)
        {
            Vector3 n = normals[i];
            n = o2w.MultiplyVector(n);
            n.x *= -1;
            n.Normalize();
            normals[i] = n;
        }

        WriteToBuffer(vertices, uvs, normals, triangles, sw, false); 
    }

    private void ExportTerrains(StreamWriter sw)
    {
        Terrain[] terrains = Object.FindObjectsOfType<Terrain>();
        int numTerrains = terrains == null ? 0 : terrains.Length;
        for (int i = 0; i < numTerrains; ++i)
        {
            sw.WriteLine("g terrain" + i);

            Progress("生成地形");
            Matrix4x4 o2w = Matrix4x4.identity;
            o2w = o2w * Matrix4x4.TRS(new Vector3(-terrains[i].transform.position.x, terrains[i].transform.position.y, terrains[i].transform.position.z), Quaternion.identity, Vector3.one);
            o2w = o2w * Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(-90, Vector3.up), Vector3.one);
            ExportTerrain(terrains[i].terrainData, o2w, sw);
        }
    }

    private void ExportTerrain(TerrainData terrainData, Matrix4x4 o2w, StreamWriter sw)
    {
        string terrainName = name;
        float tRes = 2.5f;
        TerrainData terrain = terrainData;
        int w = terrain.heightmapResolution;
        int h = terrain.heightmapResolution;
        Vector3 meshScale = terrain.size;
        meshScale = new Vector3(meshScale.x / (h - 1) * tRes, meshScale.y, meshScale.z / (w - 1) * tRes);
        Vector2 uvScale = new Vector2((float)(1.0 / (w - 1)), (float)(1.0 / (h - 1)));

        float[,] tData = terrain.GetHeights(0, 0, w, h);
        w = (int)((w - 1) / tRes + 1);
        h = (int)((h - 1) / tRes + 1);
        Vector3[] tVertices = new Vector3[w * h];
        Vector2[] tUV = new Vector2[w * h];
        Vector3[] tNormals = new Vector3[w * h];
        int[] tPolys = new int[(w - 1) * (h - 1) * 6];
        int y = 0;
        int x = 0;
        for (y = 0; y < h; y++)
        {
            for (x = 0; x < w; x++)
            {
                tVertices[y * w + x] = o2w.MultiplyPoint(Vector3.Scale(meshScale, new Vector3(x, tData[(int)(x * tRes), (int)(y * tRes)], y)));
                tUV[y * w + x] = Vector2.Scale(new Vector2(y * tRes, x * tRes), uvScale);
            }
        }

        y = 0;
        x = 0;
        int index = 0;
        for (y = 0; y < h - 1; y++)
        {
            for (x = 0; x < w - 1; x++)
            {
                tPolys[index++] = (y * w) + x;
                tPolys[index++] = ((y + 1) * w) + x;
                tPolys[index++] = (y * w) + x + 1;

                tPolys[index++] = ((y + 1) * w) + x;
                tPolys[index++] = ((y + 1) * w) + x + 1;
                tPolys[index++] = (y * w) + x + 1;
            }
        }

        Progress("重新计算地形法线");
        RecalculateNormals(tPolys, tVertices, ref tNormals);

        WriteToBuffer(tVertices, tUV, tNormals, tPolys, sw, true);
    }

    private void RecalculateNormals(int[] triangles, Vector3[] vertices, ref Vector3[] o_normals)
    {
        int numVertices = vertices.Length;
        for (int i = 0; i < numVertices; ++i)
        {
            Vector3 avgNormal = Vector3.zero;
            
            for (int j = 0; j < triangles.Length; j += 3)
            {
                if (triangles[j] == i || triangles[j + 1] == i || triangles[j + 2] == i)
                {
                    Vector3 v0 = vertices[triangles[j + 1]] - vertices[triangles[j]];
                    Vector3 v1 = vertices[triangles[j + 2]] - vertices[triangles[j]];
                    Vector3 tn = Vector3.Cross(v0, v1);
                    avgNormal += tn;
                }
            }

            avgNormal.Normalize();
            o_normals[i] = avgNormal;
        }
    }

    private void WriteToBuffer(Vector3[] tVertices, Vector2[] tUV, Vector3[] tNormals, int[] tPolys, StreamWriter sw, bool pIsInverseVertexOrder)
    {
        try
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            if (tNormals == null || tNormals.Length == 0)
            {
                tNormals = new Vector3[tUV.Length];
                for (int i = 0; i < tUV.Length; ++i)
                {
                    tNormals[i] = Vector3.up;
                }
            }

            for (int i = 0; i < tVertices.Length; i++)
            {
                StringBuilder sb = new StringBuilder("v ", 20);
                sb.Append(tVertices[i].x.ToString()).Append(" ").
                    Append(tVertices[i].y.ToString()).Append(" ").
                    Append(tVertices[i].z.ToString());
                sw.WriteLine(sb);
            }

            for (int i = 0; i < tUV.Length; i++)
            {
                StringBuilder sb = new StringBuilder("vt ", 22);
                sb.Append(tUV[i].x.ToString()).Append(" ").
                    Append(tUV[i].y.ToString());
                sw.WriteLine(sb);
            }

            for (int i = 0; i < tNormals.Length; i++)
            {
                StringBuilder sb = new StringBuilder("vn ", 50);
                sb.Append(tNormals[i].x.ToString()).Append(" ").
                    Append(tNormals[i].y.ToString()).Append(" ").
                    Append(tNormals[i].z.ToString());
                sw.WriteLine(sb);
            }

            for (int i = 0; i < tPolys.Length; i += 3)
            {
                bool isInverseVertexOrder = pIsInverseVertexOrder;

                Vector3 p0 = tVertices[tPolys[i + 0]];
                Vector3 p1 = tVertices[tPolys[i + 1]];
                Vector3 p2 = tVertices[tPolys[i + 2]];
                Vector3 nt = tNormals[tPolys[i + 0]] + tNormals[tPolys[i + 1]] + tNormals[tPolys[i + 2]];
                nt.Normalize();
                Vector3 n = Vector3.Cross(p2 - p1, p1 - p0);
                n.Normalize();

                if (Vector3.Dot(n, nt) < -0.1f)
                {
                    isInverseVertexOrder = true;
                }

                int i0 = isInverseVertexOrder ? i + 0 : i + 2;
                int i1 = isInverseVertexOrder ? i + 1 : i + 1;
                int i2 = isInverseVertexOrder ? i + 2 : i + 0;
                StringBuilder sb = new StringBuilder("f ", 43);
                sb.Append(tPolys[i0] + 1 + indexOffset).Append("/").Append(tPolys[i0] + 1 + indexOffset).Append("/").Append(tPolys[i0] + 1 + indexOffset).Append(" ").
                    Append(tPolys[i1] + 1 + indexOffset).Append("/").Append(tPolys[i1] + 1 + indexOffset).Append("/").Append(tPolys[i1] + 1 + indexOffset).Append(" ").
                    Append(tPolys[i2] + 1 + indexOffset).Append("/").Append(tPolys[i2] + 1 + indexOffset).Append("/").Append(tPolys[i2] + 1 + indexOffset);
                sw.WriteLine(sb); 
            }

            indexOffset += tVertices.Length;
        }
        catch (System.Exception err)
        {
            //Debug.LogError("Error saving file: " + err.Message);
        }
    }

    private void Progress(string msg)
    {
        EditorUtility.DisplayProgressBar("Message", msg, 0.5f);
    }
}
