using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fm_ClientNet.Interface;

public enum LoginUIState
{
    LoginUIState_Login = 1,
    LoginUIState_CreateRole = 2,
    LoginUIState_ChooseRole = 3,
}

public class RoleLoginInfo
{
    public GameObject mRole;

    public string mRoleName;
    public int mRoleLevel;
    public int mRoleSex;
    public int mRoleIndex;
    public int mRoleCreateTime;
}

public class LoginUI : MonoBehaviour
{
    private LoginUIState mLoginUIState = LoginUIState.LoginUIState_Login;

    private GameObject mLogin;
    private GameObject mCreateRole;
    private GameObject mChooseRole;

    /// <summary>
    /// 账号密码
    /// </summary>
    private UIEventListener mLoginButton;
    private UIInput mAccountInput;
    private UIInput mPasswordInput;
    private string mAccount;
    private string mPassword;

    /// <summary>
    /// 创角
    /// </summary>
    private UIInput mRoleNameInput;
    private UIEventListener mRandomNameButton;
    private UIEventListener mEnterGameButton;
    private UIGrid mCreateRoleListGrid;

    /// <summary>
    /// 选择角色
    /// </summary>
    private UIEventListener mEnterButton;
    private UIGrid mChooseRoleListGrid;

    private int mRoleIndex = 0;
    private List<RoleLoginInfo> mRoleLoginInfoList = new List<RoleLoginInfo>();

    public struct ServerInfo
    {
        public int mId;
        public string mServerName;
        public string mIp;
        public string mPort;
        public int mState;
    }

    void Awake()
    {
        mLogin = transform.Find("Login").gameObject;
        mCreateRole = transform.Find("CreateRole").gameObject;
        mChooseRole = transform.Find("ChooseRole").gameObject;

        mLoginButton = transform.Find("Login/Button").GetComponent<UIEventListener>();
        mAccountInput = transform.Find("Login/Input").GetComponent<UIInput>();
        if (!string.IsNullOrEmpty(RestoreManager.Instance.Account))
        {
            mAccountInput.value = RestoreManager.Instance.Account;
        }

        mPasswordInput = transform.Find("Login/PasswordInput").GetComponent<UIInput>();
        if (!string.IsNullOrEmpty(RestoreManager.Instance.Password))
        {
            mPasswordInput.value = RestoreManager.Instance.Password;
        }

        mRoleNameInput = transform.Find("CreateRole/Input").GetComponent<UIInput>();
        mRandomNameButton = transform.Find("CreateRole/Button").GetComponent<UIEventListener>();
        mEnterGameButton = transform.Find("CreateRole/EnterGame").GetComponent<UIEventListener>();
        mCreateRoleListGrid = transform.Find("CreateRole/Grid").GetComponent<UIGrid>();
        
        mEnterButton = transform.Find("ChooseRole/Button").GetComponent<UIEventListener>();
        mChooseRoleListGrid = transform.Find("ChooseRole/Grid").GetComponent<UIGrid>();
        
        AddEvent();
    }

    void AddEvent()
    {
        mLoginButton.onClick = LoginGame;

        mRandomNameButton.onClick = RandomName;
        mEnterGameButton.onClick = EnterGame;

        mEnterButton.onClick = EnterGame;

        List<Transform> list = mCreateRoleListGrid.GetChildList();
        for (int i = 0; i < list.Count; ++i)
        {
            list[i].GetComponent<UIEventListener>().onClick = ClickRole;
        }

        list.Clear();
        list = mChooseRoleListGrid.GetChildList();
        for (int i = 0; i < list.Count; ++i)
        {
            list[i].GetComponent<UIEventListener>().onClick = ClickRole;
        }

        list.Clear();
        list = null;
    }

    public void SwitchUI(LoginUIState state)
    {
        mLoginUIState = state;

        switch (mLoginUIState)
        {
            case LoginUIState.LoginUIState_Login:
                mLogin.SetActive(true);
                mCreateRole.SetActive(false);
                mChooseRole.SetActive(false);
                break;

            case LoginUIState.LoginUIState_CreateRole:
                mLogin.SetActive(false);
                mCreateRole.SetActive(true);
                mChooseRole.SetActive(false);
                RandomName(null);
                break;

            case LoginUIState.LoginUIState_ChooseRole:
                UpdateChooseRoleInfo();

                mLogin.SetActive(false);
                mCreateRole.SetActive(false);
                mChooseRole.SetActive(true);
                break;

            default:
                break;
        }
    }

    void UpdateChooseRoleInfo()
    {
        IGameReceiver receive = Game.Instance.mGameRecv;
        if (receive == null)
        {
            return;
        }

        int roleCount = receive.GetRoleCount();
        if (roleCount < 1)
        {
            return;
        }

        RoleLoginInfo info = new RoleLoginInfo();
        for (int i = 0; i < roleCount; ++i)
        {
            int isDelete = receive.GetRoleDeleted(i);
            if (isDelete != 0)
            {
                continue;
            }

            info.mRoleIndex = receive.GetRoleIndex(i);
            info.mRoleName = receive.GetRoleName(i);

            string roleParam = receive.GetRolePara(i);
            string[] roleParamList = roleParam.Split(';');
            for (int j = 0; j < roleParamList.Length; ++j)
            {
                string[] param = roleParamList[j].Split(',');
                if (param.Length > 0)
                {
                    if (param[0] == "Level")
                    {
                        info.mRoleLevel = int.Parse(param[1]);
                    }
                    if (param[0] == "RoleCreateTime")
                    {
                        info.mRoleCreateTime = int.Parse(param[1]);
                    }
                }
            }

            mRoleLoginInfoList.Add(info);
        }

        if (mRoleLoginInfoList.Count > 1)
        {
            mRoleLoginInfoList.Sort(RoleLoginInfoSort);
        }

        ShowRoleLoginInfo();
    }

    int RoleLoginInfoSort(object l, object r)
    {
        RoleLoginInfo ll = (RoleLoginInfo)l;
        RoleLoginInfo rr = (RoleLoginInfo)r;

        if (ll.mRoleCreateTime != 0 && rr.mRoleCreateTime != 0)
        {
            if (ll.mRoleCreateTime > rr.mRoleCreateTime)
            {
                return 1;
            }
            else if (ll.mRoleCreateTime < rr.mRoleCreateTime)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return -1;
        }
    }

    void ShowRoleLoginInfo()
    {
        if (mRoleLoginInfoList.Count < 1)
        {
            return;
        }

        List<Transform> list = mChooseRoleListGrid.GetChildList();
        if (list == null)
        {
            return;
        }

        int count = list.Count;
        if (count < 1)
        {
            return;
        }

        for (int i = 0; i < count; ++i)
        {
            Transform trans = list[i];
            if (trans == null)
            {
                return;
            }

            if (i < mRoleLoginInfoList.Count)
            {
                RoleLoginInfo info = mRoleLoginInfoList[i];
                trans.Find("Name").GetComponent<UILabel>().text = info.mRoleName;
            }
            else
            {
                trans.gameObject.SetActive(false);
            }
        }
    }

    void LoginGame(GameObject go)
    {
        mAccount = mAccountInput.value;
        mPassword = mPasswordInput.value;

        if (string.IsNullOrEmpty(mAccount) ||
            string.IsNullOrEmpty(mPassword))
        {
            return;
        }
        
        if (ConnectStage.ConnecToMember("127.0.0.1", 2001, mAccount, mPassword))
        {
            RestoreManager.Instance.Account = mAccount;
            RestoreManager.Instance.Password = mPassword;
        }
        else
        {
            PromptManager.Instance.ShowPromptUI(XmlManager.Instance.GetCommonText("System0001"));
        }
    }

    void RandomName(GameObject go)
    {
        mRoleNameInput.value = "氪计划" + Random.Range(Random.Range(1, 200), Random.Range(300, 800));
    }

    void EnterGame(GameObject go)
    {
        string roleName = "";
        SysUtils.VarList args = new SysUtils.VarList();

        switch (mLoginUIState)
        {
            case LoginUIState.LoginUIState_CreateRole:
                {
                    roleName = mRoleNameInput.value;
                    if (string.IsNullOrEmpty(roleName))
                    {
                        return;
                    }

                    args.AddInt(mRoleIndex);
                    args.AddWideStr(roleName);
                    args.AddInt(1);
                    args.AddInt(1);
                    args.AddInt(1);

                    if (Game.Instance.mGameSender.CreateRole(ref args))
                    {
                        // 创建成功后，保存最后登录的角色位置与名字
                        RestoreManager.Instance.LastRoleName = roleName;
                        RestoreManager.Instance.LastRoleIndex = mRoleIndex;
                    }
                    else
                    {
                        PromptManager.Instance.ShowPromptUI(XmlManager.Instance.GetCommonText("System0002"));
                    }
                }
                break;

            case LoginUIState.LoginUIState_ChooseRole:
                {
                    if (!Game.Instance.mGameSender.ChooseRole(RestoreManager.Instance.LastRoleName))
                    {
                        PromptManager.Instance.ShowPromptUI(XmlManager.Instance.GetCommonText("System0003"));
                    }
                    else
                    {
                        UIManager.Instance.CloseUI("LoginUI");
                    }
                }
                break;

            default:
                break;
        }
    }

    void ClickRole(GameObject go)
    {
        mRoleIndex = int.Parse(go.name);

        Transform trans = go.transform.Find("Name");
        if (trans != null)
        {
            UILabel name = trans.GetComponent<UILabel>();
            if (name != null)
            {
                RestoreManager.Instance.LastRoleName = name.text;
            }
        }
    }

    void OnDestroy()
    {
        mLogin = null;
        mCreateRole = null;
        mChooseRole = null;

        mLoginButton = null;
        mAccountInput = null;
        mPasswordInput = null;

        mRoleNameInput = null;
        mRandomNameButton = null;
        mEnterGameButton = null;
        mCreateRoleListGrid = null;

        mEnterButton = null;
        mChooseRoleListGrid = null;

        for (int i = 0; i < mRoleLoginInfoList.Count; ++i)
        {
            mRoleLoginInfoList[i] = null;
        }
        mRoleLoginInfoList.Clear();
        mRoleLoginInfoList = null;
    }
}