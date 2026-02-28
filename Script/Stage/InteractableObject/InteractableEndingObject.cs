using RoboClean.Character.Player;
using RoboClean.Data;
using RoboClean.Managers;

namespace RoboClean.Stage
{
    public class InteractableEndingObject : InteractableBase
    {
        public override void InvokeInteraction()
        {
            System.Action onDialgoueEnd = () => PlayerCharacterController.Instance.IsInputBlocked = true;
            onDialgoueEnd += () => UIManager.Instance.EndingCreditUI.SetActive(true);

            UIManager.Instance.DialogueManager.PlayDialouge(eDialogueTag.Stage2Boss, onDialgoueEnd);
            base.InvokeInteraction();
        }
    }
}