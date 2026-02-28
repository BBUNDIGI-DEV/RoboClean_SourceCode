using RoboClean.Managers;
using UnityEngine;

namespace RoboClean.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject GAME_OVER_UI;

        private void Awake()
        {
            GAME_OVER_UI.gameObject.SetActive(false);
        }

        public void RestartGameUnityEvent()
        {
            SceneSwitchingManager.Instance.LoadOtherScene(SceneSwitchingManager.Instance.CurrentScene, true, true);
            GAME_OVER_UI.SetActive(false);
            return;
        }

        public void ReturnToTitleUnityEvent()
        {
            SceneSwitchingManager.Instance.LoadOtherScene(eSceneName.scene_title, true);
            GAME_OVER_UI.SetActive(false);
        }

    }
}