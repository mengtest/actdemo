using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BakeNormalSpecularToDiffuse : EditorWindow
{
    [MenuItem("KPlaneTools/烘培法线高光")]
    private static void Execute()
    {
        var window = EditorWindow.CreateInstance<BakeNormalSpecularToDiffuse>();
        window.minSize = new Vector2(340, 450);
        window.Show();
    }

    private enum BakedSize
    {
        Size_32 = 32,
        Size_64 = 64,
        Size_128 = 128,
        Size_512 = 512,
        Size_1024 = 1024, 
    }

    private Texture2D diffuse = null;

    private Texture2D specular = null;

    private Texture2D normal = null;

    private string saveWorkingDataName = string.Empty;

    private string tempRootGoName = "BakeNormalSpecularToDiffuseRootGo";

    private string tempGoName = "BakeNormalSpecularToDiffuseGo";

    private string workingDataDir = "Assets/BakeNormalSpecularToDiffuseWorkingData";

    private List<Object> library = new List<Object>();

    private Vector2 libraryScroll = Vector2.zero;

    private BakedSize bakedSize = BakedSize.Size_512;

    private void OnEnable()
    {
        RefreshLibrary();
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "将法线高光烘培到漫反射贴图上\n主要用于面片模型\n" + 
            "\n" +
            "设置漫反射贴图、高光贴图、法线贴图\n" + 
            "在<保存名称>处填写一个名字，烘培后所有的参数都将被保存起来，以便下次继续编辑\n" + 
            "点击<开始调整>，会创建一个面片，修改面片的角度和材质球，调整光照效果\n" + 
            "点击<开始烘培>，保存带有伪法线高光效果的纹理", 
            MessageType.Info
        );

        saveWorkingDataName = EditorGUILayout.TextField("保存名称", saveWorkingDataName);
        bakedSize = (BakedSize)EditorGUILayout.EnumPopup("烘培尺寸", bakedSize);
        diffuse = EditorGUILayout.ObjectField("漫反射贴图", diffuse, typeof(Texture2D)) as Texture2D;
        normal = EditorGUILayout.ObjectField("法线贴图", normal, typeof(Texture2D)) as Texture2D;
        specular = EditorGUILayout.ObjectField("高光贴图", specular, typeof(Texture2D)) as Texture2D;

        if (GUILayout.Button("开始调整") && diffuse != null)
        {
            DeleteRoot();

            GameObject rootGo = new GameObject(tempRootGoName);
            rootGo.transform.parent = null;
            rootGo.transform.position = Vector3.zero;

            GameObject planeGo = GameObject.CreatePrimitive(PrimitiveType.Plane);
            planeGo.transform.parent = rootGo.transform;
            planeGo.name = tempGoName;
            Transform plane = planeGo.transform;
            MeshFilter planeMF = planeGo.GetComponent<MeshFilter>();
            MeshRenderer planeMR = planeGo.GetComponent<MeshRenderer>();

            plane.position = new Vector3(5000, 0, 0);

            Material planeMtrl = new Material(Shader.Find("BakeNormalSpecularToDiffuse"));
            planeMR.sharedMaterial = planeMtrl;
            planeMtrl.SetTexture("_MainTex", diffuse);
            planeMtrl.SetTexture("_BumpMap", normal);
            planeMtrl.SetTexture("_SpecMap", specular);
            planeMtrl.EnableKeyword("SPECULAR");
            planeMtrl.EnableKeyword("NORMAL_MAP");
            planeMtrl.EnableKeyword("SPECULAR_MAP");

            SelectAndLookAt(planeGo);
        }

        if (GUILayout.Button("开始烘培"))
        {
            if (SceneView.sceneViews == null && SceneView.sceneViews.Count == 0)
            {
                return;
            }

            GameObject rootGo = GameObject.Find(tempRootGoName);
            if (rootGo == null)
            {
                return;
            }

            GameObject planeGo = GameObject.Find(tempGoName);
            if (planeGo == null)
            {
                return;
            }
            MeshRenderer planeMR = planeGo.GetComponent<MeshRenderer>();
            if (planeMR == null)
            {
                return;
            }
            Material planeMtrl = planeMR.sharedMaterial;
            if (planeMtrl == null)
            {
                return;
            }

            planeMtrl.EnableKeyword("SCREEN_SPACE");

            GameObject camGo = new GameObject("BakeCamera");
            camGo.transform.parent = rootGo.transform;
            Camera cam = camGo.AddComponent<Camera>();
            cam.enabled = false;
            RenderTexture bakeRT = new RenderTexture((int)bakedSize, (int)bakedSize, 32, RenderTextureFormat.ARGB32);
            cam.targetTexture = bakeRT;
            foreach (SceneView sceneView in SceneView.sceneViews)
            {
                cam.orthographic = sceneView.camera.orthographic;
                camGo.transform.position = sceneView.camera.transform.position;
                camGo.transform.forward = sceneView.camera.transform.forward;
                break;
            }
            cam.Render();
            RenderTexture.active = bakeRT;
            Texture2D bakeTex = new Texture2D(bakeRT.width, bakeRT.height, TextureFormat.RGB24, false);
            bakeTex.ReadPixels(new Rect(0, 0, bakeRT.width, bakeRT.height), 0, 0, false);
            bakeTex.Apply();
            RenderTexture.active = null;

            planeMtrl.DisableKeyword("SCREEN_SPACE");

            string savePath = EditorUtility.SaveFilePanel("保存", Application.dataPath, "FakeDiffuse.png", null);
            if (!string.IsNullOrEmpty(savePath))
            {
                File.WriteAllBytes(savePath, bakeTex.EncodeToPNG());
                SaveWorkingData(saveWorkingDataName, rootGo, planeMR);
                AssetDatabase.Refresh();
            }

            Object.DestroyImmediate(camGo);

            RefreshLibrary();
        }

        EditorGUILayout.Space();

        DrawLibrary();
    }

    private void SelectAndLookAt(GameObject go)
    {
        Selection.activeGameObject = go;
        foreach (SceneView sceneView in SceneView.sceneViews)
        {
            sceneView.FrameSelected();
        }
    }

    private void SaveWorkingData(string name, GameObject rootGo, MeshRenderer planeMR)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        if (!Directory.Exists(workingDataDir))
        {
            Directory.CreateDirectory(workingDataDir);
            AssetDatabase.Refresh();
        }

        if (File.Exists(GetWorkingDataPath(name)))
        {
            File.Delete(GetWorkingDataPath(name));
            AssetDatabase.Refresh();
        }

        Material origMtrl = planeMR.sharedMaterial;
        Material copyMtrl = new Material(origMtrl);
        planeMR.sharedMaterial = copyMtrl;

        GameObject prefab = PrefabUtility.CreatePrefab(GetWorkingDataPath(name), rootGo);
        AssetDatabase.ImportAsset(GetWorkingDataPath(name));
        AssetDatabase.AddObjectToAsset(planeMR.sharedMaterial, prefab);
        AssetDatabase.ImportAsset(GetWorkingDataPath(name));
        prefab.transform.Find(planeMR.gameObject.name).GetComponent<MeshRenderer>().sharedMaterial = copyMtrl;
        EditorUtility.SetDirty(prefab);

        planeMR.sharedMaterial = origMtrl;
    }

    private bool IsWorkingDataExist(string name)
    {
        return File.Exists(GetWorkingDataPath(name));
    }

    private string GetWorkingDataPath(string name)
    {
        return workingDataDir + "/" + name + ".prefab";
    }

    private void DrawLibrary()
    {
        libraryScroll = EditorGUILayout.BeginScrollView(libraryScroll);
        {
            int numItems = library == null ? 0 : library.Count;
            for (int i = 0; i < numItems; ++i)
            {
                Object item = library[i];
                if (item == null)
                {
                    RefreshLibrary();
                    return;
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.ObjectField(item, typeof(Object));
                        if (GUILayout.Button("编辑"))
                        {
                            DeleteRoot();
                            saveWorkingDataName = item.name;
                            GameObject rootGo = GameObject.Instantiate(item) as GameObject;
                            rootGo.name = tempRootGoName;
                            Camera camera = rootGo.GetComponentInChildren<Camera>();
                            foreach (SceneView sceneView in SceneView.sceneViews)
                            {
                                sceneView.camera.transform.position = camera.transform.position;
                                sceneView.pivot = camera.transform.position;
                                sceneView.rotation = camera.transform.rotation;
                                sceneView.orthographic = camera.orthographic;
                                sceneView.Repaint();
                            }
                            SceneView.RepaintAll();
                            Object.DestroyImmediate(camera.gameObject);
                            MeshRenderer planeMR = rootGo.GetComponentInChildren<MeshRenderer>();
                            planeMR.sharedMaterial = new Material(planeMR.sharedMaterial);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void RefreshLibrary()
    {
        library.Clear();

        if (!Directory.Exists(workingDataDir))
        {
            return;
        }

        string[] paths = Directory.GetFiles(workingDataDir);
        foreach (string path in paths)
        {
            if (path.EndsWith(".prefab"))
            {
                library.Add(AssetDatabase.LoadMainAssetAtPath(path));
            }
        }
    }

    private void DeleteRoot()
    {
        GameObject rootGo = GameObject.Find(tempRootGoName);
        if (rootGo != null)
        {
            Object.DestroyImmediate(rootGo);
        }
    }

    private void OnDestroy()
    {
        DeleteRoot();
    }
}
