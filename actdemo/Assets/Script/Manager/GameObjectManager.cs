using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectManager : MonoBehaviour
{
    private static GameObjectManager _Instance = null;
    public static GameObjectManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    /// <summary>
    /// 对象池
    /// </summary>
    private Dictionary<string, Dictionary<string, GameObject>> mPoolDic = new Dictionary<string, Dictionary<string, GameObject>>();

    void Awake()
    {
        _Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 创建池
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public Dictionary<string, GameObject> CreatePool(string poolName)
    {
        Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();
        if (!mPoolDic.ContainsKey(poolName))
        {
            mPoolDic.Add(poolName, dic);
        }

        return mPoolDic[poolName];
    }

    /// <summary>
    /// 添加对象到池中
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="obj"></param>
    public void AddPool(string poolName, string objName, GameObject obj)
    {
        if (string.IsNullOrEmpty(poolName) || !string.IsNullOrEmpty(objName) || obj == null)
        {
            return;
        }

        GameObject go = GameObject.Instantiate(obj);
        if (go == null)
        {
            return;
        }

        if (mPoolDic.ContainsKey(poolName))
        {
            mPoolDic[poolName].Add(objName, go);
        }
        else
        {
            CreatePool(poolName).Add(objName, go);
        }
    }

    public GameObject GetGameObject(string poolName, string objName)
    {
        if (mPoolDic.ContainsKey(poolName))
        {
            return mPoolDic[poolName][objName];
        }

        return null;
    }
}