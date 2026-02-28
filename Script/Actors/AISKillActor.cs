using RoboClean.Data;
using RoboClean.Player;
using RoboClean.Utils;
using System.Collections;
using UnityEngine;

namespace RoboClean.Character.AI
{
    public class AISkillActor : ActorBase
    {
        protected readonly AISkillConfig CONFIG;
        protected readonly SkillActor SKILL_ACTOR;
        protected readonly SkillActor TOXIC_SPREAD_SKILL_ACTOR;

        private RuntimePlayerInfo PlayerInfo
        {
            get
            {
                return RuntimeDataLoader.PlayerRuntimeInfo;
            }
        }
        private bool mIsOnAttack;
        private int mLastSkillTear;

        public AISkillActor(Rigidbody rb, CharacterAnimator anim, ActorStateMachine ownerStateMachine, AISkillConfig config, GameObject gameobject) :
            base(rb, anim, ownerStateMachine, config.BaseConfig, config.NameID, gameobject)
        {
            CONFIG = config;
            switch (CONFIG.CombatType)
            {
                case eCombatType.None:
                    break;
                case eCombatType.MeleeAttackInRange:
                    SKILL_ACTOR = new SkillActor(rb, anim, ownerStateMachine, CONFIG.SkillConfigs[0], gameobject, onAttackEnd);
                    OWNER.AddActor(SKILL_ACTOR);
                    TOXIC_SPREAD_SKILL_ACTOR = new SkillActor(rb, anim, ownerStateMachine, CONFIG.SludgeSpreadSkillConfig, gameobject, onAttackEnd);
                    OWNER.AddActor(TOXIC_SPREAD_SKILL_ACTOR);

                    RuntimeDataLoader.RuntimeStageData.ToxicGuage.AddListener(tryChangeSkillConfigByToxicGuage, true);
                    break;
                case eCombatType.ShootProjectileTowardPlayer:
                    SKILL_ACTOR = new SkillActor(rb, anim, ownerStateMachine, CONFIG.SkillConfigs[0], gameobject, onAttackEnd);
                    OWNER.AddActor(SKILL_ACTOR);
                    TOXIC_SPREAD_SKILL_ACTOR = new SkillActor(rb, anim, ownerStateMachine, CONFIG.SludgeSpreadSkillConfig, gameobject, onAttackEnd);
                    OWNER.AddActor(TOXIC_SPREAD_SKILL_ACTOR);

                    RuntimeDataLoader.RuntimeStageData.ToxicGuage.AddListener(tryChangeSkillConfigByToxicGuage, true);
                    break;
                default:
                    Debug.LogError($"AI Skill Config Cannot handle this combat type [{CONFIG.CombatType}]");
                    break;
            }
            SetEnabledUpdating(false); //Update will be set as true when movement ai actor search player
        }

        public override void InovkeActing()
        {
            SetEnabledUpdating(true);
        }

        public override void StopActing()
        {
            SetEnabledUpdating(false);
        }

        public override void UpdateActing()
        {
            switch (CONFIG.CombatType)
            {
                case eCombatType.None:
                    break;
                case eCombatType.MeleeAttackInRange:
                    doMeleeAttackInRange();
                    break;
                case eCombatType.ShootProjectileTowardPlayer:
                    doShootProjectileTowardPlayer();
                    break;
                case eCombatType.RobotBoss:
                default:
                    Debug.LogError(CONFIG.CombatType);
                    break;
            }
        }

        public override void DestoryActor()
        {
            if (CONFIG.CombatType != eCombatType.None)
            {
                RuntimeDataLoader.RuntimeStageData.ToxicGuage.RemoveListener(tryChangeSkillConfigByToxicGuage);
            }
        }


        private void tryChangeSkillConfigByToxicGuage(float newToxicGuage)
        {
            int skillTear;
            SkillConfig config = checkToxicGaugeAndChangeSkillConfig(out skillTear, newToxicGuage);

            if (mLastSkillTear == skillTear)
            {
                return;
            }
            mLastSkillTear = skillTear;
            SKILL_ACTOR.TryChangeConfigOrStackedIn(config);
        }

        private SkillConfig checkToxicGaugeAndChangeSkillConfig(out int newTear, float newToxicGuage)
        {
            int skillTear = 0;
            skillTear = CONFIG.UpgradingToxicGauge.FindCloseIndex(newToxicGuage);
            newTear = skillTear;
            return CONFIG.SkillConfigs[skillTear];
        }

        private void doMeleeAttackInRange()
        {
            Vector3 playerPos = PlayerInfo.Trans.position;
            float playerToEnemey = (playerPos - RB.position).sqrMagnitude;
            float attackRange = CONFIG.MeleeAttackRange * CONFIG.MeleeAttackRange;

            if (playerToEnemey > attackRange)
            {
                return;
            }

            if (TOXIC_SPREAD_SKILL_ACTOR.CanAttack && !mIsOnAttack)
            {
                OWNER.TrySwitchActor(eActorType.SpecialAttack);
                mIsOnAttack = true;
            }
            else if (SKILL_ACTOR.CanAttack && !mIsOnAttack)
            {
                OWNER.TrySwitchActor(eActorType.NormalAttack);
                mIsOnAttack = true;
            }
        }

        private void doShootProjectileTowardPlayer()
        {
            Vector3 playerPos = PlayerInfo.Trans.position;
            float playerToEnemey = (playerPos - RB.position).sqrMagnitude;
            float minRange = CONFIG.ProjectileMinRange * CONFIG.ProjectileMinRange;

            if (TOXIC_SPREAD_SKILL_ACTOR.CanAttack && !mIsOnAttack)
            {
                OWNER.TrySwitchActor(eActorType.SpecialAttack);
                mIsOnAttack = true;
                return;
            }

            if (playerToEnemey < minRange || !SKILL_ACTOR.CanAttack || mIsOnAttack)
            {
                return;
            }

            OWNER.TrySwitchActor(eActorType.NormalAttack);
        }

        private void onAttackEnd()
        {
            switchToMovement();
            mIsOnAttack = false;
        }

        private void switchToMovement()
        {
            OWNER.TrySwitchActor(eActorType.AIMovement);
        }
    }
}