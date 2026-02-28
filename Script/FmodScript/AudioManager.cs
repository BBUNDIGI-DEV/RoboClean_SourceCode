using FMODUnity;
using FMOD.Studio;

namespace RoboClean.Sound
{
    public static class AudioManager
    {
        public static EventInstance RowHPSound = RuntimeManager.CreateInstance("event:/SFX/PC/Extra/SFX_RowHp");
        public static EventInstance BossIdleSound = RuntimeManager.CreateInstance("event:/SFX/Monster/Boss/SFX_BossIdle");

        public static void PlayUIButtonSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/UI/SFX_ButtonClicked");
        }

        public static void PlayUIDialogSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/UI/SFX_DialogueClick");
        }

        public static void PlayRowHPSound(float NormalizedHP)
        {
            RowHPSound.getPlaybackState(out PLAYBACK_STATE bgmState);
            if (bgmState == PLAYBACK_STATE.STOPPED)
            {
                RowHPSound.start();
            }

            RowHPSound.setParameterByName("HP", NormalizedHP);
        }


        public static void RelasePlayerRowHPSound()
        {
            RowHPSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public static void PlayPlayerDashSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/PC/move/SFX_PlayerDash");
        }

        public static void PlayMonsterHitSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/All/SFX_OnHit");
        }

        public static void PlayerHitVoiceSound()
        {
            RuntimeManager.PlayOneShot("event:/SFX/PC/Extra/SFX_PlayerHit");
        }

        public static void PlayBossHitSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/SFX_BossBodyHit");
        }

        public static void PlayBossDeathEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/Boss/DeathTimeline/SFX_BossDeathImpact0");
        }

        public static void PlayMonsterRangedAttackSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/All/SFX_RacoonRangeAttack");
        }

        public static void PlaySludgeSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/All/SFX_Sludge");
        }

        public static void PlaySkipButtonSound()
        {
            RuntimeManager.PlayOneShot("event:/SFX/UI/SFX_ButtonClicked");
        }

        public static void PlayCutSceneTextSFX()
        {
            RuntimeManager.PlayOneShot("event:/SFX/CutScene/Opening/SFX_CutSceneSpeech");
        }

        public static void PlayBossIdleSound()
        {
            BossIdleSound.start();
        }

        public static void ReleaseBossIdleSound()
        {
            BossIdleSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public static void PlaySound(SimpleSoundInvokingData soundData)
        {
            BasicTimer timer = GlobalTimer
                .Instance
                .TryGetBasicTimerOrAdd("SoundInvoker", eTimerUpdateMode.Update, null);

            if (timer.IsActivate)
            {
                timer.PauseTimer();
            }

            timer.ChangeDuration(soundData.Delay);
            timer.ChangeCallback(() => RuntimeManager.PlayOneShot(soundData.SoundPath));
            timer.StartTimer();
        }
    }

    [System.Serializable]
    public struct SimpleSoundInvokingData
    {
        public string SoundPath;
        public float Delay;
    }
}