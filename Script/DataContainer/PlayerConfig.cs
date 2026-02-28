using RoboClean.Utils;
using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        public const float RUN_ANIM_INVOKE_THRESHOLD = 0.05f;
        public const int RUN_TO_IDLE_TRANSITION_THRESHOLD = 1;

        public float InitialMaxHP = 100;

        public float[] ToxicDebuffTearChecker;
        public PlayerDebuffByToxicConfig[] DebuffByToxicConfig;

        public MovementConfig MovementConfig;
        public DashConfig DashConfig;
        public DamagedActorConfig DamagedActorConfig;
        public StunActorConfig StunConfig;
        public SkillConfig NormalAttack;
        public SkillConfig DashAttack;
        public SkillConfig SpecialDashAttack;
        public SkillConfig SpecialAttack;

        [Range(0, 1)] public float LowhpEffectThreshold;

        private void OnValidate()
        {
            Debug.Assert(ToxicDebuffTearChecker.IsDescendingSorted(), "Toxic Tear Checker is must be descending sorted");
        }
    }
}