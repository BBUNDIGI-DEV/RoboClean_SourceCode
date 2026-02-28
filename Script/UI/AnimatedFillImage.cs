using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RoboClean.UI
{
    public class AnimatedFillImage : MonoBehaviour
    {
        [SerializeField] private Image HP_BAR;
        [SerializeField] private Image HP_SHADOW_BAR;
        [SerializeField] private RectTransform EDGE_IMAGE;
        [SerializeField] private float EDIGE_WIDTH;
        [SerializeField] private float INITIALIZE_FILL_UP;
        [SerializeField] private float MOVEMENT_SPEED;
        [SerializeField] private float SHADOW_FILL_UP_DELAY;

        private Queue<float> mDestQueue;
        private float mLastFillUp;
        private float mCurrentFillUp;
        private float mCurrentShadowFillUp;
        private float mDestFillUp;
        private bool mIsIncrease;

        private void Awake()
        {
            mCurrentFillUp = INITIALIZE_FILL_UP;
            mDestFillUp = float.MinValue;
            mDestQueue = new Queue<float>();
            EDGE_IMAGE.gameObject.SetActive(false);
        }

        public void EnqueueValue(float dest)
        {
            dest = Mathf.Clamp01(dest);
            mDestQueue.Enqueue(dest);
        }

        private void Update()
        {
            if (mDestQueue.Count == 0 && mDestFillUp == float.MinValue)
            {
                return;
            }
            if (mDestFillUp == float.MinValue)
            {
                mDestFillUp = mDestQueue.Dequeue();
                mLastFillUp = mCurrentFillUp;
                mCurrentShadowFillUp = mCurrentFillUp + SHADOW_FILL_UP_DELAY;
                mIsIncrease = Mathf.Sign(mDestFillUp - mCurrentFillUp) > 0.0f;
                EDGE_IMAGE.gameObject.SetActive(true);
            }

            float newFillUp = Mathf.Lerp(mCurrentFillUp, mDestFillUp, Time.deltaTime * MOVEMENT_SPEED);
            float newShadowFillUp = 0.0f;

            if (mIsIncrease)
            {
                newShadowFillUp = mDestFillUp;
            }
            else
            {
                newShadowFillUp = Mathf.Lerp(mCurrentShadowFillUp, mDestFillUp, Time.deltaTime * MOVEMENT_SPEED);
            }

            if (Mathf.Abs(newFillUp - mDestFillUp) < 0.005f && Mathf.Abs(newShadowFillUp - mDestFillUp) < 0.005f)
            {
                newFillUp = mDestFillUp;
                mDestFillUp = float.MinValue;
                EDGE_IMAGE.gameObject.SetActive(false);
            }

            HP_BAR.fillAmount = newFillUp;
            HP_SHADOW_BAR.fillAmount = Mathf.Clamp(newShadowFillUp, mDestFillUp, mLastFillUp); ;
            EDGE_IMAGE.anchoredPosition =
                new Vector2(Mathf.Lerp(-EDIGE_WIDTH, EDIGE_WIDTH, newFillUp), EDGE_IMAGE.anchoredPosition.y);

            mCurrentFillUp = newFillUp;
            mCurrentShadowFillUp = newShadowFillUp;
        }
    }
}

