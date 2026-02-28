using RoboClean.Data;
using RoboClean.Player;
using UnityEngine;

namespace RoboClean.Character.AI
{
    public class RoboBossAISkillActor : ActorBase
    {
        protected readonly RoboBossAISkillConfig CONFIG;
        protected readonly SkillActor NORMAL_ATTACK_ACTOR;
        protected readonly ComboSkillActor RUSH_ATTACK_ACTOR; // Rush Skill
        protected readonly SkillActor PROJECTILE_ATTACK_ACTOR; // Projectile Actor

        private bool IsRushHitPlayer;
        private bool IsRushHitObstacle;


        private int mLastProjectileAttackIndex;
        private int mBufferedProjectileAttackIndex = -1;
        private eRoboAttackState mCurrentAttackState;

        private bool isRushable
        {
            get
            {
                return mPlayerInfo.IsPlayerInRange(RB.position, CONFIG.RushableRange)
                    && RUSH_ATTACK_ACTOR.CanAttack;
            }
        }

        private bool isNormalAttackable
        {
            get
            {
                return mPlayerInfo.IsPlayerInRange(RB.position, CONFIG.NormalAttackRange)
                    && NORMAL_ATTACK_ACTOR.CanAttack;
            }
        }

        private bool isProjectileAttackable
        {
            get
            {
                return PROJECTILE_ATTACK_ACTOR.CanAttackWithoutAfterCastDelay;
            }
        }


        private RuntimePlayerInfo mPlayerInfo
        {
            get
            {
                return RuntimeDataLoader.PlayerRuntimeInfo;
            }
        }

        public RoboBossAISkillActor(Rigidbody rb, CharacterAnimator anim, ActorStateMachine ownerStateMachine, RoboBossAISkillConfig config, GameObject gameobject) :
            base(rb, anim, ownerStateMachine, config.BaseConfig, config.NameID, gameobject)
        {
            CONFIG = config;

            NORMAL_ATTACK_ACTOR = new SkillActor(rb, anim, ownerStateMachine, CONFIG.NormalSkills[0], gameobject, onAttackEnd);
            OWNER.AddActor(NORMAL_ATTACK_ACTOR);
            RUSH_ATTACK_ACTOR = new ComboSkillActor(rb, anim, ownerStateMachine, CONFIG.RushSkills[0], gameobject, onAttackEnd);
            OWNER.AddActor(RUSH_ATTACK_ACTOR);
            PROJECTILE_ATTACK_ACTOR = new SkillActor(rb, anim, ownerStateMachine, CONFIG.ProjectileSkills[0], gameobject, onAttackEnd);
            OWNER.AddActor(PROJECTILE_ATTACK_ACTOR);

            mCurrentAttackState = eRoboAttackState.NoneAttack;
            gameobject.GetComponentInParent<MonsterBase>().Status.NormalizedHP.AddListener(trySetProjectileAttackIndex);
        }

        public override void StopActing()
        {

        }


        public override void DestoryActor()
        {

        }

        public override void UpdateActing()
        {
            switch (mCurrentAttackState)
            {
                case eRoboAttackState.NoneAttack:
                    eRoboAttackState newAttack = tryAttack();
                    mCurrentAttackState = newAttack;
                    break;
                case eRoboAttackState.NormalAttack:
                    if (NORMAL_ATTACK_ACTOR.IsOnAttack)
                    {
                        return;
                    }
                    mCurrentAttackState = eRoboAttackState.NoneAttack;
                    break;
                case eRoboAttackState.RushAttack:
                    if (IsRushHitPlayer)
                    {
                        OWNER.TrySwitchActor(RUSH_ATTACK_ACTOR.ActorType);
                        mCurrentAttackState = eRoboAttackState.NoneAttack;
                        IsRushHitPlayer = false;
                        return;
                    }
                    else if (IsRushHitObstacle)
                    {
                        mCurrentAttackState = eRoboAttackState.NoneAttack;
                        IsRushHitObstacle = false;
                        return;
                    }
                    if (!RUSH_ATTACK_ACTOR.IsOnAttack)
                    {
                        mCurrentAttackState = eRoboAttackState.NoneAttack;
                        return;
                    }

                    break;
                case eRoboAttackState.ProjectileAttack:
                    if (PROJECTILE_ATTACK_ACTOR.IsOnAttack)
                    {
                        return;
                    }
                    mCurrentAttackState = eRoboAttackState.NoneAttack;
                    break;
                default:
                    break;
            }
        }


        public void SetRushHitPlayerFlag()
        {
            IsRushHitPlayer = true;
        }

        public void SetRushHitObastacleFlag()
        {
            IsRushHitObstacle = true;
        }

        private eRoboAttackState tryAttack()
        {
            if (OWNER.CurrentActorType == eActorType.Stun)
            {
                return eRoboAttackState.NoneAttack;
            }

            if (mBufferedProjectileAttackIndex != -1)
            {
                if (!isProjectileAttackable)
                {
                    return eRoboAttackState.NoneAttack;
                }

                PROJECTILE_ATTACK_ACTOR.TryChangeConfigOrStackedIn(CONFIG.ProjectileSkills[mBufferedProjectileAttackIndex]);
                OWNER.TrySwitchActor(PROJECTILE_ATTACK_ACTOR.ActorType);
                mBufferedProjectileAttackIndex = -1;
                return eRoboAttackState.ProjectileAttack;
            }

            if (isRushable)
            {
                OWNER.TrySwitchActor(RUSH_ATTACK_ACTOR.ActorType);
                return eRoboAttackState.RushAttack;
            }

            if (isNormalAttackable)
            {
                OWNER.TrySwitchActor(NORMAL_ATTACK_ACTOR.ActorType);
                return eRoboAttackState.NormalAttack;
            }

            return eRoboAttackState.NoneAttack;
        }

        private void onAttackEnd()
        {
            OWNER.TrySwitchActor(eActorType.AIMovement);
        }

        private void trySetProjectileAttackIndex(float normalizedHP)
        {
            if (mLastProjectileAttackIndex == CONFIG.ProjectileSkillInvokeHP.Length)
            {
                return;
            }
            float invokeHP = CONFIG.ProjectileSkillInvokeHP[mLastProjectileAttackIndex];

            if (invokeHP > normalizedHP)
            {
                mBufferedProjectileAttackIndex = mLastProjectileAttackIndex;
                mLastProjectileAttackIndex++;
            }
        }
    }

    public enum eRoboAttackState
    {
        NoneAttack,
        NormalAttack,
        RushAttack,
        ProjectileAttack,
    }
}