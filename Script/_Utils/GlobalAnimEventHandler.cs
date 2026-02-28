using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RoboClean.Utils
{
    public static class GlobalAnimEventHandler
    {
        private static Dictionary<string, UnityAction> mTagExitEventDic;

        static GlobalAnimEventHandler()
        {
            mTagExitEventDic = new Dictionary<string, UnityAction>();
        }

        public static void AddGlobalStateExitEvent(string tag, UnityAction action)
        {
            mTagExitEventDic.Add(tag, action);
        }

        public static void TryInvokeOnExitEvent(AnimatorStateInfo stateInfo)
        {
            foreach (var item in mTagExitEventDic)
            {
                if (stateInfo.IsTag(item.Key))
                {
                    item.Value.Invoke();
                }
            }
        }
    }
}
