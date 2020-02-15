using UnityEngine;
using System.Collections;

public class PromptUI : MonoBehaviour
{
    private UILabel mContent;
    private UIEventListener mClose;

    void Awake()
    {
        mContent = transform.Find("Background/Content").GetComponent<UILabel>();
        mClose = transform.Find("Background").GetComponent<UIEventListener>();
        mClose.onClick = OnClose;
    }

    /// <summary>
    /// 设置提示内容
    /// </summary>
    /// <param name="content"></param>
    public void SetContent(string content)
    {
        if (mContent == null)
        {
            return;
        }

        mContent.text = content;
    }

    void OnClose(GameObject go)
    {
        UIManager.Instance.CloseUI("PromptUI");
    }

    void OnDestroy()
    {
        mContent = null;
        mClose = null;
    }
}