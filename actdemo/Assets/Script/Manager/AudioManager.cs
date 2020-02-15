
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Filename: AudioManager.cs
/// Description: 管理音效播放
/// @author yz & xmc
/// </summary>
public class AudioManager : MonoBehaviour
{
    private static AudioManager _Instance = null;

    /// <summary>
    /// 场景背景音乐固定音源
    /// </summary>
    private AudioSource m_BgAudio = null;

    /// <summary>
    /// 当前说话的英雄
    /// </summary>
    private GameObject m_CurrentSpeakingHero;

    /// <summary>
    /// 当前处于播放中的Story和FloatStory中的语音对话
    /// </summary>
    private GameObject m_CurrentStoryAudio;

    /// <summary>
    /// 当前新手引导播放的音效
    /// </summary>
    private GameObject m_CurrentUserGuideAudio;

    /// <summary>
    /// 场景背景音乐路径
    /// </summary>
    private string m_SceneBgMusicPath = "Audio/SceneBg/";

    /// <summary>
    /// UI音效路径
    /// </summary>
    private string m_UIAudioPath = "Audio/UiAudio/";

    /// <summary>
    /// 战斗音效路径
    /// </summary>
    private string m_BattleAudioPath = "Audio/BattleAudio/";

    /// <summary>
    /// 英雄说话音效路径
    /// </summary>
    private string m_HeroSpeakingPath = @"Audio/HeroVocal/";

    /// <summary>
    /// 新手引导音效路径
    /// </summary>
    private string m_UserGuideAudioClipPath = @"Audio/UserGuide/";

    /// <summary>
    /// floatStory专用音效路径
    /// </summary>
    private string m_StoryAudioClipPath = @"Audio/StoryAudio/";

    /// <summary>
    /// 音效表
    /// </summary>
    private Dictionary<string, AudioProperty> mSoundPropertyDic = new Dictionary<string, AudioProperty>();
    private int mSoundPropertyCount = 0;

    public static AudioManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    void Awake()
    {
        _Instance = this;

        mSoundPropertyDic = XmlManager.Instance.GetAudioProperty();
        mSoundPropertyCount = mSoundPropertyDic.Count;
    }

    private void OnDestroy()
    {
        _Instance = null;

        m_BattleEffectAudioDict = null;
    }

    private void OnExitBattle()
    {
        m_BattleEffectAudioDict.Clear();
    }

    AudioProperty GetKeyByAnimator(Animator animator, AnimatorStateInfo stateInfo)
    {
        List<string> list = new List<string>(mSoundPropertyDic.Keys);
        for (int i = 0; i < list.Count; ++i)
        {
            AudioProperty sp = mSoundPropertyDic[list[i]];
            if (sp == null)
            {
                continue;
            }

            if (!sp.mAudioModel.Equals(animator.name))
            {
                continue;
            }

            if (!stateInfo.IsName(sp.mAudioAction))
            {
                continue;
            }

            return sp;
        }

        return null;
    }

    /// <summary>
    /// 获取目标场景音效名
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private string GetSceneMusicName(string sceneName)
    {
        string musicName = "";
        switch (sceneName)
        {
            case "Login":
                musicName = "login";
                break;
            case "MainCity_Wei":
            case "MainCity_luoyang":
                musicName = "city_001";
                break;
            case "WorldMap":
                musicName = "world";
                break;
            case "zc_HuLaoGuan":
                musicName = "fb_001";
                break;
            case "zc_jingjichang":
                musicName = "pvp_001";
                break;
            case "zc_luoyangcheng":
                musicName = "fb_001";
                break;
            default:
                musicName = "fb_001";
                break;
        }

        return musicName;
    }

    /// <summary>
    /// 播放场景背景音乐
    /// </summary>
    /// <param name="sceneName"></param>
    public void PlayBgMusic(string sceneName)
    {
        if (m_BgAudio != null)
        {
            m_BgAudio.Stop();
        }

        if (!GlobalData.MusicOpen)
        {
            return;
        }

        if (sceneName.Equals(""))
        {
            LogSystem.Log("场景音效不存在");

            return;
        }

        AudioProperty sp = XmlManager.Instance.GetAudioProperty(sceneName);
        if (sp == null)
        {
            LogSystem.Log("场景音效不存在:" + sceneName);

            return;
        }

        string soundName = sp.mAudioResource;

        AudioClip sceneBgMusicClip = ResourceManager.Instance.GetSceneBgAudio(soundName);
        if (sceneBgMusicClip == null)
        {
            LogSystem.Log("场景背景音乐切片不存在");

            return;
        }

        if (m_BgAudio == null)
        {
            m_BgAudio = gameObject.AddComponent<AudioSource>();
        }

        m_BgAudio.loop = sp.mAudioLoop;
        m_BgAudio.clip = sceneBgMusicClip;
        m_BgAudio.volume = sp.mAudioVolume;

        m_BgAudio.Play();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBgMusic()
    {
        if (!GlobalData.MusicOpen)
        {
            return;
        }

        if (m_BgAudio == null)
        {
            return;
        }

        AudioSource source = gameObject.GetComponent<AudioSource>();
        if (source == null)
        {
            return;
        }

        m_BgAudio = null;
        
        Destroy(source);
    }

    /// <summary>
    /// 背景音乐是否在播放
    /// </summary>
    public bool GetBgMusicState()
    {
        return m_BgAudio != null;
    }

    /// <summary>
    /// 播放UI音效
    /// </summary>
    public void PlayUIAudio(string comName)
    {
        if (!GlobalData.AudioOpen)
        {
            return;
        }
        
        AudioProperty sp = XmlManager.Instance.GetAudioProperty(comName);
        if (sp == null || string.IsNullOrEmpty(sp.mAudioResource))
        {
            LogSystem.Log("未找到对应编号音效路径:" + comName);

            return;
        }

        AudioClip audioClip = ResourceManager.Instance.GetAudio(sp.mAudioResource) as AudioClip;
        if (audioClip == null)
        {
            LogSystem.Log("加载音效错误:" + m_UIAudioPath + sp.mAudioResource);

            return;
        }

        GameObject go = new GameObject("UIAudio:" + comName);
        go.transform.parent = gameObject.transform;

        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = sp.mAudioVolume;
        audioSource.loop = sp.mAudioLoop;

        audioSource.PlayDelayed(sp.mAudioFrontTime);
        
        if (!audioSource.loop)
        {
            Destroy(go, audioClip.length + sp.mAudioFrontTime);
        }
    }

    /// <summary>
    /// 停止UI音效
    /// </summary>
    /// <param name="type"></param>
    public void StopUIAudio(string comName)
    {
        if (!GlobalData.AudioOpen)
        {
            return;
        }

        string goName = "UIAudio:" + comName;
        Transform transform = gameObject.transform.Find(goName);
        if (transform == null)
        {
            return;
        }

        AudioSource audioSource = transform.gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            LogSystem.Log("停止UI音效，音效对象不存在音源组件");

            return;
        }

        audioSource.Stop();

        Destroy(transform.gameObject);
    }

    /// <summary>
    /// 播放技能音效
    /// </summary>
    /// <param name="skillNo">技能ID</param>
    /// <param name="transform">位置</param>
    /// <param name="type">音效类型 0-施法，1-受击，2-打到空地</param>
    /// <param name="skillSender">技能施放者</param>
    public void PlayAttackAudio(int index, Animator animator)
    {
        if (!GlobalData.AudioOpen)
        {
            return;
        }

        if (index < 0 || animator == null)
        {
            return;
        }

        UtilTools.StringBuilder(animator.name, "_attack_", index);
        string resource = string.Format("{0}_attack_{1}", animator.name, index);

        AudioProperty sp = XmlManager.Instance.GetAudioProperty(resource);
        if (sp == null)
        {
            LogSystem.Log("未找到技能对应音效:" + resource);

            return;
        }

        AudioClip audioClip = ResourceManager.Instance.GetAudio(sp.mAudioResource) as AudioClip;
        if (audioClip == null)
        {
            LogSystem.Log("加载音效错误:" + m_BattleAudioPath + sp.mAudioResource);

            return;
        }

        GameObject go = new GameObject("SkillAudio:" + animator.name + "-" + resource);
        go.transform.parent = animator.transform;
        go.transform.localPosition = Vector3.zero;

        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 40f;
        audioSource.clip = audioClip;
        audioSource.volume = sp.mAudioVolume;
        audioSource.loop = false;

        audioSource.Play();
        Destroy(go, audioClip.length);
    }

    /// <summary>
    /// 播放被击中音效
    /// </summary>
    /// <param name="audioId"></param>
    /// <param name="trans"></param>
    public void PlayBeSkilledAudio(int audioId, Transform trans)
    {
        if (trans == null)
        {
            return;
        }

        AudioProperty sp = XmlManager.Instance.GetAudioProperty(audioId.ToString());
        if (sp == null)
        {
            LogSystem.Log("未找到技能对应音效:" + audioId);

            return;
        }

        AudioClip audioClip = ResourceManager.Instance.GetAudio(sp.mAudioResource) as AudioClip;
        if (audioClip == null)
        {
            LogSystem.Log("加载音效错误:" + m_BattleAudioPath + sp.mAudioResource);

            return;
        }

        GameObject go = new GameObject("BeSkilledAudio:" + trans.name + "-" + audioId);
        go.transform.parent = trans;
        go.transform.localPosition = Vector3.zero;

        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 40f;
        audioSource.clip = audioClip;
        audioSource.volume = sp.mAudioVolume;
        audioSource.loop = false;

        audioSource.Play();
        Destroy(go, audioClip.length);
    }

    /// <summary>
    /// 播放技能音效
    /// </summary>
    /// <param name="skillNo">技能ID</param>
    /// <param name="transform">位置</param>
    /// <param name="type">音效类型 0-施法，1-受击，2-打到空地</param>
    /// <param name="skillSender">技能施放者</param>
    public void PlaySkillAudio(Animator animator, AnimatorStateInfo stateInfo)
    {
        if (!GlobalData.AudioOpen)
        {
            return;
        }

        if (animator == null)
        {
            return;
        }

        Dictionary<string, AudioProperty> dic = XmlManager.Instance.GetAudioProperty();
        if (dic == null)
        {
            return;
        }

        List<AudioProperty> spList = new List<AudioProperty>();
        List<string> list = new List<string>(dic.Keys);
        for (int i = 0; i < list.Count; ++i)
        {
            AudioProperty tempSp = dic[list[i]];
            if (tempSp == null)
            {
                continue;
            }

            if (tempSp.mAudioModel.Equals(animator.name) && stateInfo.IsName(tempSp.mAudioAction))
            {
                spList.Add(tempSp);
            }
        }

        for (int i = 0; i < spList.Count; ++i)
        { 
            AudioProperty ap = spList[i];
            if (ap == null)
            {
                continue;
            }

            AudioClip audioClip = ResourceManager.Instance.GetBattleAudio(ap.mAudioResource);
            if (audioClip == null)
            {
                LogSystem.Log("加载音效错误:" + m_BattleAudioPath + ap.mAudioResource);

                return;
            }

            GameObject go = new GameObject("SkillAudio:" + animator.name + "-" + stateInfo.fullPathHash.ToString());
            go.transform.parent = animator.transform;
            go.transform.localPosition = Vector3.zero;

            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 40f;
            audioSource.clip = audioClip;
            audioSource.volume = ap.mAudioVolume;
            audioSource.loop = ap.mAudioLoop;

            audioSource.Play();

            if (!audioSource.loop)
            {
                Destroy(go, audioClip.length);
            }
        }
    }

    public void StopSkillAudio(Animator animator, AnimatorStateInfo stateInfo)
    {
        string audio = "SkillAudio:" + animator.name + "-" + stateInfo.fullPathHash;
        if (string.IsNullOrEmpty(audio))
        {
            return;
        }

        Transform trans = animator.transform;
        if (trans == null)
        {
            return;
        }

        Transform audioTrans = trans.Find(audio);
        if (audioTrans == null)
        {
            return;
        }

        Destroy(audioTrans.gameObject);
    }

    /// <summary>
    /// 播放场景NPC对话
    /// </summary>
    /// <param name="sound"></param>
    public void PlaySceneNpcAudio(int npcId)
    {
        if (!GlobalData.AudioOpen)
        {
            return;
        }

        if (npcId < 1)
        {
            return;
        }

        AudioProperty sp = XmlManager.Instance.GetAudioProperty(npcId.ToString());
        if (sp == null)
        {
            return;
        }

        string sound = sp.mAudioResource; ;
        if (sound == null || "".Equals(sound))
        {
            LogSystem.Log("场景NPC没有对应音效");

            return;
        }

        AudioClip audioClip = ResourceManager.Instance.GetAudio(sound) as AudioClip;
        if (audioClip == null)
        {
            LogSystem.Log("加载音效错误:" + m_HeroSpeakingPath + sound);

            return;
        }

        GameObject go = new GameObject("UIAudio:SceneNpc-" + sound);
        go.transform.parent = gameObject.transform;

        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.clip = audioClip;
        audioSource.volume = 1;
        audioSource.loop = false;

        audioSource.Play();

        Destroy(go, audioClip.length);
    }

    /// <summary>
    /// 暂停背景音乐与音效(战斗使用)
    /// </summary>
    public void PauseBGMAndAudio(bool pause)
    {
        if (!GlobalData.MusicOpen && m_BgAudio != null)
        {
            if (pause)
            {
                m_BgAudio.Pause();
            }
            else
            {
                m_BgAudio.Play();
            }
        }
    }

    #region Battle Effect Audio

    /// <summary>
    /// 战斗中其他3D音效，例如陷阱爆炸声
    /// </summary>
    private Dictionary<int, AudioSource> m_BattleEffectAudioDict = new Dictionary<int, AudioSource>();

    /// <summary>
    /// loop audio index
    /// </summary>
    private int m_BattleEffectIndex = 0;

    /// <summary>
    /// 播放战斗特殊音效
    /// </summary>
    /// <param name="trans">音效父节点</param>
    /// <param name="soundNo">音效No</param>
    /// <returns>音效索引</returns>
    public void PlayBattleEffectAudio(int audioId, IObject obj)
    {
        if (!GlobalData.AudioOpen)
        {
            return;
        }

        if (obj == null)
        {
            return;
        }

        AudioProperty sp = XmlManager.Instance.GetAudioProperty(audioId.ToString());
        if (sp == null)
        {
            LogSystem.Log("音效不存在");

            return;
        }

        AudioClip audioClip = ResourceManager.Instance.GetBattleAudio(sp.mAudioResource);
        if (audioClip == null)
        {
            LogSystem.Log("加载音效错误:" + m_BattleAudioPath + sp.mAudioResource);

            return;
        }

        GameObject go = new GameObject("BattleEffectAudio:" + sp.mAudioResource);
        go.transform.parent = obj.mTransform;
        go.transform.localPosition = Vector3.zero;

        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.clip = audioClip;

        audioSource.volume = sp.mAudioVolume;
        audioSource.loop = sp.mAudioLoop;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 40f;

        audioSource.PlayDelayed(sp.mAudioFrontTime);

        Destroy(go, audioClip.length + sp.mAudioFrontTime);
    }

    /// <summary>
    /// 播放战斗特殊音效
    /// </summary>
    /// <param name="trans">音效父节点</param>
    /// <param name="soundNo">音效No</param>
    /// <returns>音效索引</returns>
    public void PlayBattleEffectAudio(Animator animator, AnimatorStateInfo stateInfo)
    {
        if (!GlobalData.AudioOpen)
        {
            return;
        }

        if (animator == null)
        {
            return;
        }

        AudioProperty sp = GetKeyByAnimator(animator, stateInfo);
        if (sp == null)
        {
            LogSystem.Log("音效不存在");

            return;
        }

        AudioClip audioClip = ResourceManager.Instance.GetBattleAudio(sp.mAudioResource);
        if (audioClip == null)
        {
            LogSystem.Log("加载音效错误:" + m_BattleAudioPath + sp.mAudioResource);

            return;
        }

        GameObject go = new GameObject("BattleEffectAudio:" + sp.mAudioResource);
        go.transform.parent = animator.transform;
        go.transform.localPosition = Vector3.zero;

        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.clip = audioClip;

        audioSource.volume = sp.mAudioVolume;
        audioSource.loop = sp.mAudioLoop;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 40f;

        audioSource.PlayDelayed(sp.mAudioFrontTime);

        Destroy(go, audioClip.length + sp.mAudioFrontTime);
    }

    public void PlayRunAudio(IObject obj, bool bRun = true)
    {
        if (obj == null)
        {
            return;
        }

        int runAudio = obj.GetProperty("RunAudio").GetInt();
        if (runAudio < 1)
        {
            return;
        }

        AudioProperty ap = XmlManager.Instance.GetAudioProperty(runAudio.ToString());
        if (ap == null)
        {
            return;
        }

        AudioClip audioClip = ResourceManager.Instance.GetBattleAudio(ap.mAudioResource);
        if (audioClip == null)
        {
            LogSystem.Log("加载音效错误:" + m_BattleAudioPath + ap.mAudioResource);

            return;
        }

        GameObject go = new GameObject("SpecialAudio:RunAudio");
        go.transform.parent = obj.mTransform;
        go.transform.localPosition = Vector3.zero;

        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.clip = audioClip;

        audioSource.volume = ap.mAudioVolume;
        audioSource.loop = ap.mAudioLoop;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 40f;

        audioSource.Play();
    }

    public void StopRunAudio(IObject obj)
    {
        if (obj == null)
        {
            return;
        }

        Transform trans = obj.mTransform.Find("SpecialAudio:RunAudio");
        if (trans == null)
        {
            return;
        }

        Destroy(trans.gameObject);
    }

    #endregion
}