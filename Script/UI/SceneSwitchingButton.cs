using RoboClean.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace RoboClean.UI
{
    [RequireComponent(typeof(Button))]
    public class SceneSwitchingButton : MonoBehaviour
    {
        [SerializeField] private eSceneName SCENE_NAME;
        [SerializeField] private bool imshi;
        [SerializeField] private GameObject parent;

        private void Awake()
        {
            if (imshi)
            {
                GetComponent<Button>().onClick.AddListener(() => SceneSwitchingManager.Instance.LoadOtherScene(
                    SceneSwitchingManager.Instance.CurrentScene, true));
                GetComponent<Button>().onClick.AddListener(() => parent.SetActive(false));
                return;
            }
            GetComponent<Button>().onClick.AddListener(() => SceneSwitchingManager.Instance.LoadOtherScene(SCENE_NAME, true));
        }
    }
}