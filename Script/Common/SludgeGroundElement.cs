using RoboClean.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace RoboClean.Character
{
    public class SludgeGroundElement : MonoBehaviour
    {
        [SerializeField] private ParticleSystem SLUDGE_PARTICLE;
        private eSludgeGroundParticleState mActingState;
        private UnityAction mOnSludgeDisabled;

        public void InitializeSludge(Vector3 position, UnityAction onSludgeDisabled)
        {
            gameObject.SetActive(true);
            transform.position = position;
            mOnSludgeDisabled = onSludgeDisabled;
            GetComponent<BoxCollider>().enabled = true;
        }

        public void DisableSludge()
        {
            if (mActingState == eSludgeGroundParticleState.Disable)
            {
                return;
            }

            Debug.Assert(mActingState != eSludgeGroundParticleState.None
                , $"Disalbe Sludge Must be Called when acting state is initialize or disable {mActingState}");

            if (mActingState == eSludgeGroundParticleState.Initialize)
            {
                float halfLifeTime = SLUDGE_PARTICLE.main.startLifetimeMultiplier / 2;
                float skipTime = halfLifeTime - SLUDGE_PARTICLE.time;
                SLUDGE_PARTICLE.time = skipTime + halfLifeTime;
            }

            SLUDGE_PARTICLE.Play();
            mActingState = eSludgeGroundParticleState.Disable;
            StageToxicManager.Instance.DecreaseSludge();
        }

        private void OnEnable()
        {
            if (SLUDGE_PARTICLE.isPlaying)
            {
                SLUDGE_PARTICLE.Pause();
                SLUDGE_PARTICLE.time = 0.0f;
            }
            SLUDGE_PARTICLE.Play();
            mActingState = eSludgeGroundParticleState.Initialize;
        }

        private void OnDisable()
        {
            SLUDGE_PARTICLE.time = 0.0f;
        }

        private void Update()
        {
            switch (mActingState)
            {
                case eSludgeGroundParticleState.None:
                    break;
                case eSludgeGroundParticleState.Initialize:
                    float reachTime = SLUDGE_PARTICLE.main.startLifetimeMultiplier;
                    reachTime *= 0.5f;
                    //Debug.Log(reachTime);

                    if (SLUDGE_PARTICLE.time > reachTime)
                    {
                        mActingState = eSludgeGroundParticleState.Paused;
                    }
                    break;
                case eSludgeGroundParticleState.Paused:
                    if (SLUDGE_PARTICLE.isPlaying)
                    {
                        SLUDGE_PARTICLE.Pause();
                    }
                    break;
                case eSludgeGroundParticleState.Disable:
                    if (!SLUDGE_PARTICLE.IsAlive(true))
                    {
                        mActingState = eSludgeGroundParticleState.None;
                        mOnSludgeDisabled?.Invoke();
                        gameObject.SetActive(false);
                    }
                    break;
                default:
                    Debug.LogError(mActingState);
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Cleaner")
            {
                return;
            }

            GetComponent<BoxCollider>().enabled = false;
            DisableSludge();
        }
    }

    public enum eSludgeGroundParticleState
    {
        None,
        Initialize,
        Paused,
        Disable,
    }
}