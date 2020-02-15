using UnityEngine;
using System.Collections;
using EZCameraShake;

public class RoleControllerManager : MonoBehaviour
{
    private static RoleControllerManager _Instance = null;

    public static RoleControllerManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    /// <summary>
    /// 角色模型
    /// </summary>
    private GameObject mRoleGo = null;

    /// <summary>
    /// 角色属性对象
    /// </summary>
    private CRoleObject mRoleObject = null;

    /// <summary>
    /// 主相机
    /// </summary>
    private GameObject mMainCamera = null;

    /// <summary>
    /// 相机跟随
    /// </summary>
    private CameraMove mCameraMove = null;

    /// <summary>
    /// 相机抖动
    /// </summary>
    private CameraShaker mCameraShaker = null;

    public CameraShaker CameraShakerInstance
    {
        private set { }
        get
        {
            return mCameraShaker;
        }
    }

    void Awake()
    {
        _Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void CreateRole(int roleId)
    {
        // 添加角色相关数据
        //mRoleObject = ObjectManager.mRole;

        Vector3 pos = Vector3.zero;
        if (mRoleObject != null)
        {
            pos = mRoleObject.mPosition;
            ObjectManager.DestroyObject(mRoleObject);
        }

        if (mRoleObject == null)
        {
            mRoleObject = new CRoleObject();
        }

        ObjectManager.mRole = mRoleObject;
        ObjectManager.mRole.mObjId = roleId;

        // 加载主角资源
        mRoleGo = ResourceManager.Instance.GetRolePrefab(roleId + "_Tpose");
        if (mRoleGo == null)
        {
            return;
        }

        mRoleGo = NGUITools.AddChild(gameObject, mRoleGo);
        mRoleGo.name = roleId.ToString();
        if (pos.Equals(Vector3.zero))
        {
            mRoleObject.mPosition = new Vector3(-95f, 4.78f, -84f);
        }
        
        mRoleGo.transform.localPosition = mRoleObject.mPosition;
		mRoleGo.transform.localScale = new Vector3 (1.2f,1.2f,1.2f);

        mRoleObject.mObjModel = roleId;
        mRoleObject.mGameObject = mRoleGo;
        mRoleObject.mTransform = mRoleGo.transform;

        // 创建主相机
        mMainCamera = GameObject.Find("Main Camera");
        if (mMainCamera == null)
        {
            mMainCamera = new GameObject("Main Camera");
            Camera camera = mMainCamera.AddComponent<Camera>();
        }

        // 相机跟随
        mCameraMove = mMainCamera.GetComponent<CameraMove>();
        if (mCameraMove == null)
        {
            mCameraMove = mMainCamera.AddComponent<CameraMove>();
        }
        mCameraMove.SetRoleGameObject(mRoleGo.transform.Find("Bip01").gameObject);
        
        // 添加角色移动控制器
        PlayerCtrlManager mPlayerCtrlManager = mRoleGo.GetComponent<PlayerCtrlManager>();
        if (mPlayerCtrlManager == null)
        {
            mPlayerCtrlManager = mRoleGo.AddComponent<PlayerCtrlManager>();
        }

        if (mRoleObject.mGameObject.GetComponent<AudioListener>() == null)
        {
            mRoleObject.mGameObject.AddComponent<AudioListener>();
        }

        // 添加角色逻辑组件
        mRoleObject.AddComponent<FindingPathComponent>();
        mRoleObject.AddComponent<AnimatorComponent>();
        mRoleObject.AddComponent<SkillComponent>();
        mRoleObject.AddComponent<ViewComponent>();
        mRoleObject.AddComponent<MoveComponent>();

        mRoleObject.AddComponent<RestoreComponent>();
        mRoleObject.AddComponent<InjuredComponent>();
        
		mRoleObject.AddComponent<EffectComponent>();
    }
}