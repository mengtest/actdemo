using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private Dictionary<string, GameObject> mUIDic;

    /// <summary>
    /// 可隐藏界面栈
    /// </summary>
    private Stack<string> mHideUIStack;

    /// <summary>
    /// 当前打开界面名字
    /// </summary>
    private string mCurrentOpenUIName = "";

    public float mRectWidth = Screen.width;
    public float mRectHeight = Screen.height;

    /// <summary>
    /// 当前界面最大层
    /// </summary>
    private int mCurrentMaxLayer = 0;

    private static UIManager _Instance = null;
    public static UIManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    public UHUDUI mHUDUIPanel;

    GameObject root;

    void Awake()
    {
        _Instance = this;

        mUIDic = new Dictionary<string, GameObject>();
        mHideUIStack = new Stack<string>();

        //UIRoot root = GameObject.Find("UI Root").GetComponent<UIRoot>();
        //float scale = root.activeHeight / mRectHeight;
        //mRectWidth *= scale;
        //mRectHeight *= scale;
        root = GameObject.Find("UIRoot");

        // 先隐藏摇杆
        //GameObject parent = GameObject.Find("UI Root/Camera/JoyStickUI");
        //parent.SetActive(false);

        // 获取头顶文字，伤害飘字，血条等panel
 //       CreateHUDUI();
    }

    void CreateHUDUI()
    {
        GameObject go = ResourceManager.Instance.GetUIPrefab("UHUDUI");
        if (go == null)
        {
            return;
        }

        go = NGUITools.AddChild(gameObject, go);
        if (go == null)
        {
            return;
        }

        go.name = "UHUDUI";
        mHUDUIPanel = go.AddComponent<UHUDUI>();
    }

    public Transform OpenUI(string uiName)
    {
        if (string.IsNullOrEmpty(uiName))
        {
            LogSystem.LogError("OpenUI uiName is null");

            return null;
        }

        // 不重复打开相同界面
        if (mCurrentOpenUIName == uiName)
        {
            return mUIDic[mCurrentOpenUIName].transform;
        }

        mCurrentOpenUIName = uiName;

        // 界面存在于内存中，之前被隐藏的。
        if (mHideUIStack.Contains(uiName))
        {
            ShowUI(uiName);

            return mUIDic[mCurrentOpenUIName].transform;
        }

        GameObject uiPrefab = ResourceManager.Instance.GetUIPrefab(uiName);
        if (uiPrefab == null)
        {
            LogSystem.LogError("uiPrefab is null");

            return null;
        }

        //uiPrefab.transform.SetParent(root.transform);
        uiPrefab.SetActive(true);
        uiPrefab.name = uiName;
        uiPrefab.AddComponent(System.Type.GetType(uiName));
        uiPrefab.transform.localPosition = Vector3.zero;
        uiPrefab.transform.localScale = Vector3.one;
        //uiPrefab.GetComponent<UIPanel>().depth = mCurrentMaxLayer++;
        uiPrefab.layer = LayerMask.NameToLayer("UI");

        // 将当前界面隐藏
        if (mHideUIStack.Count > 0)
        {
            HideUI(mHideUIStack.Peek());
        }

        mHideUIStack.Push(uiName);

        mUIDic.Add(uiName, uiPrefab);

        return uiPrefab.transform;
    }

    public void CloseUI(string uiName)
    {
        if (string.IsNullOrEmpty(uiName))
        {
            return;
        }

        if (!mUIDic.ContainsKey(uiName))
        {
            return;
        }

        GameObject go = mUIDic[uiName];
        if (go != null)
        {
            GameObject.Destroy(go);
        }

        mUIDic.Remove(uiName);

        --mCurrentMaxLayer;

        mHideUIStack.Pop();

        if (mHideUIStack.Count > 0)
        {
            OpenUI(mHideUIStack.Peek());
        }
    }

    public void CloseAllUI()
    {
        int count = mUIDic.Count;
        if (count < 1)
        {
            return;
        }

        GameObject go = null;
        List<string> list = new List<string>(mUIDic.Keys);

        for (int i = 0; i < count; ++i)
        {
            go = mUIDic[list[i]];
            if (go == null)
            {
                continue;
            }

            GameObject.Destroy(go);
        }

        mUIDic.Clear();
    }

    private void HideUI(string uiName)
    {
        if (string.IsNullOrEmpty(uiName))
        {
            return;
        }

        if (!mHideUIStack.Contains(uiName))
        {
            return;
        }

        if (!mUIDic.ContainsKey(uiName))
        {
            return;
        }

        if (mHideUIStack.Peek() != uiName)
        {
            return;
        }

        GameObject go = mUIDic[uiName];
        if (go == null)
        {
            return;
        }

        go.SetActive(false);
    }

    private void ShowUI(string uiName)
    {
        if (string.IsNullOrEmpty(uiName))
        {
            return;
        }

        if (!mHideUIStack.Contains(uiName))
        {
            return;
        }

        if (!mUIDic.ContainsKey(uiName))
        {
            return;
        }

        GameObject go = mUIDic[uiName];
        if (go == null)
        {
            return;
        }

        go.SetActive(true);
    }

    public T GetUI<T>(string uiName) where T : MonoBehaviour
    {
        if (!mUIDic.ContainsKey(uiName))
        {
            OpenUI(uiName);
        }

        return mUIDic[uiName].GetComponent<T>();
    }
}