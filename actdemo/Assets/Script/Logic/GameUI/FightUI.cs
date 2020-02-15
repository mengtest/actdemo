using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightUI : MonoBehaviour
{
    /// <summary>
    /// 主角翻滚按钮
    /// </summary>
    private UIEventListener mRollButton;

    /// <summary>
    /// 角色1切换
    /// </summary>
    private GameObject mChangeRoleF;

    /// <summary>
    /// 角色2切换
    /// </summary>
    private GameObject mChangeRoleS;

    private UISprite sprite;
    private UISprite sprite1;

    /// <summary>
    /// 主角技能组件
    /// </summary>
    private SkillComponent mSkillCom;

    /// <summary>
    /// 主角动画组件
    /// </summary>
	private AnimatorComponent mAnimator;

    /// <summary>
    /// 连击界面
    /// </summary>
    private GameObject mCombo;
    private GameObject mComboBackground;
    private GameObject mComboGrid;
    private GameObject mComboNumber;

    private Vector3 mPosition;
    private TweenPosition mTweenBackground;
    private int[] indexarr = { 0, 1, 2, 3, 4, 5 };

    public class SkillUIIndex
    {
        public int mIndex;
        public bool mCanUse;
        public GameObject mSkillIcon;
        public UISprite mCDSprite;
    }

    /// <summary>
    /// 技能对应关系
    /// </summary>
    private Dictionary<int, SkillUIIndex> mSkillIndexDic = new Dictionary<int, SkillUIIndex>();

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        mSkillCom = ObjectManager.mRole.GetComponent<SkillComponent>();
        if (mSkillCom == null)
        {
            return;
        }

		mAnimator = ObjectManager.mRole.GetComponent<AnimatorComponent>();
        if (mAnimator == null)
        {
            return;
        }

        int count = mSkillCom.SkillDic.Count;
        if (count < 1)
        {
            return;
        }

        mSkillIndexDic.Clear();

        foreach (KeyValuePair<int, Skill> kvp in mSkillCom.SkillDic)
        {
            mSkillIndexDic.Add(kvp.Key, new SkillUIIndex());
        }
        
        int index = 0;
        foreach (KeyValuePair<int, SkillUIIndex> kvp in mSkillIndexDic)
        {
            kvp.Value.mIndex = index;
            kvp.Value.mCanUse = true;
            
            kvp.Value.mSkillIcon = transform.Find("BottomRight/Skill_" + indexarr[index].ToString()).gameObject;
            indexarr[index] = kvp.Key;
            kvp.Value.mSkillIcon.GetComponent<UIEventListener>().onClick = OnClickSkill;
            kvp.Value.mSkillIcon.GetComponent<UIEventListener>().onPress = OnPressSkill;
            kvp.Value.mSkillIcon.name = "Skill_" + kvp.Key;
            kvp.Value.mSkillIcon.SetActive(true);
            
            kvp.Value.mCDSprite = kvp.Value.mSkillIcon.transform.Find("CDSprite").GetComponent<UISprite>();

            ++index;
        }
        
        mRollButton = transform.Find("BottomRight/Roll").GetComponent<UIEventListener>();
        mRollButton.onClick += OnClickRoll;

        mChangeRoleF = transform.Find("touxiang/Button1").gameObject;
        mChangeRoleS = transform.Find("touxiang/Button2").gameObject;

        mChangeRoleF.GetComponent<UIEventListener>().onClick += OnClickChangeRole;
        sprite1 = mChangeRoleF.transform.Find("Sprite").GetComponent<UISprite>();
        sprite1.atlas = ResourceManager.Instance.GetAtlas("Fight");

        sprite = transform.Find("touxiang/touxiang").GetComponent<UISprite>();
        sprite.atlas = ResourceManager.Instance.GetAtlas("Fight");

        mCombo = transform.Find("TopRight").gameObject;
        mComboBackground = transform.Find("TopRight/Background").gameObject;
        mComboGrid = transform.Find("TopRight/Grid").gameObject;
        mPosition = mComboGrid.transform.localPosition;
        mComboNumber = transform.Find("TopRight/temp").gameObject;
    }

    /// <summary>
    /// 设置combo数字
    /// </summary>
    /// <param name="num"></param>
    public void SetComboNumber(int num)
    {
        if (num < 1)
        {
            return;
        }

        mCombo.SetActive(true);

        if (mTweenBackground != null)
        {
            mTweenBackground.ResetToBeginning();
        }
        
        mComboBackground.transform.localPosition = Vector3.zero;
        mComboBackground.SetActive(true);

        char[] strNum = num.ToString().ToCharArray();
        for (int i = 0; i < strNum.Length; ++i)
        {
            if (mComboGrid.transform.childCount < i + 1)
            {
                NGUITools.AddChild(mComboGrid, mComboNumber);
            }

            Transform trans = mComboGrid.transform.GetChild(i);
            if (trans == null)
            {
                trans = NGUITools.AddChild(mComboGrid, mComboNumber).transform;
            }

            if (trans == null)
            {
                continue;
            }

            trans.name = strNum[i].ToString();
            trans.GetComponent<UISprite>().spriteName = "combo_" + strNum[i].ToString();
            trans.gameObject.SetActive(true);
        }

        mComboGrid.GetComponent<UIGrid>().Reposition();

        TweenPosition tween = TweenPosition.Begin(mComboGrid, 0.01f, mComboGrid.transform.localPosition + new Vector3(0.0f, 20.0f, 0.0f));
        EventDelegate.Add(tween.onFinished, ChangePosition);

        // 重置Combo时间
        StopCoroutine("ComboDisappear");
        StartCoroutine("ComboDisappear");
    }

    void ChangePosition()
    {
        GameObject go = TweenPosition.current.gameObject;
        if (go == null)
        {
            return;
        }

        TweenPosition.Begin(go, 0.01f, mPosition);
    }

    IEnumerator ComboDisappear()
    {
        yield return new WaitForSeconds(2.0f);

        ObjectManager.mRole.ClearComboCount();

        SetComboInvisible();
    }

    public void SetComboInvisible()
    {
        UtilTools.DestroyChild(mComboGrid);

        mTweenBackground = TweenPosition.Begin(mComboBackground, 0.2f, new Vector3(350.0f, 0.0f, 0.0f));
        EventDelegate.Add(mTweenBackground.onFinished, ComboMoveOut);
    }

    void ComboMoveOut()
    {
        mCombo.SetActive(false);
    }

    /// <summary>
    /// 点击技能
    /// </summary>
    /// <param name="go"></param>
    void OnClickSkill(GameObject go)
    {
        int skillId = int.Parse(go.name.Split('_')[1]);
        if (mSkillCom == null)
        {
            return;
        }

        mSkillCom.DoSkill(skillId);
    }

    /// <summary>
    /// 按住技能
    /// </summary>
    /// <param name="go"></param>
    void OnPressSkill(GameObject go, bool state)
    {
        int skillId = int.Parse(go.name.Split('_')[1]);
        if (mSkillCom == null)
        {
            return;
        }

        if (state)
        {
            //mSkillCom.DoSkill(skillId);
            //TimerManager.AddTimerRepeat(skillId.ToString(), 0.1f, PressSkill, skillId);
        }
        else
        {
            //TimerManager.Destroy(skillId.ToString());
        }
    }

    public void PressSkill(params object[] args)
    {
        mSkillCom.DoSkill((int)(args[0]));
    }

    /// <summary>
    /// 翻滚
    /// </summary>
    /// <param name="go"></param>
    void OnClickRoll(GameObject go) 
    {
        mAnimator.RollState();
    }

    /// <summary>
    /// 切换角色
    /// </summary>
    /// <param name="go"></param>
    void OnClickChangeRole(GameObject go)
    {
        if (sprite.spriteName == "nan")
        {
            RoleControllerManager.Instance.CreateRole(100008);
            
            sprite1.spriteName = "nan";
            sprite.spriteName = "zhaoyun";
        }
        else
        {
            RoleControllerManager.Instance.CreateRole(140008);
            sprite1.spriteName = "zhaoyun";
            sprite.spriteName = "nan";
        }


        mSkillCom = ObjectManager.mRole.GetComponent<SkillComponent>();
        if (mSkillCom == null)
        {
            return;
        }

        mAnimator = ObjectManager.mRole.GetComponent<AnimatorComponent>();
        if (mAnimator == null)
        {
            return;
        }

        int count = mSkillCom.SkillDic.Count;
        if (count < 1)
        {
            return;
        }

        mSkillIndexDic.Clear();

        foreach (KeyValuePair<int, Skill> kvp in mSkillCom.SkillDic)
        {
            mSkillIndexDic.Add(kvp.Key, new SkillUIIndex());
        }

        int index = 0;
        foreach (KeyValuePair<int, SkillUIIndex> kvp in mSkillIndexDic)
        {
            kvp.Value.mIndex = index;
            kvp.Value.mCanUse = true;

            kvp.Value.mSkillIcon = transform.Find("BottomRight/Skill_" + indexarr[index].ToString()).gameObject;
            indexarr[index] = kvp.Key;
            //kvp.Value.mSkillIcon.GetComponent<UIEventListener>().onClick = OnClickSkill;
            //kvp.Value.mSkillIcon.GetComponent<UIEventListener>().onPress = OnPressSkill;
            kvp.Value.mSkillIcon.name = "Skill_" + kvp.Key;
            kvp.Value.mSkillIcon.SetActive(true);

            kvp.Value.mCDSprite = kvp.Value.mSkillIcon.transform.Find("CDSprite").GetComponent<UISprite>();

            ++index;
        }
    }

    /// <summary>
    /// 设置技能CD
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="cdTime"></param>
    public void SetSkillCD(int skillId, float cdTime)
    {
        if (!mSkillIndexDic.ContainsKey(skillId))
        {
            return;
        }

        TimerManager.AddTimerCount(skillId.ToString(), cdTime, SkillCountDown, skillId, cdTime);
    }

    void SkillCountDown(int count, params object[] args)
    {
        int skillId = (int)args[0];
        float cdTime = (float)args[1];

        float value = count / cdTime;

        if (!mSkillIndexDic.ContainsKey(skillId))
        {
            TimerManager.Destroy(skillId.ToString());

            return;
        }

        mSkillIndexDic[skillId].mCDSprite.fillAmount = value;
    }
}