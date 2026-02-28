using UnityEngine;
using UnityEngine.Events;
namespace RoboClean.Utils
{
    public class AnimEventReciever : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent[] TargetEvent;


        public void Raise(int eventIndex)
        {
            Debug.Assert(eventIndex < TargetEvent.Length, $"{gameObject.name}");
            TargetEvent[eventIndex].Invoke();
        }
    }
}