namespace RoboClean.Data
{
    public abstract class BossAISkillConfig : ActorConfigDataContainerBase
    {
        public SkillConfig[] NormalSkills;
        public SkillConfig[] Special1Skills;
        public SkillConfig[] Special2Skills;
        public float[] UpgradingToxicGauge;
    }
}