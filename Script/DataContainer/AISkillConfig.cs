using UnityEngine;
using Sirenix.OdinInspector;
using RoboClean.Character;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/AISKillConfig")]
    public class AISkillConfig : ActorConfigDataContainerBase
    {
        public eCombatType CombatType;
        public SkillConfig[] SkillConfigs;
        public float[] UpgradingToxicGauge;
        public SkillConfig SludgeSpreadSkillConfig;

        [ShowIf("CombatType", eCombatType.MeleeAttackInRange)] public float MeleeAttackRange;
        [ShowIf("CombatType", eCombatType.ShootProjectileTowardPlayer)] public float ProjectileMinRange;

        protected override void initializeConfig()
        {
            BaseConfig.ActorType = eActorType.AIAttack;
            BaseConfig.IsUpdatedActor = true;
        }
    }


    public enum eCombatType
    {
        None,
        MeleeAttackInRange,
        ShootProjectileTowardPlayer,
        RobotBoss,
    }
}