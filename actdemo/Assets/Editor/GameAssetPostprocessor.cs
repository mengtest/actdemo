using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using TinyBinaryXml;

public class GameAssetPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (TinyBinaryXmlSerialization(importedAssets))
        {
            AssetDatabase.Refresh();
        }
    }

    private static bool TinyBinaryXmlSerialization(string[] pathList)
    {
        bool flag = false;
        foreach (var path in pathList)
        {
            if (ConvertToBinaryXml(path))
            {
                flag = true;
            }
        }
        return flag;
    }

    public static bool ConvertToBinaryXml(string path)
    {
        if (path.StartsWith("Assets/Resources/Config/"))
        {
            string saveToDir = Application.dataPath + "/Resources/ConfigBytes";
            if (!Directory.Exists(saveToDir))
            {
                Directory.CreateDirectory(saveToDir);
            }

            if (path.ToLower().EndsWith(".xml"))
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(path);
                    string fileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);

                    TbXmlSerializer serializer = new TbXmlSerializer();
                    byte[] bytes = serializer.SerializeXmlString((AssetDatabase.LoadMainAssetAtPath(path) as TextAsset).text);

                    File.WriteAllBytes(saveToDir + "/" + fileName + ".bytes", bytes);
                    return true;
                }
                catch (System.Exception exception)
                {
                    //Debug.LogError("Error:" + path);
                    //Debug.LogError(exception.Message);
                    //Debug.LogError(exception.StackTrace);
                    //Debug.LogError("");
                }
            }
            else
            {
                if (!path.ToLower().EndsWith(".meta"))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    File.Copy(path, saveToDir + "/" + fileInfo.Name, true);
                    return true;
                }
            }
        }
        return false;
    }
}
