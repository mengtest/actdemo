using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class LoadingUI : MonoBehaviour {

    private Slider mScrollbar;

    //void Awake()
    //{
    //    mScrollbar = gameObject.GetComponent<Scrollbar>();
    //    mScrollbar.size = 0.0f;
    //    //XmlManager.Instance.LoadAllConfig();
    //}
    /// <summary>
    /// 改变进度
    /// </summary>
    /// <param name="progress"></param>
    public void ChangeProgress(float progress)
    {
        if (mScrollbar == null)
        {
            mScrollbar = gameObject.GetComponent<Slider>();
            mScrollbar.value = 0.0f;
        }

        if (progress < 0.0f)
        {
            return;
        }

        mScrollbar.value = progress;
    }
}
