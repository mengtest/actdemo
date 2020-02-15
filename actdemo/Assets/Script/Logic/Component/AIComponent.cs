using UnityEngine;
using System.Collections;

public class AIComponent : IComponent
{
    private AIProperty mCurAIProperty;
    private AIActionType mCurAIActionType = AIActionType.AIActionType_Wait;

    private AnimatorComponent mAnimatorComponent;
    private RestoreComponent mRestoreComponent;

    public override void SetObject(IObject obj)
    {
        mAnimatorComponent = obj.GetComponent<AnimatorComponent>();
        mRestoreComponent = obj.GetComponent<RestoreComponent>();

        InitAI();
    }

    void InitAI()
    {
        if (mObj == null)
        {
            return;
        }

        int aiId = mObj.GetProperty("AI").GetInt();
        mCurAIProperty = XmlManager.Instance.GetAIProperty(aiId);
        if (mCurAIProperty == null)
        {
            return;
        }

        if (mCurAIProperty.mAIStageList == null)
        {
            return;
        }

        if (mCurAIProperty.mAIStageList.Count < 1)
        {
            return;
        }

        mCurAIActionType = mCurAIProperty.mAIStageList[0].mAIActionType;
    }

    void CheckAI()
    {
        switch (mCurAIActionType)
        {
            case AIActionType.AIActionType_Wait:
                Wait();
                break;

            case AIActionType.AIActionType_Pursuit:
                break;

            case AIActionType.AIActionType_Escape:
                break;

            case AIActionType.AIActionType_Attack:
                break;

            case AIActionType.AIActionType_Skill:
                break;

            default:
                break;
        }
    }

    #region <>等待状态<>

    void Wait()
    {
        mAnimatorComponent.Stand();
    }

    #endregion

    #region <>追击状态<>

    void Pursuit()
    {
        
    }

    #endregion

    void Update()
    {
        
    }
}