using RoboClean.Input;
using RoboClean.Managers;
using UnityEngine;

namespace RoboClean.UI
{
    public class GameTitleManager : MonoBehaviour
    {
        private bool mIsLoading;

        private void Awake()
        {
            mIsLoading = false;
        }

        public void Start()
        {
            InputManager.Instance.SwitchInputSection(eInputSections.Menu);
        }

        public void StartGame()
        {
            if (mIsLoading)
            {
                return;
            }

            mIsLoading = true;
            SceneSwitchingManager.Instance.LoadOtherScene(eSceneName.scene_story_cutscene, true);
        }

        public void QuitGame()
        {
            if (mIsLoading)
            {
                return;
            }

            Application.Quit();
        }
    }
}