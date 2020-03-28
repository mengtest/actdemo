using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HUDNumberType
{
    HUDNumberType_Normal    = 1,
    HUDNumberType_Crit      = 2,
    HUDNumberType_Lost      = 3,
    HUDNumberType_Restore   = 4,
}

public class HUDUI : MonoBehaviour
{
    ///// <summary>
    ///// 数字模板
    ///// </summary>
    //private GameObject mNumberGo;
    ////private UIGrid mNumberGrid;
    //private GameObject mNumberTemp;

    ///// <summary>
    ///// 文字模板
    ///// </summary>
    //private GameObject mText;

    ///// <summary>
    ///// 飘字模板
    ///// </summary>
    //private GameObject mFloatText;

    ///// <summary>
    ///// 飘图模板
    ///// </summary>
    //private GameObject mFloatSprite;

    ///// <summary>
    ///// 血量等模板
    ///// </summary>
    //private GameObject mBar;

    //private List<GameObject> mNumberList = new List<GameObject>();

    //void Awake()
    //{
    //    mNumberGo = transform.Find("HUDNumber").gameObject;
    //    mNumberGrid = transform.Find("HUDNumber/Grid").GetComponent<UIGrid>();
    //    mNumberTemp = transform.Find("HUDNumber/temp").gameObject;

    //    mText = transform.Find("HUDText").gameObject;
    //    mFloatText = transform.Find("HUDFloatText").gameObject;
    //    mFloatSprite = transform.Find("HUDFloatText").gameObject;
    //    mBar = transform.Find("HUDBar").gameObject;
    //}

    //GameObject CreateGrid()
    //{
    //    GameObject go = NGUITools.AddChild(transform.gameObject, mNumberGo);
    //    if (go == null)
    //    {
    //        return null;
    //    }

    //    return go;
    //}

    //GameObject CreateText()
    //{
    //    GameObject go = NGUITools.AddChild(transform.gameObject, mText);
    //    if (go == null)
    //    {
    //        return null;
    //    }

    //    return go;
    //}

    //GameObject CreateFloatText()
    //{
    //    GameObject go = NGUITools.AddChild(transform.gameObject, mFloatText);
    //    if (go == null)
    //    {
    //        return null;
    //    }

    //    return go;
    //}

    //GameObject CreateFloatSprite()
    //{
    //    GameObject go = NGUITools.AddChild(transform.gameObject, mFloatSprite);
    //    if (go == null)
    //    {
    //        return null;
    //    }

    //    return go;
    //}

    //GameObject CreateBar()
    //{
    //    GameObject go = NGUITools.AddChild(transform.gameObject, mBar);
    //    if (go == null)
    //    {
    //        return null;
    //    }

    //    return go;
    //}

    ///// <summary>
    ///// 显示飘字
    ///// </summary>
    ///// <param name="num"></param>
    ///// <param name="obj"></param>
    //public void HudNumber(long num, IObject obj, HUDNumberType type = HUDNumberType.HUDNumberType_Normal)
    //{
    //    if (num < 0)
    //    {
    //        num = -num;
    //    }
    //    else if (num == 0)
    //    {
    //        return;
    //    }

    //    Vector3 pos = UtilTools.SwitchVector(obj.mPosition + new Vector3(0.0f, 3.0f, 0.0f));

    //    // 创建飘字对象
    //    GameObject go = CreateGrid();
    //    if (go == null)
    //    {
    //        return;
    //    }

    //    UISprite typeSprite = go.transform.Find("Type").GetComponent<UISprite>();
    //    if (typeSprite == null)
    //    {
    //        return;
    //    }

    //    string prefix = string.Empty;
    //    // 设置不同类型标记和飘字图集
    //    switch (type)
    //    {
    //        case HUDNumberType.HUDNumberType_Normal:
    //            prefix = "attack_";
    //            break;

    //        case HUDNumberType.HUDNumberType_Crit:
    //            prefix = "crirical_";
    //            typeSprite.spriteName = "";
    //            typeSprite.gameObject.SetActive(true);
    //            break;

    //        case HUDNumberType.HUDNumberType_Lost:
    //            prefix = "hit_";
    //            break;

    //        case HUDNumberType.HUDNumberType_Restore:
    //            prefix = "4_";
    //            typeSprite.spriteName = "";
    //            typeSprite.gameObject.SetActive(true);
    //            break;

    //        default:
    //            break;
    //    }

    //    UIGrid grid = go.transform.Find("Grid").GetComponent<UIGrid>();

    //    // 给飘字赋值
    //    string strNum = num.ToString();
    //    for (int i = 0; i < strNum.Length; ++i)
    //    {
    //        GameObject temp = NGUITools.AddChild(grid.gameObject, mNumberTemp);
    //        if (temp == null)
    //        {
    //            continue;
    //        }

    //        UISprite sprite = temp.GetComponent<UISprite>();
    //        if (sprite == null)
    //        {
    //            continue;
    //        }
            
    //        sprite.spriteName = prefix + strNum[i].ToString();

    //        temp.SetActive(true);
    //    }

    //    grid.Reposition();

    //    // 设置飘字跟随对象
    //    pos.z = 0.0f;
    //    go.transform.localPosition = pos;
    //    go.SetActive(true);

    //    TweenScale tween = TweenScale.Begin(grid.gameObject, 0.35f, Vector3.one * 1.3f);
    //    EventDelegate.Add(tween.onFinished, Disappear);
    //}

    //void Disappear()
    //{
    //    GameObject go = TweenScale.current.transform.parent.gameObject;
    //    if (go == null)
    //    {
    //        return;
    //    }

    //    Destroy(go);
    //    go = null;
    //}

    ///// <summary>
    ///// 固定文字
    ///// </summary>
    ///// <param name="str"></param>
    //public void Text(IObject obj, string str)
    //{
    //    if (obj == null || string.IsNullOrEmpty(str))
    //    {
    //        return;
    //    }

    //    GameObject go = CreateText();
    //    if (go == null)
    //    {
    //        return;
    //    }

    //    go.transform.Find("Label").GetComponent<UILabel>().text = str;

    //    go.AddComponent<UIFollowTarget>().target = obj.mGameObject.transform;
    //}

    ///// <summary>
    ///// 飘字
    ///// </summary>
    ///// <param name="str"></param>
    //public void FloatText(IObject obj, string str)
    //{
    //    if (obj == null || string.IsNullOrEmpty(str))
    //    {
    //        return;
    //    }
    //}

    ///// <summary>
    ///// 飘图
    ///// </summary>
    //public void FloatSprite(IObject obj)
    //{
    //    if (obj == null)
    //    {
    //        return;
    //    }
    //}

    //public void Bar(IObject obj)
    //{
    //    if (obj == null)
    //    {
    //        return;
    //    }

    //    GameObject go = CreateBar();
    //    if (go == null)
    //    {
    //        return;
    //    }

    //    go.transform.localScale = Vector3.one * 0.5f;
    //    go.AddComponent<UIFollowTarget>().target = obj.mGameObject.transform;

    //    go.SetActive(true);
    //}

    //void Update()
    //{
    //    if (mNumberList.Count > 0)
    //    {
    //        for (int i = 0; i < mNumberList.Count; ++i)
    //        {
    //            GameObject go = mNumberList[i];
    //            if (go == null)
    //            {
    //                continue;
    //            }

    //            Vector3 dir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 1.0f);
    //            go.transform.position += dir * Time.deltaTime * 0.3f;
    //            Transform grid = go.transform.Find("Grid");
    //            int count = grid.childCount;
    //            for (int j = 0; j < count; ++j)
    //            {
    //                GameObject child = grid.GetChild(j).gameObject;
    //                if (child.activeSelf)
    //                {
    //                    child.GetComponent<UISprite>().alpha += Time.deltaTime;
    //                    if (child.GetComponent<UISprite>().alpha >= 0.8f)
    //                    {
    //                        grid.parent.gameObject.SetActive(false);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    for (int i = 0; i < mNumberList.Count; ++i)
    //    {
    //        GameObject go = mNumberList[i];
    //        if (go == null)
    //        {
    //            continue;
    //        }

    //        if (!go.activeSelf)
    //        {
    //            mNumberList.Remove(go);
    //            Destroy(go);
    //            go = null;
    //        }
    //    }
    //}
}