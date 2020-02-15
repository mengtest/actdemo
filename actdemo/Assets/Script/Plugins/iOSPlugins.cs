using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
/// Unity iOS插件包
/// </summary>
public class iOSPlugins
{
    #region <>注册iOS方法<>

    [DllImport("__Internal")]
    public static extern void iOSInitPlugins(int screenOrientation, bool isDebug, bool isAnysdk);

    [DllImport("__Internal")]
    public static extern void iOSGetDeviceMac();

    #endregion

    private static iOSPlugins _Instance = null;
    private iOSPlugins() { }

    public static  iOSPlugins Instance
    {
        private set { }
        get
        {
            if (_Instance == null)
            {
                _Instance = new iOSPlugins();
            }

            return _Instance;
        }
    }

    public void InitPlugins(int screenOrientation, bool isDebug, bool isAnysdk)
    {
        iOSInitPlugins(screenOrientation, isDebug, isAnysdk);
    }

    public void GetDeviceMac()
    {
        iOSGetDeviceMac();
    }
}