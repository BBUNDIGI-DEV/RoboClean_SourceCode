using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.UI
{
    public class OnScreenFadingHelper : MonoBehaviour
    {
        public bool IsFadeInPlaying;
        public bool IsFadeOutPlaying;
        private const string ON_SCREEN_FADE_IN_KEY = "ANI_OnScreen_Fade_In";
        private const string ON_SCREEN_FADE_OUT_KEY = "ANI_OnScreen_Fade_Out";

        [SerializeField] private Animation ON_SCREEN_FADING_ANIMATION;

        private void Awake()
        {
            PlayOnScreenFadeOut();
        }

        public void PlayOnScreenFadeIn()
        {
            ON_SCREEN_FADING_ANIMATION.Play(ON_SCREEN_FADE_IN_KEY);
            IsFadeInPlaying = true;
        }

        public void PlayOnScreenFadeOut()
        {
            ON_SCREEN_FADING_ANIMATION.Play(ON_SCREEN_FADE_OUT_KEY);
            IsFadeOutPlaying = true;
        }

        public void OnFadeInEnd_UnityEvent()
        {
            IsFadeInPlaying = false;
        }

        public void OnFadeOutEnd_UnityEvent()
        {
            IsFadeOutPlaying = false;
        }
    }

}
