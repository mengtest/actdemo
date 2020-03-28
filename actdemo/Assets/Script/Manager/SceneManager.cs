using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    private static SceneManager _Instance = null;
    private SceneManager () { }

    public static SceneManager Instance
    {
        private set { }
        get
        {
            return _Instance;
        }
    }

    private AsyncOperation mAsyncOper;

    private ULoadingUI mLoadingUI;

    private string mCurrentSceneName = "";
    public string CurrentSceneName
    {
        private set { }
        get { return mCurrentSceneName; }
    }

    private float mLoadSceneProgress = 0.0f;
    private float mCreateRoleProgress = 0.0f;

    void Awake()
    {
        _Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return;
        }
        UIManager.Instance.CloseAllUI();
        UIManager.Instance.OpenUI("ULoadingUI");
        mLoadingUI = GameObject.Find("ULoadingUI").GetComponent<ULoadingUI>();

        StartCoroutine(LoadScene(sceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        mAsyncOper = Application.LoadLevelAsync(sceneName);

        yield return mAsyncOper;

        //CreateScene(sceneName);

//        AudioManager.Instance.PlayBgMusic("9");

        RoleControllerManager.Instance.CreateRole(100008);

        FightManager.Instance.InitFight(1);
    }

    void CreateScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return;
        }

        GameObject sceneGo = ResourceManager.Instance.GetScenePrefab(sceneName);
        if (sceneGo == null)
        {
            return;
        }

        //sceneGo = NGUITools.AddChild(this.gameObject, sceneGo);
        //if (sceneGo == null)
        //{
        //    return;
        //}

        //sceneGo.name = sceneName;
        //sceneGo.transform.position = Vector3.zero;

        mCurrentSceneName = sceneName;

        mLoadSceneProgress = 1.0f;
    }

    void Update()
    {
        if (mAsyncOper == null)
        {
            return;
        }

        if (mAsyncOper.isDone)
        {
            UIManager.Instance.CloseUI("ULoadingUI");

            mLoadingUI = null;
            mAsyncOper = null;

            return;
        }

        mLoadingUI.ChangeProgress(mAsyncOper.progress);
    }
}