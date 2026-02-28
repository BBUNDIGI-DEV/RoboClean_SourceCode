using RoboClean.Data;
using RoboClean.Utils;
using UnityEngine;

namespace RoboClean.Character
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private eAnimatorType ANIM_TYPE;
        public Animator Anim
        {
            get
            {
                if (mAnim == null)
                {
                    mAnim = GetComponent<Animator>();
                }
                return mAnim;
            }
        }

        private Animator mAnim;

        public void UpdateMovementAnim(Vector3 velocity, bool isFixedUpdate = true)
        {
            float deltaTime = isFixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
            float runAnimPlayThreashold = PlayerConfig.RUN_ANIM_INVOKE_THRESHOLD * PlayerConfig.RUN_ANIM_INVOKE_THRESHOLD;
            Vector3 horiMoveVelocity = new Vector3(velocity.x, 0.0f, velocity.z);

            if (horiMoveVelocity.sqrMagnitude >= runAnimPlayThreashold)
            {
                Anim.SetFloat(eAnimatorParams.MovementBlendFactor.ToString(), 1.0f, 0.1f, deltaTime);
                Anim.SetBool(eAnimatorParams.IsRun.ToString(), true);
            }
            else
            {
                Anim.SetFloat(eAnimatorParams.MovementBlendFactor.ToString(), 0.0f, 0.1f, deltaTime);
                Anim.SetBool(eAnimatorParams.IsRun.ToString(), false);
            }
        }

        public void SetMovementBlendFactor(float value)
        {
            Anim.SetFloat(eAnimatorParams.MovementBlendFactor.ToString(), value);
        }


        public void TriggerDashAnim(float speed)
        {
            Anim.PlayOrRewind("Dash");
            Anim.SetFloat("DashSpeedMultiplier", speed);
        }

        public void TriggerAttackAnim(SkillConfig config)
        {
            if (config.IsComboAttack)
            {
                Anim.SetInteger("ComboCount", config.ComboIndex);
                AnimatorStateInfo curInfo = Anim.GetCurrentAnimatorStateInfo(0);
                if (Anim.IsInTransition(0) || !curInfo.IsName(config.NameID))
                {
                    Anim.PlayOrRewind($"{config.BaseConfig.ActorType}_Combo{config.ComboIndex + 1}");
                }
            }
            else
            {
                Anim.PlayOrRewind($"{config.BaseConfig.ActorType}_Combo1");
            }
            Anim.SetBool(eAnimatorParams.IsOnAttack.ToString(), true);
        }

        public void PlayStunAnim()
        {
            Anim.SetBool(eAnimatorParams.IsStuned.ToString(), true);
            Anim.PlayOrRewind(eGotoStateName.Stuned.ToString());
        }

        public void PlayStunAnim(float speed)
        {
            PlayStunAnim();
            Anim.SetFloat(eAnimatorParams.StunSpeedMultiplier.ToString(), speed);
        }

        public void PlayOnHitMaterailAnim()
        {
            Anim.SetTrigger(eAnimatorParams.OnHitMaterialAnim.ToString());
        }

        public void PauseStunAnim()
        {
            Anim.SetBool(eAnimatorParams.IsStuned.ToString(), false);
        }


        public void SetAttackSpeed(float speedMultiplier)
        {
            Anim.SetFloat(eAnimatorParams.AttackSpeedMultiplier.ToString(), speedMultiplier);
        }

        public void EndAttackAnim()
        {
            Anim.SetBool(eAnimatorParams.IsOnAttack.ToString(), false);
        }

        public void PlayDamagedAnim(float speed)
        {
            Anim.PlayOrRewind(eGotoStateName.Damaged.ToString());
            Anim.SetFloat(eAnimatorParams.DamagedSpeedMultiplier.ToString(), speed);
            Anim.SetTrigger(eAnimatorParams.OnHitMaterialAnim.ToString());
        }

        public void PlayAppearanceAnim(float speed)
        {
            Anim.PlayOrRewind(eGotoStateName.Appearance.ToString());
            Anim.SetFloat(eAnimatorParams.ApperanceSpeedMultiplier.ToString(), speed);
        }

        public void PlayDeadAnim(float speed)
        {
            Anim.PlayOrRewind(eGotoStateName.Dead.ToString());
            Anim.SetFloat(eAnimatorParams.DeadSpeedMultiplier.ToString(), speed);
        }
    }


    public enum eAnimatorParams
    {
        None = -1,
        IsOnAttack,
        IsRun,
        IsStuned,
        MovementBlendFactor,
        AttackTrigger,
        BasicAttackComboCount,
        DamagedSpeedMultiplier,
        DashSpeedMultiplier,
        AttackSpeedMultiplier,
        ApperanceSpeedMultiplier,
        StunSpeedMultiplier,
        DeadSpeedMultiplier,
        ToxicGuage,
        OnHitMaterialAnim,
    }

    public enum eGotoStateName
    {
        Damaged,
        Dash,
        Stuned,
        Appearance,
        Dead,
    }

    public enum eAnimatorType
    {
        PlayerBase,
        RoboBoss,
    }
}