using UnityEngine;
using UnityEditor;
using RoboClean.Data;

namespace RoboClean.Character.AI
{
    public class MonsterActingDebugger : MonoBehaviour
    {
        [SerializeField] private MonsterConfig CONFIG;

        private void Awake()
        {
            if (!Application.isEditor)
            {
                Destroy(this);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (CONFIG == null)
            {
                return;
            }

            Handles.color = Color.blue;
            Handles.DrawWireDisc(transform.position, Vector3.up, CONFIG.AIMovementConfig.DetectionRange);
            //Handles.color = Color.red;
            //Handles.DrawWireDisc(transform.position, Vector3.up, CONFIG.AISkillConfig.MeleeAttackRange);
        }
#endif
    }
}
