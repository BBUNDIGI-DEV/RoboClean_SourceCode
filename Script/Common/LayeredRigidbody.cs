using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.Character
{
    public class LayeredRigidbody
    {
        private readonly ActorPriortyValueContainer<Vector3> VELOCITY_PRIORITY;
        private readonly ActorPriortyValueContainer<Quaternion> ROT_PRIORITY;

        private Dictionary<eSpeedMultiplierSource, float> mSpeedMultiplierDic;
        private float mZAxisPusher = 1.2f;

        public LayeredRigidbody()
        {
            VELOCITY_PRIORITY = new ActorPriortyValueContainer<Vector3>(new eActorType[]
            {
                eActorType.Damaged,
                eActorType.Stun,
                eActorType.Dash,
                eActorType.NormalAttack,
                eActorType.SpecialAttack,
                eActorType.SpecialAttack2,
                eActorType.DashAttack,
                eActorType.InputMovement,
                eActorType.AIMovement,
            }
            );

            ROT_PRIORITY = new ActorPriortyValueContainer<Quaternion>(new eActorType[]
            {
            eActorType.DashAttack,
            eActorType.Damaged,
            eActorType.Dash,
            eActorType.NormalAttack,
            eActorType.SpecialAttack,
            eActorType.SpecialAttack2,
            eActorType.AIMovement,
            eActorType.InputMovement,
            }, new eActorType[]
            {
            eActorType.InputMovement
            });

            mSpeedMultiplierDic = new Dictionary<eSpeedMultiplierSource, float>();

            for (int i = 0; i < (int)eSpeedMultiplierSource.Count; i++)
            {
                eSpeedMultiplierSource speedMulti = (eSpeedMultiplierSource)i;
                mSpeedMultiplierDic.Add(speedMulti, 1.0f);
            }
        }

        public void EnrollVelocity(Vector3 velocity, eActorType actorType)
        {
            VELOCITY_PRIORITY.EnrollValue(velocity, actorType);
        }

        public void EnrollRotation(Quaternion quaternion, eActorType actorType)
        {
            ROT_PRIORITY.EnrollValue(quaternion, actorType);
        }

        public void DisEnrollVelocity(eActorType actorType)
        {
            VELOCITY_PRIORITY.DisEnrollValue(actorType);
        }

        public void DisEnrollRotation(eActorType actorType)
        {
            ROT_PRIORITY.DisEnrollValue(actorType);
        }

        public bool TryGetVelocity(out Vector3 velocity, out eActorType actor)
        {
            bool result = VELOCITY_PRIORITY.TryGetValue(out velocity, out actor);
            if (!result)
            {
                return false;
            }
            velocity *= getSpeedMultiplier();
            velocity.Set(velocity.x, velocity.y, velocity.z * mZAxisPusher);
            return true;
        }

        public bool TryGetRotation(Quaternion current, out Quaternion quaternion, out eActorType actor)
        {
            bool result = ROT_PRIORITY.TryGetValue(out quaternion, out actor);
            if (!result)
            {
                return false;
            }

            quaternion = Quaternion.Slerp(current, quaternion, 0.35f);
            return true;
        }

        public void SetVelocityMultiplier(float multiplier, eSpeedMultiplierSource source)
        {
            mSpeedMultiplierDic[source] = multiplier;
        }

        public void SetZAxisPusher(float zAxisPusher)
        {
            mZAxisPusher = zAxisPusher;
        }

        private float getSpeedMultiplier()
        {
            float totalMultiplier = 1.0f;
            foreach (var item in mSpeedMultiplierDic)
            {
                totalMultiplier *= item.Value;
            }
            return totalMultiplier;
        }
    }

    public enum eSpeedMultiplierSource
    {
        Toxic,
        GotHit,
        Count,
    }

    public class ActorPriortyValueContainer<T> where T : struct
    {
        private readonly eActorType[] PROGRESS_PRIORITY;
        private readonly eActorType[] OVERIDED_ACTOR;
        private T[] mEnrolledValues;
        private bool[] mIsValueEnrolled;
        private int mTopPriorityIndex;

        public ActorPriortyValueContainer(eActorType[] proirty, eActorType[] overidedActors = null)
        {
            PROGRESS_PRIORITY = proirty;
            OVERIDED_ACTOR = overidedActors;
            mEnrolledValues = new T[(int)eActorType.Count];
            mIsValueEnrolled = new bool[(int)eActorType.Count];
            mTopPriorityIndex = int.MaxValue;

#if UNITY_EDITOR
            for (int i = 0; i < PROGRESS_PRIORITY.Length; i++)
            {
                int count = 0;
                eActorType current = PROGRESS_PRIORITY[i];
                for (int j = 0; j < PROGRESS_PRIORITY.Length; j++)
                {
                    if (current == PROGRESS_PRIORITY[j])
                    {
                        count++;
                        if (count > 1)
                        {
                            Debug.LogError($"Duplicate progress priority detected [{current}]");
                        }
                    }

                }
            }
#endif
        }

        public bool TryGetValue(out T outValue, out eActorType actor)
        {
            if (mTopPriorityIndex == int.MaxValue)
            {
                actor = eActorType.None;
                outValue = default(T);
                return false;
            }

            Debug.Assert(mIsValueEnrolled[mTopPriorityIndex],
                $"You Cannot get value not enrolled [{findActorFromPriority(mTopPriorityIndex)}]");

            actor = findActorFromPriority(mTopPriorityIndex);
            outValue = mEnrolledValues[mTopPriorityIndex];

            return true;
        }


        public void EnrollValue(T value, eActorType actorType)
        {
            int priorityIndex = findPriority(actorType);

            mIsValueEnrolled[priorityIndex] = true;
            mEnrolledValues[priorityIndex] = value;
            if (priorityIndex < mTopPriorityIndex)
            {
                mTopPriorityIndex = priorityIndex;
            }

            if (OVERIDED_ACTOR != null)
            {
                tryUpdateOveridedActor(actorType);
            }
        }

        public void DisEnrollValue(eActorType actorType)
        {
            int priorityIndex = findPriority(actorType);
            mIsValueEnrolled[priorityIndex] = false;
            if (mTopPriorityIndex != priorityIndex)
            {
                return;
            }

            bool isTopPriorityUpdated = false;
            for (int i = 0; i < mEnrolledValues.Length; i++)
            {
                if (mIsValueEnrolled[i])
                {
                    mTopPriorityIndex = i;
                    isTopPriorityUpdated = true;
                    break;
                }
            }

            if (!isTopPriorityUpdated)
            {
                mTopPriorityIndex = int.MaxValue;
            }
        }

        private int findPriority(eActorType actorType)
        {
            for (int i = 0; i < PROGRESS_PRIORITY.Length; i++)
            {
                if (actorType == PROGRESS_PRIORITY[i])
                {
                    return i;
                }
            }

            return PROGRESS_PRIORITY.Length;
        }

        private eActorType findActorFromPriority(int i)
        {
            if (i == PROGRESS_PRIORITY.Length)
            {
                return eActorType.None;
            }
            return PROGRESS_PRIORITY[i];
        }

        private void tryUpdateOveridedActor(eActorType enrolledActor)
        {
            int newEnrolledPriority = findPriority(enrolledActor);
            for (int i = 0; i < OVERIDED_ACTOR.Length; i++)
            {
                int priority = findPriority(OVERIDED_ACTOR[i]);

                if (!mIsValueEnrolled[priority] || priority <= newEnrolledPriority)
                {
                    continue;
                }
                mEnrolledValues[priority] = mEnrolledValues[newEnrolledPriority];
            }
        }
    }
}