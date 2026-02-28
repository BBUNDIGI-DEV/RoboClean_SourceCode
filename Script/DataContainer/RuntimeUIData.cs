using RoboClean.Player;
using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "RuntimeData/RuntimeUIData")]
    public class RuntimeUIData : ScriptableObject
    {
        [HideInInspector] public ObservedData<bool> IsActiveDialogue;//RuntimeUIData
        [HideInInspector] public ObservedData<string> DialogueString;//RuntimeUIData
        [HideInInspector] public ObservedData<int> DialogueID;//RuntimeUIData
        [HideInInspector] public ObservedData<bool> IsFadeAllUI;
    }
}
