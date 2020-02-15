using UnityEngine;
using System.Collections;

public class GlobalData
{
    private static string mVersion = string.Empty;
    public static string Version
    {
        set { mVersion = value; }
        get { return mVersion; }
    }

    public static bool mAnySDKInit = false;
    public static string mDeviceMac = string.Empty;
    public static float mMusicVolume = 1.0f;
    public static bool MusicOpen
    {
        get
        {
            if (mMusicVolume < 0.1f)
            {
                return false;
            }

            return true;
        }
    }

    public static float mAudioVolume = 1.0f;
    public static bool AudioOpen
    {
        get
        {
            if (mAudioVolume < 0.1f)
            {
                return false;
            }

            return true;
        }
    }

    public static int mScreenOrientation = 0;
    public static bool mDebug = true;
    public static bool mAnySDK = false;

    public static float TimeScale
    {
        get
        {
            return Time.timeScale;
        }

        set
        {
            Time.timeScale = value;
        }
    }
}