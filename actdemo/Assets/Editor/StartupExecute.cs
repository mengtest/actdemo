using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using TinyBinaryXml;

[InitializeOnLoad]
public class StartupExecute
{
    // Unity启动时注册回调
    static StartupExecute()
    {
        ConvertBinaryXML();
    }

    private static void ConvertBinaryXML()
    {
        EditorApplication.playmodeStateChanged -= OnPlayModeStateChange;
        EditorApplication.playmodeStateChanged += OnPlayModeStateChange;
    }

    private static void OnPlayModeStateChange()
    {
        bool changed = false;
        string[] files = Directory.GetFiles("Assets/Resources/Config");
        foreach (var file in files)
        {
            if (GameAssetPostprocessor.ConvertToBinaryXml(file.Replace('\\', '/')))
            {
                changed = true;
            }
        }

        if (changed)
        {
            AssetDatabase.Refresh();
        }
    }
}
