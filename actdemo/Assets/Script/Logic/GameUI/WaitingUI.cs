using UnityEngine;
using System.Collections;

public class WaitingUI : MonoBehaviour
{
    private UILabel mContentLabel;

    void Awake()
    {
        mContentLabel = transform.Find("Content/Label").GetComponent<UILabel>();
    }

    /// <summary>
    /// 设置等待界面文字
    /// </summary>
    /// <param name="content"></param>
    public void SetContent(string content = "")
    {
        if (string.IsNullOrEmpty(content))
        {
            content = XmlManager.Instance.GetCommonText("Game0001");
        }

        mContentLabel.text = content;
    }
}