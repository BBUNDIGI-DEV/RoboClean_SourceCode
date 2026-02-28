using UnityEngine;
using Sirenix.OdinInspector;

namespace RoboClean.Data
{
    [CreateAssetMenu(fileName = "MonsterConfig", menuName = "DataContainer/MonsterConfig")]
    public class MonsterConfig : DataConfigBase
    {
        public eMonsterType MonsterType;
        public int InitialMaxHP;
        public int InitialHP;
        public float InitialSpeed;
        public float InitialDefense;
        public bool InitialSuperArmor;

        public DeadActorConfig DeadConfig;
        public AIMovementConfig AIMovementConfig;
        public DamagedActorConfig DamagedConfig;
        public StunActorConfig StunConfig;

        [ShowIf("MonsterType", eMonsterType.Commom)] public AISkillConfig AISkillConfig;
        [ShowIf("MonsterType", eMonsterType.Commom)] public AppearanceActorConfig AppearanceConfig;
        [ShowIf("MonsterType", eMonsterType.RoboBoss)] public RoboBossAISkillConfig RoboBossSkillConfig;

        [Range(1.0f, 20f)] public float DefenceIncreaseByToxic;
        [Range(1.0f, 1.5f)] public float SpeedIncreaseByToxic;
        [AssetsOnly] public GameObject AuraEffect;
        [Range(0.0f, 1.0f)] public float AuraThreshold;

        private void OnEnable()
        {
            checkValidate();
        }
        private void OnValidate()
        {
            checkValidate();
        }

        private void checkValidate()
        {
            //Debug.Assert(AIMovementConfig != null, $"Movement Config Is null [{NameID}]");
            //Debug.Assert(DamagedConfig != null, $"Damaged Config Is null [{NameID}]");
            //Debug.Assert(DeadConfig != null, $"Dead Config Is null [{NameID}]");

            //Debug.Assert(MonsterType != eMonsterType.Commom || AppearanceConfig != null
            //    , $"Appearance Config Is null [{NameID}]");
            //Debug.Assert(MonsterType != eMonsterType.Commom || AISkillConfig != null
            //    , $"AISkill Config Is null [{NameID}]");

            //Debug.Assert(MonsterType != eMonsterType.RoboBoss || RoboBossSkillConfig != null
            //    , $"RoboBossSkill Config Is null [{NameID}]");
            //Debug.Assert(MonsterType != eMonsterType.RoboBoss || RoboStunByRushConfig != null
            //    , $"RoboBossStun Config Is null [{NameID}]");
        }
    }


    public enum eMonsterType
    {
        Commom,
        RoboBoss,
    }
}
