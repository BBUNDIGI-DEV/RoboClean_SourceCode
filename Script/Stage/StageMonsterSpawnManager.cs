using RoboClean.Common;
using RoboClean.Data;
using UnityEngine;

namespace RoboClean.Stage
{
    public class StageMonsterSpawnManager : SingletonClass<StageMonsterSpawnManager>
    {
        private SpawnConfig CONFIG
        {
            get
            {
                return RuntimeDataLoader.SpawnConfig;
            }
        }

        public GameObject SpawnMonster(eSpawnMonsterType spawnMonsterType)
        {
            GameObject spawnMonster = null;
            switch (spawnMonsterType)
            {
                case eSpawnMonsterType.Melee:
                    spawnMonster = Instantiate(CONFIG.MeleeMonster, transform);
                    break;
                case eSpawnMonsterType.Ranged:
                    spawnMonster = Instantiate(CONFIG.RangedMonster, transform);
                    break;
                case eSpawnMonsterType.Tanker:
                    spawnMonster = Instantiate(CONFIG.TankerMonster, transform);
                    break;
                case eSpawnMonsterType.Boss:
                    spawnMonster = Instantiate(CONFIG.BossMonster, transform);
                    break;
                default:
                    Debug.LogError(spawnMonsterType);
                    break;
            }
            spawnMonster.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            return spawnMonster;
        }

        protected override void Awake()
        {
            base.Awake();
        }
    }
}