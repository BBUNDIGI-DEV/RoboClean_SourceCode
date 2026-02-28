using RoboClean.Input;
using RoboClean.Managers;
using UnityEngine;

namespace RoboClean.UI
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject MENU_UI;
        private bool mIsPaused;

        private void Awake()
        {
            MENU_UI.gameObject.SetActive(false);
        }

        private void Start()
        {
            InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.PauseGame.ToString(), setGamePausedInputEvent);
        }

        private void OnEnable()
        {
            InputManager.Instance.AddInputCallback(eInputSections.UI, eUIInputName.PauseGame.ToString(), setResumeInputEvent);
        }

        private void OnDisable()
        {
            if (InputManager.IsExist)
            {
                InputManager.Instance.RemoveInputCallback(eInputSections.UI, eBattleInputName.PauseGame.ToString(), setResumeInputEvent);
            }
        }

        private void OnDestroy()
        {
            if (InputManager.IsExist)
            {
                InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eUIInputName.PauseGame.ToString(), setGamePausedInputEvent);
            }
        }

        public void ResumeGameUnityEvent()
        {
            MENU_UI.SetActive(false);
            mIsPaused = false;
            Time.timeScale = 1.0f;
            InputManager.Instance.SwitchInputSection(eInputSections.BattleGamePlay);
        }

        public void BackToTitleUnityEvent()
        {
            ResumeGameUnityEvent();
            SceneSwitchingManager.Instance.LoadOtherScene(eSceneName.scene_title, true);
        }

        private void setGamePaused()
        {
            MENU_UI.SetActive(true);
            mIsPaused = true;
            Time.timeScale = 0.0f;
            InputManager.Instance.SwitchInputSection(eInputSections.UI);
        }

        private void setGamePausedInputEvent(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {

            setGamePaused();
        }

        private void setResumeInputEvent(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (mIsPaused)
            {
                ResumeGameUnityEvent();
            }
        }
    }
}
