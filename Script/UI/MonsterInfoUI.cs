using UnityEngine;
using TMPro;
using RoboClean.Data;

namespace RoboClean.UI
{
    public class MonsterInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text MONSTER_COUNT_TEXT;

        private int mTotalMonsterCount;
        private int mCurrentMonsterCount;

        public void Start()
        {
            RuntimeDataLoader.RuntimeStageData.LastMonsterCount.AddListener(updateLastMonsterCount, true);
            RuntimeDataLoader.RuntimeStageData.TotalMonsterCount.AddListener(updateTotalMonsterCount, true);
        }

        public void OnDestroy()
        {
            RuntimeDataLoader.RuntimeStageData.LastMonsterCount.RemoveListener(updateLastMonsterCount);
            RuntimeDataLoader.RuntimeStageData.TotalMonsterCount.RemoveListener(updateTotalMonsterCount);
        }

        private void updateLastMonsterCount(int newCurrentMonsterCount)
        {
            mCurrentMonsterCount = newCurrentMonsterCount;
            updateMonsterCountUI();
        }

        private void updateTotalMonsterCount(int newTotalMonsterCount)
        {
            mTotalMonsterCount = newTotalMonsterCount;
            updateMonsterCountUI();
        }

        private void updateMonsterCountUI()
        {
            MONSTER_COUNT_TEXT.text = mCurrentMonsterCount + "/" + mTotalMonsterCount;
        }
    }
}