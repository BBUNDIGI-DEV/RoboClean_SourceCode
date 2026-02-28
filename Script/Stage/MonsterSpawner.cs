
using UnityEngine;

namespace RoboClean.Stage
{
    public class MonsterSpawner : MonoBehaviour
    {
        [SerializeField] private eSpawnMonsterType SPAWN_MONSTER_TYPE;

        public void SpawnMonster(System.Action onMonsterDead)
        {
            GameObject monster = StageMonsterSpawnManager.Instance.SpawnMonster(SPAWN_MONSTER_TYPE);

            MonsterBase mosnterController = monster.GetComponentInChildren<MonsterBase>();
            Debug.Assert(mosnterController != null, "Monster Contorller not founded");
            mosnterController.Status.IsDead.AddListener(onMonsterDead);
            mosnterController.SetPositionAndRotation(transform.position, transform.rotation);
        }
    }
    public enum eSpawnMonsterType
    {
        Melee,
        Ranged,
        Tanker,
        Boss,
    }
}


