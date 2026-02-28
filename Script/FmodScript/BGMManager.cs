using FMODUnity;
using FMOD.Studio;
using RoboClean.Managers;
using RoboClean.Common;
using RoboClean.Utils;

namespace RoboClean.Sound
{
    public class BGMManager : SingletonClass<BGMManager>
    {
        private EventInstance mCurrentBGMInstance;

        private EventInstance mMainSceneSoundInstance;
        private EventInstance mBattleSceneInstance;
        private EventInstance mAMBSoundInstance;
        private EventInstance mCutSceneBGMInstance;
        private eSceneName prevSceneName = eSceneName.None;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            mMainSceneSoundInstance = RuntimeManager.CreateInstance("event:/BGM/BGM_Title");
            mBattleSceneInstance = RuntimeManager.CreateInstance("event:/BGM/BGM_BattleScene");
            mCutSceneBGMInstance = RuntimeManager.CreateInstance("event:/BGM/BGM_CutScene");
            mAMBSoundInstance = RuntimeManager.CreateInstance("event:/AMB/AMB_BattleSceneDefault");


            SceneSwitchingManager.Instance.OnSceneSwitchingStart.AddListener(tryRealesBGM, true);
            SceneSwitchingManager.Instance.CurrentScene.AddListener(onSceneChange, true);
        }

        public void SetBGMLabel(string label)
        {
            mBattleSceneInstance.setParameterByNameWithLabel("Battle", label);
        }

        private void tryRealesBGM(eSceneName newScene)
        {
            if (newScene == prevSceneName)
            {
                return;
            }

            PLAYBACK_STATE bgmState;
            mCurrentBGMInstance.getPlaybackState(out bgmState);

            if (bgmState == PLAYBACK_STATE.PLAYING)
            {
                switch (prevSceneName)
                {
                    case eSceneName.scene_title:
                    case eSceneName.scene_story_cutscene:
                        mCurrentBGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        break;
                    case eSceneName.scene_mainplay_stage0:
                    case eSceneName.scene_mainplay_stage1:
                    case eSceneName.scene_mainplay_stage2:
                    case eSceneName.scene_mainplay_stage3:
                        if (!newScene.CheckMainPlayScene())
                        {
                            mAMBSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                            mCurrentBGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        }
                        break;
                    default:
                        break;
                }
            }

            switch (newScene)
            {
                case eSceneName.scene_mainplay_stage0:
                case eSceneName.scene_mainplay_stage1:
                case eSceneName.scene_mainplay_stage3:
                    mBattleSceneInstance.setParameterByNameWithLabel("Battle", "Main");
                    break;
                case eSceneName.scene_mainplay_stage2:
                    mBattleSceneInstance.setParameterByNameWithLabel("Battle", "Boss");
                    break;
                default:
                    break;
            }

        }

        private void onSceneChange(eSceneName newScene)
        {
            if (newScene == prevSceneName)
            {
                return;
            }

            PLAYBACK_STATE bgmState;
            mCurrentBGMInstance.getPlaybackState(out bgmState);

            if (bgmState != PLAYBACK_STATE.PLAYING)
            {
                switch (newScene)
                {
                    case eSceneName.scene_title:
                        mCurrentBGMInstance = mMainSceneSoundInstance;
                        break;
                    case eSceneName.scene_story_cutscene:
                        mCurrentBGMInstance = mCutSceneBGMInstance;
                        break;
                    case eSceneName.scene_mainplay_stage0:
                    case eSceneName.scene_mainplay_stage1:
                    case eSceneName.scene_mainplay_stage2:
                    case eSceneName.scene_mainplay_stage3:
                        mCurrentBGMInstance = mBattleSceneInstance;
                        mAMBSoundInstance.start();
                        break;
                    default:
                        break;
                }
                mCurrentBGMInstance.start();
            }

            prevSceneName = newScene;
        }
    }
}