using FMODUnity;
using UnityEngine;

namespace RoboClean.Character.Player
{
    public class PlayerAnimSoundEffect : MonoBehaviour
    {
        private CharacterAnimator mPlayerAnimator;

        private void Start()
        {
            mPlayerAnimator = GetComponent<CharacterAnimator>();
        }

        public void PlayPlayerFootstepSoundEffect()
        {
            if (!mPlayerAnimator.Anim.GetBool("IsRun"))
            {
                return;
            }
            RuntimeManager.PlayOneShot("event:/SFX/PC/move/SFX_PlayerFootStep");
        }

        public void PlayPlayerAttack1or2SoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/PC/Attack/SFX_NormalAttackSwing1&2");
        }

        public void PlayPlayerFinalAttackSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/PC/Attack/SFX_NormalAttackSwing3");
        }

        public void PlayPlayerFinalAttackCrashSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/PC/Attack/SFX_NormalAttac3Impact");
        }

        public void PlayPlayerSpecialAttackSoundEffect()
        {
            //q 공격 사운드
            RuntimeManager.PlayOneShot("event:/SFX/PC/Attack/SFX_QSkillSwing");
        }

        public void PlayPlayerSpecialAttackCrashSoundEffect()
        {
            //q 공격 크래쉬 사운드
            RuntimeManager.PlayOneShot("event:/SFX/PC/Attack/SFX_QSkillImpact");
        }
    }
}