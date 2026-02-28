using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(fileName = "StageToxicConfig", menuName = "DataContainer/StageToxicConfig")]
    public class StageToxicConfig : ScriptableObject
    {
        public int MaxSludgeAmount;
        public GameObject InitialSludgesParent;
    }
}