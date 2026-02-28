using RoboClean.Character;
using RoboClean.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/DeadActorConfig")]
    public class DeadActorConfig : ActorConfigDataContainerBase
    {
        private const float MAX_ACCELARTION_FACTOR = 3.0f;
        [Range(-1, 1)]
        public float DeadAccel;

        [ShowInInspector]
        private float DURATION
        {
            get
            {
                return GetDuration();
            }
        }

        [SerializeField] private AnimationClip DEAD_CLIP;

        public float GetDuration()
        {
            if (DEAD_CLIP == null)
            {
                return 0.0f;
            }

            return DEAD_CLIP.length
                * ActorSpeedUtil.ConvertInspectorAccelToMultiplier(DeadAccel, MAX_ACCELARTION_FACTOR);
        }

        public float GetCaculatedAnimSpeed()
        {
            return 1.0f / ActorSpeedUtil.ConvertInspectorAccelToMultiplier(DeadAccel, MAX_ACCELARTION_FACTOR);
        }


        protected override void initializeConfig()
        {
            BaseConfig.ActorType = eActorType.Dead;
            BaseConfig.IsUpdatedActor = false;
        }
    }
}
