using UnityEngine;
using RoboClean.Data;
using RoboClean.Input;
using UnityEditor;
using RoboClean.Editor.Utils;

namespace RoboClean.Character
{
    [RequireComponent(typeof(Rigidbody))]
    public class SkillConfigDebugger : MonoBehaviour
    {
        [SerializeField] private SkillConfig CONFIG;
        private void Awake()
        {
            if (!Application.isEditor)
            {
                Destroy(this);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (CONFIG == null)
            {
                return;
            }

            Vector3 attackDir = Vector3.zero;
            if (!InputManager.IsExist)
            {
                attackDir = transform.forward;
            }
            else
            {
                attackDir = InputManager.Instance.GetAttackAim(GetComponent<Rigidbody>());
            }
            Handles.color = Color.green;
            const float DRAW_FREQUENCY = 100;
            for (int i = 0; i < DRAW_FREQUENCY; i++)
            {
                float factor = i / DRAW_FREQUENCY;
                float angleFactor = CONFIG.AimAisstanceConfig.AdjustCurve.Evaluate(factor);
                float curDegree = Mathf.Lerp(CONFIG.AimAisstanceConfig.AssistanceDegree * 2, 0, angleFactor);
                float curDistance = Mathf.Lerp(0, CONFIG.AimAisstanceConfig.MaxAssistanceDistance, factor);
                HandlesDrawUtil.DrawWireArc(transform.position + new Vector3(0.0f, 0.0f, 0.0f), Vector3.up, attackDir,
                    curDegree, curDistance);

            }
            Handles.DrawLine(transform.position, transform.position + attackDir * CONFIG.AimAisstanceConfig.MaxAssistanceDistance);
        }
#endif
    }
}