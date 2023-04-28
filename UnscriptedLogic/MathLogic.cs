using System;
using UnityEngine;

namespace UnscriptedLogic.MathUtils
{
    public class MathLogic
    {
        public static void ModifyValue(ModifyType modificationType, ref float value, float amount)
        {
            switch (modificationType)
            {
                case ModifyType.Add:
                    value += amount;
                    break;
                case ModifyType.Subtract:
                    value -= amount;
                    break;
                case ModifyType.Set:
                    value = amount;
                    break;
                case ModifyType.Divide:
                    value /= amount;
                    break;
                case ModifyType.Multiply:
                    value *= amount;
                    break;
                case ModifyType.None:
                    break;
                default:
                    break;
            }
        }

        public static bool isWithinRange(float num, Vector2 range)
        {
            return range.x <= num && range.y >= num;
        }

        public static bool isWithinRange(int num, Vector2Int range)
        {
            return range.x <= num && range.y >= num;
        }

        public static Quaternion RotateSmooth(Transform startTransform, Transform targetTransform, float turnSpeed)
        {
            return RotateSmooth(startTransform.position, startTransform.rotation, targetTransform.position, turnSpeed);
        }

        public static Quaternion RotateSmooth(Transform startTransform, Vector3 targetPos, float turnSpeed)
        {
            return RotateSmooth(startTransform.position, startTransform.rotation, targetPos, turnSpeed);
        }

        public static Quaternion RotateSmooth(Vector3 startPos, Quaternion startRotation, Vector3 targetPos, float turnSpeed)
        {
            Vector3 direction = (targetPos - startPos).normalized;
            Quaternion refGoal = Quaternion.LookRotation(direction);
            return Quaternion.Slerp(startRotation, refGoal, turnSpeed);
        }
    }
}
