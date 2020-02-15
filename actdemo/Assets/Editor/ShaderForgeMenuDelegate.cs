using UnityEngine;
using UnityEditor;
using System.Collections;

public class ShaderForgeMenuDelegate
{
    [MenuItem("KPlaneTools/Shader编辑器（ShaderForge）")]
    private static void Execute()
    {
        EditorApplication.ExecuteMenuItem("Window/Shader Forge");
    }
}
