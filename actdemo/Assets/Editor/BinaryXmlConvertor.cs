using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using TinyBinaryXml;

public class BinaryXmlConvertor : EditorWindow
{
    private static BinaryXmlConvertor window;

    private BuildTarget buildTarget = BuildTarget.iOS;

    [MenuItem("KPlaneTools/XML格式转换")]
    public static void CreateWizard()
    {
        window = (BinaryXmlConvertor)EditorWindow.GetWindow(typeof(BinaryXmlConvertor));
    }

    public void OnGUI()
    {
        EditorGUILayout.HelpBox("将Resources/Config中的Xml转换成二进制的形式，保存到Resources/ConfigBytes。", MessageType.Info, true);
        GUILayout.Space(5);

        if (GUILayout.Button("Do"))
        {
            bool flag = false;
            DirectoryInfo dirInfo = new DirectoryInfo("Assets/Resources/Config");
            FileInfo[] fileInfos = dirInfo.GetFiles();
            foreach (var fileInfo in fileInfos)
            {
                string path = fileInfo.FullName;
                string assetPath = path.Substring(new DirectoryInfo(Application.dataPath).Parent.FullName.Length + 1);
                if (ConvertToBinaryXml(assetPath.Replace('\\', '/')))
                {
                    flag = true;
                }
            }
            EditorUtility.DisplayDialog("Message", "转换完成", "ok");
            if (flag)
            {
                AssetDatabase.Refresh();
            }
        }
    }

    private string ReplaceValueString(string line, string typeLower, string typeUpper, string typeConvert)
    {
        string pattern = typeLower + ".Parse(";

        bool flag = false;
        if (line.Contains(pattern))
        {
            flag = true;
            line = line.Replace(pattern, "");
        }

        pattern = "Convert." + typeConvert + "(";
        if (!flag && line.Contains(pattern))
        {
            flag = true;
            line = line.Replace(pattern, "");
        }

        if (flag)
        {
            pattern = ".GetValue(\"@";
            if (line.Contains(pattern))
            {
                line = line.Replace(pattern, ".Get" + typeUpper + "Value(\"");

                pattern = "));";
                if (line.Contains(pattern))
                {
                    line = line.Replace(pattern, ");");
                }
            }

            pattern = ".Trim()";
            if (line.Contains(pattern))
            {
                line = line.Replace(pattern, "");
            }
        }

        return line;
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
                    Debug.LogError("Error:" + path);
                    Debug.LogError(exception.Message);
                    Debug.LogError(exception.StackTrace);
                    Debug.LogError("");
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
