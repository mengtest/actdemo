using UnityEngine;
using System.Collections;

public class RestoreManager : MonoBehaviour
{
    private static RestoreManager _Instance = null;
    public static RestoreManager Instance
    {
        get { return _Instance; }
    }

    void Awake()
    {
        _Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 最后登录的角色名
    /// </summary>
    private const string mLastRoleName = "LastRoleName";
    public string LastRoleName
    {
        set
        {
            if (!PlayerPrefs.HasKey(mLastRoleName))
            {
                PlayerPrefs.SetString(mLastRoleName, "");
            }
            PlayerPrefs.SetString(mLastRoleName, value);
        }

        get
        {
            return PlayerPrefs.GetString(mLastRoleName, "");
        }
    }

    private const string mLastRoleIndex = "LastRoleIndex";
    public int LastRoleIndex
    {
        set
        {
            if (!PlayerPrefs.HasKey(mLastRoleIndex))
            {
                PlayerPrefs.SetInt(mLastRoleIndex, 0);
            }
            PlayerPrefs.SetInt(mLastRoleIndex, value);
        }

        get
        {
            return PlayerPrefs.GetInt(mLastRoleIndex, 0);
        }
    }

    private const string mAccount = "Account";
    public string Account
    {
        set
        {
            if (!PlayerPrefs.HasKey(mAccount))
            {
                PlayerPrefs.SetString(mAccount, "");
            }
            PlayerPrefs.SetString(mAccount, value);
        }

        get
        {
            return PlayerPrefs.GetString(mAccount, "");
        }
    }

    private const string mPassword = "Password";
    public string Password
    {
        set
        {
            if (!PlayerPrefs.HasKey(mPassword))
            {
                PlayerPrefs.SetString(mPassword, "");
            }
            PlayerPrefs.SetString(mPassword, value);
        }

        get
        {
            return PlayerPrefs.GetString(mPassword, "");
        }
    }
}