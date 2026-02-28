using System;
using UnityEngine;
using UnityEngine.AI;

namespace RoboClean.Utils
{
    public static class NavmeshExtension
    {
        public static bool GetCircularRandomPoint(this NavMeshAgent self,
            Vector3 origin,
            float range,
            out Vector3 result)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector3 randomPoint = origin + UnityEngine.Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }
    }
}