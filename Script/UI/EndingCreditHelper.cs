using RoboClean.Managers;
using TMPro;
using UnityEngine;

namespace RoboClean.UI
{
    public class EndingCreditHelper : MonoBehaviour
    {
        private const int MAX_SPEED = 1 << 3;
        [SerializeField] private TMP_Text TEXT_UI;
        private int mCurrentSpeed = 1;

        public void ReurturnToTitleAnimEvent()
        {
            SceneSwitchingManager.Instance.LoadOtherScene(eSceneName.scene_title, true);
        }

        public void SpeedButton()
        {
            if (mCurrentSpeed == MAX_SPEED)
            {
                mCurrentSpeed = 1;
            }
            else
            {
                mCurrentSpeed = mCurrentSpeed << 1;
            }

            GetComponent<Animation>()["ANI_ending"].speed = mCurrentSpeed;
            TEXT_UI.text = $"x{mCurrentSpeed}";

        }
    }
}
