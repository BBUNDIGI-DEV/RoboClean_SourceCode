using UnityEngine;

namespace RoboClean.Stage
{
    public class InteractableDoor : InteractableBase
    {
        [SerializeField] private Animation DOOR_ANIM;
        [SerializeField] private BoxCollider DOOR_OBSTACLE;

        protected override void Awake()
        {
            base.Awake();
            DOOR_OBSTACLE.enabled = true;
        }

        public override void InvokeInteraction()
        {
            DOOR_ANIM.Play();
            DOOR_OBSTACLE.enabled = false;
            base.InvokeInteraction();
        }
    }
}