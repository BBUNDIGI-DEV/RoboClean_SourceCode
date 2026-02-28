using UnityEngine;
using System.Collections;
using RoboClean.Managers;
using RoboClean.Utils;
using RoboClean.Character.AI;

namespace RoboClean.UI
{
    public class BossHPBar : MonoBehaviour
    {
        [SerializeField] private AnimatedFillImage FILL_IMAGE;
        private Coroutine routine;

        private void Start()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            SceneSwitchingManager.Instance.CurrentScene.AddListener(startRoutine, true);
            FILL_IMAGE.EnqueueValue(1.0f);
        }

        private void OnDestroy()
        {
            if (!SceneSwitchingManager.IsExist)
            {
                return;
            }

            SceneSwitchingManager.Instance.CurrentScene.RemoveListener(startRoutine);
        }

        private void startRoutine(eSceneName sceneName)
        {
            if (sceneName == eSceneName.scene_mainplay_stage2)
            {
                if (routine != null)
                {
                    StopCoroutine(routine);
                }

                routine = StartCoroutine(findBoss());
            }
        }

        private IEnumerator findBoss()
        {
            while (true)
            {
                bool isFounded = tryFindActivateBossAndInitialize();
                if (isFounded)
                {
                    break;
                }
                else
                {
                    yield return CommonWaitForSeconds.GetWaitForSeconds(0.2f);
                }
            }
            routine = null;
        }

        private void setHPUI(float curHP)
        {
            FILL_IMAGE.EnqueueValue(curHP);

            if (curHP <= 0.0f)
            {
                gameObject.SetActive(false);
            }
        }


        private bool tryFindActivateBossAndInitialize()
        {
            RoboBossMonsterController roboBossController = FindFirstObjectByType<RoboBossMonsterController>();
            if (roboBossController == null)
            {
                return false;
            }
            transform.GetChild(0).gameObject.SetActive(true);

            roboBossController.Status.NormalizedHP.AddListener(setHPUI);
            return true;
        }
    }
}