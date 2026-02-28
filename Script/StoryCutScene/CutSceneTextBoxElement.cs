using UnityEngine;
using Febucci.UI.Core;
using RoboClean.Sound;

namespace RoboClean.StoryCutScene
{
    public class CutSceneTextBoxElement : MonoBehaviour
    {
        public bool IsSkipable
        {
            get
            {
                if (!mIsIntialized)
                {
                    return false;
                }
                return ANIM.isPlaying || TYPE_WRITER.isShowingText;
            }
        }

        private bool mIsOnSkipping;
        private const string SHOW_UP_ANIM_KEY = "ANI_TBox_ShowUp";
        private Animation ANIM;
        private TypewriterCore TYPE_WRITER;

        private bool mIsIntialized = false;

        private void Awake()
        {
            if (!mIsIntialized)
            {
                gameObject.SetActive(false);
            }
        }

        private void Initialize()
        {
            ANIM = GetComponentInChildren<Animation>(true);
            TYPE_WRITER = GetComponentInChildren<TypewriterCore>(true);
            mIsIntialized = true;
        }

        public void SkipTextBox()
        {
            mIsOnSkipping = true;
            if (ANIM.isPlaying)
            {
                ANIM[ANIM.clip.name].time = ANIM[ANIM.clip.name].length;
            }

            if (TYPE_WRITER.isShowingText)
            {
                TYPE_WRITER.SkipTypewriter();
            }
        }

        public void ShowUp()
        {
            if (!mIsIntialized)
            {
                Initialize();
            }
            gameObject.SetActive(true);
            ANIM.Play(SHOW_UP_ANIM_KEY);
            AudioManager.PlayCutSceneTextSFX();
        }

        public void PlayTextAnimator_AnimEvent()
        {
            if (mIsOnSkipping)
            {
                mIsOnSkipping = false;
                return;
            }

            TYPE_WRITER.StartShowingText(true);
        }

        public void CloseAnimEnd_AnimEvent()
        {
            gameObject.SetActive(false);
        }
    }
}