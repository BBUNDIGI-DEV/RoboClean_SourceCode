using RoboClean.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "RuntimeData/RuntimeStageData")]
    public class RuntimeStageData : ScriptableObject
    {
        public ObservedData<int> TotalMonsterCount;
        public ObservedData<int> LastMonsterCount;
        public ObservedData<int> CurrentWaveIndex;
        public ObservedData<bool> IsStageStarted;
        public ObservedData<bool> IsStageCleared;
        public ObservedData<int> MaxSludgeCount;
        public ObservedData<int> SludgeCount;
        public ObservedData<float> ToxicGuage;
        [HideInInspector] public ObservedData<Vector3> EndPlayerPosition;
    }
}