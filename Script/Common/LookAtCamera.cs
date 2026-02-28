using RoboClean.Utils;
using UnityEngine;

namespace RoboClean.Common
{
    public class LookAtCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (CameraUtils.IsExist)
            {
                transform.rotation = CameraUtils.Instance.MainCamera.transform.rotation;
            }
        }
    }
}
