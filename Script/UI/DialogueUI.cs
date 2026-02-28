using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using RoboClean.Input;
using RoboClean.Data;
using RoboClean.Utils;
using RoboClean.Sound;

namespace RoboClean.UI
{
    public class DialogueUI : MonoBehaviour
    {
        private const string SHOW_UP_KEY = "ANI_dialogue_show_up";
        private const string CLOSE_KEY = "ANI_dialogue_close";
        private const string SHOW_TUTORIAL_IMAGE = "ANI_show_tutorial";
        [SerializeField] private Animation DIALOGUE_ANIM;
        [SerializeField] private DialogueTextBox TEXT_BOX;
        [SerializeField] private DialogueCharacter[] CHARACTER_IMAGES;
        [SerializeField] private Button SKIP_BUTTON;

        private bool mIsPlayerClick;
        private System.Action mOnDialogueEnd;
        private Coroutine mDialgoueRoutine;
        private GameObject mMainGameobject
        {
            get
            {
                return transform.GetChild(0).gameObject;
            }
        }

        private void Awake()
        {
            mMainGameobject.SetActive(false);
        }

        public void Start()
        {
            InputManager.Instance.AddInputCallback(eInputSections.Dialouge, eDialougeInputName.DialogueInteraction.ToString(), clickDialogue);
            InputManager.Instance.AddInputCallback(eInputSections.Dialouge, eDialougeInputName.Skip.ToString(), skipDialogue);

        }

        public void PlayDialouge(eDialogueTag tag, System.Action onDialogueEnd = null)
        {
            InputManager.Instance.SwitchInputSection(eInputSections.Dialouge);
            mOnDialogueEnd = onDialogueEnd;
            mDialgoueRoutine = StartCoroutine(PlayRoutine(RuntimeDataLoader.DialogueConfigDic[tag]));
            SKIP_BUTTON.gameObject.SetActive(true);
            SKIP_BUTTON.interactable = true;

        }

        private IEnumerator PlayRoutine(DialogueConfig config)
        {
            mMainGameobject.SetActive(true);
            for (int i = 0; i < config.ActingDatas.Length; i++)
            {
                DialogueActingData data = config.ActingDatas[i];

                if (data.ActingType.HasFlag(eDialogueActingType.ShowCharacter))
                {
                    CHARACTER_IMAGES[(int)data.ShowUpCharacter].ShowCharacter();
                }

                if (data.ActingType.HasFlag(eDialogueActingType.DisappearCharacter))
                {
                    CHARACTER_IMAGES[(int)data.DisappearCharacter].CloseCharacter();
                }

                if (data.ActingType.HasFlag(eDialogueActingType.SetName))
                {
                    TEXT_BOX.SetName(data.NameText);
                }

                if (data.ActingType.HasFlag(eDialogueActingType.ShowText))
                {
                    TEXT_BOX.SetText(data.DialougeText);
                }

                if (data.ActingType.HasFlag(eDialogueActingType.EnableTutorialImage))
                {
                    DIALOGUE_ANIM.Play(SHOW_TUTORIAL_IMAGE);
                }

                if (data.ActingType.HasFlag(eDialogueActingType.ShowTextBox))
                {
                    TEXT_BOX.ShowTextBox();
                }

                if (data.ActingType.HasFlag(eDialogueActingType.HideTextBox))
                {
                    TEXT_BOX.CloseTextBox();
                }

                if (i == config.ActingDatas.Length)
                {
                    break;
                }

                yield return new WaitUntil(() => mIsPlayerClick);
                mIsPlayerClick = false;
                if (TEXT_BOX.TrySkipPlaying())
                {
                    yield return CommonWaitForSeconds.GetWaitForSeconds(1.0f);
                    yield return new WaitUntil(() => mIsPlayerClick);
                    mIsPlayerClick = false;
                }
                AudioManager.PlayUIDialogSoundEffect();//대화창 클릭 사운드
            }

            closeDialouge();
        }

        public void SkipDialogue()
        {
            StartCoroutine(skipDialogueRoutine());
        }


        public void DeactivatedDialogueAnimEvent()
        {
            mMainGameobject.SetActive(false);
        }


        private void closeDialouge()
        {
            TEXT_BOX.CloseTextBox();
            DIALOGUE_ANIM.Play(CLOSE_KEY);
            SKIP_BUTTON.gameObject.SetActive(false);
            InputManager.Instance.SwitchInputSection(eInputSections.BattleGamePlay);
            mOnDialogueEnd?.Invoke();
        }


        private void skipDialogue(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            SkipDialogue();
        }

        private void clickDialogue(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            mIsPlayerClick = true;
        }

        private IEnumerator skipDialogueRoutine()
        {
            Debug.Log(!DIALOGUE_ANIM.IsPlaying(SHOW_UP_KEY));
            yield return new WaitUntil(() => !DIALOGUE_ANIM.IsPlaying(SHOW_UP_KEY));
            skipDialogue();
        }

        private void skipDialogue()
        {
            StopCoroutine(mDialgoueRoutine);
            for (int i = 0; i < CHARACTER_IMAGES.Length; i++)
            {
                CHARACTER_IMAGES[i].CloseCharacter();
            }

            closeDialouge();
        }
    }
}