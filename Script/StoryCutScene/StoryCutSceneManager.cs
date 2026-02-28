using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using RoboClean.Managers;
using RoboClean.Input;

namespace RoboClean.StoryCutScene
{
    public class StoryCutSceneManager : MonoBehaviour
    {
        private const float TEXT_COOL_DOWN = 0.5f;
        private const float SUB_CUT_COOL_DOWN = 0.5f;

        [SerializeField] private float CUTSCENE_OFFSET;
        [SerializeField] private Transform CUT_SCENE_TRANS;
        [SerializeField] private CutSceneInitialCutHandler CUT_INITIAL_TEXT;
        [SerializeField] private eSceneName LOAD_SCENE;
        private StoryCutSceneElement[] mCutScenes;
        private int mCutSceneIndex;
        private float mTouchCoolDown = 0.0f;

        private void Awake()
        {
            mCutScenes = GetComponentsInChildren<StoryCutSceneElement>();
        }

        private void Start()
        {
            for (int i = 0; i < mCutScenes.Length; i++)
            {
                if (i == 0)
                {
                    mCutScenes[i].EnableCutScene();
                }
                else
                {
                    mCutScenes[i].DisableCutScene();
                }
            }

            if (!CUT_INITIAL_TEXT.gameObject.activeInHierarchy)
            {
                CUT_INITIAL_TEXT.gameObject.SetActive(true);
            }

            InputManager.Instance.AddInputCallback(eInputSections.CutScene, eCutSceneInputName.CutSceneInteraction.ToString(), OnClick);
            InputManager.Instance.AddInputCallback(eInputSections.CutScene, eCutSceneInputName.Skip.ToString(), SkipcutScene);

            InputManager.Instance.SwitchInputSection(eInputSections.CutScene);
            InputManager.Instance.IsInputEnabled = true;
        }

        private void Update()
        {
            if (mTouchCoolDown < 0.0f)
            {
                return;
            }

            mTouchCoolDown -= Time.deltaTime;
        }

        private void OnDestroy()
        {
            if (!InputManager.IsExist)
            {
                return;
            }
            InputManager.Instance.RemoveInputCallback(eInputSections.CutScene, eCutSceneInputName.CutSceneInteraction.ToString(), OnClick);
            InputManager.Instance.RemoveInputCallback(eInputSections.CutScene, eCutSceneInputName.Skip.ToString(), SkipcutScene);

        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (CUT_INITIAL_TEXT.gameObject.activeInHierarchy)
            {
                CUT_INITIAL_TEXT.ShowFirstScene();
                return;
            }

            StoryCutSceneElement currentCut = mCutScenes[mCutSceneIndex];

            if (mTouchCoolDown >= 0.0f)
            {
                return;
            }

            if (currentCut.IsTextBoxSkipable)
            {
                currentCut.SkipTextBox();
                mTouchCoolDown = 0.0f;
                return;
            }


            eCutSceneActType actType = currentCut.InvokeClicked();

            switch (actType)
            {
                case eCutSceneActType.ShowNextTextBox:
                    mTouchCoolDown = TEXT_COOL_DOWN;
                    break;
                case eCutSceneActType.ShowSubCutScene:
                    mTouchCoolDown = SUB_CUT_COOL_DOWN;
                    break;
                case eCutSceneActType.ShowNextCut:
                    mTouchCoolDown = 1.0f;
                    if (mCutSceneIndex == mCutScenes.Length - 1)
                    {
                        SceneSwitchingManager.Instance.LoadOtherScene(LOAD_SCENE, true);
                        return;
                    }
                    moveToNextCutScene();
                    break;
                default:
                    Debug.LogError(actType);
                    break;
            }
        }

        public void SkipcutScene(InputAction.CallbackContext context)
        {
            SkipcutScene();
        }

        public void SkipcutScene()
        {
            SceneSwitchingManager.Instance.LoadOtherScene(LOAD_SCENE, true);
        }

        private void moveToNextCutScene()
        {
            CUT_SCENE_TRANS.DOMoveX(CUT_SCENE_TRANS.position.x - CUTSCENE_OFFSET, 0.3f);

            mCutScenes[mCutSceneIndex].DisableCutScene();
            mCutScenes[mCutSceneIndex + 1].EnableCutScene();
            mCutSceneIndex++;
        }
    }
}