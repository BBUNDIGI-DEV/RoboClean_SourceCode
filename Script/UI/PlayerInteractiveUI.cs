using UnityEngine;
using TMPro;
using RoboClean.Data;
using RoboClean.Stage;

namespace RoboClean.UI
{
    public class PlayerInteractiveUI : MonoBehaviour
    {
        private const string SHOW_ANIM_KEY = "ANI_interaction_info_show_up";
        private const string CLOSE_ANIM_KEY = "ANI_interaction_info_close";
        [SerializeField] private Animation ANIM;
        [SerializeField] private TMP_Text INTERACTIVE_TEXT;
        [SerializeField] private TMP_Text BUTTON_TEXT;

        private void Start()
        {
            RuntimeDataLoader.PlayerRuntimeInfo.DetectedInteractableObject.AddListener(updateInteractableObjectType);
        }

        private void OnDestroy()
        {
            RuntimeDataLoader.PlayerRuntimeInfo.DetectedInteractableObject.RemoveListener(updateInteractableObjectType);
        }
        private void updateInteractableObjectType(eInteractableType newInteractableObjectType)
        {
            if (newInteractableObjectType == eInteractableType.None)
            {
                ANIM.PlayQueued(CLOSE_ANIM_KEY);
            }
            else
            {
                ANIM.PlayQueued(SHOW_ANIM_KEY);
                switch (newInteractableObjectType)
                {
                    case eInteractableType.None:
                        break;
                    case eInteractableType.Door:
                        INTERACTIVE_TEXT.text = "들어가기";
                        break;
                    case eInteractableType.StageSwitchingDoor:
                        INTERACTIVE_TEXT.text = "들어가기";
                        break;
                    case eInteractableType.DialogueNPC:
                        INTERACTIVE_TEXT.text = "대화하기";
                        break;
                    case eInteractableType.CommonObject:
                        INTERACTIVE_TEXT.text = "상호작용";
                        break;
                    case eInteractableType.EndingObject:
                        INTERACTIVE_TEXT.text = "살펴보기";
                        break;
                    default:
                        Debug.LogError(newInteractableObjectType);
                        break;
                }

            }
        }
    }
}