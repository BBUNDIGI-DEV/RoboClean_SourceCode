using UnityEngine;
using UnityEngine.Events;

namespace RoboClean.Common
{
    public class ParticleEventReciver : MonoBehaviour
    {
        [SerializeField] private UnityEvent ON_PARITLCE_COLLISION;
        [SerializeField] private UnityEvent ON_PARTICLE_TRIGGER;

        public void OnParticleCollision(GameObject other)
        {
            ON_PARITLCE_COLLISION.Invoke();
        }
        public void OnParticleTrigger()
        {
            ON_PARTICLE_TRIGGER.Invoke();

        }
    }
}