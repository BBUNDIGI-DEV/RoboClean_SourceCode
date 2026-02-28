using UnityEngine;
using UnityEngine.UI;

namespace RoboClean.Utils.Analiyze
{
    public class FrameViewer : MonoBehaviour
    {
        [SerializeField] private Text TEXT;
        private float mDeltaTime = 0f;

        public void Awake()
        {
            Debug.Log(Application.targetFrameRate);
        }
        void Update()
        {
            mDeltaTime += (Time.unscaledDeltaTime - mDeltaTime) * 0.1f;
            float ms = mDeltaTime * 1000f;
            float fps = 1.0f / mDeltaTime;
            string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);
            TEXT.text = text;
        }
    }
}
