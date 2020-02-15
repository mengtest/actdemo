using UnityEngine;
using System.Collections;

public class LoadingUI1 : MonoBehaviour
{
    private UISlider mSlider;
    private UILabel mSchduleLabel;
    private UILabel mContent;

    private float mChangeLoadingContentTime = 2.0f;

    void Awake()
    {
        mSlider = transform.Find("Progress Bar").GetComponent<UISlider>();
        mSlider.value = 0.0f;

        mSchduleLabel = transform.Find("Progress Bar/Thumb/Thumb").GetComponent<UILabel>();
        mSchduleLabel.text = "0%";

        mContent = transform.Find("Label").GetComponent<UILabel>();
    }

    void Start()
    {
        TimerManager.AddTimer("ChangeLoadingContent", mChangeLoadingContentTime, ChangeLoadingContentTime);
    }

    void ChangeLoadingContentTime()
    {
        mContent.text = "";
    }

    /// <summary>
    /// 改变进度
    /// </summary>
    /// <param name="progress"></param>
    public void ChangeProgress(float progress)
    {
        if (progress < 0.0f)
        {
            return;
        }

        mSlider.value = progress;
        mSchduleLabel.text = string.Format("{0}%", (progress * 100).ToString("0.00"));
    }
}