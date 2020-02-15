using UnityEngine;
using System.Collections;

public class TextManager
{
    private static TextManager _Instance = null;
    private TextManager() { }

    public static TextManager Instance
    {
        private set { }
        get
        {
            if (_Instance == null)
            {
                _Instance = new TextManager();
            }

            return _Instance;
        }
    }

    /// <summary>
    /// 获取对应字符串
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetString(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            LogSystem.LogError("string key is null");

            return "";
        }

        return "";
    }
}