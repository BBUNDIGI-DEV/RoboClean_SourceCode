using UnityEngine;
using DG.Tweening;
using RoboClean.Sound;

namespace RoboClean.StoryCutScene
{
    public class StoryCutSceneElement : MonoBehaviour
    {
        public bool IsTextBoxSkipable
        {
            get
            {
                if (mTextBoxIndex == 0)
                {
                    return false;
                }

                return mTextBoxes[mTextBoxIndex - 1].IsSkipable;
            }
        }

        [SerializeField] private const string ENABLE_KEY = "ANI_enable_cutscene";
        [SerializeField] private const string DISABLE_KEY = "ANI_disable_cutscene";
        [SerializeField] private Transform MAIN_IMAGE_TRANS;
        [SerializeField] private SimpleSoundInvokingData ON_CUTSCENE_START_SOUND_DATA;

        [SerializeField] private eCutSceneActType[] CLICK_ACT_TYPE;
        [SerializeField] private SubcutsceneTransition[] SUBCUTSCENE_TRANSITION_DATA;
        [SerializeField] private SimpleSoundInvokingData[] SOUNDS_DATA;
        private CutSceneTextBoxElement[] mTextBoxes;
        private int mActIndex;
        private int mSubCutSceneIndex;
        private int mTextBoxIndex;


        private void Awake()
        {
            mTextBoxes = GetComponentsInChildren<CutSceneTextBoxElement>(true);
        }

        public void Intialize()
        {
            mActIndex = 0;
            mSubCutSceneIndex = 0;
            mTextBoxIndex = 0;
        }

        public void EnableCutScene()
        {
            GetComponent<Animation>().Play(ENABLE_KEY);
            AudioManager.PlaySound(ON_CUTSCENE_START_SOUND_DATA);
        }

        public void DisableCutScene()
        {
            GetComponent<Animation>().Play(DISABLE_KEY);
        }

        public eCutSceneActType InvokeClicked()
        {
            if (mActIndex == CLICK_ACT_TYPE.Length)
            {
                return eCutSceneActType.ShowNextCut;
            }

            eCutSceneActType actType = CLICK_ACT_TYPE[mActIndex];
            if (SOUNDS_DATA[mActIndex].SoundPath != "")
            {
                AudioManager.PlaySound(SOUNDS_DATA[mActIndex]);
            }

            mActIndex++;
            switch (actType)
            {
                case eCutSceneActType.ShowNextTextBox:
                    InvokeTextBox();
                    break;
                case eCutSceneActType.ShowSubCutScene:
                    InvokeSubCutScene();
                    break;
                default:
                    break;
            }
            Debug.Assert(actType != eCutSceneActType.ShowNextCut);
            return actType;
        }

        public void InvokeSubCutScene()
        {
            Debug.Assert(mSubCutSceneIndex < SUBCUTSCENE_TRANSITION_DATA.Length);
            SubcutsceneTransition data = SUBCUTSCENE_TRANSITION_DATA[mSubCutSceneIndex];
            mSubCutSceneIndex++;

            if (data.SubCutSceneTrans.GetComponent<Animation>() != null)
            {
                data.SubCutSceneTrans.GetComponent<Animation>().Play();
            }
            else
            {
                MAIN_IMAGE_TRANS.DOLocalMove(data.MainCutSceneDestPos, 0.495f);
                data.SubCutSceneTrans.DOLocalMove(data.SubCutSceneDestPos, 0.495f);
            }
        }

        public void InvokeTextBox()
        {
            Debug.Assert(mTextBoxIndex < mTextBoxes.Length);
            CutSceneTextBoxElement textBox = mTextBoxes[mTextBoxIndex];
            textBox.ShowUp();
            mTextBoxIndex++;
        }

        public void SkipTextBox()
        {
            mTextBoxes[mTextBoxIndex - 1].SkipTextBox();
        }
    }

    [System.Serializable]
    public struct SubcutsceneTransition
    {
        public Vector3 MainCutSceneDestPos;
        public Transform SubCutSceneTrans;
        public Vector3 SubCutSceneDestPos;
    }

    [System.Serializable]
    public struct CutsceneSpeachBox
    {
        public eSpeachBoxShowUpDir ShowUpDir;

        public enum eSpeachBoxShowUpDir
        {
            LeftToRight,
            RightToLeft,
        }
    }

    public enum eCutSceneActType
    {
        ShowNextTextBox,
        ShowSubCutScene,
        ShowNextCut,
    }
}