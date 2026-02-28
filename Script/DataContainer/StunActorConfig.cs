using UnityEngine;
using Sirenix.OdinInspector;
using RoboClean.Character;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/StunConfig")]
    public class StunActorConfig : ActorConfigDataContainerBase
    {
        public eStunAnimationType StunAnimationType;
        [ShowIf("StunAnimationType", eStunAnimationType.Onestep), SerializeField] private AnimationClip StunAnimation;

        protected override void initializeConfig()
        {
            BaseConfig.ActorType = eActorType.Stun;
            BaseConfig.IsUpdatedActor = false;
        }

        public float GetCaculatedAnimSpeed(float stunDruation)
        {
            float clipTime = StunAnimation.length;
            return clipTime / stunDruation;
        }
    }

    public enum eStunAnimationType
    {
        Roop,
        Onestep,
    }
}