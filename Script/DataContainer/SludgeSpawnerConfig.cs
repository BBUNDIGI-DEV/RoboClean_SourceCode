using UnityEngine;
using Sirenix.OdinInspector;

namespace RoboClean.Data
{
    [CreateAssetMenu(fileName = "SludgeSpawnerConfig", menuName = "DataContainer/SludgeSpawnerConfig")]
    public class SludgeSpawnerConfig : ScriptableObject
    {
        public eSludgeType SludgeType;
        public eSludgeSpawnCase SpawnCases;

        [ShowIf("@SpawnCases.HasFlag(eSludgeSpawnCase.OnHit)"), TitleGroup("OnHit")] public float[] OnHitHPPercentage;
        [ShowIf("@SpawnCases.HasFlag(eSludgeSpawnCase.OnHit)"), TitleGroup("OnHit")] public SludgeSpawnData[] OnHitSpawnDatas;
        [ShowIf("@SpawnCases.HasFlag(eSludgeSpawnCase.OnDead)"), TitleGroup("OnDead")] public SludgeSpawnData OnDeadSpawnData;
        [ShowIf("@SpawnCases.HasFlag(eSludgeSpawnCase.OnIdle)"), TitleGroup("OnIdle")] public float OnIdleRandomMinTime;
        [ShowIf("@SpawnCases.HasFlag(eSludgeSpawnCase.OnIdle)"), TitleGroup("OnIdle")] public float OnIdleRandomMaxTime;
        [ShowIf("@SpawnCases.HasFlag(eSludgeSpawnCase.OnIdle)"), TitleGroup("OnIdle")] public SludgeSpawnData OnIdleSpawnData;
    }


    [System.Serializable]
    public struct SludgeSpawnData
    {
        public eSludgeSpawnType SpawnType;
        public int SpreadAmount;
        public float SpreadRange;

        [ShowIf("SpawnType", eSludgeSpawnType.SpreadGradual)] public float SpreadDelay;
        [ShowIf("SpawnType", eSludgeSpawnType.SpreadGradual)]
        public float SpreadTotalDuration
        {
            get
            {
                return SpreadAmount * SpreadTimeInterval;
            }
        }
        [ShowIf("SpawnType", eSludgeSpawnType.SpreadGradual)] public float SpreadTimeInterval;
    }

    [System.Flags]
    public enum eSludgeSpawnCase
    {
        OnHit = 1 << 1,
        OnDead = 1 << 2,
        OnIdle = 1 << 3,
    }

    [System.Flags]
    public enum eSludgeType
    {
        Mud,
        Poison,
        Oil,
    }

    public enum eSludgeSpawnType
    {
        SpreadAtOnce,
        SpreadGradual
    }
}