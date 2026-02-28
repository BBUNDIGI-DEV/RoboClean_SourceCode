using UnityEngine;
using Cinemachine;

namespace RoboClean.Player
{
    public abstract class CameraActorBase
    {
        private readonly int INSTANCE_ID;
        private static int msInstanceCount;

        public bool mIsActivate
        {
            get
            {
                return mActingTimer.IsActivate;
            }
        }

        protected CinemachineVirtualCamera mActivateVCam;
        protected AnimationCurve mActingCurve;
        protected float mDelay;

        private DelayTimer mActingTimer
        {
            get
            {
                DelayTimer returnTimer = null;
                bool result = GlobalTimer.Instance.TryGetTimerDataOrNull("CameraActing" + INSTANCE_ID, out returnTimer);

                if (!result)
                {
                    returnTimer = new DelayTimer(eTimerUpdateMode.FixedUpdate, 0.0f, startActing, endActing);
                    returnTimer.ChangeUpdatedCallback(updateActing);
                    GlobalTimer
                        .Instance
                        .AddTimer("CameraActing" + INSTANCE_ID, returnTimer);
                }

                return returnTimer;
            }
        }

        public CameraActorBase()
        {
            msInstanceCount++;
            INSTANCE_ID = msInstanceCount;
        }

        public virtual void SetActor(CinemachineVirtualCamera vcam, AnimationCurve curve, float delay, float duration)
        {
            Debug.Assert(vcam != null, "initialize camera actor with null vcam");
            if (mActingTimer.IsActivate)
            {
                mActingTimer.PauseTimer();
            }

            mActivateVCam = vcam;
            mActingCurve = curve;
            mActingTimer.ChangeDelayTime(delay).ChangeDuration(duration);
            mActingTimer.StartTimer();
        }

        protected virtual void startActing()
        {
            //PlaceHolder
        }

        protected abstract void updateActing(float normalizedTime, float timePass);

        protected abstract void endActing();
    }
}