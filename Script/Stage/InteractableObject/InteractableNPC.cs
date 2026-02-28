using RoboClean.Data;
using RoboClean.Managers;
using UnityEngine;

namespace RoboClean.Stage
{
    public class InteractableNPC : InteractableBase
    {
        [SerializeField] private eDialogueTag TAG;


        protected override void Awake()
        {
            base.Awake();
        }

        public override void InvokeInteraction()
        {
            base.InvokeInteraction();
            UIManager.Instance.DialogueManager.PlayDialouge(TAG);
        }
    }
}
