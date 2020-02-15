using UnityEngine;
using UnityEditor;
using System.Collections;

public class BakeNormalSpecularToDiffuseShaderGUI : ShaderGUI
{
    private MaterialProperty mainColorProperty = null;

    private MaterialProperty backFaceAmbientProperty = null;

    private MaterialProperty intensityProperty = null;

    private MaterialProperty backFaceAmbientIntensityProperty = null;

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
        intensityProperty = FindProperty("_Intensity", props);
        backFaceAmbientIntensityProperty = FindProperty("_BackFaceAmbientIntensity", props);
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

        material.EnableKeyword("SPECULAR");
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        FindProperties(properties);

        Material mtrl = materialEditor.target as Material;

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
            materialEditor.RangeProperty(backFaceAmbientIntensityProperty, "Back Face Ambient Intensity");
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
