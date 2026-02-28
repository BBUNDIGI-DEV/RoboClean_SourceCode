using UnityEngine;
using Cinemachine;
using RoboClean.Player;
using RoboClean.Common;

namespace RoboClean.Utils
{
    public class CameraUtils : SingletonClass<CameraUtils>
    {
        public CameraActingHelper Actor
        {
            get; private set;
        }

        public Camera MainCamera
        {
            get
            {
                return mMainCamera;
            }
        }

        public GlitchInovker GlitchInvoker
        {
            get
            {
                {
                    return mGlitchInvoker;
                }
            }
        }

        private Camera mMainCamera;
        private GlitchInovker mGlitchInvoker;


        protected override void Awake()
        {
            base.Awake();
            mMainCamera = Camera.main;
        }

        private void Start()
        {
            Actor = new CameraActingHelper(mMainCamera.GetComponent<CinemachineBrain>());
            mGlitchInvoker = GetComponent<GlitchInovker>();
        }

        public bool ScreenPointToPlaneHitPoint(Vector3 mousePoint, Plane plane, out Vector3 hitPoint)
        {
            Ray worldMouseRay = mMainCamera.ScreenPointToRay(mousePoint);
            float distance;
            hitPoint = Vector3.zero;
            if (plane.Raycast(worldMouseRay, out distance))
            {
                hitPoint = worldMouseRay.GetPoint(distance);
                return true;
            }
            return false;
        }
    }
}

