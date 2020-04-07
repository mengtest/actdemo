using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyBinaryXml;
using SysUtils;
using UnityEngine.UI;

public class XmlManager
{
    private static XmlManager _Instance = null;
    private XmlManager() { }

    public static XmlManager Instance
    {
        private set { }
        get
        {
            if (_Instance == null)
            {
                _Instance = new XmlManager();
            }

            return _Instance;
        }
    }

    /// <summary>
    /// 加载资源总数
    /// </summary>
    private int mCount = 9;
    public int currentCount = 0;
    /// <summary>
    /// 加载全部配置
    /// </summary>
    public void LoadAllConfig()
    {
        // int currentCount = 0;

        //GameObject slider = GameObject.Find("Canvas/Slider");
        //Slider sl = slider.GetComponent<Slider>();
        
        bool success = LoadProperty();
        if (success)
        {
            //++currentCount;
            //sl.value = (float)currentCount / (float)mCount;

  //          loading.ChangeProgress((float)currentCount / (float)mCount);
        }

        success = LoadSkillProperty();
        if (success)
        {
            //++currentCount;
            //sl.value = (float)currentCount / (float)mCount;
            //loading.ChangeProgress((float)currentCount / (float)mCount);
        }

        success = LoadCommonText();
        if (success)
        {
            ++currentCount;
            //sl.value = (float)currentCount / (float)mCount;
            ////Debug.Log("Loading " + currentCount);
            //loading.ChangeProgress((float)currentCount / (float)mCount);
        }

        success = LoadCloneSceneProperty();
        if (success)
        {
            //++currentCount;
            //sl.value = (float)currentCount / (float)mCount;
            //loading.ChangeProgress((float)currentCount / (float)mCount);
        }

		success = LoadAnimatorSkillProperty();
		if (success)
		{
			//++currentCount;
   //         sl.value = (float)currentCount / (float)mCount;
            //loading.ChangeProgress((float)currentCount / (float)mCount);
        }

        success = LoadAudioProperty();
        if (success)
        {
            //++currentCount;
            //sl.value = (float)currentCount / (float)mCount;
            //loading.ChangeProgress((float)currentCount / (float)mCount);
        }

        success = LoadEffectProperty();
        if (success)
        {
            ++currentCount;
            ////Debug.Log("Loading " + currentCount);
            //sl.value = (float)currentCount / (float)mCount;
            //loading.ChangeProgress((float)currentCount / (float)mCount);
        }

        success = LoadFormula();
        if (success)
        {
            //++currentCount;
            //sl.value = (float)currentCount / (float)mCount;
            //loading.ChangeProgress((float)currentCount / (float)mCount);
        }

        success = LoadSlowMotionProperty();
        if (success)
        {
            //++currentCount;
            //sl.value = (float)currentCount / (float)mCount;
            //loading.ChangeProgress((float)currentCount / (float)mCount);
        }

        if (!success)
        {
            return;
        }

        //UIManager.Instance.CloseUI("LoadingUI");
        //UIManager.Instance.OpenUI("LoginUI");
    }

    #region <>通用文本<>

    private Dictionary<string, string> mCommonTextDic = new Dictionary<string, string>();

    bool LoadCommonText()
    {
        byte[] asset = ResourceManager.Instance.GetXml("CommonText");
        if (asset == null)
        {
            return false;
        }

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return false;
        }

        List<TbXmlNode> xmlNodeList = docNode.GetNodes("CommonText/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return false;
        }

        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            string id = node.GetStringValue("TextId");
            string text = node.GetStringValue("Text");

            mCommonTextDic.Add(id, text);
        }

        return true;
    }

    public string GetCommonText(string id)
    {
        if (!mCommonTextDic.ContainsKey(id))
        {
            return "";
        }

        return mCommonTextDic[id];
    }

    #endregion

    #region <>属性<>

    private Dictionary<int, List<Property>> mPropertyDic = new Dictionary<int, List<Property>>();

    bool LoadProperty()
    {
        byte[] asset = ResourceManager.Instance.GetXml("Property");
        if (asset == null)
        {
            return false;
        }

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return false;
        }

        List<TbXmlNode> xmlNodeList = docNode.GetNodes("Property/ObjProperty");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return false;
        }

        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            int objId = node.GetIntValue("ObjId");
            List<Property> list = new List<Property>();

            List<TbXmlNode> childNode = node.GetNodes("Propertys");
            for (int j = 0; j < childNode.Count; ++j)
            {
                TbXmlNode child = childNode[j] as TbXmlNode;
                Property fp = new Property();

                fp.mPropName = child.GetStringValue("PropName");
                fp.mVarType = child.GetIntValue("Type");
                switch (fp.mVarType)
                {
                    case VarType.Int:
                        fp.mValue = new Var(fp.mVarType, UtilTools.IntParse(child.GetStringValue("Value")));
                        break;

                    case VarType.Float:
                        fp.mValue = new Var(fp.mVarType, UtilTools.FloatParse(child.GetStringValue("Value")));
                        break;

                    case VarType.Int64:
                        fp.mValue = new Var(fp.mVarType, UtilTools.LongParse(child.GetStringValue("Value")));
                        break;

                    case VarType.String:
                        fp.mValue = new Var(fp.mVarType, child.GetStringValue("Value"));
                        break;

                    default:
                        fp.mValue = Var.zero;
                        break;
                }

                list.Add(fp);
            }

            mPropertyDic.Add(objId, list);
        }

        return true;
    }

    public List<Property> GetProperty(int objId)
    {
        if (!mPropertyDic.ContainsKey(objId))
        {
            return null;
        }

        return mPropertyDic[objId];
    }

    #endregion

    #region <>技能属性<>
    private Dictionary<int, SkillProperty> mSkillPropertyDic = new Dictionary<int, SkillProperty>();

    bool LoadSkillProperty()
    {
        byte[] asset = ResourceManager.Instance.GetXml("SkillProperty");
        if (asset == null)
        {
            return false;
        }

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return false;
        }

        List<TbXmlNode> xmlNodeList = docNode.GetNodes("SkillProperty/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return false;
        }

        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            SkillProperty sp = new SkillProperty();
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            sp.mSkillId = node.GetIntValue("No");
            sp.mSkillName = node.GetStringValue("Name");
            sp.mSkillOwnerId = node.GetIntValue("Model");
            sp.mSkillIndex = node.GetIntValue("SkillNumber");
            sp.mSkillSpace = node.GetFloatValue("SkillSpace");
            sp.mSkillConsumptionType = (SkillConsumptionType)node.GetIntValue("SkillConsumptionType");
            sp.mSkillConsumptionValue = node.GetIntValue("SkillConsumptionValue");
            sp.mAnimatorSkillProperty = new AnimatorSkillProperty();

            mSkillPropertyDic.Add(sp.mSkillId, sp);
        }

        return true;
    }

    public SkillProperty GetSkillProperty(int skillId)
    {
        if (!mSkillPropertyDic.ContainsKey(skillId))
        {
            return null;
        }

        return mSkillPropertyDic[skillId];
    }

    #endregion

    #region <>副本属性<>

    private Dictionary<int, CloneSceneProperty> mCloneScenePropertyDic = new Dictionary<int, CloneSceneProperty>();

    bool LoadCloneSceneProperty()
    {
        byte[] asset = ResourceManager.Instance.GetXml("CloneScene");
        if (asset == null)
        {
            return false;
        }

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return false;
        }

        List<TbXmlNode> xmlNodeList = docNode.GetNodes("CloneScene/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return false;
        }

        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            CloneSceneProperty cs = new CloneSceneProperty();
            int id = node.GetIntValue("CloneSceneId");

            cs.mCloneSceneMonsterInfoList = new List<CloneSceneProperty.CloneSceneMonsterInfo>();
            List<TbXmlNode> childNode = node.GetNodes("CloneSceneMonsterInfo");
            for (int j = 0; j < childNode.Count; ++j)
            {
                TbXmlNode child = childNode[j] as TbXmlNode;
                CloneSceneProperty.CloneSceneMonsterInfo csm = new CloneSceneProperty.CloneSceneMonsterInfo();
                csm.mMonsterId = child.GetIntValue("MonsterId");
                csm.mMonsterPosition = UtilTools.FormatStringVector3(child.GetStringValue("MonsterPosition"));
                csm.mMonsterRotationY = child.GetFloatValue("MonsterRotationY");
                csm.mMonsterScale = child.GetFloatValue("MonsterScale");

                cs.mCloneSceneMonsterInfoList.Add(csm);
            }

            mCloneScenePropertyDic.Add(id, cs);
        }

        return true;
    }

    public CloneSceneProperty GetCloneSceneProperty(int id)
    {
        if (!mCloneScenePropertyDic.ContainsKey(id))
        {
            return null;
        }

        return mCloneScenePropertyDic[id];
    }

    #endregion

	#region <>技能动作受击属性<>
	
	public bool LoadAnimatorSkillProperty()
	{
		byte[] asset = ResourceManager.Instance.GetXml("AnimatorConfig");
		if (asset == null)
		{
			return false;
		}
		
		TbXmlNode docNode = TbXml.Load(asset).docNode;
		if (docNode == null)
		{
			return false;
		}
		
		List<TbXmlNode> xmlNodeList = docNode.GetNodes("animatorconfig/Property");
		int xmlNodeListLength = xmlNodeList.Count;
		if (xmlNodeListLength < 1)
		{
			return false;
		}
		
		for (int i = 0; i < xmlNodeListLength; ++i)
		{
			TbXmlNode node = xmlNodeList[i] as TbXmlNode;
			AnimatorSkillProperty asp = new AnimatorSkillProperty();
			asp.mySkillId = node.GetIntValue("No");
            asp.mySkillAction = node.GetIntValue("Action");
			asp.mSkillActionProperty = new Dictionary<int, SkillActionProperty>();

			List<TbXmlNode> childNode = node.GetNodes("Action");
			asp.mySkillActionCount = childNode.Count;

			for (int j = 0; j < asp.mySkillActionCount; ++j)
			{
				TbXmlNode child = childNode[j] as TbXmlNode;
				SkillActionProperty sap = new SkillActionProperty();

				sap.ActionNumberId = child.GetIntValue("ActionNumber");

                sap.mSkillTargetType = (SkillTargetType)child.GetIntValue("TargetType");
                sap.mSkillReleaseType = (SkillReleaseType)child.GetIntValue("SkillReleaseType");
                sap.mSkillFlyRes = child.GetStringValue("SkillFlyRes");
                
				sap.mSkillType = (SkillType)child.GetIntValue("SkillType");

				string skillTypeParams = child.GetStringValue("SkillTypeParams");
				if (skillTypeParams == null || skillTypeParams.Equals(""))
				{
					sap.mSkillTypeParams = new List<float>();
					sap.mSkillTypeParams.Add(1f);
				}
				else
				{
					sap.mSkillTypeParams = new List<float>();
					string[] s = skillTypeParams.Split(',');

					for (int k = 0; k < s.Length; k++)
					{
						sap.mSkillTypeParams.Add(UtilTools.FloatParse(s[k]));
					}
				}

                sap.mPauseTime = UtilTools.FloatParse(child.GetStringValue("PauseTime"));
				sap.mCenterOffset = UtilTools.FormatStringVector3(child.GetStringValue("CenterOffset"));
				sap.mSkillRangeType = (SkillRangeType)child.GetIntValue("SkillRangeType");

                string skillRangeTypeParams = child.GetStringValue("SkillRangeTypeParams");
				if(skillRangeTypeParams == null || skillRangeTypeParams.Equals(""))
				{
					sap.mSkillRangParametr = null;
				}
				else
				{
					sap.mSkillRangParametr = new List<float>();
					string[]  s = skillRangeTypeParams.Split(',');
					for(int k= 0 ;k < s.Length ;k++)
					{
						sap.mSkillRangParametr.Add(UtilTools.FloatParse(s[k]));
					}
				}

				sap.mHurtEffectId = child.GetIntValue("HurtEffect");
				sap.mDamageStatus = child.GetIntValue("DamageStatus");
                sap.mDebuffId = child.GetIntValue("DebuffId");

                sap.mAttackerPauseTime = UtilTools.FloatParse(child.GetStringValue("AttackerPauseTime"));
				sap.mCameraShakeNumber = child.GetIntValue("CameraShakeNumber");
                sap.mCameraShakeDistance = UtilTools.FloatParse(child.GetStringValue("CameraShakeDistance"));
                sap.mCameraShakeSpeed = UtilTools.FloatParse(child.GetStringValue("CameraShakeSpeed"));
				sap.mRepelType = child.GetIntValue("RepelType");
				sap.mHurtDir = child.GetIntValue("HurtDir");
                sap.mShakeRotationX = UtilTools.FloatParse(child.GetStringValue("RotationX"));
                sap.mShakeDecay = UtilTools.FloatParse(child.GetStringValue("ShakeDecay"));
				sap.mTopplebackType = child.GetIntValue("TopplebackType");
                
                asp.mSkillActionProperty.Add(sap.ActionNumberId , sap);
			}

			if (mSkillPropertyDic.ContainsKey(asp.mySkillId))
            {
                mSkillPropertyDic[asp.mySkillId].mAnimatorSkillProperty = asp;
            }
		}

		return true;
	}
    
    #endregion

    #region <>音效表<>

    private Dictionary<string, AudioProperty> mAudioPropertyDic = new Dictionary<string, AudioProperty>();

    bool LoadAudioProperty()
    {
        byte[] asset = ResourceManager.Instance.GetXml("AudioProperty");
        if (asset == null)
        {
            return false;
        }

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return false;
        }

        List<TbXmlNode> xmlNodeList = docNode.GetNodes("AudioProperty/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return false;
        }

        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            AudioProperty sp = new AudioProperty();
            string AudioKey = node.GetStringValue("AudioKey");

            sp.mAudioModel = node.GetStringValue("AudioModel");
            sp.mAudioAction = node.GetStringValue("AudioAction");
            sp.mAudioResource = node.GetStringValue("AudioResource");
            sp.mAudioVolume = UtilTools.FloatParse(node.GetStringValue("AudioVolume"));
            sp.mAudioFrontTime = UtilTools.FloatParse(node.GetStringValue("AudioFrontTime"));
            sp.mAudioLoop = node.GetStringValue("AudioLoop").Equals("true") ? true : false;

            mAudioPropertyDic.Add(AudioKey, sp);
        }

        return true;
    }

    public AudioProperty GetAudioProperty(string id)
    {
        if (!mAudioPropertyDic.ContainsKey(id))
        {
            return null;
        }

        return mAudioPropertyDic[id];
    }

    public Dictionary<string, AudioProperty> GetAudioProperty()
    {
        return mAudioPropertyDic;
    }

    #endregion

    #region <>特效表<>

    private Dictionary<int, EffectProperty> mEffectPropertyDic = new Dictionary<int, EffectProperty>();

    bool LoadEffectProperty()
    {
        byte[] asset = ResourceManager.Instance.GetXml("effectconfig");
        if (asset == null)
        {
            return false;
        }

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return false;
        }

        List<TbXmlNode> xmlNodeList = docNode.GetNodes("effectconfig/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return false;
        }

        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            EffectProperty ep = new EffectProperty();
            int effectId = UtilTools.IntParse(node.GetStringValue("No"));

            ep.mEffectId = effectId;
            ep.mEffectModel = UtilTools.IntParse(node.GetStringValue("Model"));
            ep.mEffectAction = node.GetStringValue("active");
            ep.mEffectFile = node.GetStringValue("FileName");
            ep.mEffectPath = node.GetStringValue("Path");
            ep.mEffectRemovePath = node.GetStringValue("RemoveEffect");
            ep.mEffectFollowAction = node.GetIntValue("FollowAction") == 1 ? true : false;
            ep.mEffectDirectionType = (EffectDirectionType)UtilTools.IntParse(node.GetStringValue("EffectDirectionType"));
            ep.mEffectDirectionDistance = UtilTools.FloatParse(node.GetStringValue("EffectDirectionDistance"));
            ep.mEffectFollowRole = UtilTools.BoolParse(node.GetStringValue("EffectRotation"));
            ep.mEffectFollowBip = UtilTools.BoolParse(node.GetStringValue("EffectFollowBip"));

            mEffectPropertyDic.Add(effectId, ep);
        }

        return true;
    }

    public EffectProperty GetEffectProperty(int id)
    {
        if (!mEffectPropertyDic.ContainsKey(id))
        {
            return null;
        }

        return mEffectPropertyDic[id];
    }

    public Dictionary<int, EffectProperty> GetEffectProperty()
    {
        return mEffectPropertyDic;
    }

    #endregion

    #region <>通用文本<>

    private Dictionary<string, string> mFormulaDic = new Dictionary<string, string>();

    bool LoadFormula()
    {
        byte[] asset = ResourceManager.Instance.GetXml("Formula");
        if (asset == null)
        {
            return false;
        }

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return false;
        }

        List<TbXmlNode> xmlNodeList = docNode.GetNodes("Formula/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return false;
        }

        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            string id = node.GetStringValue("FormulaId");
            string text = node.GetStringValue("Formula");

            mFormulaDic.Add(id, text);
        }

        return true;
    }

    public string GetFormula(string id)
    {
        if (!mFormulaDic.ContainsKey(id))
        {
            return "";
        }

        return mFormulaDic[id];
    }

    #endregion

    #region <>AI<>

    private Dictionary<int, AIProperty> mAIPropertyDic = new Dictionary<int, AIProperty>();

    bool LoadAIProperty()
    {
        byte[] asset = ResourceManager.Instance.GetXml("AI");
        if (asset == null)
        {
            return false;
        }

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return false;
        }

        List<TbXmlNode> xmlNodeList = docNode.GetNodes("AI/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return false;
        }

        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            AIProperty ap = new AIProperty();
            ap.mAIId = UtilTools.IntParse(node.GetStringValue("Id"));

            //mAIPropertyDic.Add(ap.mAIId, text);
        }

        return true;
    }

    public AIProperty GetAIProperty(int id)
    {
        if (!mAIPropertyDic.ContainsKey(id))
        {
            return null;
        }

        return mAIPropertyDic[id];
    }

    #endregion

    #region <>慢动作<>

    private Dictionary<int, SlowMotionProperty> mSlowMotionPropertyDic = new Dictionary<int, SlowMotionProperty>();

    bool LoadSlowMotionProperty()
    {
        byte[] asset = ResourceManager.Instance.GetXml("SlowMotionProperty");
        if (asset == null)
        {
            return false;
        }

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return false;
        }

        List<TbXmlNode> xmlNodeList = docNode.GetNodes("SlowMotion/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return false;
        }

        for (int i = 0; i < xmlNodeListLength; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            SlowMotionProperty smp = new SlowMotionProperty();
            smp.mSlowMotionId = UtilTools.IntParse(node.GetStringValue("ActionId"));
            smp.mSlowMotionStepList = new List<SlowMotionStep>();

            List<TbXmlNode> childNode = node.GetNodes("SlowMotions");
            for (int j = 0; j < childNode.Count; ++j)
            {
                TbXmlNode child = childNode[j] as TbXmlNode;
                SlowMotionStep sms = new SlowMotionStep();

                sms.mIndex = UtilTools.IntParse(child.GetStringValue("Index"));
                sms.mTimeScale = UtilTools.FloatParse(child.GetStringValue("TimeScale"));
                sms.mTime = UtilTools.FloatParse(child.GetStringValue("Time"));
                sms.mFieldOfView = UtilTools.FloatParse(child.GetStringValue("FieldOfView"));

                smp.mSlowMotionStepList.Add(sms);
            }

            mSlowMotionPropertyDic.Add(smp.mSlowMotionId, smp);
        }

        return true;
    }

    public SlowMotionProperty GetSlowMotionProperty(int id)
    {
        if (!mSlowMotionPropertyDic.ContainsKey(id))
        {
            return null;
        }

        return mSlowMotionPropertyDic[id];
    }

    #endregion
}