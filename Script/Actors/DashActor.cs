using RoboClean.Data;
using RoboClean.Input;
using RoboClean.Sound;
using RoboClean.Utils;
using UnityEngine;

namespace RoboClean.Character.Player
{
    public class DashActor : ActorBase
    {
        public bool IsOnDash
        {
            get
            {
                return mDashTimer.IsTaggedTimerActivated(DASH_TRANSITION_KEY);
            }
        }

        public bool CanExtraDashable
        {
            get
            {
                if (IsOnDash)
                {
                    return mCurrentDashCount < CONFIG.DefaultMaxDashNumber;
                }
                else
                {
                    return mCurrentDashCount < CONFIG.DefaultMaxDashNumber && mDashTimer.IsTaggedTimerActivated(EXTRA_DASHABLE_TIME_KEY);
                }
            }
        }

        public bool CanDashAttack
        {
            get
            {
                bool result = (IsOnDash || mDashAttackableTimer.IsActivate) && !mHasDashAttack;
                if (result)
                {
                    mHasDashAttack = true;
                }
                return result;
            }
        }

        public bool CanDash
        {
            get
            {
                if (CanExtraDashable)
                {
                    return true;
                }

                return !IsOnDash && !mIsDashCooltime;
            }
        }

        public bool mIsDashCooltime
        {
            get
            {
                return mDashTimer.IsTaggedTimerActivated(DASH_COOLTIME_KEY) || mDashTimer.IsTaggedTimerActivated(EXTRA_DASHABLE_TIME_KEY);
            }
        }

        public int mCurrentDashCount
        {
            get; private set;
        }

        private readonly DashConfig CONFIG;
        private readonly GameObjectPool EFFECT_POOL;
        private readonly System.Action ON_DASH_END;
        private readonly string DASH_TRANSITION_KEY;
        private readonly string EXTRA_DASHABLE_TIME_KEY;
        private readonly string DASH_COOLTIME_KEY;
        private readonly Collider HITBOX_COLLIDER;
        private readonly Collider CLEANBOX_COLLIDER;

        private ChainedTimer mDashTimer
        {
            get
            {
                return GlobalTimer.Instance.TryGetTimerOrAdd<ChainedTimer>("Dash",
                    eTimerUpdateMode.FixedUpdate
                    , INSTANCE_ID);
            }
        }

        private BasicTimer mDashAttackableTimer
        {
            get
            {
                return GlobalTimer.Instance.TryGetTimerOrAdd<BasicTimer>("Dashattackable",
                    eTimerUpdateMode.FixedUpdate
                    , INSTANCE_ID);
            }
        }

        private bool mHasDashAttack = false;

        public DashActor(Rigidbody rb, CharacterAnimator anim, ActorStateMachine owner, DashConfig config, GameObject gameObject, System.Action onDashEndCallback)
            : base(rb, anim, owner, config.BaseConfig, config.NameID, gameObject)
        {
            CONFIG = config;
            ON_DASH_END = onDashEndCallback;

            DASH_TRANSITION_KEY = "DashTransition";
            EXTRA_DASHABLE_TIME_KEY = "ExtraDashableTime";
            DASH_COOLTIME_KEY = "DashCoolTime";

            mDashTimer
                .AddCallback(DASH_TRANSITION_KEY, config.DashDuration, onDashEnd)
                .AddCallback(EXTRA_DASHABLE_TIME_KEY, config.ExtraDashableTime, null)
                .AddCallback(DASH_COOLTIME_KEY, config.DashCooltime - config.ExtraDashableTime, this.onDashCoolDownEnd);

            EFFECT_POOL = GameObjectPool.TryGetGameobjectPool(gameObject.transform, "EffectPool", ePoolingObjectType.EffectPool);
            HITBOX_COLLIDER = gameObject.transform.FindRecursive("HitBox").GetComponent<Collider>();
            CLEANBOX_COLLIDER = gameObject.transform.FindRecursive("CleanBox").GetComponent<Collider>();
        }

        public override void InovkeActing()
        {
            AudioManager.PlayPlayerDashSoundEffect();
            setDashedLayer();

            Vector3 moveDir = InputManager.Instance.MoveDir;
            Vector3 dashDir;

            if (moveDir == Vector3.zero)
            {
                dashDir = RB.rotation * Vector3.forward;
            }
            else
            {
                dashDir = moveDir;
            }
            if (CONFIG.EffectPrefab != null)
            {
                GameObject effect = EFFECT_POOL.GetDeactiveGameobject(CONFIG.EffectPrefab);
                effect.SetActive(true);
            }

            RB.EnrollRotation(Quaternion.LookRotation(dashDir), ActorType);
            float dashDistance = mCurrentDashCount < 1 ? CONFIG.InitialDashDistance : CONFIG.SecondDashDistance;
            float speed = dashDistance / CONFIG.DashDuration;
            RB.EnrollSetVelocity(dashDir * speed, ActorType);
            ANIM.TriggerDashAnim(CONFIG.CaculateAnimSpeed());
            Vector3 destPos = RB.position + dashDir * dashDistance;
            DebugUtil.DrawLineSwitchColor(RB.position, destPos, mCurrentDashCount, 1.0f);
            mDashTimer.StartTimer();

            CameraUtils.Instance.Actor.ProcessCameraActing(CONFIG.CameraActingData);
            mHasDashAttack = false;
            mCurrentDashCount++;
            CLEANBOX_COLLIDER.enabled = true;
        }

        public override void StopActing()
        {
            onDashEnd();
        }

        public override void DestoryActor()
        {
        }

        private void onDashEnd()
        {
            if (mDashTimer.IsTaggedTimerActivated(DASH_TRANSITION_KEY))
            {
                mDashTimer.SkipTaggedTimer(DASH_TRANSITION_KEY, false);
            }
            mDashAttackableTimer.ChangeDuration(CONFIG.DashAttackableTime).StartTimer();
            RB.DisEnrollSetVelocity(ActorType);
            RB.DisEnrollRotation(ActorType);
            clearDashedLayer();
            ON_DASH_END?.Invoke();
            OWNER.CheckAndClearActor(ActorType);
        }

        private void onDashCoolDownEnd()
        {
            mCurrentDashCount = 0;
        }

        private void setDashedLayer()
        {
            Collider collider = RB.GetComponent<Collider>();
            LayerMask excludeLayers = RB.excludeLayers;
            excludeLayers.Add("PassableObstacle");
            excludeLayers.Add("Enemey");
            collider.excludeLayers = excludeLayers;

            LayerMask hitboxMask = HITBOX_COLLIDER.excludeLayers;
            hitboxMask.Add("AttackBox");
            HITBOX_COLLIDER.excludeLayers = hitboxMask;
        }

        private void clearDashedLayer()
        {
            Collider collider = RB.GetComponent<Collider>();
            LayerMask excludeLayers = collider.excludeLayers;
            excludeLayers.Remove("PassableObstacle");
            excludeLayers.Remove("Enemey");
            collider.excludeLayers = excludeLayers;

            LayerMask hitboxMask = HITBOX_COLLIDER.excludeLayers;
            hitboxMask.Remove("AttackBox");
            HITBOX_COLLIDER.excludeLayers = hitboxMask;
            CLEANBOX_COLLIDER.enabled = false;
        }
    }
}