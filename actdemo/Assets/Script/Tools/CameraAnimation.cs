using UnityEngine;
using System.Collections;

public class CameraAnimation : MonoBehaviour
{
    private static CameraAnimation _Instance = null;
    public static CameraAnimation Instance
    {
        get
        {
            return _Instance;
        }
    }

    private IObject mRole;
    private Transform mRoleBip01;
    private Vector3[] mPathArray;
    private SlowMotionProperty mSlowMotionProperty;

    void Awake()
    {
        _Instance = this;
    }

    /// <summary>
    /// 相机动画开始
    /// </summary>
    /// <param name="path"></param>
    public void Animation(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        mPathArray = iTweenPath.GetPath(path);
        if (mPathArray == null || mPathArray.Length < 1)
        {
            return;
        }

        if (ObjectManager.mRole == null)
        {
            return;
        }

        mRole = ObjectManager.mRole;
		mRoleBip01 = mRole.mTransform.Find("Bip01/Bip01 Pelvis");
        if (mRoleBip01 == null)
        {
            return;
        }

        mSlowMotionProperty = XmlManager.Instance.GetSlowMotionProperty(10000843);
        if (mSlowMotionProperty == null)
        {
            return;
        }

        for (int i = 0; i < mPathArray.Length; ++i)
        {
            mPathArray[i] += ObjectManager.mRole.mPosition;
            Vector3 dir = mPathArray[i] - ObjectManager.mRole.mPosition;
            dir = ObjectManager.mRole.mTransform.rotation * dir;
            mPathArray[i] = ObjectManager.mRole.mPosition + dir;
        }

        transform.position = mPathArray[0];

        GetComponent<CameraMove>().enabled = false;

        GetComponent<LookAt>().enabled = true;
        GetComponent<LookAt>().mTarget = mRoleBip01;

        transform.GetComponent<Camera>().fieldOfView =
            mSlowMotionProperty.mSlowMotionStepList[0].mFieldOfView;

        Hashtable table = new Hashtable();
        table.Add("position", mPathArray[1]);
        table.Add("time", mSlowMotionProperty.mSlowMotionStepList[0].mTime);
        table.Add("easetype", 
            mSlowMotionProperty.mSlowMotionStepList[0].mEaseType);
        table.Add("ignoretimescale", true);
        table.Add("onupdate", "FollowBip01");
        table.Add("onupdatetarget", gameObject);

        table.Add("oncomplete", "NextCameraAnimation");
        table.Add("oncompletetarget", gameObject);
        table.Add("oncompleteparams", 2);

        iTween.MoveTo(gameObject, table);

        GlobalData.TimeScale = mSlowMotionProperty.mSlowMotionStepList[0].mTimeScale;
    }

    void FollowBip01()
    {
        transform.position += mRoleBip01.localPosition;
    }

    void NextCameraAnimation(int index)
    {
        if (index >= mPathArray.Length)
        {
            AnimationOver();

            return;
        }

        GlobalData.TimeScale = mSlowMotionProperty.mSlowMotionStepList[index - 1].mTimeScale;
        transform.GetComponent<Camera>().fieldOfView =
            mSlowMotionProperty.mSlowMotionStepList[index - 1].mFieldOfView;

        Hashtable table = new Hashtable();
        table.Add("position", mPathArray[index]);
        table.Add("time", mSlowMotionProperty.mSlowMotionStepList[index - 1].mTime);
        table.Add("easetype", mSlowMotionProperty.mSlowMotionStepList[index - 1].mEaseType);
        table.Add("ignoretimescale", true);
        table.Add("onupdate", "FollowBip01");
        table.Add("onupdatetarget", gameObject);

        table.Add("oncomplete", "NextCameraAnimation");
        table.Add("oncompletetarget", gameObject);
        table.Add("oncompleteparams", ++index);

        iTween.MoveTo(gameObject, table);
    }

    void AnimationOver()
    {
        GlobalData.TimeScale = 1.0f;

        mSlowMotionProperty = null;

        GetComponent<LookAt>().enabled = false;

        GetComponent<CameraMove>().enabled = true;
        GetComponent<CameraMove>().Reset();
    }
}