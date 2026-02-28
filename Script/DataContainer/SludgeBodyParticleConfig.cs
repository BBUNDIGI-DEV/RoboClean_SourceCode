using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/SludgeBodyParticleConfig")]
    public class SludgeBodyParticleConfig : ScriptableObject
    {
        public float MinDruation = 0.6f;
        public float MaxDuration = 1.2f;
        public float MinHeight = 1.0f;
        public float MaxHeight = 3.5f;
        public float MaxDistance = 10;
        public AnimationCurve MovementEaseCurve;
    }
}