using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SkillEffectType
{
    SkillEffectType_Skill = 1,
    SkillEffectType_BeSkilled = 2,
}

public class EffectManager : MonoBehaviour
{
    private static EffectManager _Instance = null;
    public static EffectManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    private Dictionary<int, EffectProperty> mEffectPropertyDic = new Dictionary<int, EffectProperty>();
    private int mEffectPropertyDicCount = 0;

    private Dictionary<int, string> oldIntensityMap = new Dictionary<int, string>();

    void Awake()
    {
        _Instance = this;

        mEffectPropertyDic = XmlManager.Instance.GetEffectProperty();
        if (mEffectPropertyDic == null)
        {
            LogSystem.LogError("mEffectPropertyDic is null");
        }

        mEffectPropertyDicCount = mEffectPropertyDic.Count;
    }

    List<EffectProperty> GetEpByAnimator(Animator animator, AnimatorStateInfo stateInfo)
    {
        EffectProperty ep = null;
        List<EffectProperty> effectPropertyList = new List<EffectProperty>();
        List<int> list = new List<int>(mEffectPropertyDic.Keys);

        for (int i = 0; i < mEffectPropertyDicCount; ++i)
        {
            if (animator.GetComponent<AnimatorComponent>().mObj.mObjId != mEffectPropertyDic[list[i]].mEffectModel)
            {
                continue;
            }

            if (stateInfo.IsName(mEffectPropertyDic[list[i]].mEffectAction))
            {
                ep = mEffectPropertyDic[list[i]];

                effectPropertyList.Add(ep);
            }
        }

        return effectPropertyList;
    }
    
    Vector3 GetDirection(EffectDirectionType type, GameObject go)
    {
        Vector3 vec = Vector3.zero;
        switch (type)
        {
            case EffectDirectionType.EffectDirectionType_RoleForward:
                vec = go.transform.forward;
                break;

            case EffectDirectionType.EffectDirectionType_RoleBack:
                vec = go.transform.forward * -1.0f;
                break;

            case EffectDirectionType.EffectDirectionType_RoleLeft:
                vec = go.transform.right * -1.0f;
                break;

            case EffectDirectionType.EffectDirectionType_RoleRight:
                vec = go.transform.right;
                break;

            case EffectDirectionType.EffectDirectionType_RoleUp:
                vec = go.transform.up;
                break;

            default:
                break;
        }

        return vec;
    }

    /// <summary>
    /// 技能释放特效
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="index"></param>
    /// <param name="animator"></param>
    public void SkillEffect(Animator animator, AnimatorStateInfo stateInfo)
    {
        if (animator == null || mEffectPropertyDicCount < 1)
        {
            return;
        }

        List<EffectProperty> ep = GetEpByAnimator(animator, stateInfo);
        if (ep == null)
        {
            return;
        }

        for (int i = 0; i < ep.Count; ++i)
        {
            GameObject go = ResourceManager.Instance.GetEffect(ep[i].mEffectPath + "/" + ep[i].mEffectFile);
            if (go == null)
            {
                continue;
            }

            GameObject parent = ep[i].mEffectFollowAction ? animator.gameObject : gameObject;
            //go = NGUITools.AddChild(parent, go);
            //if (go == null)
            //{
            //    continue;
            //}

            // 如果不跟随，需要玩家当前位置作为特效播放位置
            if (!ep[i].mEffectFollowAction)
            {
                go.transform.position = animator.gameObject.transform.position +
                    GetDirection(ep[i].mEffectDirectionType, animator.gameObject) * ep[i].mEffectDirectionDistance;
            }

            if (ep[i].mEffectFollowRole)
            {
                go.transform.forward = animator.transform.forward;
            }

            go.name = ep[i].mEffectFile;
            go.SetActive(true);
        }
    }

    public void RemoveSkillEffect(string skillRes, Animator animator)
    {
        if (animator == null)
        {
            return;
        }

        Transform trans = animator.gameObject.transform.Find(skillRes);
        if (trans == null)
        {
            return;
        }

        Destroy(trans.gameObject);
    }

    /// <summary>
    /// 显示技能受击者特效
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="skillReceiver"></param>
    public void SkillHitEffect(int effectId, GameObject skillReceiver)
    {
        if (effectId < 1 || skillReceiver == null)
        {
            return;
        }

        EffectProperty ep = XmlManager.Instance.GetEffectProperty(effectId);
        if (ep == null)
        {
            return;
        }

        string path = ep.mEffectPath + "/" + ep.mEffectFile;

        GameObject go = ResourceManager.Instance.GetEffect(path);
        if (go == null)
        {
            return;
        }

        GameObject parent = skillReceiver;
        if (ep.mEffectFollowBip)
        {
            parent = parent.transform.Find("Bip01").gameObject;
        }

        //go = NGUITools.AddChild(parent, go);
        //if (go == null)
        //{
        //    return;
        //}

        //go.SetActive(true);
    }

    public void SkillHitHighLightEffect(GameObject go)
    {
        SkinnedMeshRenderer[] renderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            SkinnedMeshRenderer renderer = renderers[i];
            if (renderer != null && renderer.sharedMaterial != null)
            {
                if (!oldIntensityMap.ContainsKey(renderer.GetInstanceID()))
                {
                    oldIntensityMap.Add(renderer.GetInstanceID(), renderer.material.shader.name);
                }

                renderer.material.shader = Shader.Find("SnailHitShine");
            }
        }
    }

    public void RemoveSkillHitHighLightEffect(GameObject go)
    {
        SkinnedMeshRenderer[] renderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            SkinnedMeshRenderer renderer = renderers[i];
            if (renderer != null && renderer.sharedMaterial != null)
            {
                if (oldIntensityMap.ContainsKey(renderer.GetInstanceID()))
                {
                    renderer.material.shader = Shader.Find(oldIntensityMap[renderer.GetInstanceID()]);
                    oldIntensityMap.Remove(renderer.GetInstanceID());
                }
            }
        }
    }

    public void CreateFlyingEffect(IObject self, SkillActionProperty skillActionProperty, IObject target)
    {
        int effectId = UtilTools.IntParse(skillActionProperty.mSkillFlyRes);
        EffectProperty effectProperty = XmlManager.Instance.GetEffectProperty(effectId);
        if (effectProperty == null)
        {
            return;
        }

        if (target == null || target.mGameObject == null) return;

        //GameObject go = ResourceManager.Instance.GetEffect(effectProperty.mEffectPath);
        GameObject go = ResourceManager.Instance.GetFlySkillByFileName(effectProperty.mEffectFile);
        go.transform.SetParent(self.mGameObject.transform.parent);
        go.transform.position = self.mPosition + new Vector3(0, 1.5f, 0);
        if (go == null)
        {
            return;
        }

        Hashtable table = new Hashtable();

        // 飞行事件
        table.Add("speed", 15.0f);

        // 目前点
        Vector3 targetPosition = target.mGameObject.transform.position;
        Vector3 attPosition = new Vector3(targetPosition.x, targetPosition.y + 1.5f, targetPosition.z);

        // 箭的朝向
        go.transform.forward = (attPosition - go.transform.position).normalized;

        // 飞行技能参数
        FlySkillParams param = new FlySkillParams();
        param.self = go;
        param.target = target.mGameObject;
        param.property = skillActionProperty;

        // 目标位置
        table.Add("position", attPosition);
        //table.Add("looktarget", attPosition);

        // 飞行过程中回调，在目标受击模块实现
        table.Add("onupdate", "OniTweenUpdate");
        table.Add("onupdatetarget", target.mGameObject);
        table.Add("onupdateparams", param);
        

        // 飞行结束回调，在主角技能模块实现
        table.Add("oncomplete", "OniTweenComplete");
        table.Add("oncompletetarget", self.mGameObject);
        table.Add("oncompleteparams", param);

        // 运动轨迹类型
        table.Add("easetype", iTween.EaseType.linear);

        // go~嗨起来~
        iTween.MoveTo(go, table);
    }
}