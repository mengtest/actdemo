using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlowMotionProperty
{
    public int mSlowMotionId;
    public List<SlowMotionStep> mSlowMotionStepList;
}

public class SlowMotionStep
{
    public int mIndex;
    public float mTimeScale;
    public float mTime;
    public float mFieldOfView;
}