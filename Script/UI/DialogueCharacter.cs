using UnityEngine;

namespace RoboClean.UI
{
    public class DialogueCharacter : MonoBehaviour
    {
        private const string SHOW_ANIM_KEY = "ANI_dialogue_character_show_up";
        private const string CLOSE_ANIM_KEY = "ANI_dialogue_character_close";
        [SerializeField] private Animation CHARACTER_ANIM;
        private bool mIsActivate = false;

        public void ShowCharacter()
        {
            transform.SetAsLastSibling();
            CHARACTER_ANIM.Play(SHOW_ANIM_KEY);
            mIsActivate = true;
        }

        public void CloseCharacter()
        {
            if (!mIsActivate)
            {
                return;
            }
            CHARACTER_ANIM.Play(CLOSE_ANIM_KEY);
            mIsActivate = false;
        }
    }
}