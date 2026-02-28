using UnityEngine;
using DG.Tweening;
using RoboClean.Data;

namespace RoboClean.Character
{
    public class SludgeBody : MonoBehaviour
    {
        [SerializeField] private SludgeBodyParticleConfig CONFIG;
        private ParticleSystem BODY_PS;
        private Vector3[] mPathes;

        private void Awake()
        {
            BODY_PS = GetComponent<ParticleSystem>();
            mPathes = new Vector3[2];
        }

        public void IntializeBody(Vector3 startPos, Vector3 destPos, TweenCallback onBodyMovmentEnd)
        {
            gameObject.SetActive(true);
            BODY_PS.Play();
            transform.position = startPos;
            Vector3 middlePoint = Vector3.Lerp(startPos, destPos, 0.5f);
            float factor = (destPos - startPos).sqrMagnitude / CONFIG.MaxDistance * CONFIG.MaxDistance;
            factor = Mathf.Clamp01(factor);

            float height = Mathf.Lerp(CONFIG.MinHeight, CONFIG.MaxHeight, factor);
            float duration = Mathf.Lerp(CONFIG.MinDruation, CONFIG.MaxDuration, factor);
            middlePoint.Set(middlePoint.x, height, middlePoint.z);
            mPathes[0] = middlePoint;
            mPathes[1] = destPos;
            transform.DOPath(mPathes, duration, PathType.CatmullRom).SetEase(CONFIG.MovementEaseCurve).OnComplete(onMovementEnd + onBodyMovmentEnd);
            //transform.DOMove(destPos, 1.5f).OnComplete(onMovementEnd + onBodyMovmentEnd);
        }

        private void onMovementEnd()
        {
            gameObject.SetActive(false);
        }
    }
}
