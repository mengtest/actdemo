using UnityEngine;
using UnityEditor;
using System.Collections;

public class SnailRoleTestShaderGUI : ShaderGUI
{
    private MaterialProperty mainColorProperty = null;

    private MaterialProperty backFaceAmbientProperty = null;

    private MaterialProperty selfShadowAmbientProperty = null;

    private MaterialProperty intensityProperty = null;

    private MaterialProperty overlayAddProperty = null;

    private MaterialProperty glossinessProperty = null;

    private MaterialProperty shininessProperty = null;

    private MaterialProperty offsetLightPosProperty = null;

    private MaterialProperty mainTexProperty = null;

    private MaterialProperty specularTexProperty = null;

    private MaterialProperty normalTexProperty = null;

    private void FindProperties(MaterialProperty[] props)
    {
        mainColorProperty = FindProperty("_Color", props);
        backFaceAmbientProperty = FindProperty("_BackFaceAmbient", props);
        selfShadowAmbientProperty = FindProperty("_SelfShadowAmbient", props);
        intensityProperty = FindProperty("_Intensity", props);
        overlayAddProperty = FindProperty("_OverlayAdd", props);
        glossinessProperty = FindProperty("_Glossiness", props);
        shininessProperty = FindProperty("_Shininess", props);
        offsetLightPosProperty = FindProperty("_LightOffset", props);
        mainTexProperty = FindProperty("_MainTex", props);
        specularTexProperty = FindProperty("_SpecMap", props);
        normalTexProperty = FindProperty("_BumpMap", props);
    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if (oldShader != null && newShader != null)
        {
            if(oldShader.name != "SnailRoleTest" && oldShader.name != "SnailRoleXrayTest")
            {
                material.EnableKeyword("SPECULAR");
            }
        }
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        FindProperties(properties);

        Material mtrl = materialEditor.target as Material;

        {
            if (mtrl.shader != null)
            {
                bool xrayEnabled = mtrl.shader.name.Contains("Xray");
                EditorGUI.BeginChangeCheck();
                xrayEnabled = EditorGUILayout.Toggle("Xray Effect", xrayEnabled);
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.SetShader(xrayEnabled ? Shader.Find("SnailRoleXrayTest") : Shader.Find("SnailRoleTest"));
                    SceneView.RepaintAll();
                }
            }
        }

        {
            materialEditor.TexturePropertySingleLine(new GUIContent("Main Tex", "RGB"), mainTexProperty);
        }

        {
            EditorGUILayout.BeginHorizontal();
            materialEditor.TexturePropertySingleLine(new GUIContent("Specular Tex", "RGB"), specularTexProperty);
            EditorGUI.BeginChangeCheck();
            bool normapMapEnabled = EditorGUILayout.Toggle("", mtrl.IsKeywordEnabled("SPECULAR"));
            if (EditorGUI.EndChangeCheck())
            {
                if (normapMapEnabled)
                {
                    mtrl.EnableKeyword("SPECULAR");
                }
                else
                {
                    mtrl.DisableKeyword("SPECULAR");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        {
            materialEditor.TexturePropertySingleLine(new GUIContent("Normal Tex", "RGB"), normalTexProperty);
        }

        {
            materialEditor.TextureScaleOffsetProperty(mainTexProperty);
            specularTexProperty.textureScaleAndOffset = mainTexProperty.textureScaleAndOffset;
            normalTexProperty.textureScaleAndOffset = mainTexProperty.textureScaleAndOffset;
        }

        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        {
            EditorGUILayout.BeginHorizontal();
            materialEditor.ColorProperty(mainColorProperty, "Main Color");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        {
            EditorGUILayout.BeginHorizontal();
            materialEditor.ColorProperty(backFaceAmbientProperty, "Back Face Ambient");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        {
            EditorGUILayout.BeginHorizontal();
            materialEditor.ColorProperty(selfShadowAmbientProperty, "Self Shadow");
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            bool selfShadowEnabled = EditorGUILayout.Toggle("", mtrl.IsKeywordEnabled("SELF_SHADOW"));
            if (EditorGUI.EndChangeCheck())
            {
                if (selfShadowEnabled)
                {
                    mtrl.EnableKeyword("SELF_SHADOW");
                }
                else
                {
                    mtrl.DisableKeyword("SELF_SHADOW");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        {
            materialEditor.RangeProperty(intensityProperty, "Intensity"); 
        }

        {
            materialEditor.RangeProperty(overlayAddProperty, "Overlay Add");
        }

        {
            materialEditor.RangeProperty(glossinessProperty, "Glossiness");
        }

        {
            materialEditor.RangeProperty(shininessProperty, "Shininess");
        }

        {
            materialEditor.VectorProperty(offsetLightPosProperty, "Offset Light");
        }

        {
            bool normalMapEnabled = mtrl.GetTexture("_BumpMap") != null;
            if (normalMapEnabled)
            {
                mtrl.EnableKeyword("NORMAL_MAP");
            }
            else
            {
                mtrl.DisableKeyword("NORMAL_MAP");
            }
        }

        {
            bool specularMapEnabled = mtrl.GetTexture("_SpecMap") != null;
            if (specularMapEnabled)
            {
                mtrl.EnableKeyword("SPECULAR_MAP");
            }
            else
            {
                mtrl.DisableKeyword("SPECULAR_MAP");
            }
        }

    }
}
