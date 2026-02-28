using RoboClean.Data;
using UnityEngine;

namespace RoboClean.UI
{
    [RequireComponent(typeof(Animator))]
    public class ToxicOnScreenEffect : MonoBehaviour
    {
        private Animator mAnim;
        private void Awake()
        {
            mAnim = GetComponent<Animator>();
        }

        private void Start()
        {
            RuntimeDataLoader.RuntimeStageData.ToxicGuage.AddListener(updateToxicGuage);
        }

        private void OnDestroy()
        {
            RuntimeDataLoader.RuntimeStageData.ToxicGuage.RemoveListener(updateToxicGuage);
        }

        public void updateToxicGuage(float guage)
        {
            mAnim.SetFloat("ToxicGuage", guage);
        }
    }
}
