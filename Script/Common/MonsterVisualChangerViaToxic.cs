using RoboClean.Common;
using RoboClean.Data;
using RoboClean.Utils;
using UnityEngine;

namespace RoboClean.Character.AI
{
    public class MonsterVisualChangerViaToxic : MonoBehaviour
    {
        [SerializeField] private MonsterConfig CONFIG;
        private Animator mAnim;
        private GameObjectPool mEffectPool;
        private GameObject mActivatedAuraEffectOrNull;
        private void Awake()
        {
            mAnim = GetComponentInChildren<Animator>();
            mEffectPool = GameObjectPool.TryGetGameobjectPool(transform, "EffectPool", ePoolingObjectType.EffectPool);
        }

        public void OnEnable()
        {
            RuntimeDataLoader.RuntimeStageData.ToxicGuage.AddListener(SetToxicGauge, true);
        }

        public void OnDisable()
        {
            RuntimeDataLoader.RuntimeStageData.ToxicGuage.RemoveListener(SetToxicGauge);
        }

        public void SetToxicGauge(float toxicGuage)
        {
            mAnim.SetFloat(eAnimatorParams.ToxicGuage.ToString(), toxicGuage);
            if (toxicGuage >= CONFIG.AuraThreshold && mActivatedAuraEffectOrNull == null)
            {
                GameObject auraEffect = mEffectPool.GetDeactiveGameobject(CONFIG.AuraEffect);
                auraEffect.SetActive(true);
                mActivatedAuraEffectOrNull = auraEffect;
            }
            else if (toxicGuage < CONFIG.AuraThreshold && mActivatedAuraEffectOrNull != null)
            {
                mActivatedAuraEffectOrNull.GetComponent<ParticleLoopDeactivator>().LoopPause();
                mActivatedAuraEffectOrNull = null;
            }
        }
    }
}
