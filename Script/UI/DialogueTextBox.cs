using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Febucci.UI.Core;

namespace RoboClean.UI
{
    public class DialogueTextBox : MonoBehaviour
    {
        private const string SHOW_ANIM_KEY = "ANI_dialogue_textbox_show_up";
        private const string CLOSE_ANIM_KEY = "ANI_dialogue_textbox_close";

        [SerializeField] private Animation TEXT_ANIM;
        [SerializeField] private TypewriterCore DIALOGUE_TEXT;
        [SerializeField] private TMP_Text NAME_TEXT;
        [SerializeField] private Image MOVE_NEXT_INDICATOR;
        private bool mIsAcivtaed = false;

        private void Awake()
        {
            DIALOGUE_TEXT.GetComponent<TMP_Text>().text = "";
            NAME_TEXT.text = "";
            DIALOGUE_TEXT.onTextShowed.AddListener(() => MOVE_NEXT_INDICATOR.enabled = true);
        }

        public void ShowTextBox()
        {
            TEXT_ANIM.Play(SHOW_ANIM_KEY);
            mIsAcivtaed = true;
        }

        public void CloseTextBox()
        {
            if (!mIsAcivtaed)
            {
                return;
            }
            mIsAcivtaed = false;
            TEXT_ANIM.Play(CLOSE_ANIM_KEY);
        }

        public void SetText(string text)
        {
            if (MOVE_NEXT_INDICATOR.enabled)
            {
                MOVE_NEXT_INDICATOR.enabled = false;
            }
            DIALOGUE_TEXT.ShowText(text);
        }

        public bool TrySkipPlaying()
        {
            if (DIALOGUE_TEXT.isShowingText)
            {
                DIALOGUE_TEXT.SkipTypewriter();
                return true;
            }
            return false;
        }

        public void SetName(string name)
        {
            NAME_TEXT.text = name;
        }
    }
}