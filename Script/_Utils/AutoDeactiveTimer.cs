using System.Collections;
using UnityEngine;
using System;

namespace RoboClean.Utils
{
    public class AutoDeactivateTimer : MonoBehaviour
    {
        private WaitForSeconds mWaitingTime;
        private Action mOnTimerEnd;

        public void SetWaitingTime(float time)
        {
            mWaitingTime = new WaitForSeconds(time);
        }

        public void SetOnTimerEndCallback(Action onTimerEnd)
        {
            if (mOnTimerEnd == null)
            {
                mOnTimerEnd = onTimerEnd;
            }
            else
            {
                mOnTimerEnd += onTimerEnd;
            }
        }

        public void StartTimer()
        {
            gameObject.SetActive(true);
            StartCoroutine(TimerRoutine());
        }

        private IEnumerator TimerRoutine()
        {
            yield return mWaitingTime;
            gameObject.SetActive(false);
            if (mOnTimerEnd != null)
            {
                mOnTimerEnd.Invoke();
            }
        }
    }
}