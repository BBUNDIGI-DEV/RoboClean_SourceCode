using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using RoboClean.UI;
using RoboClean.Common;
using RoboClean.Player;

namespace RoboClean.Managers
{
    public class SceneSwitchingManager : SingletonClass<SceneSwitchingManager>
    {
        public ObservedData<eSceneName> OnSceneSwitchingStart;
        public ObservedData<eSceneName> CurrentScene;

        [SerializeField] private OnScreenFadingHelper SCREEN_FADING_HELPER;
        private bool mIsOnLoading;

        protected override void Awake()
        {
            base.Awake();
            CurrentScene.Value = eSceneName.None;
        }

        protected void Start()
        {
            tryUpdateCurrentScene();
        }

        private void Update()
        {
            if (CurrentScene == eSceneName.None)
            {
                tryUpdateCurrentScene();
            }
            else
            {
                enabled = false;
            }
        }


        public void LoadOtherScene(eSceneName newScene, bool withFadingInOut = false, bool reloadAll = false)
        {
            if (mIsOnLoading)
            {
                return;
            }

            StartCoroutine(LoadSceneRoutine(newScene, withFadingInOut, reloadAll));
        }

        public void ReloadScene()
        {

        }

        private void tryUpdateCurrentScene()
        {
            for (int i = 0; i < (int)eSceneName.Count; i++)
            {
                eSceneName checkSceneName = (eSceneName)i;

                if (SceneManager.GetSceneByName(checkSceneName.ToString()).isLoaded)
                {
                    CurrentScene.Value = checkSceneName;
                    OnSceneSwitchingStart.Value = checkSceneName;

                    return;
                }
            }

            LoadOtherScene(eSceneName.scene_title);
            CurrentScene.Value = eSceneName.scene_title;
            OnSceneSwitchingStart.Value = eSceneName.scene_title;
        }

        private IEnumerator LoadSceneRoutine(eSceneName newScene, bool withFadingInOut = false, bool reloadAll = false)
        {
            Debug.Assert(newScene != eSceneName.None,
                "You Cannot load None Scene this is for checking state");
            Debug.Assert(newScene != eSceneName.Count,
                "You Cannot load count scene this is for count list of enum");
            mIsOnLoading = true;
            OnSceneSwitchingStart.Value = newScene;
            if (withFadingInOut)
            {
                SCREEN_FADING_HELPER.PlayOnScreenFadeIn();
                yield return new WaitUntil(() => !SCREEN_FADING_HELPER.IsFadeInPlaying);
            }

            if (reloadAll)
            {
                foreach (eAutoLoadedSceneName sceneName in GetAutoLoadingScene(newScene))
                {
                    if (SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
                    {
                        AsyncOperation unloadAdditiveSceneAsync = SceneManager.UnloadSceneAsync(sceneName.ToString());
                    }
                }
            }

            if (CurrentScene != eSceneName.None)
            {
                AsyncOperation unloadAsync = SceneManager.UnloadSceneAsync(CurrentScene.ToString());
                yield return new WaitUntil(() => unloadAsync.isDone);
            }

            yield return sceneLoadingRoutine(newScene);
            CurrentScene.Value = newScene;

            if (withFadingInOut)
            {
                SCREEN_FADING_HELPER.PlayOnScreenFadeOut();
                yield return new WaitUntil(() => !SCREEN_FADING_HELPER.IsFadeInPlaying);
            }
            mIsOnLoading = false;
        }

        private IEnumerator sceneLoadingRoutine(eSceneName newScene)
        {
            foreach (eAutoLoadedSceneName sceneName in GetAutoUnLoadingScene(newScene))
            {
                if (SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
                {
                    Debug.Log(sceneName);
                    AsyncOperation unloadAsync = SceneManager.UnloadSceneAsync(sceneName.ToString(), UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                    yield return new WaitUntil(() => unloadAsync.isDone);

                }
            }

            SceneManager.LoadScene(newScene.ToString(), LoadSceneMode.Additive);

            foreach (eAutoLoadedSceneName sceneName in GetAutoLoadingScene(newScene))
            {
                if (!SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
                {
                    SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Additive);
                }
            }

        }

        private IEnumerable<eAutoLoadedSceneName> GetAutoLoadingScene(eSceneName sceneName)
        {
            switch (sceneName)
            {
                case eSceneName.scene_mainplay_stage0:
                case eSceneName.scene_mainplay_stage1:
                case eSceneName.scene_mainplay_stage2:
                    yield return eAutoLoadedSceneName.scene_global_ui;
                    yield return eAutoLoadedSceneName.scene_player;
                    break;
                case eSceneName.scene_title:
                    break;
                default:
                    break;
            }
        }

        private IEnumerable<eAutoLoadedSceneName> GetAutoUnLoadingScene(eSceneName sceneName)
        {
            switch (sceneName)
            {
                case eSceneName.scene_mainplay_stage0:
                case eSceneName.scene_mainplay_stage1:
                case eSceneName.scene_mainplay_stage2:
                    break;
                case eSceneName.scene_title:
                    yield return eAutoLoadedSceneName.scene_global_ui;
                    yield return eAutoLoadedSceneName.scene_player;
                    break;
                default:
                    break;
            }

        }
    }

    public enum eSceneName
    {
        None = -1,
        scene_title,
        scene_story_cutscene,
        scene_mainplay_stage0,
        scene_mainplay_stage1,
        scene_mainplay_stage2,
        scene_mainplay_stage3,
        Count,
    }

    public enum eAutoLoadedSceneName
    {
        scene_global_managers,
        scene_global_ui,
        scene_player,
    }
}