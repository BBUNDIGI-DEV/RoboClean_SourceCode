using RoboClean.Data;
using RoboClean.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace RoboClean.Character
{
    public class SludgeSpawner : MonoBehaviour
    {
        [SerializeField] public SludgeSpawnerConfig CONFIG;

        private int mOnHitCheckIndex = 0;

        private bool mNeedSludgeSpreadOnIdle;
        private float mCurrentWaitTime;

        public void InitializeSpanwer(MonsterStatus ownerStatus)
        {
            if (CONFIG.SpawnCases.HasFlag(eSludgeSpawnCase.OnHit))
            {
                ownerStatus.NormalizedHP.AddListener(tryInvokeSludgeOnHit);
            }

            if (CONFIG.SpawnCases.HasFlag(eSludgeSpawnCase.OnDead))
            {
                ownerStatus.IsDead.AddListener((palceHolder) => invokeSpreadingSludge(CONFIG.OnDeadSpawnData));
            }

            if (CONFIG.SpawnCases.HasFlag(eSludgeSpawnCase.OnIdle))
            {
                mNeedSludgeSpreadOnIdle = true;
                mCurrentWaitTime = Random.Range(CONFIG.OnIdleRandomMinTime, CONFIG.OnIdleRandomMaxTime);
            }

        }

        public void Update()
        {
            if (!mNeedSludgeSpreadOnIdle)
            {
                return;
            }

            mCurrentWaitTime -= Time.deltaTime;
            if (mCurrentWaitTime <= 0.0f)
            {
                mCurrentWaitTime = Random.Range(CONFIG.OnIdleRandomMinTime, CONFIG.OnIdleRandomMaxTime);
                invokeSpreadingSludge(CONFIG.OnIdleSpawnData);
            }
        }

        private void invokeSpreadingSludge(SludgeSpawnData spawnData)
        {
            StageToxicManager.Instance.SpreadSludge(transform.position, CONFIG.SludgeType, spawnData.SpreadAmount, spawnData.SpreadRange); ;
        }

        private void tryInvokeSludgeOnHit(float normalizedHP)
        {
            Debug.Assert(normalizedHP >= 0.0f && normalizedHP <= 1.0f, $"Normalized HP Out of range {normalizedHP}");
            if (CONFIG.OnHitHPPercentage.Length == mOnHitCheckIndex)
            {
                return;
            }
            float checkHP = CONFIG.OnHitHPPercentage[mOnHitCheckIndex];

            if (normalizedHP < checkHP)
            {
                SludgeSpawnData spawnData = CONFIG.OnHitSpawnDatas[mOnHitCheckIndex];
                invokeSpreadingSludge(spawnData);
                mOnHitCheckIndex++;
            }
        }
    }
}
