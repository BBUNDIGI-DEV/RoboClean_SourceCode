using RoboClean.Character.Player;
using RoboClean.Data;
using RoboClean.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace RoboClean.Character.AI
{
    public class MonsterStateMachine : MonoBehaviour
    {
        public ActorStateMachine MonsterSM
        {
            get; private set;
        }

        public MonsterConfig Config
        {
            get; private set;
        }

        protected Rigidbody RB;
        protected CharacterAnimator ANIM;
        protected NavMeshAgent AGENT;

        private void FixedUpdate()
        {
            /*
            if(RuntimeDataLoader.RuntimePlayData.IsActiveDialogue)
            {
                return;
            }
            */

            if (MonsterSM.CurrentActorType == eActorType.Appearance)
            {
                return;
            }

            if (MonsterSM.CurrentActorType == eActorType.Dead)
            {
                return;
            }

            if (MonsterSM.CurrentActorType == eActorType.None)
            {
                MonsterSM.TrySwitchActor(eActorType.AIMovement);
            }

            MonsterSM.UpdateActor();
            if (!RB.isKinematic)
            {
                RB.UpdateVelocity();
                RB.UpdateRotation();
                NavMeshHit hitPoint;
                if (NavMesh.SamplePosition(RB.position, out hitPoint, 1.0f, NavMesh.AllAreas))
                {
                    RB.position = hitPoint.position;
                }
            }
        }

        private void OnEnable()
        {
            AimHelper.AddEnemeyTrans(transform);
        }

        private void OnDisable()
        {
            AimHelper.TryRemoveEnemeyTrans(transform);
            if (!GlobalTimer.IsExist)
            {
                return;
            }

            GlobalTimer.Instance.RemoveAllTimerByInstance(gameObject.GetInstanceID());
        }

        public void OnDestroy()
        {
            if (MonsterSM.CurrentActorType != eActorType.Dead)
            {
                MonsterSM.DestoryActors();
            }
        }
        public void Initialize(MonsterConfig config, MonsterStatus status)
        {
            Config = config;
            ANIM = GetComponentInChildren<CharacterAnimator>();
            AGENT = GetComponentInChildren<NavMeshAgent>();
            RB = GetComponentInChildren<Rigidbody>();

            MonsterSM = new ActorStateMachine();

            DamagedActor damagedActor = new DamagedActor(RB, ANIM, MonsterSM, Config.DamagedConfig, gameObject, onDamagedEnd);
            MonsterSM.AddActor(damagedActor);
            DeadActor deadActor = new DeadActor(RB, ANIM, MonsterSM, Config.DeadConfig, gameObject);
            MonsterSM.AddActor(deadActor);
            switch (Config.MonsterType)
            {
                case eMonsterType.Commom:
                    AISkillActor aiAttackActor = new AISkillActor(RB, ANIM, MonsterSM, Config.AISkillConfig, gameObject);
                    MonsterSM.AddActor(aiAttackActor);

                    AppearanceActor appreanceActor = new AppearanceActor(RB, ANIM, MonsterSM, Config.AppearanceConfig, gameObject);
                    MonsterSM.AddActor(appreanceActor);


                    break;
                case eMonsterType.RoboBoss:
                    RoboBossAISkillActor roboSkillActor = new RoboBossAISkillActor(RB, ANIM, MonsterSM, Config.RoboBossSkillConfig, gameObject);
                    MonsterSM.AddActor(roboSkillActor);
                    break;
                default:
                    break;
            }

            AIMovementActor aiMovementActor = new AIMovementActor(RB, ANIM, MonsterSM, Config.AIMovementConfig, gameObject, AGENT
                , () => status.IsEnemeyFoundPlayer.Value = true);
            MonsterSM.AddActor(aiMovementActor);
            StunActor stunActor = new StunActor(RB, ANIM, MonsterSM, gameObject, Config.StunConfig, onStunEnd);
            MonsterSM.AddActor(stunActor);

            switch (Config.MonsterType)
            {
                case eMonsterType.Commom:
                    MonsterSM.TrySwitchActor(eActorType.Appearance);
                    break;
                case eMonsterType.RoboBoss:
                    MonsterSM.TrySwitchActor(eActorType.AIMovement);
                    break;
                default:
                    break;
            }
        }

        public void DoGotDamagedAction(SkillConfig config, bool skipHitStun = false)
        {
            ANIM.PlayOnHitMaterailAnim();
            if (skipHitStun)
            {
                return;
            }
            DamagedActor damagedActor = MonsterSM.GetActor<DamagedActor>(eActorType.Damaged);
            damagedActor.HitSkillConfig = config;
            MonsterSM.TrySwitchActor(eActorType.Damaged);
        }

        public void DoDeadAction(System.Action onDeadEnd)
        {
            MonsterSM.GetActor<DeadActor>(eActorType.Dead).SetOnDeadCallback(onDeadEnd);
            MonsterSM.TrySwitchActor(eActorType.Dead);
        }

        private void onDamagedEnd()
        {
            MonsterSM.TrySwitchActor(eActorType.AIMovement);
        }

        private void onStunEnd()
        {
            MonsterSM.TrySwitchActor(eActorType.AIMovement);
        }
    }
}