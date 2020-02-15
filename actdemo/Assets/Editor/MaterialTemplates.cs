using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class MaterialTemplates : EditorWindow
{
    private static MaterialTemplates window;

    [MenuItem("KPlaneTools/材质球模板")]
    public static void CreateWizard()
    {
        window = (MaterialTemplates)EditorWindow.GetWindow(typeof(MaterialTemplates));
        window.Refresh();
    }

    private List<Material> templates = new List<Material>();

    private Vector2 scroll = Vector2.zero;

    private bool onlyShader = true;

    public void Refresh()
    {
        templates.Clear();
        if(Directory.Exists("Assets/MaterialTemplates"))
        {
            DirectoryInfo dir = new DirectoryInfo("Assets/MaterialTemplates");
            FileInfo[] files = dir.GetFiles();
            foreach(var file in files)
            {
                if(file.Extension.ToLower() == ".mat")
                {
                    Object asset = AssetDatabase.LoadMainAssetAtPath("Assets/MaterialTemplates/" + file.Name);
                    if(asset is Material)
                    {
                        Material mtrl = (Material)asset;
                        templates.Add(mtrl);
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("材质球模板\n位于Assets/MaterialTemplates\n选择场景中对象，点击使用", MessageType.Info, true);
        
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("刷新"))
            {
                Refresh();
            }
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space();

        onlyShader = EditorGUILayout.Toggle("仅使用Shader：", onlyShader);

        EditorGUILayout.Space();

        scroll = EditorGUILayout.BeginScrollView(scroll);
        {
            EditorGUILayout.BeginVertical();
            {
                int numTemplates = templates == null ? 0 : templates.Count;
                for (int i = 0; i < numTemplates; ++i)
                {
                    Material template = templates[i];
                    if(template != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Box(template.mainTexture, GUILayout.Width(30), GUILayout.Height(30));
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.ObjectField(template, typeof(Material));
                                    if (GUILayout.Button("使用"))
                                    {
                                        GameObject[] targets = Selection.gameObjects;
                                        foreach (var target in targets)
                                        {
                                            if (target != null)
                                            {
                                                Renderer[] renderers = target.GetComponents<Renderer>();
                                                foreach (var renderer in renderers)
                                                {
                                                    if (renderer != null)
                                                    {
                                                        Material[] mtrls = renderer.sharedMaterials;
                                                        if (mtrls != null)
                                                        {
                                                            foreach (var mtrl in mtrls)
                                                            {
                                                                if (mtrl != null)
                                                                {
                                                                    mtrl.shader = template.shader;

                                                                    if (!onlyShader)
                                                                    {
                                                                        mtrl.CopyPropertiesFromMaterial(template);
                                                                    }

                                                                    EditorUtility.SetDirty(mtrl);
                                                                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(mtrl));
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
    }
}
