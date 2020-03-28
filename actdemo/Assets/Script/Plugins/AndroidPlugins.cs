using UnityEngine;
using System.Collections;

/// <summary>
/// Unity Android插件包
/// </summary>
public class AndroidPlugins
{
    private static AndroidPlugins _Instance = null;

    public static AndroidPlugins Instance
    {
        private set { }
        get
        {
            if (_Instance == null)
            {
                _Instance = new AndroidPlugins();
            }

            return _Instance;
        }
    }

#if UNITY_ANDROID
    private AndroidJavaClass androidJC = null;
    private const string mJavaPackageName = "com.snailgame.anysdk.MainActivity";

    private AndroidPlugins()
    {
        androidJC = new AndroidJavaClass(mJavaPackageName);
    }
    
    /// <summary>
    /// 初始化Android平台插件
    /// </summary>
    /// <param name="screenOrientation"></param>
    /// <param name="is//Debug"></param>
    /// <param name="isAnysdk"></param>
    public void InitPlugins(int screenOrientation, bool is//Debug, bool isAnysdk)
    {
        androidJC.CallStatic("InitPlugins", screenOrientation, is//Debug, isAnysdk);
    }

    /// <summary>
    /// 获取设备mac地址
    /// </summary>
    public void GetDeviceMac()
    {
        androidJC.CallStatic("GetDeviceMac");
    }
#endif
}