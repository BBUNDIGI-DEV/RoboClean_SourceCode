using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/SpawnConfig")]
    public class SpawnConfig : ScriptableObject
    {
        public GameObject MeleeMonster;
        public GameObject RangedMonster;
        public GameObject TankerMonster;
        public GameObject BossMonster;
    }
}