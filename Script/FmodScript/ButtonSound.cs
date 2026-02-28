using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoboClean.Sound
{
    public class ButtonSound : MonoBehaviour
    {
        private Button mButton;

        private void Awake()
        {
            mButton = GetComponent<Button>();
        }

        private void Start()
        {
            if (mButton != null)
            {
                mButton.onClick.AddListener(playSound);
            }
        }

        private void playSound()
        {
            AudioManager.PlayUIButtonSoundEffect();
        }

        public void PlayUIButtonOnEnableSoundEffect()
        {
            RuntimeManager.PlayOneShot("event:/SFX/UI/SFX_ButtonHighlighted");
        }
    }
}
