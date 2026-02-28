using RoboClean.Character.Player;
using RoboClean.Data;
using UnityEngine;
using UnityEngine.Events;

namespace RoboClean.Stage
{
    [RequireComponent(typeof(Collider))]
    public abstract class InteractableBase : MonoBehaviour
    {
        public eInteractableType ObjectType
        {
            get
            {
                return M_OBJECT_TYPE;
            }
        }

        [SerializeField] private eInteractableType M_OBJECT_TYPE;
        [SerializeField] private UnityEvent ON_INTERACTING;

        protected virtual void Awake()
        {
            RuntimeDataLoader.RuntimeStageData.LastMonsterCount.AddListener(toggleInteractableObject, true);
        }

        private void OnDestroy()
        {
            RuntimeDataLoader.RuntimeStageData.LastMonsterCount.RemoveListener(toggleInteractableObject);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player")
            {
                return;
            }
            PlayerInteractiveManager interactiveManager = other.GetComponentInParent<PlayerInteractiveManager>();
            Debug.Assert(interactiveManager != null,
                "interactive Manager Not Founded");
            interactiveManager.SetDetectedObject(this);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.tag != "Player")
            {
                return;
            }

            PlayerInteractiveManager interactiveManager = other.GetComponentInParent<PlayerInteractiveManager>();
            Debug.Assert(interactiveManager != null,
                "interactive Manager Not Founded");
            interactiveManager.SetDetectedObject(null);
        }

        public virtual void InvokeInteraction()
        {
            GetComponent<Collider>().enabled = false;
            ON_INTERACTING?.Invoke();
        }

        private void toggleInteractableObject(int lastMonsterCount)
        {
            if (lastMonsterCount != 0)
            {
                GetComponent<Collider>().enabled = false;
            }
            else
            {
                GetComponent<Collider>().enabled = true;
            }
        }
    }


    public enum eInteractableType
    {
        None,
        Door,
        StageSwitchingDoor,
        DialogueNPC,
        CommonObject,
        EndingObject,
    }
}