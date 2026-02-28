using RoboClean.Data;
using UnityEngine;

namespace RoboClean.Character.AI
{
    public class AppearanceActor : ActorBase
    {
        private readonly AppearanceActorConfig CONFIG;
        private BasicTimer mAppearanceTimer
        {
            get
            {
                return GlobalTimer.Instance
               .TryGetTimerOrAdd<BasicTimer>("AppearanceTimer"
               , eTimerUpdateMode.FixedUpdate
               , INSTANCE_ID);
            }
        }


        public AppearanceActor(Rigidbody rb, CharacterAnimator anim, ActorStateMachine onwerStateMachine, AppearanceActorConfig config,
            GameObject gameobject)
            : base(rb, anim, onwerStateMachine, config.BaseConfig, config.NameID, gameobject)
        {
            CONFIG = config;
            mAppearanceTimer.ChangeCallback(StopActing);
        }

        public override void InovkeActing()
        {
            mAppearanceTimer.ChangeDuration(CONFIG.GetDuration()).StartTimer();
            ANIM.PlayAppearanceAnim(CONFIG.GetCaculatedAnimSpeed());
        }

        public override void StopActing()
        {
            OWNER.CheckAndClearActor(eActorType.Appearance);
        }

        public override void DestoryActor()
        {
        }
    }
}