using RoboClean.Data;
using RoboClean.Utils;
using UnityEngine;

namespace RoboClean.Character
{
    public class StunActor : ActorBase
    {
        private StunActorConfig mStunConfig;

        private BasicTimer mStunTimer;
        public StunActor(Rigidbody rb, CharacterAnimator anim, ActorStateMachine ownerStateMachine, GameObject gameobject, StunActorConfig config, System.Action onStunEndAction) :
            base(rb, anim, ownerStateMachine, new ActorConfig(eActorType.Stun, false), string.Empty, gameobject)
        {
            mStunConfig = config;
            mStunTimer = new BasicTimer(eTimerUpdateMode.FixedUpdate, 0.0f, onStunEnd + onStunEndAction);
            GlobalTimer.Instance.AddTimerWithInstacneID("Stun", INSTANCE_ID, mStunTimer);
        }

        public void SetStunData(StunData stunData)
        {
            mStunTimer.ChangeDuration(stunData.StunDuration);
            RB.EnrollSetVelocity(Vector3.zero, eActorType.Stun);
            switch (mStunConfig.StunAnimationType)
            {
                case eStunAnimationType.Roop:
                    ANIM.PlayStunAnim();
                    break;
                case eStunAnimationType.Onestep:
                    ANIM.PlayStunAnim(mStunConfig.GetCaculatedAnimSpeed(stunData.StunDuration));
                    break;
                default:
                    break;
            }
        }

        public override void InovkeActing()
        {
            mStunTimer.StartTimer();
        }

        public override void StopActing()
        {
            if (mStunTimer.IsActivate)
            {
                mStunTimer.PauseTimerWithCallback();
            }
        }

        public override void DestoryActor()
        {

        }

        private void onStunEnd()
        {
            OWNER.CheckAndClearActor(eActorType.Stun);
            RB.DisEnrollSetVelocity(eActorType.Stun);
            ANIM.PauseStunAnim();

        }
    }
}