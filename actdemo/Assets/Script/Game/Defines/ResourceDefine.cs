

public class ResourceDefine
{
    /// <summary>
    /// ��������
    /// </summary>
    public enum LanguageType
    {
        LT_ChineseS = 0,///�������� 
        LT_ChineseT,///��������
        LT_English,///Ӣ��
        LT_Thai,///̩��
        LT_Vietnamese,///Խ����
        LT_French,///����
        LT_German,///����
        LT_Polish,///������
        LT_Italian,///�������
        LT_Turkish,///��������
        LT_Russian,///����˹
        LT_Korean,///����
        LT_Japanese,///����
        LT_Portuguese,///��������
        LT_Spanish///��������
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
    /// ������������·��
    /// </summary>
    public static string mSceneBgMusicPath = "Audio/SceneAudio/";

    /// <summary>
    /// UI��Ч·��
    /// </summary>
    public static string mUIAudioPath = "Audio/UiAudio/";

    /// <summary>
    /// ս����Ч·��
    /// </summary>
    public static string mBattleAudioPath = "Audio/BattleAudio/";

    /// <summary>
    /// Ӣ��˵����Ч·��
    /// </summary>
    public static string mHeroSpeakingPath = @"Audio/HeroVocal/";

    /// <summary>
    /// ����������Ч·��
    /// </summary>
    public static string mUserGuideAudioClipPath = @"Audio/UserGuide/";

    /// <summary>
    /// floatStoryר����Ч·��
    /// </summary>
    public static string mStoryAudioClipPath = @"Audio/StoryAudio/";

    /// <summary>
    /// ������Ч
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
    /// ��ǰ���Ի���
    /// </summary>
    private static LanguageType seLanguageType = 0;
    /// <summary>
    /// �������Ի���
    /// </summary>
    /// <param name="iType">��������0:���� 1:Ӣ�� 2������</param>
    public static void SetLanguage(LanguageType iType)
    {
        seLanguageType = iType;
        switch (seLanguageType)
        {
            case LanguageType.LT_ChineseS:///�������� 
                {
                    LanguagePath = "Language/china/";
                }
                break;
            case LanguageType.LT_ChineseT:///��������
                {
                    LanguagePath = "Language/chinaT/";
                }
                break;
            case LanguageType.LT_English:///Ӣ��
                {
                    LanguagePath = "Language/english/";
                }
                break;
            case LanguageType.LT_Thai:///̩��
                {
                    LanguagePath = "Language/thai/";
                }
                break;
            case LanguageType.LT_Vietnamese:///Խ����
                {
                    LanguagePath = "Language/vietnamese/";
                }
                break;
            case LanguageType.LT_French:///����
                {
                    LanguagePath = "Language/french/";
                }
                break;
            case LanguageType.LT_German:///����
                {
                    LanguagePath = "Language/german/";
                }
                break;
            case LanguageType.LT_Polish:///������
                {
                    LanguagePath = "Language/polish/";
                }
                break;
            case LanguageType.LT_Italian:///�������
                {
                    LanguagePath = "Language/italian/";
                }
                break;
            case LanguageType.LT_Turkish:///��������
                {
                    LanguagePath = "Language/turkish/";
                }
                break;
            case LanguageType.LT_Russian:///����˹
                {
                    LanguagePath = "Language/russian/";
                }
                break;
            case LanguageType.LT_Korean:///����
                {
                    LanguagePath = "Language/korean/";
                }
                break;
            case LanguageType.LT_Japanese:///����
                {
                    LanguagePath = "Language/japanese/";
                }
                break;
            case LanguageType.LT_Portuguese:///��������
                {
                    LanguagePath = "Language/portuguese/";
                }
                break;
            case LanguageType.LT_Spanish:///��������
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