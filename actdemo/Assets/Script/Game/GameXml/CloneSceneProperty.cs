using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 副本信息
/// </summary>
public class CloneSceneProperty
{
    /// <summary>
    /// 副本怪物信息
    /// </summary>
    public class CloneSceneMonsterInfo
    {
        /// <summary>
        /// 怪物Id
        /// </summary>
        public int mMonsterId;

        /// <summary>
        /// 怪物位置
        /// </summary>
        public Vector3 mMonsterPosition;

        /// <summary>
        /// 怪物旋转
        /// </summary>
        public float mMonsterRotationY;

        /// <summary>
        /// 怪物缩放
        /// </summary>
        public float mMonsterScale;
    }

    /// <summary>
    /// 副本Id
    /// </summary>
    public int mCloneSceneId;

    /// <summary>
    /// 副本怪物信息列表
    /// </summary>
    public List<CloneSceneMonsterInfo> mCloneSceneMonsterInfoList;
}