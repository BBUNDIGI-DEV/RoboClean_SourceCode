using UnityEngine;

namespace RoboClean.Utils
{
    public static class DebugUtil
    {
        public static readonly Color[] COLOR_LIST = new Color[] { Color.red, Color.blue, Color.green, Color.cyan, Color.yellow };

        public static void DrawLineSwitchColor(Vector3 start, Vector3 end, int colorIndex, float duration)
        {
            Debug.DrawLine(start, end, COLOR_LIST[colorIndex % COLOR_LIST.Length], 1.0f);
        }
    }
}
