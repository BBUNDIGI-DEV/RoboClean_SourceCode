using UnityEngine;
using Cinemachine;
using RoboClean.Data;

namespace RoboClean.Player
{
    public class CameraFollowMouse : MonoBehaviour
    {
        private float mDefaultScreenX;
        private float mDefaultScreenY;

        private CinemachineVirtualCamera cine;

        private CameraConfig CONFIG
        {
            get
            {
                return RuntimeDataLoader.CameraConfig;
            }
        }

        private RuntimePlayData mRuntimePlayData
        {
            get
            {
                return RuntimeDataLoader.RuntimePlayData;
            }
        }

        private void Awake()
        {
            cine = GetComponent<CinemachineVirtualCamera>();
            mDefaultScreenX = cine.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX;
            mDefaultScreenY = cine.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY;
        }

        private void Update()
        {
            cameraFollowCursor();
        }

        private void cameraFollowCursor()
        {
            Vector2 mousePos = UnityEngine.Input.mousePosition;
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            float deltaX = (mousePos.x - screenCenter.x) / screenCenter.x;
            float deltaY = (mousePos.y - screenCenter.y) / screenCenter.y;

            var framingTransposer = cine.GetCinemachineComponent<CinemachineFramingTransposer>();
            framingTransposer.m_ScreenX = Mathf.Lerp(framingTransposer.m_ScreenX, mDefaultScreenX - deltaX * CONFIG.CameraMoveSensitivity, CONFIG.DampingSpeedX);
            framingTransposer.m_ScreenY = Mathf.Lerp(framingTransposer.m_ScreenY, mDefaultScreenY + deltaY * CONFIG.CameraMoveSensitivity, CONFIG.DampingSpeedY);
        }
    }
}