#if UNITY_EDITOR
using RoboClean.Managers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class AutoSceneLoader
{
    static AutoSceneLoader()
    {
        EditorSceneManager.activeSceneChangedInEditMode += autoSceneLoading;
    }

    private static void autoSceneLoading(Scene prevScene, Scene newScene)
    {
        if (BuildPipeline.isBuildingPlayer)
        {
            return;
        }

        bool isParsed = System.Enum.TryParse(newScene.name, out eSceneName sceneName);
        if (isParsed)
        {
            autoSceneLoading(sceneName);
        }
    }

    private static void autoSceneLoading(eSceneName sceneName)
    {
        string globalScenePath = AssetDatabaseUtils.GetSceneAssetPathBySceneNameEnum(eAutoLoadedSceneName.scene_global_managers.ToString());
        Scene globalScene = EditorSceneManager.OpenScene(globalScenePath, OpenSceneMode.Additive);

        switch (sceneName)
        {
            case eSceneName.scene_mainplay_stage0:
            case eSceneName.scene_mainplay_stage1:
            case eSceneName.scene_mainplay_stage2:
            case eSceneName.scene_mainplay_stage3:
                EditorSceneManager.OpenScene(globalScenePath, OpenSceneMode.Additive);

                string globalUIScenePath = AssetDatabaseUtils.GetSceneAssetPathBySceneNameEnum(eAutoLoadedSceneName.scene_global_ui.ToString());
                EditorSceneManager.OpenScene(globalUIScenePath, OpenSceneMode.Additive);

                string playerScenePath = AssetDatabaseUtils.GetSceneAssetPathBySceneNameEnum(eAutoLoadedSceneName.scene_player.ToString());
                EditorSceneManager.OpenScene(playerScenePath, OpenSceneMode.Additive);
                break;
            default:
                break;
        }

        SceneManager.SetActiveScene(globalScene);
    }
}
#endif
