using RoboClean.Character;
using RoboClean.Data;
using RoboClean.Input;
using RoboClean.Managers;
using UnityEngine;

namespace RoboClean.Utils
{
    public static class EnumExtension
    {
        public static bool IsAttackType(this eActorType actorType)
        {
            const eActorType ATTACK_TYPE_START = eActorType.NormalAttack;
            const eActorType ATTACK_TYPE_END = eActorType.SpecialAttack2;
            return (int)actorType >= (int)ATTACK_TYPE_START && (int)actorType <= (int)ATTACK_TYPE_END;
        }

        public static bool CheckTarget(this eTargetTag tag, string targetTag)
        {
            switch (tag)
            {
                case eTargetTag.Player:
                    return targetTag == "Player";
                case eTargetTag.Enemey:
                    return targetTag == "Enemey";
                case eTargetTag.All:
                    return targetTag == "Enemey" || targetTag == "Player";
                default:
                    Debug.LogError(tag);
                    break;
            }
            return false;
        }

        public static bool CheckMainPlayScene(this eSceneName sceneName)
        {
            return sceneName.ToString().Contains("mainplay");
        }

        public static System.Type GetInputNameEnumType(this eInputSections inputSections, string targetTag)
        {
            switch (inputSections)
            {
                case eInputSections.BattleGamePlay:
                    return typeof(eBattleInputName);
                case eInputSections.CutScene:
                    return typeof(eCutSceneInputName);
                case eInputSections.Dialouge:
                    return typeof(eDialogueActingType);
                case eInputSections.Menu:
                    return typeof(eMenuInputName);
                case eInputSections.UI:
                default:
                    Debug.LogError($"dfeault switch [{inputSections}]");
                    break;
            }
            return null;
        }
    }
}