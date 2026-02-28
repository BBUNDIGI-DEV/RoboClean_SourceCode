using UnityEngine;

namespace RoboClean.Stage
{
    public class WaveElement : MonoBehaviour
    {
        public int MonsterCount
        {
            get; private set;
        }
        private System.Action mOnWaveCleared;
        private System.Action mOnDecreaseMonsterCount;
        private int mRemainMonsterCount;

        private MonsterSpawner[] mSpawners;

        private void Awake()
        {
            mSpawners = GetComponentsInChildren<MonsterSpawner>();
            MonsterCount = mSpawners.Length;
        }


        public void SetOnStageCleared(System.Action action)
        {
            mOnWaveCleared += action;
        }

        public void SetOnDecreaseMonster(System.Action action)
        {
            mOnDecreaseMonsterCount += action;
        }

        public void StartWave()
        {
            for (int i = 0; i < mSpawners.Length; i++)
            {
                mSpawners[i].SpawnMonster(decreaseMonsterCount);
            }
            mRemainMonsterCount = mSpawners.Length;
        }

        private void decreaseMonsterCount()
        {
            mRemainMonsterCount--;
            mOnDecreaseMonsterCount.Invoke();
            if (mRemainMonsterCount == 0)
            {
                mOnWaveCleared?.Invoke();
            }
        }
    }
}