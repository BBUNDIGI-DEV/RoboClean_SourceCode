using RoboClean.Character.Player;
using RoboClean.Managers;
using UnityEngine;

namespace RoboClean.Stage
{
    public class InteractableSceneSwitchingDoor : InteractableDoor
    {
        [field: SerializeField]
        public eSceneName SwitchingScene
        {
            get; private set;
        }

        [field: SerializeField]
        public Transform DoorApproachPoint
        {
            get; private set;
        }

        [field: SerializeField]
        public Transform PlayerExitPoint
        {
            get; private set;
        }

        public override void InvokeInteraction()
        {
            base.InvokeInteraction();
            PlayerCharacterController.Instance.MoveToAnotherScene(DoorApproachPoint.position, PlayerExitPoint.position, SwitchingScene);
        }
    }
}
