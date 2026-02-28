#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RoboClean.Editor.Utils
{
    public static class HandlesDrawUtil
    {
        public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 centerFrom, float angle, float distance, float thickness = 1.0f)
        {
            Handles.DrawWireArc(center, normal, centerFrom,
               angle / 2, distance / 2, thickness);
            Handles.DrawWireArc(center, normal, centerFrom,
               -(angle / 2), distance / 2, thickness);
        }
    }
}
#endif