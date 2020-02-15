

public class ResourceDefine
{
    /// <summary>
    /// 语言类型
    /// </summary>
    public enum LanguageType
    {
        LT_ChineseS = 0,///简体中文 
        LT_ChineseT,///繁体中文
        LT_English,///英文
        LT_Thai,///泰语
        LT_Vietnamese,///越南文
        LT_French,///法文
        LT_German,///德语
        LT_Polish,///波兰语
        LT_Italian,///意大利语
        LT_Turkish,///土耳其语
        LT_Russian,///俄罗斯
        LT_Korean,///韩语
        LT_Japanese,///日语
        LT_Portuguese,///葡萄牙语
        LT_Spanish///西班牙语
    }

    public static string LanguagePath = "Language/";
    public static string RolePath = "Model/Role/";
    public static string NPCPath = "Model/NPC/";
    public static string MonsterPath = "Model/Monster/";
    public static string ActionPath = "Actions/";
    public static string OtherPath = "Prefabs/Map/Other/";
    public static string UIPath = "Prefabs/UI/";
    public static string HUDTextPath = "UI/HUDText/HUDText";
    public static string HUDBarPath = "UI/HUDText/HUDBar";
    public static string HUDNumberPath = "UI/HUDText/HUDNumber";
    public static string UIAtlasPath = "UI/Atlas/";
    public static string EffectsPath = "Effect/skill/";
    public static string Gongjian = "Model/Gongjian/";

    /// <summary>
    /// 场景背景音乐路径
    /// </summary>
    public static string mSceneBgMusicPath = "Audio/SceneAudio/";

    /// <summary>
    /// UI音效路径
    /// </summary>
    public static string mUIAudioPath = "Audio/UiAudio/";

    /// <summary>
    /// 战斗音效路径
    /// </summary>
    public static string mBattleAudioPath = "Audio/BattleAudio/";

    /// <summary>
    /// 英雄说话音效路径
    /// </summary>
    public static string mHeroSpeakingPath = @"Audio/HeroVocal/";

    /// <summary>
    /// 新手引导音效路径
    /// </summary>
    public static string mUserGuideAudioClipPath = @"Audio/UserGuide/";

    /// <summary>
    /// floatStory专用音效路径
    /// </summary>
    public static string mStoryAudioClipPath = @"Audio/StoryAudio/";

    /// <summary>
    /// 特殊音效
    /// </summary>
    public static string mSpecialAudioClipPath = @"Audio/SpecialAudioClipPath/";

    public static string FontPath = "UI/Font/";
    public static string EffectPath = "Effect/";
    public static string SkillEffectPath = "Effect/Skill/";
    public static string SkillEffectPathPath = "Effect/SkillPath/";

    public static string FlySkillPath = "Model/Weapon/";

    public static string WeaponPath = "Prefabs/Map/Weapon/";
    public static string UITexPath = "Textures/Interface/";
    public static string UIComponentsPath = "Prefabs/UIComponents/";
    
    public static string PrefabPath = "Prefabs/";
    public static string EffectsModelPath = "Prefabs/Effect/Model/";
    public static string Character = "Prefabs/Effect/Character/";
    public static string UIAtlas = "Prefabs/UIAtlas/";
    public static string PropPackPath = "Config/PropPack/";
    public static string XMLPath = "Config/";
    public static string MapTexPath = "Textures/Interface/Maps/";
    public static string IconEquipPath = "Textures/Photo/Equip/";

    public static string IconItemPath = "Textures/Photo/Item/";
    public static string AchievementIconPath = "Textures/Photo/Achievement/";
    public static string IconRoleHeadPath = "Textures/Photo/HeadPic/RoleHeard/";
    public static string IconMonsterHeadPath = "Textures/Photo/HeadPic/MonestHeard/";
    public static string IconHalfPicPath = "Textures/Photo/HalfPic/";
    public static string IconRidePath = "Textures/Photo/RidePic/";
    public static string IconRankPath = "Textures/RankIcon/";
    public static string MoviesPath = "Movies/Prefabs/";
    public static string SecretTextures = "Textures/Secret/";
    public static string Textures = "Textures/";
    public static string PetModelPath = "Prefabs/Map/Pet/";
    public static string LoginModelPath = "Prefabs/Map/Login/";
    public static string IconPetPath = "Textures/Photo/PetHeadPic/";
    public static string PetSkillIconPath = "Textures/Photo/PetSkill/";
    public static string OfficialSkillIconPath = "Textures/Photo/OfficialSkill/";
    public static string WingSkillIconPath = "Textures/Photo/WingSkill/";
    public static string BloodSkillIconPath = "Textures/Photo/BloodSkill/";
    public static string skillSelectFrame = "Prefabs/UI/SkillSelect";
    public static string UIEffectPath = "Prefabs/Effect/UI/";
    public static string AchieticsPath = "Textures/Achietics/";
    public static string IconPeteadPath = "Textures/Photo/HeadPic/PetHead/";
    public static string ModelPath = "Prefabs/Map/";
    public static string ModelEffectPath = "Prefabs/Effect/ModelEffect/";
    /// <summary>
    /// 当前语言环境
    /// </summary>
    private static LanguageType seLanguageType = 0;
    /// <summary>
    /// 设置语言环境
    /// </summary>
    /// <param name="iType">环境类型0:中文 1:英文 2：俄文</param>
    public static void SetLanguage(LanguageType iType)
    {
        seLanguageType = iType;
        switch (seLanguageType)
        {
            case LanguageType.LT_ChineseS:///简体中文 
                {
                    LanguagePath = "Language/china/";
                }
                break;
            case LanguageType.LT_ChineseT:///繁体中文
                {
                    LanguagePath = "Language/chinaT/";
                }
                break;
            case LanguageType.LT_English:///英文
                {
                    LanguagePath = "Language/english/";
                }
                break;
            case LanguageType.LT_Thai:///泰语
                {
                    LanguagePath = "Language/thai/";
                }
                break;
            case LanguageType.LT_Vietnamese:///越南文
                {
                    LanguagePath = "Language/vietnamese/";
                }
                break;
            case LanguageType.LT_French:///法文
                {
                    LanguagePath = "Language/french/";
                }
                break;
            case LanguageType.LT_German:///德语
                {
                    LanguagePath = "Language/german/";
                }
                break;
            case LanguageType.LT_Polish:///波兰语
                {
                    LanguagePath = "Language/polish/";
                }
                break;
            case LanguageType.LT_Italian:///意大利语
                {
                    LanguagePath = "Language/italian/";
                }
                break;
            case LanguageType.LT_Turkish:///土耳其语
                {
                    LanguagePath = "Language/turkish/";
                }
                break;
            case LanguageType.LT_Russian:///俄罗斯
                {
                    LanguagePath = "Language/russian/";
                }
                break;
            case LanguageType.LT_Korean:///韩语
                {
                    LanguagePath = "Language/korean/";
                }
                break;
            case LanguageType.LT_Japanese:///日语
                {
                    LanguagePath = "Language/japanese/";
                }
                break;
            case LanguageType.LT_Portuguese:///葡萄牙语
                {
                    LanguagePath = "Language/portuguese/";
                }
                break;
            case LanguageType.LT_Spanish:///西班牙语
                {
                    LanguagePath = "Language/spanish/";
                }
                break;
            default:
                {
                    LanguagePath = "LanguagePath/china/";
                }
                break;
        }
    }
}