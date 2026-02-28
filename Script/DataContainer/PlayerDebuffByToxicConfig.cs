using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/PlayerDebuffByToxicConfig")]
    public class PlayerDebuffByToxicConfig : ScriptableObject
    {
        [Range(0, 1)] public float DecreaseSpeedAmount;
        [Range(-1, 1)] public float DecreaseAttackSpeed;
    }
}
