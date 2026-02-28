using UnityEngine;
using Sirenix.OdinInspector;
using RoboClean.Data;
using RoboClean.Common;
using RoboClean.Utils;
using RoboClean.Sound;

namespace RoboClean.Character
{
    [RequireComponent(typeof(AttackBoxElement))]
    public class ProjectileHandler : MonoBehaviour
    {
        private const float TRIGGER_PROTETING_TIME = 0.25f;
        private const float TIME_LIMIT = 10.0f;
        private const float DEACTIVE_DELAY_TIME = 1.0f;
        [SerializeField, ChildGameObjectsOnly] private ParticleSystem PROJECTILE_PARTICLE;
        [SerializeField, ChildGameObjectsOnly] private ParticleSystem ON_HIT;
        private Rigidbody RB;
        private AttackBoxElement ATTACK_BOX;

        private eTargetTag mTargetTag;
        private float mThrowingSpeed;
        private float mCurrentTime;
        private float mCurrentProtectingTime;
        private float mThrowDelay;
        private bool mIsHit;
        private bool mIsSoundLoaded;

        private void Awake()
        {
            ATTACK_BOX = GetComponent<AttackBoxElement>();
            RB = GetComponent<Rigidbody>();
            //RB.isKinematic = true;
        }

        private void OnEnable()
        {
            mIsSoundLoaded = false;
        }

        private void FixedUpdate()
        {
            mCurrentTime += Time.fixedDeltaTime;
            mCurrentProtectingTime -= Time.fixedDeltaTime;

            if (mCurrentTime < mThrowDelay)
            {
                return;
            }

            if (!mIsSoundLoaded)//원거리 공격 사운드 출력
            {
                AudioManager.PlayMonsterRangedAttackSoundEffect();
                mIsSoundLoaded = true;
            }


            if (!mIsHit)
            {
                RB.velocity = transform.forward * mThrowingSpeed;
            }
            else
            {
                RB.velocity = Vector3.zero;
            }

            if (mCurrentTime > TIME_LIMIT)
            {
                gameObject.SetActive(false);
                ON_HIT.gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (mIsHit)
            {
                return;
            }

            if (mCurrentProtectingTime >= 0.0f)
            {
                return;
            }

            bool isTarget = mTargetTag.CheckTarget(other.tag);
            bool isObstacle = other.tag == "Obstacle";
            if (!isTarget && !isObstacle)
            {
                return;
            }

            if (isObstacle)
            {
                ObstacleTypeDefiner definer = other.GetComponent<ObstacleTypeDefiner>();
                if (definer != null && definer.Type.HasFlag(eObstacleType.ProjectilePassable))
                {
                    return;
                }
            }

            if (isTarget)
            {
                ATTACK_BOX.ProcessOnHit(other.gameObject);
            }

            ClearProjectile();
        }

        public void InitializeProjectile(float throwingDelay, float throwingSpeed, Vector3 startPos, Vector3 throwingDir, eTargetTag targetTag)
        {
            mThrowingSpeed = throwingSpeed;
            transform.forward = throwingDir;
            mCurrentTime = 0.0f;
            gameObject.SetActive(true);
            PROJECTILE_PARTICLE.gameObject.SetActive(true);
            transform.position = startPos;
            mIsHit = false;
            mTargetTag = targetTag;
            mThrowDelay = throwingDelay;
            mCurrentProtectingTime = TRIGGER_PROTETING_TIME + mThrowDelay;
        }

        public void ClearProjectile()
        {
            mIsHit = true;
            mCurrentTime = TIME_LIMIT - DEACTIVE_DELAY_TIME;
            PROJECTILE_PARTICLE.gameObject.SetActive(false);
            ON_HIT.gameObject.SetActive(true);
        }
    }
}