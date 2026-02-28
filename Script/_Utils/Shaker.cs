using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.Common
{
    public class Shaker : MonoBehaviour
    {
        [SerializeField] private float DURATION = 1.0f;
        [SerializeField] private float SHAKE_POWER = 1.0f;
        [SerializeField] private AnimationCurve SHAKE_CURVE;
        public void PlayShake()
        {
            StopAllCoroutines();
            StartCoroutine(Shaking());
        }

        public void PauseShake()
        {
            StopCoroutine(Shaking());
        }

        public IEnumerator Shaking()
        {
            Vector3 startPos = transform.position;
            float elapseTime = 0.0f;
            while (elapseTime < DURATION)
            {
                elapseTime += Time.deltaTime;
                float strength = SHAKE_CURVE.Evaluate(elapseTime / DURATION) * SHAKE_POWER;
                transform.position = startPos + Random.insideUnitSphere * strength;
                yield return null;
            }

            transform.position = startPos;
            enabled = false;
        }
    }
}