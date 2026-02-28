using RoboClean.Common;
using RoboClean.Data;
using RoboClean.Utils;
using UnityEngine;

namespace RoboClean.Character.AI
{
    public class RoboRushAttackBox : AttackBoxElement
    {
        private const float STUN_PROJECTION_TIME = 1.0f;
        [SerializeField] private StunData STUN_DATA;
        private RoboBossMonsterController MONSTER_CONTROLLER;

        private float mCurrentProtectionTime;

        protected override void Awake()
        {
            base.Awake();
            MONSTER_CONTROLLER = GetComponentInParent<RoboBossMonsterController>();
            Debug.Assert(MONSTER_CONTROLLER != null, $"Cannot found boss monster controller [{MONSTER_CONTROLLER}]");
        }

        private void FixedUpdate()
        {
            if (mCurrentProtectionTime < 0.0f)
            {
                return;
            }

            mCurrentProtectionTime -= Time.fixedDeltaTime;
        }

        private void OnEnable()
        {
            GetComponent<Collider>().enabled = true;
            mCurrentProtectionTime = STUN_PROJECTION_TIME;
        }


        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (isInHitStack(other.gameObject))
            {
                return;
            }
            base.OnTriggerEnter(other);

            if (CONFIG.TargetTag.CheckTarget(other.tag))
            {
                MONSTER_CONTROLLER.DoRushComboAttack();
            }
            else if (other.tag == "Obstacle")
            {
                ObstacleTypeDefiner definer;
                bool isDefinerExist = other.TryGetComponent(out definer);
                if (isDefinerExist && definer.Type.HasFlag(eObstacleType.RushStoppable))
                {
                    MONSTER_CONTROLLER.GotStunedByRush(STUN_DATA);
                }
            }
        }
    }
}