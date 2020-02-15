using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

public class AnimatedVegetationEditor : EditorWindow
{
    public static AnimatedVegetationEditor instance = null;

	[MenuItem("KPlaneTools/动态植物编辑器")]
    private static void Execute()
    {
        if (instance == null)
        {
            instance = EditorWindow.CreateInstance<AnimatedVegetationEditor>();
			instance.ShowUtility();
            instance.minSize = new Vector2(640, 540);
            instance.titleContent = new GUIContent("动态植物编辑");
            instance.RefreshLibrary();
            instance.PreviewWithAnimatedMaterial(true);
        }
    }

    private GameObject sourceTarget = null;

    private string newName = string.Empty;

	private string saveToDir = "Assets/AnimatedVegetation/Library";

    private GameObject editGo = null;

	private Material editMaterial = null;

    private List<Object> library = new List<Object>();

    private Vector2 scroll = Vector2.zero;

    private void Update()
    {
        AnimatedVegetationAssistant.isGlobalWind = GetIsGlobalWind();
        if (editGo != null && VPaint.Instance != null && Selection.activeGameObject != editGo)
        {
            Selection.activeGameObject = editGo;
        }
    }

    private AnimBool ui_sceneManagerFade;
    private AnimBool ui_createNewFade;
    private AnimBool ui_libraryFade;

    private void OnEnable()
    {
        ui_sceneManagerFade = new AnimBool(false);
        ui_sceneManagerFade.valueChanged.AddListener(Repaint);

        ui_createNewFade = new AnimBool(false);
        ui_createNewFade.valueChanged.AddListener(Repaint);

        ui_libraryFade = new AnimBool(true);
        ui_libraryFade.valueChanged.AddListener(Repaint);
    }

    private void OnGUI()
    {
		if(Application.isPlaying)
		{
			Close();
			return;
		}
		if(AnimatedVegetationEditor.instance == null)
		{
			Close();
			return;
		}

		GUI.enabled = VPaint.Instance == null;
		{
	        try
	        {
                DrawUI_SceneManager();
                EditorGUILayout.Space();
	            DrawUI_CreateNew();
	            EditorGUILayout.Space();
	            DrawUI_Library();  
	        }
	        catch (System.Exception e)
	        {
	            if (!e.StackTrace.Contains("GUILayout"))
	            {
	                throw e;
	            }
	        }
		}
		GUI.enabled = true;
		Repaint();
    }

    private void DrawUI_Seperator()
    {
        EditorGUILayout.Space();
        Color c = GUI.color;
        GUI.color = Color.gray;
        GUILayout.Label("----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
        GUI.color = c;
        EditorGUILayout.Space();
    }

    private void DrawUI_SceneManager()
    {
        ui_sceneManagerFade.target = EditorGUILayout.Toggle("", ui_sceneManagerFade.target, EditorStyles.toolbarButton);
        Rect r = GUILayoutUtility.GetLastRect();
        GUI.Label(r, "场景管理");
        if (EditorGUILayout.BeginFadeGroup(ui_sceneManagerFade.faded))
        {
            EditorGUILayout.HelpBox("动态植物场景管理\n每个场景都需要一个管理器来管理所有的动态植物\n当植物被修改（包括编辑、调整位置角度缩放）时，都需要刷新场景中的管理器", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space();

                if (GUILayout.Button("刷新当前场景"))
                {
                    RefreshLibrary();
                    ConvertColors();
                    RefreshSceneManager();
                    EditorUtility.DisplayDialog("提示", "刷新完成", "OK");
                }
                if (GUILayout.Button("刷新所有场景"))
                {
                    EditorApplication.SaveScene();
                    string currentScene = EditorApplication.currentScene;
                    RefreshLibrary();
                    foreach (var scene in EditorBuildSettings.scenes)
                    {
                        EditorApplication.OpenScene(scene.path);
                        ConvertColors();
                        RefreshSceneManager();
                        EditorApplication.SaveScene();
                    }
                    EditorUtility.DisplayDialog("提示", "刷新完成", "OK");
                    if (!string.IsNullOrEmpty(currentScene))
                    {
                        EditorApplication.OpenScene(currentScene);
                    }
                }
                if (GUILayout.Button("移除当前场景"))
                {
                    RefreshLibrary();
                    RevertColors();
                    RemoveSceneManager();
                    EditorUtility.DisplayDialog("提示", "移除完成", "OK");
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndHorizontal();   
        } 
        EditorGUILayout.EndFadeGroup();
    }

    private void DrawUI_CreateNew()
    {
        ui_createNewFade.target = EditorGUILayout.Toggle("", ui_createNewFade.target, EditorStyles.toolbarButton);
        Rect r = GUILayoutUtility.GetLastRect();
        GUI.Label(r, "新建");
        if (EditorGUILayout.BeginFadeGroup(ui_createNewFade.faded))
        {
            EditorGUILayout.HelpBox("新建一个动态植物\n以场景中的一个植物模型为基础，创建一个新的动态植物", MessageType.Info);
            sourceTarget = EditorGUILayout.ObjectField("把场景中的一个植物拖到这里", sourceTarget, typeof(GameObject)) as GameObject;
            EditorGUILayout.Space();

            newName = EditorGUILayout.TextField("起个名字，以便保存", newName);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("添加到资源库"))
                {
                    DoCreateNew();
                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndFadeGroup();
    }

    private void DrawUI_Library()
    {
        ui_libraryFade.target = EditorGUILayout.Toggle("", ui_libraryFade.target, EditorStyles.toolbarButton);
        Rect r = GUILayoutUtility.GetLastRect();
        GUI.Label(r, "资源库");
        if (EditorGUILayout.BeginFadeGroup(ui_libraryFade.faded))
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.HelpBox("资源库", MessageType.Info);

                if (GUILayout.Button("刷新", GUILayout.Width(40), GUILayout.Height(40)))
                {
                    RefreshLibrary();
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();

            int cellSize = 150;
            scroll = EditorGUILayout.BeginScrollView(scroll);
            {
                EditorGUILayout.BeginVertical();
                {
                    int cols = Screen.width / cellSize;
                    int rows = (int)Mathf.Ceil((float)library.Count / (float)cols);
                    for (int row = 0; row < rows; ++row)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            for (int i = row * cols; i < Mathf.Min(row * cols + cols, library.Count); ++i)
                            {
                                if (library[i] == null)
                                {
                                    RefreshLibrary();
                                    return;
                                }

                                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(cellSize));
                                {
                                    if (GUILayout.Button(AssetPreview.GetAssetPreview(library[i]), GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                                    {
                                        Selection.activeObject = library[i];
                                    }

                                    GUILayout.Label(library[i].name);

                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        if (GUILayout.Button("添加到场景"))
                                        {
                                            AddToScene(library[i] as GameObject);
                                        }
                                        if (GUILayout.Button("编辑"))
                                        {
                                            DoEdit(library[i] as GameObject);
                                        }
                                        if (GUILayout.Button("X"))
                                        {
                                            if (EditorUtility.DisplayDialog("提示", "确定要删除？", "删除", "No"))
                                            {
                                                AssetDatabase.DeleteAsset(GetPrefabPath(library[i].name));
                                                AssetDatabase.Refresh();
                                                RefreshLibrary();
                                            }
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndFadeGroup();
    }

    private void OnDestroy()
    {
		instance = null;
        if (VPaint.Instance != null)
        {
            VPaint.Instance.Close();
        }
        PreviewWithAnimatedMaterial(false);
    }

    private void DoCreateNew()
	{
        if (sourceTarget == null)
        {
			ShowNotification(new GUIContent("设置要编辑的场景中的植物"));
            return;
        }

        if (string.IsNullOrEmpty(newName))
        {
            ShowNotification(new GUIContent("取个新的名字"));
            return;
        }

        PrefabType prefabType = PrefabUtility.GetPrefabType(sourceTarget);
        if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab)
        {
            ShowNotification(new GUIContent("要编辑的对象必须放置到场景中"));
            return;
        }
        
		if(!IsLegalTarget(sourceTarget))
        {
            ShowNotification(new GUIContent("要编辑的对象不符合要求"));
            return;
        }

        if (File.Exists(GetPrefabPath(newName)))
        {
            bool isOverride = EditorUtility.DisplayDialog("提示", "已经存在同名的文件，是否覆盖？", "覆盖", "取消");
            if (!isOverride)
            {
                return;
            }
        }

        if (!Directory.Exists(saveToDir))
        {
            Directory.CreateDirectory(saveToDir);
            AssetDatabase.Refresh();
        }

        GameObject prefab = PrefabUtility.CreatePrefab(GetPrefabPath(newName), sourceTarget);
        AssetDatabase.ImportAsset(GetPrefabPath(newName));

        Mesh mesh = ExtractMesh(sourceTarget.GetComponent<MeshFilter>());
        prefab.GetComponent<MeshFilter>().sharedMesh = mesh;
        AssetDatabase.AddObjectToAsset(mesh, prefab);

        Material mtrl = new Material(prefab.GetComponent<MeshRenderer>().sharedMaterial);
		mtrl.shader = Shader.Find("AnimatedVegetation");
        prefab.GetComponent<MeshRenderer>().sharedMaterial = mtrl;
        AssetDatabase.AddObjectToAsset(mtrl, prefab);
        AssetDatabase.ImportAsset(GetPrefabPath(newName));
        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
		RefreshLibrary();
    }

    private void AddToScene(GameObject prefab)
    {
		if(prefab == null)
		{
			return;
		}

		GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        instance.name = prefab.name;
        SetGameObjectWithStaticBatchingFlag(instance);
		Object.DestroyImmediate(instance.GetComponent<VPaintGroup>());
		Object.DestroyImmediate(instance.GetComponent<VPaintObject>());
		Object.DestroyImmediate(instance.GetComponent<AnimatedVegetationAssistant>());

        foreach (SceneView sceneView in SceneView.sceneViews)
        {
			RaycastHit hit;
			if(Physics.Raycast(new Ray(sceneView.camera.transform.position, sceneView.camera.transform.forward), out hit))
			{
				instance.transform.position = hit.point;
			}
			else
			{
            	instance.transform.position = sceneView.camera.transform.position + sceneView.camera.transform.forward * 5;
			}
            break;
        }

		SelectAndLookAt(instance);
    }

    private void DoEdit(GameObject prefab)
    {
		if(!IsLegalTarget(prefab))
		{
			Close();
			return;
		}

        if (EditorApplication.isSceneDirty)
        {
            int isSaveScene = EditorUtility.DisplayDialogComplex("提示", "是否保存当前场景", "保存", "No", "取消");
            if (isSaveScene == 0)
            {
                EditorApplication.SaveScene();
            }
            else if (isSaveScene == 2)
            {
                return;
            }
        }

        EditorApplication.NewScene();

        editGo = GameObject.Instantiate(prefab) as GameObject;
		editGo.name = prefab.name;

        Selection.activeGameObject = editGo;
        foreach (SceneView sceneView in SceneView.sceneViews)
        {
            sceneView.FrameSelected();
        }

        VPaintGroup vpaintGroup = editGo.GetComponent<VPaintGroup>();
        if (vpaintGroup == null)
        {
            vpaintGroup = editGo.AddComponent<VPaintGroup>();
        }
        VPaintObject vpaintObject = editGo.GetComponent<VPaintObject>();
        if (vpaintObject == null)
        {
            vpaintObject = editGo.AddComponent<VPaintObject>();
        }
		editMaterial = new Material(prefab.GetComponent<MeshRenderer>().sharedMaterial);
        CopyKeywordsState(prefab.GetComponent<MeshRenderer>().sharedMaterial, editMaterial);
		editGo.GetComponent<MeshRenderer>().sharedMaterial = editMaterial;
        vpaintGroup.AddColorer(vpaintObject);
        if (editGo.GetComponent<AnimatedVegetationAssistant>() != null)
        {
            editGo.GetComponent<AnimatedVegetationAssistant>().autoDestroy = false;
        }
        else
        {
            editGo.AddComponent<AnimatedVegetationAssistant>().autoDestroy = false;
        }

        VPaint.OpenEditorAsUtility();
        if (VPaint.Instance != null)
        {
            VPaint.Instance.EnableVertexColorPreview(VPaint.VertexColorPreviewMode.RGB);
        }
    }

    public void CompleteEdit()
    {
        if (editGo == null)
        {
            EditorUtility.DisplayDialog("提示", "没有要保存的", "OK");
            return;
        }

		if(!IsLegalTarget(editGo))
		{
			Close();
			return;
		}

        if (VPaint.Instance != null)
        {
            VPaint.Instance.clearCacheOfAnimatedVegatationEditor = false;
            VPaint.Instance.Close();
        }

        DeleteEditorCollider(editGo);

        if (!Directory.Exists(saveToDir))
        {
            Directory.CreateDirectory(saveToDir);
            AssetDatabase.Refresh();
        }

        if (editGo.GetComponent<AnimatedVegetationAssistant>() != null)
        {
            editGo.GetComponent<AnimatedVegetationAssistant>().autoDestroy = true;
        }

        Material mtrl = new Material(editGo.GetComponent<MeshRenderer>().sharedMaterial);
		mtrl.shader = Shader.Find("AnimatedVegetation");
        CopyKeywordsState(editGo.GetComponent<MeshRenderer>().sharedMaterial, mtrl);
		
		GameObject existPrefab = AssetDatabase.LoadMainAssetAtPath(GetPrefabPath(editGo.name)) as GameObject;
		bool createNewOrReplace = existPrefab == null || existPrefab.GetComponent<AnimatedVegetationAssistant>() == null;
		GameObject prefab = null;
		if(createNewOrReplace)
		{
			prefab = PrefabUtility.CreatePrefab(GetPrefabPath(editGo.name), editGo, ReplacePrefabOptions.Default);
		}
		else
		{
			prefab = PrefabUtility.ReplacePrefab(editGo, AssetDatabase.LoadMainAssetAtPath(GetPrefabPath(editGo.name)), ReplacePrefabOptions.ReplaceNameBased);
			Object[] assets = AssetDatabase.LoadAllAssetsAtPath(GetPrefabPath(editGo.name));
			foreach(var asset in assets)
			{
				if(asset is Mesh || asset is Material)
				{
					Object.DestroyImmediate(asset, true);
				}
			}
		}
		AssetDatabase.ImportAsset(GetPrefabPath(editGo.name));
        MeshFilter meshFilter = editGo.GetComponent<MeshFilter>();
        Mesh mesh = ExtractMesh(meshFilter);
        prefab.GetComponent<MeshFilter>().sharedMesh = mesh;
        prefab.GetComponent<MeshRenderer>().sharedMaterial = mtrl;
        AssetDatabase.AddObjectToAsset(mesh, prefab);
        AssetDatabase.AddObjectToAsset(mtrl, prefab);
		AssetDatabase.ImportAsset(GetPrefabPath(editGo.name));
        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();

		ClearEditGo();
    }

    private void DeleteEditorCollider(GameObject go)
    {
        if (go == null || go.transform == null)
        {
            return;
        }

        Transform transform = go.transform;
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            if (child != null && child.gameObject != null && child.gameObject.name == "Editor Collider")
            {
                child.parent = null;
				Object.DestroyImmediate(child.gameObject);
                return;
            }
        }
    }

    private Mesh ExtractMesh(MeshFilter meshFilter)
    {
        if (meshFilter == null)
        {
            return null;
        }

        if (meshFilter.sharedMesh == null)
        {
            return null;
        }

        Mesh srcMesh = meshFilter.sharedMesh;
        Mesh newMesh = new Mesh();
        newMesh.vertices = srcMesh.vertices;
        newMesh.normals = srcMesh.normals;
        if (srcMesh.tangents != null && srcMesh.tangents.Length > 0)
        {
            newMesh.tangents = srcMesh.tangents;
        }
        else
        {
            newMesh.tangents = null;
        }
        if (srcMesh.colors != null && srcMesh.colors.Length > 0)
        {
            newMesh.colors = srcMesh.colors;
        }
        else
        {
            newMesh.colors = null;
        }
        if (srcMesh.uv != null && srcMesh.uv.Length > 0)
        {
            newMesh.uv = srcMesh.uv;
        }
        else
        {
            newMesh.uv = null;
        }
        if (srcMesh.uv2 != null && srcMesh.uv2.Length > 0)
        {
            newMesh.uv2 = srcMesh.uv2;
        }
        else
        {
            newMesh.uv2 = null;
        }
        if (srcMesh.uv3 != null && srcMesh.uv3.Length > 0)
        {
            newMesh.uv3 = srcMesh.uv3;
        }
        else
        {
            newMesh.uv3 = null;
        }

        newMesh.triangles = srcMesh.triangles;
        newMesh.RecalculateBounds();

        return newMesh;
    }

    private string GetPrefabPath(string assetName)
    {
        return saveToDir + "/" + assetName + ".prefab";
    }

    private void RefreshSceneManager()
    {
        RemoveSceneManager();

        GameObject go = new GameObject("AnimatedVegetationSceneManager");
        AnimatedVegetationSceneManager sceneManager = go.AddComponent<AnimatedVegetationSceneManager>();
        AnimatedVegetationSceneCache sceneCache = go.AddComponent<AnimatedVegetationSceneCache>();
        sceneManager.FillSceneCache(sceneCache);
        EditorUtility.SetDirty(go);
    }

    private void RemoveSceneManager()
    {
        AnimatedVegetationSceneManager[] managers = Object.FindObjectsOfType<AnimatedVegetationSceneManager>();
        foreach (var manager in managers)
        {
            GameObject.DestroyImmediate(manager.gameObject);
        }
    }

    private void ConvertColors()
    {
        GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var gameObject in gameObjects)
        {
            Object prefab = PrefabUtility.GetPrefabObject(gameObject);
            if (prefab != null)
            {
                foreach (var libItem in library)
                {
                    if (prefab == libItem && IsLegalTarget(gameObject) && IsLegalTarget(prefab as GameObject))
                    {
                        if (gameObject.GetComponent<MeshRenderer>().sharedMaterial.IsKeywordEnabled("IS_GLOBAL_WIND"))
                        {
                            gameObject.GetComponent<MeshFilter>().sharedMesh = (prefab as GameObject).GetComponent<MeshFilter>().sharedMesh;
                            SetGameObjectWithStaticBatchingFlag(gameObject);
                            EditorUtility.SetDirty(gameObject);
                        }
                        else
                        {
                            Mesh newMesh = ExtractMesh((prefab as GameObject).GetComponent<MeshFilter>());
                            if (newMesh != null)
                            {
                                Color[] colors = newMesh.colors;
                                int numColors = colors == null ? 0 : colors.Length;
                                for (int colorIndex = 0; colorIndex < numColors; ++colorIndex)
                                {
                                    Color color = colors[colorIndex];
                                    Vector3 colorV = new Vector3(color.r, color.g, color.b);
                                    Vector3 newColorV = gameObject.transform.localToWorldMatrix.MultiplyVector(colorV);
                                    colors[colorIndex] = new Color(newColorV.x, newColorV.y, newColorV.z);
                                }
                                newMesh.colors = colors;

                                gameObject.GetComponent<MeshFilter>().sharedMesh = newMesh;
                                SetGameObjectWithStaticBatchingFlag(gameObject);
                                EditorUtility.SetDirty(gameObject);
                            }
                        }
                        break;
                    }
                }
            }
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private void RevertColors()
    {
        GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var gameObject in gameObjects)
        {
            Object prefab = PrefabUtility.GetPrefabObject(gameObject);
            if (prefab != null)
            {
                foreach (var libItem in library)
                {
                    if (prefab == libItem && IsLegalTarget(gameObject) && IsLegalTarget(prefab as GameObject))
                    {
                        if (gameObject.GetComponent<MeshFilter>().sharedMesh != (prefab as GameObject).GetComponent<MeshFilter>().sharedMesh)
                        {
                            gameObject.GetComponent<MeshFilter>().sharedMesh = (prefab as GameObject).GetComponent<MeshFilter>().sharedMesh;
                            EditorUtility.SetDirty(gameObject);
                        }
                        break;
                    }
                }
            }
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    public void RefreshLibrary()
    {
        library.Clear();

        if (!Directory.Exists(saveToDir))
        {
            return;
        }

        string[] paths = Directory.GetFiles(saveToDir);
        foreach (string path in paths)
        {
            if (path.EndsWith(".prefab"))
            {
                library.Add(AssetDatabase.LoadMainAssetAtPath(path));
            }
        }
    }

    public void ClearEditGo()
    {
		Object.DestroyImmediate(editGo);
        editGo = null;
    }

    private void SetGameObjectWithStaticBatchingFlag(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        GameObjectUtility.SetStaticEditorFlags(target, GameObjectUtility.GetStaticEditorFlags(target) | StaticEditorFlags.BatchingStatic);
    }

	private bool IsLegalTarget(GameObject target)
	{
		return target != null && 
				target.GetComponent<MeshFilter>() != null && 
				target.GetComponent<MeshFilter>().sharedMesh != null && 
				target.GetComponent<MeshFilter>().sharedMesh.subMeshCount == 1 && 
				target.GetComponent<MeshRenderer>() != null && 
				target.GetComponent<MeshRenderer>().sharedMaterial != null && 
				target.GetComponent<MeshRenderer>().sharedMaterials.Length == 1;
	}

    public void PreviewWithAnimatedMaterial(bool isAnimated)
    {
        try
        {
            foreach (SceneView view in UnityEditor.SceneView.sceneViews)
            {
                FieldInfo stateField = view.GetType().GetField("m_SceneViewState", BindingFlags.NonPublic | BindingFlags.Instance);
                object state = stateField.GetValue(view);
                FieldInfo mtrlUpdateField = state.GetType().GetField("showMaterialUpdate", BindingFlags.Public | BindingFlags.Instance);
                mtrlUpdateField.SetValue(state, isAnimated);
            }
        }
        catch (System.Exception e)
        {
            // Do nothing
        }
    }

	private void SelectAndLookAt(GameObject go)
	{
		Selection.activeGameObject = go;
		foreach (SceneView sceneView in SceneView.sceneViews)
		{
			sceneView.FrameSelected();
		}
	}

    private void CopyKeywordsState(Material srcMtrl, Material destMtrl)
    {
        if (srcMtrl == null || destMtrl == null)
        {
            return;
        }

        string[] keywords = srcMtrl.shaderKeywords;
        foreach (var keyword in keywords)
        {
            if (srcMtrl.IsKeywordEnabled(keyword))
            {
                destMtrl.EnableKeyword(keyword);
            }
            else
            {
                destMtrl.DisableKeyword(keyword);
            }
        }
    }

    public float GetWaveRange()
    {
		return GetMaterialFloatProperty(editMaterial, "_WaveRange");
    }

    public void SetWaveRange(float value)
    {
		SetMaterialFloatProperty(editMaterial, "_WaveRange", value);
    }

    public float GetWaveFrequency()
    {
		return GetMaterialFloatProperty(editMaterial, "_WaveFrequency");
    }

    public void SetWaveFrequency(float value)
    {
		SetMaterialFloatProperty(editMaterial, "_WaveFrequency", value);
    }

    public float GetWaveOffset()
    {
		return GetMaterialFloatProperty(editMaterial, "_WaveOffset");
    }

    public void SetWaveOffset(float value)
    {
		SetMaterialFloatProperty(editMaterial, "_WaveOffset", value);
    }

	public bool GetIsDoubleSide()
	{
		return Mathf.Approximately(GetMaterialFloatProperty(editMaterial, "_CullMode"), (float)UnityEngine.Rendering.CullMode.Off);
	}

	public void SetIsDoubleSide(bool isDoubleSide)
	{
		SetMaterialFloatProperty(editMaterial, "_CullMode", isDoubleSide ? (float)UnityEngine.Rendering.CullMode.Off : (float)UnityEngine.Rendering.CullMode.Back);
	}

    public bool GetIsForceEnabled()
    {
        return GetMaterialKeywordEnabled(editMaterial, "FORCE_ENABLED");
    }

    public void SetIsForceEnabled(bool isForceEnabled)
    {
        SetMaterialKeywordEnabled(editMaterial, "FORCE_ENABLED", isForceEnabled);
    }

    public float GetCulloff()
    {
        return GetMaterialFloatProperty(editMaterial, "_Cutoff");
    }

    public void SetCulloff(float value)
    {
        SetMaterialFloatProperty(editMaterial, "_Cutoff", value);
    }

    public Color GetColor()
    {
        return GetMaterialColorProperty(editMaterial, "_Color");
    }

    public void SetColor(Color color)
    {
        SetMaterialColorProperty(editMaterial, "_Color", color);
    }

	private void SetMaterialFloatProperty(Material mtrl, string propertyName, float value)
	{
		if(mtrl == null)
		{
			return;
		}
		mtrl.SetFloat(propertyName, value);
	}

	private float GetMaterialFloatProperty(Material mtrl, string propertyName)
	{
		if(mtrl == null)
		{
			return 0.0f;
		}
		return mtrl.GetFloat(propertyName);
	}

    private void SetMaterialColorProperty(Material mtrl, string propertyName, Color color)
    {
        if (mtrl == null)
        {
            return;
        }
        mtrl.SetColor(propertyName, color);
    }

    private Color GetMaterialColorProperty(Material mtrl, string propertyName)
    {
        if (mtrl == null)
        {
            return Color.black;
        }
        return mtrl.GetColor(propertyName);
    }

    public void SetIsGlobalWind(bool value)
    {
        SetMaterialKeywordEnabled(editMaterial, "IS_GLOBAL_WIND", value);
    }

    public bool GetIsGlobalWind()
    {
        return GetMaterialKeywordEnabled(editMaterial, "IS_GLOBAL_WIND");
    }

    public void SetAlphaTestEnabled(bool value)
    {
        SetMaterialKeywordEnabled(editMaterial, "ALPHA_TEST_ENABLED", value);
    }

    public bool GetAlphaTestEnabled()
    {
        return GetMaterialKeywordEnabled(editMaterial, "ALPHA_TEST_ENABLED");
    }

    private void SetMaterialKeywordEnabled(Material mtrl, string keyword, bool enabled)
    {
        if (mtrl == null)
        {
            return;
        }

        if (enabled)
        {
            mtrl.EnableKeyword(keyword);
        }
        else
        {
            mtrl.DisableKeyword(keyword);
        }
    }

    private bool GetMaterialKeywordEnabled(Material mtrl, string keyword)
    {
        if (mtrl == null)
        {
            return false;
        }
        return mtrl.IsKeywordEnabled(keyword);
    }
}