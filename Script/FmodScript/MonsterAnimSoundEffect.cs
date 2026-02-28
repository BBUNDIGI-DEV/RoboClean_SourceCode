using FMODUnity;
using UnityEngine;

namespace RoboClean.Sound
{
    public class MonsterAnimSoundEffect : MonoBehaviour
    {
        public void PlayMeleeMonsterDeathSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/All/SFX_HeyenaDeath");
        }

        public void PlayRangedMonsterDeathSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Monster/All/SFX_RacconDeath");
        }
    }
}
