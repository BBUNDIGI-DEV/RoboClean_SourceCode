using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace RoboClean.Sound
{
    public class BossAnimSoundEffect : MonoBehaviour
    {
        private EventInstance mRushSound;

        private void Start()
        {
            mRushSound = RuntimeManager.CreateInstance("event:/SFX/Monster/Boss/SFX_BossRushing");
        }

        public void PlayBossAttackSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/SFX_BossAttack");
        }

        public void PlayBossSpinSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/SFX_BossSpin");
        }

        public void PlayBossReadyRushSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/SFX_BossPreparingRush");
        }

        public void PlayBossRushSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/SFX_BossRushing");
        }

        public void PlayRushSoundEffect()
        {
            mRushSound.start();
        }

        public void PauseRushSoundEffect()
        {
            mRushSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void PlayBossRushHitSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/SFX_BossRushStunned");
        }
    }
}