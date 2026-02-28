using RoboClean.Character.Player;
using RoboClean.Data;
using RoboClean.Utils;
using System;
using UnityEngine;

namespace RoboClean.Character
{
    public class DeadActor : ActorBase
    {
        private readonly DeadActorConfig CONFIG;

        private BasicTimer mDeadActorTimer
        {
            get
            {
                return GlobalTimer.Instance
               .TryGetTimerOrAdd<BasicTimer>("DeadActorTimer"
               , eTimerUpdateMode.FixedUpdate
               , INSTANCE_ID);
            }
        }
        private Action mOnDead;
        private readonly Collider HITBOX_COLLIDER;

        public DeadActor(Rigidbody rb, CharacterAnimator anim, ActorStateMachine onwerStateMachine, DeadActorConfig config,
            GameObject gameObject)
            : base(rb, anim, onwerStateMachine, config.BaseConfig, config.NameID, gameObject)
        {
            CONFIG = config;
            mDeadActorTimer.ChangeCallback(StopActing);
            HITBOX_COLLIDER = gameObject.transform.FindRecursive("HitBox").GetComponent<Collider>();
        }

        public override void InovkeActing()
        {
            Vector3 placeHolder;
            eActorType actorType;
            RB.GetLayeredRigidbody().TryGetVelocity(out placeHolder, out actorType);
            HITBOX_COLLIDER.enabled = false;
            AimHelper.RemoveEnemeyTrans(RB.transform);
            if (CONFIG.GetDuration() != 0.0f)
            {
                mDeadActorTimer.ChangeDuration(CONFIG.GetDuration()).StartTimer();
                ANIM.PlayDeadAnim(CONFIG.GetCaculatedAnimSpeed());
            }
            else
            {
                StopActing();
            }
        }

        public override void StopActing()
        {
            OWNER.DestoryActors();
            mOnDead?.Invoke();
        }

        public override void DestoryActor()
        {
        }

        public void SetOnDeadCallback(Action onDead)
        {
            mOnDead = onDead;
        }
    }
}