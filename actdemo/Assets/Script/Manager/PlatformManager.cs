using UnityEngine;
using System.Collections;
using LitJson;

public class PlatformManager : MonoBehaviour
{
    public delegate void PlatformInitFinished();
    public PlatformInitFinished mPlatformInitFinished;

    public delegate void PlatformGetDeviceMac(string mac);
    public PlatformGetDeviceMac mPlatformGetDeviceMac;

    private static PlatformManager _Instance = null;
    public static PlatformManager Instance
    {
        private set { }
        get
        {
            return _Instance;
        }
    }

    void Awake()
    {
        _Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    #region <>初始化平台<>

    public void Init(int screenOrientation, bool isDebug, bool isAnysdk, PlatformInitFinished callback)
    {
        mPlatformInitFinished = callback;

        // 创建平台对象
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidPlugins.Instance.InitPlugins(screenOrientation, isDebug, isAnysdk);
#elif UNITY_IPHONE && !UNITY_EDITOR
        iOSPlugins.Instance.InitPlugins(screenOrientation, isDebug, isAnysdk);
#else
        if (mPlatformInitFinished != null)
        {
            mPlatformInitFinished();
        }
#endif
    }

    #endregion

    #region <>初始化完成回调<>

    void GetInitFinished(string result)
    {
        JsonData json = UtilTools.GetJsonData(result);
        if (json == null)
        {
            return;
        }

        GlobalData.mAnySDKInit = true;

        if (mPlatformInitFinished != null)
        {
            mPlatformInitFinished();
        }
    }

    #endregion

    #region <>获取设备Mac地址<>

    public void GetDeviceMac(PlatformGetDeviceMac callback)
    {
        mPlatformGetDeviceMac = callback;

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidPlugins.Instance.GetDeviceMac();
#elif UNITY_IPHONE && !UNITY_EDITOR
        iOSPlugins.Instance.GetDeviceMac();
#else
        if (mPlatformGetDeviceMac != null)
        {
            mPlatformGetDeviceMac(GlobalData.mDeviceMac);
        }
#endif
    }

    public void OnDeviceMac(string result)
    {
        JsonData json = UtilTools.GetJsonData(result);
        if (json == null)
        {
            return;
        }

        GlobalData.mDeviceMac = json["mac"].ToString();

        if (mPlatformGetDeviceMac != null)
        {
            mPlatformGetDeviceMac(GlobalData.mDeviceMac);
        }
    }

    #endregion
}