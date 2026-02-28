using FMODUnity;
using UnityEngine;

namespace RoboClean.Sound
{
    public class DoorAnimSoundEffect : MonoBehaviour
    {
        public void PlayDoorOpenSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/Stage/SFX_CommonGate");
        }
    }
}
