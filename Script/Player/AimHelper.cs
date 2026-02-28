using RoboClean.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.Character.Player
{
    public static class AimHelper
    {
        private static List<Transform> mEnemyTransList;
        private static List<Vector3> mNearlestPlayerToEnemey;

        static AimHelper()
        {
            mEnemyTransList = new List<Transform>(32);
            mNearlestPlayerToEnemey = new List<Vector3>(32);
        }

        public static void AddEnemeyTrans(Transform trans)
        {
            Debug.Assert(!checkContain(trans), $"you just passed aready contained enemey to aim helper[{trans.gameObject.name}]");
            mEnemyTransList.Add(trans);
        }

        public static void RemoveEnemeyTrans(Transform trans)
        {
            Debug.Assert(checkContain(trans), $"you try to remove not in list enemey in aim helper[{trans.gameObject.name}]");
            mEnemyTransList.Remove(trans);
        }
        public static void TryRemoveEnemeyTrans(Transform trans)
        {
            if (!checkContain(trans))
            {
                return;
            }
            mEnemyTransList.Remove(trans);
        }

        public static bool TryToGetAimAssist(ref Vector3 attackAim, out Vector3 outTargetEnemey, Vector3 playerPos, AimAssistanceConfig config)
        {
            outTargetEnemey = Vector3.zero;
            float minQuantanizeCos = float.MinValue;
            float minMagnitude = float.MinValue;
            bool isFounded = false;

            for (int i = 0; i < mEnemyTransList.Count; i++)
            {
                Vector3 enemyPos = mEnemyTransList[i].position;
                Vector3 playerToEnemy = enemyPos - playerPos;
                playerToEnemy.Set(playerToEnemy.x, 0.0f, playerToEnemy.z);
                float playerToEnemyDistance = playerToEnemy.magnitude;
                if (playerToEnemyDistance > config.MaxAssistanceDistance)
                {
                    continue;
                }

                Vector3 normalizedPlayerToEnemey = playerToEnemy.normalized;
                float normalizedDistance = playerToEnemyDistance / config.MaxAssistanceDistance;
                float assistanceAngle = config.EvaluateAngle(normalizedDistance);
                float assitanceCos = Mathf.Cos(assistanceAngle * Mathf.Deg2Rad);
                float cos = Vector3.Dot(normalizedPlayerToEnemey, attackAim);

                if (cos < assitanceCos)
                {
                    continue;
                }

                if (isFounded == false)
                {
                    outTargetEnemey = enemyPos;
                    minQuantanizeCos = cos;
                    minMagnitude = playerToEnemyDistance;
                    attackAim = normalizedPlayerToEnemey;
                    isFounded = true;
                    continue;
                }

                const int COS_QUNTANIZE_ANGLE = 10;
                float quntanizedAngle = quantanizeCosByAngle(cos, COS_QUNTANIZE_ANGLE);

                if (quntanizedAngle > minQuantanizeCos)
                {
                    outTargetEnemey = enemyPos;
                    minQuantanizeCos = quntanizedAngle;
                    minMagnitude = playerToEnemyDistance;
                    attackAim = normalizedPlayerToEnemey;
                }
                else if (quntanizedAngle == minQuantanizeCos)
                {
                    if (playerToEnemyDistance < minMagnitude)
                    {
                        outTargetEnemey = enemyPos;
                        minQuantanizeCos = cos;
                        minMagnitude = playerToEnemyDistance;
                        attackAim = normalizedPlayerToEnemey;
                    }
                }
            }

            return isFounded;
        }

        private static bool checkContain(Transform trans)
        {
            for (int i = 0; i < mEnemyTransList.Count; i++)
            {
                if (mEnemyTransList[i].GetInstanceID() == trans.GetInstanceID())
                {
                    return true;
                }
            }
            return false;
        }
        private static float quantanizeCosByAngle(float cos, float threholdByAngle)
        {
            int step = (int)Mathf.Round((180 / threholdByAngle));

            cos += 1; //ReRange To Cos from -1 ~ 1 to 0 ~ 2;
            cos *= step; //ReRange to cos from 0 ~ 2 to 0 ~ 2 * step;
            float ceiling = Mathf.Ceil(cos);
            cos = ceiling / step;
            cos -= 1;
            return cos;
        }
    }
}