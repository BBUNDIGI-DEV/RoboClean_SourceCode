using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.Utils
{
    public static class CommonWaitForSeconds
    {
        public static readonly WaitForSeconds WAIT_ONE_SECONDS;
        public static readonly WaitForSeconds WAIT_POINT_ONE_SECONDS;
        public static readonly WaitForSeconds WAIT_POINT_ZERO_ONE_SECONDS;
        public static Dictionary<float, WaitForSeconds> WaitForSecondsPool;

        static CommonWaitForSeconds()
        {
            WaitForSecondsPool = new Dictionary<float, WaitForSeconds>(10);
            WAIT_ONE_SECONDS = new WaitForSeconds(1.0f);
            WAIT_POINT_ONE_SECONDS = new WaitForSeconds(0.1f);
            WAIT_POINT_ZERO_ONE_SECONDS = new WaitForSeconds(0.01f);
        }

        public static WaitForSeconds GetWaitForSeconds(float waitTime)
        {
            if (WaitForSecondsPool.ContainsKey(waitTime))
            {
                return WaitForSecondsPool[waitTime];
            }
            else
            {
                WaitForSeconds newWait = new WaitForSeconds(waitTime);
                WaitForSecondsPool.Add(waitTime, newWait);
                return newWait;
            }
        }
    }
}