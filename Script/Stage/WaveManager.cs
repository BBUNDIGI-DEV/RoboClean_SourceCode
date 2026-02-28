using RoboClean.Character.Player;
using RoboClean.Common;
using RoboClean.Data;
using UnityEngine;
using UnityEngine.Playables;

namespace RoboClean.Stage
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private SimpleTrigger WAVE_TRIGGER_OR_NULL;
        [SerializeField] private PlayableDirector INITIAL_TIMELINE_OR_NULL;
        [SerializeField] private PlayableDirector STAGE_ENDING_TIMELINE_OR_NULL;

        private RuntimeStageData mRuntimeStageData
        {
            get
            {
                return RuntimeDataLoader.RuntimeStageData;
            }
        }

        private WaveElement[] mWaves;
        private int mCurrentWaveIndex;

        private void Awake()
        {
            if (GetComponentsInChildren<WaveElement>() != null)
            {
                mWaves = GetComponentsInChildren<WaveElement>();
            }

            for (int i = 0; i < mWaves.Length; i++)
            {
                mWaves[i].SetOnStageCleared(onWaveCleared);
                mWaves[i].SetOnDecreaseMonster(onMonsterDecrease);
            }
        }

        private void Start()
        {
            mRuntimeStageData.TotalMonsterCount.Value = 0;
            mRuntimeStageData.LastMonsterCount.Value = 0;

            if (WAVE_TRIGGER_OR_NULL != null)
            {
                WAVE_TRIGGER_OR_NULL.SetCallback(startWave);
                WAVE_TRIGGER_OR_NULL.SetCallback(() => WAVE_TRIGGER_OR_NULL.SetEnable(false));
            }
            else if (INITIAL_TIMELINE_OR_NULL != null)
            {
                INITIAL_TIMELINE_OR_NULL.gameObject.SetActive(true);
                PlayerCharacterController.Instance.IsInputBlocked = true;

                INITIAL_TIMELINE_OR_NULL.stopped += onInitialCutSceneEnd;
                INITIAL_TIMELINE_OR_NULL.Play();
            }
            else if (WAVE_TRIGGER_OR_NULL == null)
            {
                if (mWaves.Length == 0)
                {
                    return;
                }
                startWave();
            }
        }

        private void OnDestroy()
        {
            mRuntimeStageData.IsStageStarted.Value = false;
            mRuntimeStageData.IsStageCleared.Value = false;
            mRuntimeStageData.TotalMonsterCount.Value = 0;
            mRuntimeStageData.LastMonsterCount.Value = 0;
        }


        private void startWave()
        {
            mRuntimeStageData.IsStageStarted.Value = true;
            if (mCurrentWaveIndex == 0)
            {
                mRuntimeStageData.TotalMonsterCount.Value = GetComponentsInChildren<MonsterSpawner>().Length;
                mRuntimeStageData.LastMonsterCount.Value = mRuntimeStageData.TotalMonsterCount;
            }
            mWaves[mCurrentWaveIndex].StartWave();
        }

        private void onWaveCleared()
        {
            mCurrentWaveIndex++;
            if (mCurrentWaveIndex == mWaves.Length)
            {
                if (STAGE_ENDING_TIMELINE_OR_NULL != null)
                {
                    STAGE_ENDING_TIMELINE_OR_NULL.stopped += onWaveEndingCutSceneEnd;
                    PlayerCharacterController.Instance.IsInputBlocked = true;
                    PlayerCharacterController.Instance.gameObject.SetActive(false);
                    STAGE_ENDING_TIMELINE_OR_NULL.gameObject.SetActive(true);
                    STAGE_ENDING_TIMELINE_OR_NULL.Play();
                }
                else
                {
                    mRuntimeStageData.IsStageCleared.Value = true;
                }
            }
            else
            {
                startWave();
            }
        }

        private void onMonsterDecrease()
        {
            mRuntimeStageData.LastMonsterCount.Value--;
        }

        private void onInitialCutSceneEnd(PlayableDirector director)
        {
            startWave();
            INITIAL_TIMELINE_OR_NULL.gameObject.SetActive(false);
            PlayerCharacterController.Instance.IsInputBlocked = false;
        }

        private void onWaveEndingCutSceneEnd(PlayableDirector director)
        {
            INITIAL_TIMELINE_OR_NULL.gameObject.SetActive(false);
            PlayerCharacterController.Instance.IsInputBlocked = false;
            PlayerCharacterController.Instance.gameObject.SetActive(true);
            mRuntimeStageData.IsStageCleared.Value = true;
        }
    }
}
