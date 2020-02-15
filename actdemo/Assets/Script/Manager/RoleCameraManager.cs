using UnityEngine;
using System.Collections;

public class RoleCameraManager : MonoBehaviour
{
    private static RoleCameraManager _Instance = null;
    public static RoleCameraManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    private CRoleObject mRole;
    private GameObject mMainCamera;
    private Transform mRoleTransform;
    private GameObject mRoleCameraGo;
    private Transform mCameraTransform;

    void Awake()
    {
        _Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void InitRoleCamera()
    {
        mRole = ObjectManager.mRole;
        if (mRole == null)
        {
            return;
        }

        mRoleTransform = mRole.mTransform;
        if (mRoleTransform == null)
        {
            return;
        }

        mCameraTransform = mRoleTransform.Find("AnimationCamera");
        if (mCameraTransform == null)
        {
            return;
        }

        mRoleCameraGo = mCameraTransform.gameObject;
        if (mRoleCameraGo == null)
        {
            return;
        }
    }

    /// <summary>
    /// 技能相机动画
    /// </summary>
    public void CameraAnimation(string animationClip)
    {
        // 隐藏主相机
        mMainCamera = GameObject.Find("Main Camera");
        if (mMainCamera == null)
        {
            return;
        }

        mMainCamera.SetActive(false);

        // 打开角色相机
        if (mRoleCameraGo == null)
        {
            return;
        }

        mRoleCameraGo.SetActive(true);

        Animator animator = mRoleCameraGo.GetComponent<Animator>();
        if (animator == null)
        {
            return;
        }

        animator.Play(animationClip, -1, 0.0f);
    }

    public void FinishCameraAnimation()
    {
        // 关闭角色相机
        if (mRoleCameraGo == null)
        {
            return;
        }

        mRoleCameraGo.SetActive(false);

        // 显示主相机
        if (mMainCamera == null)
        {
            return;
        }

        mMainCamera.SetActive(true);
    }
}