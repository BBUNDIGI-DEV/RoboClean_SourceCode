using RoboClean.Data;
using RoboClean.Player;
using UnityEngine;

namespace RoboClean.Character.AI
{
    public class AIMovementActor : ActorBase
    {
        public eAIMovementState CurState
        {
            get; private set;
        }

        private readonly UnityEngine.AI.NavMeshAgent C_NAV_AGENT;
        private readonly AIMovementConfig CONFIG;
        private readonly System.Action ON_PLAYER_FOUND;

        private RuntimePlayerInfo mPlayerInfo;
        private WanderingMovementHelper mWanderingHelper;
        private CombatMovementHelper mCombatMovementHelper;

        public AIMovementActor(Rigidbody rb, CharacterAnimator anim, ActorStateMachine ownerStateMachine, AIMovementConfig config, GameObject gameObject,
            UnityEngine.AI.NavMeshAgent navAgent, System.Action onPlayerFound)
            : base(rb, anim, ownerStateMachine, config.BaseConfig, config.NameID, gameObject)
        {
            C_NAV_AGENT = navAgent;
            CONFIG = config;
            mPlayerInfo = RuntimeDataLoader.PlayerRuntimeInfo;

            if (CONFIG.IsSkipWandering)
            {
                CurState = eAIMovementState.Combat;
                OWNER.SetEnabledUpdate(true, eActorType.AIAttack);
            }
            else
            {
                CurState = eAIMovementState.Wandering;
            }
            ON_PLAYER_FOUND = onPlayerFound;
            SetEnabledUpdating(true);
        }

        public override void UpdateActing()
        {
            if (!NeedUpdate)
            {
                return;
            }
            doMovementAction();
            ANIM.UpdateMovementAnim(C_NAV_AGENT.velocity);
        }

        public override void InovkeActing()
        {
            C_NAV_AGENT.enabled = true;
            RB.isKinematic = true;
            //C_NAV_AGENT.Warp(RB.position);
            ANIM.UpdateMovementAnim(Vector3.zero);
            SetEnabledUpdating(true);
        }

        public override void StopActing()
        {
            C_NAV_AGENT.enabled = false;
            RB.isKinematic = false;
            SetEnabledUpdating(false);
        }

        private void doMovementAction()
        {
            switch (CurState)
            {
                case eAIMovementState.Wandering:
                    doWanderingAction();
                    if (searchPlayer())
                    {
                        CurState = eAIMovementState.Combat;
                        ON_PLAYER_FOUND?.Invoke();
                        OWNER.SetEnabledUpdate(true, eActorType.AIAttack);
                    }
                    break;
                case eAIMovementState.Combat:
                    doCombatMovementAction();
                    break;
                default:
                    break;
            }
        }

        private void doWanderingAction()
        {
            if (mWanderingHelper == null)
            {
                mWanderingHelper = new WanderingMovementHelper(CONFIG.WanderingOption, C_NAV_AGENT);
            }

            mWanderingHelper.UpdateWandering();
        }

        private void doCombatMovementAction()
        {
            if (mCombatMovementHelper == null)
            {
                mCombatMovementHelper = new CombatMovementHelper(CONFIG.CombatMovementOption, C_NAV_AGENT);
            }

            mCombatMovementHelper.UpdateCombat();
        }

        private bool searchPlayer()
        {
            float sqrtDetection = CONFIG.DetectionRange * CONFIG.DetectionRange;
            Vector3 playerToEnemey = mPlayerInfo.Trans.position - RB.position;
            return playerToEnemey.sqrMagnitude < sqrtDetection;
        }

        public override void DestoryActor()
        {

        }
    }

    public enum eAIMovementState
    {
        Pause,
        Wandering,
        Combat,
    }
}