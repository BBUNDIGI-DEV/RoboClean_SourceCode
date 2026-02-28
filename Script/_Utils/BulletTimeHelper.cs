using RoboClean.Data;
using UnityEngine;

namespace RoboClean.Utils
{
    public static class TimeUtils
    {
        private static BulletTimeHelper mHelper
        {
            get
            {
                if (_Helper == null)
                {
                    _Helper = new BulletTimeHelper();
                }
                return _Helper;
            }
        }

        private static BulletTimeHelper _Helper;

        public static void PlayBulletTime(BulletTimeData inputData)
        {
            mHelper.PlayBulletTime(inputData);
        }
    }

    public class BulletTimeHelper
    {
        private readonly float DFEAULT_TIMESCALE;
        private readonly float DEFAULT_FIXED_TIMESCALE;
        private BulletTimeData mCurrentUsedData;
        private UpdateTimer mBulletTimeTimer
        {
            get
            {
                UpdateTimer returnTimer = null;
                bool result = GlobalTimer.Instance.TryGetTimerDataOrNull("BulletTime", out returnTimer);

                if (!result)
                {
                    returnTimer = new UpdateTimer(eTimerUpdateMode.LateUpdate, 0.0f, endBulletTime, updateBulletTime);
                    GlobalTimer.Instance
                        .AddTimer("BulletTime", returnTimer);
                    mBulletTimeTimer.SetIsAffectedByBulletTime(true);
                }

                return returnTimer;
            }
        }

        public BulletTimeHelper()
        {
            DFEAULT_TIMESCALE = Time.timeScale;
            DEFAULT_FIXED_TIMESCALE = Time.fixedDeltaTime;
        }

        public void PlayBulletTime(BulletTimeData data)
        {
            mCurrentUsedData = data;
            mBulletTimeTimer.ChangeDuration(mCurrentUsedData.BulletDuration).StartTimer();
        }

        private void updateBulletTime(float normalizedTime, float timePass)
        {
            float bulletTimeFactor = mCurrentUsedData.BulletTimeCurve.Evaluate(normalizedTime);
            bulletTimeFactor = Mathf.Clamp(bulletTimeFactor, 0.05f, 1.0f);
            Time.timeScale = DFEAULT_TIMESCALE * bulletTimeFactor;
            Time.fixedDeltaTime = DEFAULT_FIXED_TIMESCALE * bulletTimeFactor;
            GlobalTimer.Instance.SetBulletPowerFactor(bulletTimeFactor);
        }

        private void endBulletTime()
        {
            Time.timeScale = DFEAULT_TIMESCALE;
            Time.fixedDeltaTime = DEFAULT_FIXED_TIMESCALE;
            GlobalTimer.Instance.SetBulletPowerFactor(1.0f);

        }
    }
}
