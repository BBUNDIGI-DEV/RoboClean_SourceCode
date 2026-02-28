using UnityEngine;
using Cinemachine;


namespace RoboClean.Player
{
    public class CameraZoomAndOutHelper : CameraActorBase
    {
        private readonly float DEFAULT_SIZE;

        public CameraZoomAndOutHelper(float defaultSize) : base()
        {
            DEFAULT_SIZE = defaultSize;
        }

        public override void SetActor(CinemachineVirtualCamera vcam, AnimationCurve curve, float delay, float duration)
        {
            base.SetActor(vcam, curve, delay, duration);
        }

        protected override void updateActing(float normalizedTime, float timePass)
        {
            float curveValue = mActingCurve.Evaluate(normalizedTime) + 1;
            if (curveValue < 0.005f)
            {
                return;
            }

            float sizeMultiplier = Mathf.Abs(curveValue);
            float newSize = curveValue < 0 ? DEFAULT_SIZE * sizeMultiplier : DEFAULT_SIZE / sizeMultiplier;
            mActivateVCam.m_Lens.OrthographicSize = newSize;
        }

        protected override void endActing()
        {
            mActivateVCam.m_Lens.OrthographicSize = DEFAULT_SIZE;
        }
    }
}