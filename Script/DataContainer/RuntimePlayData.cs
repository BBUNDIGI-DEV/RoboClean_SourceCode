using RoboClean.Player;
using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "RuntimeData/RuntimePlayData")]
    public class RuntimePlayData : ScriptableObject
    {
        [HideInInspector] public ObservedData<bool> IsQuitGame;
        [HideInInspector] public ObservedData<bool> IsPaused;
        [HideInInspector] public ObservedData<bool> IsScenePlaying;
        [HideInInspector] public ObservedData<bool> IsMonsterActorBlocked;
    }
}