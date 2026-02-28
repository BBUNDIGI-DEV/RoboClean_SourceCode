using RoboClean.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoboClean.UI
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private RuntimePlayData PLAY_DATA;

        private void Awake()
        {
            StopAllSceneCoroutines();
        }

        public static void StopAllSceneCoroutines()
        {
            foreach (var go in FindObjectsOfType<GameObject>())
            {
                var monoBehaviours = go.GetComponents<MonoBehaviour>();

                foreach (var mono in monoBehaviours)
                {
                    if (mono == null)
                    {
                        continue;
                    }
                    mono.StopAllCoroutines();
                }
            }
        }
        public void OnClickContinueButton()
        {
            SetSelectedNull();
            PLAY_DATA.IsPaused.Value = false;
        }

        public void OnClickTitleButton()
        {
            PLAY_DATA.IsQuitGame.Value = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("scene_title");
        }

        public void SetSelectedNull()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}