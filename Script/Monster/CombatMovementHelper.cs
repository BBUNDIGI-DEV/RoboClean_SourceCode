using UnityEngine.AI;
using UnityEngine;
using RoboClean.Data;
using RoboClean.Utils;

namespace RoboClean.Character.AI
{
    public class CombatMovementHelper
    {
        public MonsterCombatMovementOption CombatMovementOption
        {
            get; private set;
        }

        private readonly NavMeshAgent mAgent;
        private readonly int mInstanceID;

        private BasicTimer mEvadeRestTimer
        {
            get
            {
                return GlobalTimer.Instance.TryGetTimerOrAdd<BasicTimer>("EvadeResting", eTimerUpdateMode.FixedUpdate, mInstanceID);
            }
        }

        private Vector3 PlayerPos
        {
            get
            {
                return RuntimeDataLoader.PlayerRuntimeInfo.Trans.position;
            }
        }

        private eAIMovementProgress mMovementState;

        public CombatMovementHelper(MonsterCombatMovementOption combatMovementOption, NavMeshAgent agent)
        {
            CombatMovementOption = combatMovementOption;
            mMovementState = eAIMovementProgress.SetDestination;
            mAgent = agent;
            mInstanceID = mAgent.GetInstanceID();
            if (CombatMovementOption.MovementType == eCombatMovementType.RushToPlayer)
            {
                mAgent.speed = mAgent.speed * CombatMovementOption.RushSpeedIncreasement;
            }
        }

        public void UpdateCombat()
        {
            switch (CombatMovementOption.MovementType)
            {
                case eCombatMovementType.RushToPlayer:
                    rushToPlayer();
                    break;
                case eCombatMovementType.EvadeFromPlayer:
                    evadeFromPlayer();
                    break;
                default:
                    Debug.LogError($"switch default state[{CombatMovementOption.MovementType}]");
                    break;
            }
        }

        private void rushToPlayer()
        {
            mAgent.SetDestination(PlayerPos);
        }

        private void evadeFromPlayer()
        {
            switch (mMovementState)
            {
                case eAIMovementProgress.GoToDest:
                    if (!mAgent.hasPath || mAgent.remainingDistance <= mAgent.stoppingDistance)
                    {
                        mMovementState = eAIMovementProgress.SetRestTime;
                    }
                    break;
                case eAIMovementProgress.SetDestination:
                    Vector3 agentPos = mAgent.transform.position;
                    Vector3 playerToEnemey = agentPos - PlayerPos;
                    float playerToEnemeyDistance = playerToEnemey.magnitude;
                    float evadeDistance = Random.Range(3.5f, 4.0f);
                    float lerpFactor = Mathf.Min(1.0f, playerToEnemeyDistance / CombatMovementOption.MaxEvadeDistance);
                    evadeDistance = Mathf.Lerp(evadeDistance, 0, lerpFactor);

                    Vector3 destPos = agentPos + playerToEnemey * evadeDistance;
                    bool result = mAgent.GetCircularRandomPoint(destPos, 5.0f, out destPos);

                    if (result)
                    {
                        mAgent.SetDestination(destPos);
                        mMovementState = eAIMovementProgress.GoToDest;
                    }
                    else
                    {
                        mMovementState = eAIMovementProgress.SetRestTime;
                    }
                    break;
                case eAIMovementProgress.Rest:
                    if (!mEvadeRestTimer.IsActivate)
                    {
                        mMovementState = eAIMovementProgress.SetDestination;
                    }

                    break;
                case eAIMovementProgress.SetRestTime:
                    mEvadeRestTimer
                        .ChangeDuration(Random.Range(CombatMovementOption.MinWaitTime, CombatMovementOption.MaxWaitTime))
                        .StartTimer();
                    mMovementState = eAIMovementProgress.Rest;
                    break;
                default:
                    break;
            }
        }
    }
}
