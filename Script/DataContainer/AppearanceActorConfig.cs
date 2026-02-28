using UnityEngine;
using Sirenix.OdinInspector;
using RoboClean.Utils;
using RoboClean.Character;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/AppearanceActorConfig")]
    public class AppearanceActorConfig : ActorConfigDataContainerBase
    {
        private const float MAX_ACCELARTION_FACTOR = 3.0f;
        [Range(-1, 1)]
        public float AppearanceAccel;

        [ShowInInspector]
        private float DURATION
        {
            get
            {
                return GetDuration();
            }
        }

        [SerializeField] private AnimationClip APPEARANCE_CLIP;

        public float GetDuration()
        {
            if (APPEARANCE_CLIP == null)
            {
                return 0.0f;
            }

            return APPEARANCE_CLIP.length
                * ActorSpeedUtil.ConvertInspectorAccelToMultiplier(AppearanceAccel, MAX_ACCELARTION_FACTOR);
        }

        public float GetCaculatedAnimSpeed()
        {
            return 1.0f / ActorSpeedUtil.ConvertInspectorAccelToMultiplier(AppearanceAccel, MAX_ACCELARTION_FACTOR);
        }


        protected override void initializeConfig()
        {
            BaseConfig.ActorType = eActorType.Appearance;
            BaseConfig.IsUpdatedActor = false;
        }
    }
}
