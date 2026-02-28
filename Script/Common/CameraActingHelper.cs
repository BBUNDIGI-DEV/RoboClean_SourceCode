using UnityEngine;
using Cinemachine;
using RoboClean.Data;

namespace RoboClean.Player
{
    public class CameraActingHelper
    {
        private readonly CinemachineBrain CINE_BRAIN;

        private CinemachineVirtualCamera mActiveVCam;
        private CameraShaker mCameraShakeHelper;
        private CameraZoomAndOutHelper mZoomAndOutHelper;

        public CameraActingHelper(CinemachineBrain cineBrain)
        {
            Debug.Assert(cineBrain != null, "Create Camera Acting Helper with null cineBrain");
            CINE_BRAIN = cineBrain;
            tryUpdateVCAM();
            mCameraShakeHelper = new CameraShaker();
            mZoomAndOutHelper = new CameraZoomAndOutHelper(5.5f);
        }

        public void ProcessCameraActing(CameraActingEventData data)
        {
            tryUpdateVCAM();

            if (data.ActingType.HasFlag(eCameraActingType.Shake))
            {
                mCameraShakeHelper.SetActor(mActiveVCam, data.ShakeCurve, data.ShakeDelay, data.ShakeDuration);
            }
            if (data.ActingType.HasFlag(eCameraActingType.ZoomInAndOutCurve))
            {
                mZoomAndOutHelper.SetActor(mActiveVCam, data.ZoomInOutCurve, data.ZoomInOutDelay, data.ZoomInAndOutDuration);
            }

        }

        private void tryUpdateVCAM()
        {
            if (mActiveVCam != null && CINE_BRAIN.IsLive(mActiveVCam))
            {
                return;
            }

            if (CINE_BRAIN.ActiveVirtualCamera == null)
            {
                return;
            }

            mActiveVCam = CINE_BRAIN.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        }
    }
}