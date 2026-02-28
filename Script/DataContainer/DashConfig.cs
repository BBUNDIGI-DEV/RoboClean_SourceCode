using RoboClean.Character;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/DashConfig")]
    public class DashConfig : ActorConfigDataContainerBase
    {
        //Dash
        public int DefaultMaxDashNumber = 2;
        public float DashDuration = 0.2f;

        public float InitialDashDistance = 5.0f;
        public float SecondDashDistance = 2.0f;

        public float DashCooltime = 0.35f;
        [ValidateInput("@DashCooltime >= ExtraDashableTime", "ExtraDashableTime은 DashCooltime보다 클 수 없습니다.")]
        public float ExtraDashableTime = 0.15f;
        public float DashAttackableTime = 0.15f;

        public GameObject EffectPrefab;
        public CameraActingEventData CameraActingData;
        [SerializeField] private AnimationClip DASH_CLIP;

        public float CaculateAnimSpeed()
        {
            float clipTime = DASH_CLIP.length;
            return clipTime / DashDuration;
        }


        protected override void initializeConfig()
        {
            BaseConfig.ActorType = eActorType.Dash;
            BaseConfig.IsUpdatedActor = false;
        }
    }
}