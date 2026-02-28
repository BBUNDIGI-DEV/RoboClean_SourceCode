using RoboClean.Character.Player;
using RoboClean.Data;
using RoboClean.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.Character
{
    [RequireComponent(typeof(Collider))]
    public class AttackBoxElement : MonoBehaviour
    {
        [SerializeField] private bool CAN_CLEAR_PROJECTILE;
        [SerializeField] protected bool DEACTIVE_ON_HIT;
        [SerializeField] protected bool IS_TRIGGER_BY_MANUAL;
        [SerializeField] protected SkillConfig CONFIG;
        protected List<int> HIT_STACK;


        protected virtual void Awake()
        {
            HIT_STACK = new List<int>(16);
        }

        private void OnEnable()
        {
            if (!IS_TRIGGER_BY_MANUAL)
            {
                GetComponent<Collider>().enabled = true;
            }
        }

        protected virtual void OnDisable()
        {
            HIT_STACK.Clear();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (IS_TRIGGER_BY_MANUAL)
            {
                return;
            }

            if (CAN_CLEAR_PROJECTILE)
            {
                if (other.tag == "Projectile")
                {
                    ProjectileHandler projHandler = other.GetComponent<ProjectileHandler>();
                    Debug.Assert(projHandler != null,
                        $"Projectile tagged object must have ProjectileHandler Element [{other.name}]");
                    projHandler.ClearProjectile();
                    return;
                }
            }

            if (isInHitStack(other.gameObject))
            {
                return;
            }

            if (!CONFIG.TargetTag.CheckTarget(other.tag))
            {
                return;
            }

            ProcessOnHit(other.gameObject);
        }

        public void ProcessOnHit(GameObject hitObject)
        {
            deactiveOnHit();
            HIT_STACK.Add(hitObject.GetInstanceID());
            if (CONFIG.NockbackData.NockBackType == eNockBackType.CircularToHitPoint)
            {
                CONFIG.NockbackData.NockbackDir = transform.position;
            }

            var monster = hitObject.GetComponentInParent<MonsterBase>();

            if (monster != null)
            {
                monster.GotAttack(CONFIG);
                CameraUtils.Instance.Actor.ProcessCameraActing(CONFIG.CameraActingOnHit);
                if (CONFIG.UseBulletTime)
                {
                    TimeUtils.PlayBulletTime(CONFIG.BulletTime);
                }
                return;
            }


            var character = hitObject.GetComponentInParent<PlayerCharacterController>();

            if (character != null)
            {
                character.GotAttack(CONFIG);
                return;
            }

            Debug.Assert(false, $"Can't handle hitbox gameobject [{name}]");
        }

        protected bool isInHitStack(GameObject otherGameobject)
        {
            return HIT_STACK.Contains(otherGameobject.GetInstanceID());
        }


        private void deactiveOnHit()
        {
            if (!DEACTIVE_ON_HIT)
            {
                return;
            }
            GetComponent<Collider>().enabled = false;
        }
    }
}