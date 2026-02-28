using RoboClean.Data;
using RoboClean.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace RoboClean.Character.AI
{
    public class WanderingMovementHelper
    {
        public MonsterWanderingOption WanderingOption
        {
            get; private set;
        }

        private readonly NavMeshAgent C_AGENT;
        private readonly int INSTANCE_ID;
        private Vector3 mOriginPoint;

        private BasicTimer mRestTimer
        {
            get
            {
                return GlobalTimer.Instance.TryGetTimerOrAdd<BasicTimer>("WanderingRest", eTimerUpdateMode.FixedUpdate, INSTANCE_ID);
            }
        }

        private eAIMovementProgress mWanderingState;

        public WanderingMovementHelper(MonsterWanderingOption wanderingOption, NavMeshAgent agent)
        {
            WanderingOption = wanderingOption;
            mWanderingState = eAIMovementProgress.SetDestination;
            C_AGENT = agent;
            INSTANCE_ID = C_AGENT.GetInstanceID();
            mOriginPoint = C_AGENT.transform.position;
        }

        public void UpdateWandering()
        {
            switch (WanderingOption.Type)
            {
                case eWanderingActionType.Stop:
                    break;
                case eWanderingActionType.RandomWalkAroundRange:
                    doRandomWalkAround();
                    break;
                default:
                    break;
            }
        }

        private void doRandomWalkAround()
        {
            switch (mWanderingState)
            {
                case eAIMovementProgress.GoToDest:
                    if (!C_AGENT.hasPath || C_AGENT.remainingDistance <= C_AGENT.stoppingDistance)
                    {
                        mWanderingState = eAIMovementProgress.SetRestTime;
                    }
                    break;
                case eAIMovementProgress.SetDestination:
                    Vector3 outPoint;
                    bool result = C_AGENT.GetCircularRandomPoint(C_AGENT.transform.position, WanderingOption.WanderingRange, out outPoint);

                    if (result)
                    {
                        C_AGENT.SetDestination(outPoint);
                        mWanderingState = eAIMovementProgress.GoToDest;
                    }
                    else
                    {
                        mWanderingState = eAIMovementProgress.SetRestTime;
                    }
                    break;
                case eAIMovementProgress.Rest:
                    if (!mRestTimer.IsActivate)
                    {
                        mWanderingState = eAIMovementProgress.SetDestination;
                    }
                    break;
                case eAIMovementProgress.SetRestTime:
                    mRestTimer
                        .ChangeDuration(Random.Range(WanderingOption.MinWaitTime, WanderingOption.MaxWaitTime))
                        .StartTimer();
                    mWanderingState = eAIMovementProgress.Rest;
                    break;
                default:
                    Debug.LogError("DefaultDrop");
                    break;
            }
        }
    }

    public enum eAIMovementProgress
    {
        GoToDest,
        SetDestination,
        Rest,
        SetRestTime,
    }
}