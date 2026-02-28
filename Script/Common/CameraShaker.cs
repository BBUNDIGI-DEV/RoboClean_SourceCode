using UnityEngine;
using Cinemachine;

namespace RoboClean.Player
{
    public sealed class CameraShaker : CameraActorBase
    {
        private CinemachineBasicMultiChannelPerlin mVcamNoiseActor;

        public CameraShaker() : base()
        {
        }


        public override void SetActor(CinemachineVirtualCamera vcam, AnimationCurve curve, float delay, float duration)
        {
            base.SetActor(vcam, curve, delay, duration);
            mVcamNoiseActor = mActivateVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        protected override void startActing()
        {
            mVcamNoiseActor.m_FrequencyGain = 0.0f;
            mVcamNoiseActor.m_AmplitudeGain = 0.0f;
        }

        protected override void updateActing(float normalizedTime, float timePass)
        {
            float curveValue = mActingCurve.Evaluate(normalizedTime);
            curveValue = Mathf.Abs(curveValue);
            mVcamNoiseActor.m_FrequencyGain = curveValue;
            mVcamNoiseActor.m_AmplitudeGain = curveValue;
        }

        protected override void endActing()
        {
            mVcamNoiseActor.m_FrequencyGain = 0.0f;
            mVcamNoiseActor.m_AmplitudeGain = 0.0f;
        }
    }
}