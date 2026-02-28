using RoboClean.Managers;
using System;
using UnityEditor;
using UnityEngine;

namespace RoboClean.Editor.Utils
{
    [InitializeOnLoad]
    public static class EnumValidation
    {
        static EnumValidation()
        {
            ValidateEnumRepresentSceneName<eSceneName>();
            ValidateEnumRepresentSceneName<eAutoLoadedSceneName>();
        }

        public static void ValidateEnumRepresentSceneName<T>() where T : Enum
        {
            string[] sceneAssets = AssetDatabaseUtils.GetScenesAssetPath();

            string[] enumNameArray = Enum.GetNames(typeof(T));

            for (int i = 0; i < enumNameArray.Length; i++)
            {
                if (enumNameArray[i] == "Count" || enumNameArray[i] == "None")
                {
                    continue;
                }

                bool isFounded = false;
                for (int j = 0; j < sceneAssets.Length; j++)
                {
                    if (sceneAssets[j].Contains(enumNameArray[i]))
                    {
                        isFounded = true;
                        break;
                    }
                }

                if (!isFounded)
                {
                    Debug.LogError($"Validation Fail" +
                        $"Enum Name [{typeof(T).Name}] / Failed Enum : [{enumNameArray[i]}]");
                }
            }
        }
    }
}