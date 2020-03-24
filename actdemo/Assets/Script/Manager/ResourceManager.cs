using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 资源类型(与服务端发送的资源类型统一)
/// </summary>
public enum ResourceType
{
    ResourceType_UI         = 1,
    ResourceType_Role       = 2,
    ResourceType_Npc        = 3,
    ResourceType_Monster    = 4,
    ResourceType_Audio      = 5,
    ResourceType_Xml        = 6,
    ResourceType_Effect     = 7,
    ResourceType_Max,
}

public class ResourceManager
{
    private static ResourceManager _Instance = null;
    private ResourceManager() { }

    public static ResourceManager Instance
    {
        private set { }
        get
        {
            if (_Instance == null)
            {
                _Instance = new ResourceManager();
            }

            return _Instance;
        }
    }

    /// <summary>
    /// 资源缓存
    /// </summary>
    private Dictionary<string, Object> mResourceCacheDic = new Dictionary<string, Object>();

    public GameObject GetPrefabFromServer(ResourceType type, string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        GameObject go = null;
        switch (type)
        {
            case ResourceType.ResourceType_UI:
                go = GetUIPrefab(id);
                break;

            case ResourceType.ResourceType_Role:
                go = GetRolePrefab(id);
                break;

            case ResourceType.ResourceType_Npc:
                go = GetNpcPrefab(id);
                break;

            case ResourceType.ResourceType_Monster:
                go = GetMonsterPrefab(id);
                break;

            case ResourceType.ResourceType_Audio:
                //go = GetSound(id);
                break;

            case ResourceType.ResourceType_Xml:
                //go = GetXml(id);
                break;

            case ResourceType.ResourceType_Effect:
                go = GetEffect(id);
                break;

            default:
                break;
        }

        return go;
    }

    /// <summary>
    /// 获取UI预设
    /// </summary>
    /// <param name="uiName"></param>
    /// <returns></returns>
    public GameObject GetUIPrefab(string uiName)
    {
        GameObject go = null;

        if (string.IsNullOrEmpty(uiName))
        {
            return go;
        }

        return GameObject.Instantiate(LoadObject(uiName, "UI/Prefab/")) as GameObject;
    }

    /// <summary>
    /// 获取Xml
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public byte[] GetXml(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        TextAsset ta = LoadObject(fileName, "ConfigBytes/") as TextAsset;
        if (ta == null)
        {
            return null;
        }

        return ta.bytes;
    }

    public GameObject GetRolePrefab(string roleId)
    {
        if (string.IsNullOrEmpty(roleId))
        {
            return null;
        }

        return LoadObject(roleId, ResourceDefine.RolePath) as GameObject;
    }

    public GameObject GetNpcPrefab(string npcId)
    {
        if (string.IsNullOrEmpty(npcId))
        {
            return null;
        }

        return LoadObject(npcId, ResourceDefine.NPCPath) as GameObject;
    }

    /// <summary>
    /// 加载Monster预设
    /// </summary>
    /// <param name="monsterId"></param>
    /// <returns></returns>
    public GameObject GetMonsterPrefab(string monsterId)
    {
        if (string.IsNullOrEmpty(monsterId))
        {
            return null;
        }

        if (mResourceCacheDic.ContainsKey(monsterId))
        {
            return GameObject.Instantiate(mResourceCacheDic[monsterId]) as GameObject;
        }

        Object obj = LoadObject(monsterId, ResourceDefine.MonsterPath);
        if (obj == null)
        {
            return null;
        }

        mResourceCacheDic.Add(monsterId, obj);

        return GameObject.Instantiate(obj) as GameObject;
    }

    /// <summary>
    /// 清除怪物预设缓存
    /// </summary>
    public void ClearCacheObject()
    {
        if (mResourceCacheDic == null || mResourceCacheDic.Count < 1)
        {
            return;
        }

        foreach (KeyValuePair<string, Object> temp in mResourceCacheDic)
        {
            if (temp.Value == null)
            {
                continue;
            }

            Object.Destroy(temp.Value);
        }

        mResourceCacheDic.Clear();
    }

    public GameObject GetScenePrefab(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return null;
        }

        return LoadObject(sceneName, "Scenes/") as GameObject;
    }

    public GameObject GetHUDTextPrefab()
    {
        return LoadObject("", ResourceDefine.HUDTextPath) as GameObject;
    }

    public GameObject GetHUDBarPrefab()
    {
        return LoadObject("", ResourceDefine.HUDBarPath) as GameObject;
    }

    public GameObject GetHUDNumberPrefab()
    {
        return LoadObject("", ResourceDefine.HUDNumberPath) as GameObject;
    }

    /// <summary>
    /// 获取图集对象
    /// </summary>
    /// <param name="atlas"></param>
    /// <returns></returns>
    public UIAtlas GetAtlas(string atlas)
    {
        return (LoadObject(atlas, ResourceDefine.UIAtlasPath) as GameObject).GetComponent<UIAtlas>();
    }

    /// <summary>
    /// 获得指定字体
    /// </summary>
    /// <param name="fontName"></param>
    /// <returns></returns>
    public Font GetFont(string fontName)
    {
        return LoadObject(fontName, ResourceDefine.FontPath) as Font;
    }

    /// <summary>
    /// 获得技能特效预设
    /// </summary>
    /// <param name="effectId"></param>
    /// <returns></returns>
    public GameObject GetSkillEffectPrefab(string effectId)
    {
        if (mResourceCacheDic.ContainsKey(effectId))
        {
            return GameObject.Instantiate(mResourceCacheDic[effectId]) as GameObject;
        }

        Object obj = LoadObject(effectId, ResourceDefine.SkillEffectPath);
        if (obj == null)
        {
            return null;
        }

        mResourceCacheDic.Add(effectId, obj);

        return GameObject.Instantiate(obj) as GameObject;
    }

    /// <summary>
    /// 获取飞行技能
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public GameObject GetFlySkill(int skillId)
    {
        return LoadObject(skillId.ToString(), ResourceDefine.FlySkillPath) as GameObject;
    }

    public GameObject GetFlySkillByFileName(string fileName)
    {
        return GameObject.Instantiate(LoadObject(fileName, ResourceDefine.FlySkillPath)) as GameObject;
    }

    /// <summary>
    /// 获取特效预设
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public GameObject GetEffect(string effectPath)
    {
        return LoadObject(effectPath, "") as GameObject;
    }

    public Object GetAudio(string sound)
    {
        return LoadObject(sound, "");
    }

    public AudioClip GetSceneBgAudio(string audio)
    {
        return LoadObject(audio, ResourceDefine.mSceneBgMusicPath) as AudioClip;
    }

    public AudioClip GetBattleAudio(string sound)
    {
        return LoadObject(sound, ResourceDefine.mBattleAudioPath) as AudioClip;
    }

    public AudioClip GetSpecialAudio(string sound)
    {
        return LoadObject(sound, ResourceDefine.mSpecialAudioClipPath) as AudioClip;
    }

    /// <summary>
    /// 获取技能路径预设
    /// </summary>
    /// <param name="pathName"></param>
    /// <returns></returns>
    public GameObject GetSkillPath(string pathName)
    {
        return LoadObject(pathName, ResourceDefine.SkillEffectPathPath) as GameObject;
    }

    public GameObject GetMainCamera()
    {
        return LoadObject("Main Camera", "") as GameObject;
    }

    private Object LoadObject(string name, string path)
    {
        return Resources.Load(path + name);
    }
}