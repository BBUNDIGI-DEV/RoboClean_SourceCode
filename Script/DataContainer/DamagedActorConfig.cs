using RoboClean.Character;
using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/DamagedActorConfig")]
    public class DamagedActorConfig : ActorConfigDataContainerBase
    {
        public Vector3 HitEffectSpawnPoint;
        [SerializeField] private AnimationClip DAMAGED_ANIM_CLIP;

        public float GetCaculatedAnimSpeed(float stunTime)
        {
            float clipTime = DAMAGED_ANIM_CLIP.length;
            return clipTime / stunTime;
        }


        protected override void initializeConfig()
        {
            BaseConfig.ActorType = eActorType.Damaged;
            BaseConfig.IsUpdatedActor = false;
        }
    }
}