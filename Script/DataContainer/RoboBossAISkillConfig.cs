using RoboClean.Character;
using RoboClean.Utils;
using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/RobotBossAISkillConfig")]
    public class RoboBossAISkillConfig : ActorConfigDataContainerBase
    {
        public SkillConfig[] NormalSkills;
        public SkillConfig[] RushSkills;
        public float[] UpgradingToxicGauge;

        public SkillConfig[] ProjectileSkills;
        public float[] ProjectileSkillInvokeHP;
        public float RushableRange;
        public float NormalAttackRange;

        protected override void initializeConfig()
        {
            BaseConfig.ActorType = eActorType.AIAttack;
            BaseConfig.IsUpdatedActor = true;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            Debug.Assert(ProjectileSkillInvokeHP.IsAscendingSorted());
        }
#endif
    }
}