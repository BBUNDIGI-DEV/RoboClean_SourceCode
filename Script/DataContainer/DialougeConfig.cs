using UnityEngine;
using Sirenix.OdinInspector;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/DialogueConfig")]
    public class DialogueConfig : ScriptableObject
    {
        public eDialogueTag Tag;
        public DialogueActingData[] ActingDatas;
    }

    [System.Serializable]
    public struct DialogueActingData
    {
        public eDialogueActingType ActingType;

        [ShowIf("@ActingType.HasFlag(eDialogueActingType.ShowCharacter)")] public eDialogueCharacterType ShowUpCharacter;
        [ShowIf("@ActingType.HasFlag(eDialogueActingType.DisappearCharacter)")] public eDialogueCharacterType DisappearCharacter;
        [ShowIf("@ActingType.HasFlag(eDialogueActingType.SetName)")] public string NameText;
        [ShowIf("@ActingType.HasFlag(eDialogueActingType.ShowText)"), TextArea] public string DialougeText;
    }

    [System.Flags]
    public enum eDialogueActingType
    {
        ShowCharacter = 1 << 1,
        DisappearCharacter = 1 << 2,
        ShowTextBox = 1 << 3,
        HideTextBox = 1 << 4,
        EnableTutorialImage = 1 << 5,
        ShowText = 1 << 6,
        SetName = 1 << 7,
    }

    [System.Flags]
    public enum eDialogueCharacterType
    {
        Player,
        RoboBoss,
        Helper,
    }


    public enum eDialogueTag
    {
        Stage0Helper,
        Stage2Boss,
    }
}