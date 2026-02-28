using RoboClean.Common;
using RoboClean.Data;
using RoboClean.Sound;
using RoboClean.Utils;
using UnityEngine;

namespace RoboClean.Character.Player
{
    public class PlayerVisualChangerViaHP : MonoBehaviour
    {
        [SerializeField] private PlayerConfig PLAYER_CONFIG;
        [SerializeField] private GameObject AURA_EFFECT;
        private GameObject mActivatedAuraEffectOrNull;
        private GameObjectPool mEffectPool;

        private void Awake()
        {
            mEffectPool = GameObjectPool.TryGetGameobjectPool(transform, "EffectPool", ePoolingObjectType.EffectPool);
        }
        public void OnEnable()
        {
            RuntimeDataLoader.PlayerRuntimeInfo.NormalizedHP.AddListener(CheckHP);
        }

        public void OnDisable()
        {
            RuntimeDataLoader.PlayerRuntimeInfo.NormalizedHP.RemoveListener(CheckHP);
            AudioManager.RelasePlayerRowHPSound();
        }

        public void CheckHP(float normalizedHP)
        {
            if (normalizedHP <= PLAYER_CONFIG.LowhpEffectThreshold && mActivatedAuraEffectOrNull == null)
            {
                GameObject auraEffect = mEffectPool.GetDeactiveGameobject(AURA_EFFECT);
                auraEffect.SetActive(true);
                mActivatedAuraEffectOrNull = auraEffect;
            }
            else if (normalizedHP > PLAYER_CONFIG.LowhpEffectThreshold && mActivatedAuraEffectOrNull != null)
            {
                mActivatedAuraEffectOrNull.GetComponent<ParticleLoopDeactivator>().LoopPause();
                mActivatedAuraEffectOrNull = null;
            }
            Debug.Log(normalizedHP);
            //AudioManager.PlayRowHPSound(normalizedHP);
        }
    }
}