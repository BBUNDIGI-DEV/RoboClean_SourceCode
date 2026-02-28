using UnityEngine;
using UnityEditor;

public class AnimationClipImporHelper : AssetPostprocessor
{
    private const string MODEL_PATH = "Assets/ArtResource/Models";
    private static string mImportedFBXName = string.Empty;
    private static string mImportedClipName = string.Empty;

    private void OnPostprocessAnimation(GameObject obj, AnimationClip clip)
    {
        if(assetPath.Contains(MODEL_PATH))
        {
            return;
        }
        mImportedFBXName = obj.name;
        mImportedClipName = clip.name;
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        if(mImportedFBXName == string.Empty)
        {
            return;
        }
        string assetPath = string.Empty;
        foreach (string str in importedAssets)
        {
            if (str.Contains(mImportedFBXName))
            {
                assetPath = str;
                break;
            }
        }
        Debug.Assert(assetPath != string.Empty);

        mImportedFBXName = string.Empty;
        Object[] subassets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        AnimationClip updatedClipData = null;
        foreach (var item in subassets)
        {
            if(item.name == mImportedClipName)
            {
                updatedClipData = item as AnimationClip;
                break;
            }
        }

        string[] searchedClipList = AssetDatabase.FindAssets(mImportedClipName, new string[] { MODEL_PATH });
        if (searchedClipList.Length == 0)
        {
            return;
        }

        bool autoImportUserOK = EditorUtility.DisplayDialog("Anim motion update helper", 
            "Old animation clip detected, Do you want to update animation clip?",
            "Do It",
            "Nope");

        if (!autoImportUserOK)
        {
            return;
        }

        string oldClipFilePath = string.Empty;
        string progressDebugString = string.Empty;
        bool isSuccess = true;
        if (searchedClipList.Length >= 2)
        {
            oldClipFilePath = EditorUtility.OpenFolderPanel("Duplicated animation clip name more then two detected, Please select clip for update", MODEL_PATH, "ANI_clip");
            if (oldClipFilePath == string.Empty)
            {
                progressDebugString = "File path is not selected, you must select specific file path for udpate";
                isSuccess = false;
                goto LogAndReturn;
            }
        }
        else
        {
            oldClipFilePath = AssetDatabase.GUIDToAssetPath(searchedClipList[0]);
        }

        progressDebugString += $"Old animation clip file path successfully catched [{oldClipFilePath}] \n";

        //Catch Events and loop type in old data
        AnimationClip prevClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(oldClipFilePath);
        AnimationEvent[] prevEvents = prevClip.events;
        bool prevLoopType = AnimationUtility.GetAnimationClipSettings(prevClip).loopTime;

        progressDebugString += $"Events is catched Count: [{ prevEvents.Length}]\n";
        progressDebugString += $"loop time check box catached [{prevLoopType}]\n";

        //Create clone of subasset animation clip in fbx file
        var clone = Object.Instantiate(updatedClipData);
        clone.name = prevClip.name;
        //Update old events and loop type into new updated animation clip
        AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(clone);
        clipSetting.loopTime = prevLoopType;
        AnimationUtility.SetAnimationClipSettings(clone, clipSetting);
        AnimationUtility.SetAnimationEvents(clone, prevEvents);
        //Save updated animation clip into old clip file path
        
        EditorUtility.CopySerialized(clone, prevClip);
        AssetDatabase.SaveAssets();
    //Log
    LogAndReturn:
        if(isSuccess)
        {
            progressDebugString += "Now Animation Update is successfully done";
            Debug.Log(progressDebugString);
        }
        else
        {
            Debug.LogError(progressDebugString);
        }
        return;
    }
}