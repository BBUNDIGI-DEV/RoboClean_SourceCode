using RoboClean.Data;
using RoboClean.Utils;
using UnityEngine;

namespace RoboClean.Character.Player
{
    public class DamagedActor : ActorBase
    {
        public bool IsInHitStun
        {
            get; private set;
        }

        public SkillConfig HitSkillConfig;
        private bool mNeedSkipHitStun;

        private readonly DamagedActorConfig CONFIG;
        private readonly GameObjectPool EFFECT_POOL;
        private readonly System.Action ON_DAMAGED_END;

        private ChainedTimer mStunTimer
        {
            get
            {
                return GlobalTimer.Instance
               .TryGetTimerOrAdd<ChainedTimer>("StunTimer" + CONFIG.NameID, eTimerUpdateMode.FixedUpdate, INSTANCE_ID);
            }
        }

        public DamagedActor(Rigidbody rb, CharacterAnimator anim, ActorStateMachine onwer, DamagedActorConfig config, GameObject gameobject, System.Action onDamagedEnd)
            : base(rb, anim, onwer, config.BaseConfig, config.NameID, gameobject)
        {
            CONFIG = config;
            ON_DAMAGED_END = onDamagedEnd;
            mStunTimer.AddCallback("NockBack", 0.0f, onKnockbackEnd)
                .AddCallback("StunEnd", 0.0f, onDamagedActingEnd);
            EFFECT_POOL = GameObjectPool.TryGetGameobjectPool(gameobject.transform, "EffectPool", ePoolingObjectType.EffectPool);
        }

        public override void StopActing()
        {
            if (IsInHitStun)
            {
                mStunTimer.PauseTimerWithCallback();
            }
        }

        public override void DestoryActor()
        {
        }

        public override void InovkeActing()
        {
            Debug.Assert(HitSkillConfig != null);

            if (IsInHitStun)
            {
                mStunTimer.PauseTimerWithCallback();
            }

            IsInHitStun = true;
            float stunTime = HitSkillConfig.NockbackData.HitFreezeTime;
            if (stunTime != 0.0f)
            {
                ANIM.PlayDamagedAnim(CONFIG.GetCaculatedAnimSpeed(stunTime));
            }
            processNockback(HitSkillConfig.NockbackData);

            mStunTimer.ChangeTiming("NockBack", HitSkillConfig.NockbackData.NockbackTime)
                .ChangeTiming("StunEnd", HitSkillConfig.NockbackData.HitFreezeTime - HitSkillConfig.NockbackData.NockbackTime)
                .StartTimer();

            if (HitSkillConfig.HitEffect != null)
            {
                GameObject effect = EFFECT_POOL.GetDeactiveGameobject(HitSkillConfig.HitEffect);
                effect.transform.localPosition = CONFIG.HitEffectSpawnPoint;
            }
        }

        protected virtual void onDamagedActingEnd()
        {
            IsInHitStun = false;
            OWNER.CheckAndClearActor(eActorType.Damaged);
            RB.DisEnrollSetVelocity(eActorType.Damaged);
            ON_DAMAGED_END?.Invoke();
        }

        protected virtual void processNockback(NockbackData data)
        {
            Vector3 nockbackSpeed = Vector3.zero;
            switch (data.NockBackType)
            {
                case eNockBackType.None:
                    break;
                case eNockBackType.PushToAttackDir:
                    nockbackSpeed = data.NockbackDir * data.NockbackPower;
                    RB.EnrollSetVelocity(nockbackSpeed, eActorType.Damaged);
                    break;
                case eNockBackType.CircularToHitPoint:
                    Vector3 nockbackDir = (RB.position - data.NockbackDir).normalized;
                    nockbackSpeed = nockbackDir * data.NockbackPower;
                    RB.EnrollSetVelocity(nockbackSpeed, eActorType.Damaged);
                    break;
                case eNockBackType.Deaccelerate:
                    RB.GetLayeredRigidbody().SetVelocityMultiplier(data.Deacceleration, eSpeedMultiplierSource.GotHit);
                    break;

                default:
                    break;
            }
        }


        private void onKnockbackEnd()
        {
            RB.EnrollSetVelocity(Vector3.zero, eActorType.Damaged);
            RB.GetLayeredRigidbody().SetVelocityMultiplier(1.0f, eSpeedMultiplierSource.GotHit);
        }
    }
}