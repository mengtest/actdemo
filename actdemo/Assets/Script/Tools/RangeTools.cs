using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangeTools
{
    /// <summary>
    /// 判断对象是否在一个对象的范围内
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target"></param>
    /// <param name="maxDistance"></param>
    /// <param name="width"></param>
    /// <param name="rangeType"></param>
    /// <returns></returns>
    public static bool IsObjectInRange(IObject self, IObject target, float maxDistance, SkillRangeType rangeType, float width, float minDistance = 0.0f)
    {
        if (self == null || target == null)
        {
            return false;
        }

        if (Mathf.Abs(maxDistance) < 0.01)
        {
            return false;
        }

        if (rangeType >= SkillRangeType.SkillRangeType_MaxCount ||
            rangeType <= SkillRangeType.SkillRangeType_Unknown )
        {
            return false;
        }

        Vector3 selfPosition = self.mPosition;
        Vector3 selfForward = self.mGameObject.transform.forward;
        Quaternion r = self.mGameObject.transform.rotation;
        Vector3 targetPosition = target.mPosition;
        Vector3 direction = (targetPosition - selfPosition).normalized;

        bool isIn = false;
        switch (rangeType)
        {
            case SkillRangeType.SkillRangeType_Single:

                break;

            case SkillRangeType.SkillRangeType_SelfLine:
//                Vector3 left = selfPosition + (r * Vector3.left) * width / 2;
//                Vector3 right = selfPosition + (r * Vector3.right) * width / 2;
//                Vector3 leftEnd = (left + (r * Vector3.forward) * maxDistance);
//                Vector3 rightEnd = (right + (r * Vector3.forward) * maxDistance);
//				Debug.Log("left = "+left);
//				Debug.Log("right = "+right);
//				Debug.Log("leftEnd = "+leftEnd);
//				Debug.Log("rightEnd = "+rightEnd);
//			singletonNpcTest.GetSingletonNpcTest().svv[0] = left;
//			singletonNpcTest.GetSingletonNpcTest().svv[1] = right;
//			singletonNpcTest.GetSingletonNpcTest().svv[2] = leftEnd;
//			singletonNpcTest.GetSingletonNpcTest().svv[3] = rightEnd;
//
//
//                if (IsInRect(targetPosition, leftEnd, rightEnd, right, left))
//                {
//				Debug.Log("============true=================");
//                    isIn = true;
//                }
				
                if (AttackRangeDefine.IsLine(target.mPosition ,self.mPosition,self.mGameObject.transform.forward,width,maxDistance)) 
                {
                    isIn = true;
                }
				else
				{
					isIn = false;
				}
                break;

            // 此处width为扇形夹角
            case SkillRangeType.SkillRangeType_SelfSector:
                Vector3 f0 = selfPosition + (r * Vector3.forward) * maxDistance;

                Quaternion r0 = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y - width / 2, r.eulerAngles.z);
                Quaternion r1 = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y + width / 2, r.eulerAngles.z);

                Vector3 f1 = selfPosition + (r0 * Vector3.forward) * maxDistance;
                Vector3 f2 = selfPosition + (r1 * Vector3.forward) * maxDistance;

                if (IsInTriangle(targetPosition, selfPosition, f1, f0) || IsInTriangle(targetPosition, selfPosition, f2, f0))
                {
                    isIn = true;
                }
                break;

            case SkillRangeType.SkillRangeType_SelfRound:
                if (Vector3.Distance(selfPosition, targetPosition) <= maxDistance &&
                    Vector3.Distance(selfPosition, targetPosition) > minDistance)
                {
                    isIn = true;
                }
                break;

            default:
                break;
        }

        return isIn;
    }

    /// <summary>
    /// 修改对象AOI列表
    /// </summary>
    /// <param name="role"></param>
    /// <param name="target"></param>
    public static void MotifyObjectAoi(IObject self, IObject target)
    {
        if (self == null || self.mAOIList == null || target == null || self == target)
        {
            return;
        }

        if (self.mAOIRange < 0.01)
        {
            LogSystem.LogError("RoleAOIRange less than 0.01");

            return;
        }

        if (Vector3.Distance(self.mPosition, target.mPosition) <= self.mAOIRange && !target.mIsDead)
        {
            if (!self.mAOIList.Contains(target))
            {
                self.mAOIList.Add(target);

                MotifyObjectAoi(target, self);
            }
        }
        else
        {
            if (self.mAOIList.Contains(target))
            {
                self.mAOIList.Remove(target);
            }
        }
    }

    public static void MotifyRoleAoi()
    {
        if (ObjectManager.mRole == null || ObjectManager.mObjectsList == null)
        {
            return;
        }

        int count = ObjectManager.mObjectsList.Count;
        if (count < 1)
        {
            return;
        }

        for (int i = 0; i < count; ++i)
        {
            IObject obj = ObjectManager.mObjectsList[i];
            if (obj == null)
            {
                continue;
            }

            MotifyObjectAoi(ObjectManager.mRole, obj);
        }
    }

    static float triangleArea(float v0x, float v0y, float v1x, float v1y, float v2x, float v2y)
    {
        return Mathf.Abs((v0x * v1y + v1x * v2y + v2x * v0y - v1x * v0y - v2x * v1y - v0x * v2y) / 2.0f);
    }

    static bool IsInTriangle(Vector3 target, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float x = target.x;
        float y = target.z;

        float v0x = v0.x;
        float v0y = v0.z;

        float v1x = v1.x;
        float v1y = v1.z;

        float v2x = v2.x;
        float v2y = v2.z;

        float t = triangleArea(v0x, v0y, v1x, v1y, v2x, v2y);
        float a = triangleArea(v0x, v0y, v1x, v1y, x, y) + 
            triangleArea(v0x, v0y, x, y, v2x, v2y) + 
            triangleArea(x, y, v1x, v1y, v2x, v2y);

        if (Mathf.Abs(t - a) <= 0.01f)
        {
            return true;
        }

        return false;
    }

    static float Multiply(float p1x, float p1y, float p2x, float p2y, float p0x, float p0y)
    {
        return ((p1x - p0x) * (p2y - p0y) - (p2x - p0x) * (p1y - p0y));
    }

    static bool IsInRect(Vector3 target, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float x = target.x;
        float y = target.y;

        float v0x = v0.x;
        float v0y = v0.y;

        float v1x = v1.x;
        float v1y = v1.y;

        float v2x = v2.x;
        float v2y = v2.y;

        float v3x = v3.x;
        float v3y = v3.y;

        if (Multiply(x, y, v0x, v0y, v1x, v1y) * Multiply(x, y, v3x, v3y, v2x, v2y) <= 0 &&
            Multiply(x, y, v3x, v3y, v0x, v0y) * Multiply(x, y, v2x, v2y, v1x, v1y) <= 0)
        {
            return true;
        }

        return false;
    }

//     public static List<IObject> GetEnemyByAOI(Skill skillObj)
//     {
//         if (skillObj == null)
//         {
//             return null;
//         }
// 
//         return GetEnemyByAOI(skillObj.mSelfObj, (SkillRangeType)1, 90.0f, skillObj.mSkillProperty.mSkillRange, 0.0f);
//     }

    /// <summary>
    /// 根据AOI获取可攻击范围内列表
    /// </summary>
    /// <param name="self"></param>
    /// <param name="type"></param>
    /// <param name="width"></param>
    /// <param name="maxDistance"></param>
    /// <param name="minDistance"></param>
    public static List<IObject> GetEnemyByAOI(IObject self, SkillRangeType type, float maxDistance, float width, float minDistance = 0.0f)
    {
        if (self == null || self.mAOIList == null)
        {
            return null;
        }

        List<IObject> list = new List<IObject>();

        if (type == SkillRangeType.SkillRangeType_Self)
        {
            list.Add(self);
        }
        else if (type == SkillRangeType.SkillRangeType_Single)
        {
            IObject target = null;
            float tempDistance = float.MaxValue;
            for (int i = 0; i < self.mAOIList.Count; ++i)
            {
                IObject obj = self.mAOIList[i];
                if (obj == null)
                {
                    continue;
                }

                float distance = UtilTools.Vec2Distance(self, obj);
                if (distance <= maxDistance && distance < tempDistance)
                {
                    tempDistance = distance;
                    target = obj;
                }
            }

            list.Add(target);
        }
        else
        {
            for (int i = 0; i < self.mAOIList.Count; ++i)
            {
                IObject obj = self.mAOIList[i];

                if (!IsObjectInRange(self, obj, maxDistance, type, width, minDistance))
                {
                    continue;
                }

                list.Add(obj);
            }
        }

        return list;
    }
}