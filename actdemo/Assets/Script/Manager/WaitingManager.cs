using UnityEngine;
using System.Collections;

public class WaitingManager : MonoBehaviour
{
    private static WaitingManager _Instance = null;
    public static WaitingManager Instance
    {
        get { return _Instance; }
    }

    //private WaitingUI mWaitingUI;

    void Awake()
    {
        _Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        GameObject uiPrefab = ResourceManager.Instance.GetUIPrefab("WaitingUI");
        if (uiPrefab == null)
        {
            LogSystem.LogError("WaitingUI is null");
        }

        GameObject parent = GameObject.Find("UI Root/Camera");
        //GameObject go = NGUITools.AddChild(parent, uiPrefab);
        //go.name = "WaitingUI";
        //go.AddComponent(System.Type.GetType("WaitingUI"));
        //go.transform.localPosition = Vector3.zero;
        //go.transform.localScale = Vector3.one;

        //mWaitingUI = go.GetComponent<WaitingUI>();
    }

    public void Show(string content = "")
    {
        //if (mWaitingUI == null)
        //{
        //    return;
        //}

        //mWaitingUI.SetContent(content);
        //mWaitingUI.gameObject.SetActive(true);
    }

    public void Close()
    {
        //if (mWaitingUI == null)
        //{
        //    return;
        //}

        //mWaitingUI.gameObject.SetActive(false);
    }
}