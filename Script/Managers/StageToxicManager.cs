using RoboClean.Character;
using RoboClean.Common;
using RoboClean.Data;
using RoboClean.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace RoboClean.Managers
{
    public class StageToxicManager : SingletonClass<StageToxicManager>
    {
        private RuntimeStageData mRuntimeStageData
        {
            get
            {
                return RuntimeDataLoader.RuntimeStageData;
            }
        }

        [SerializeField] private StageToxicConfig TOXIC_CONFIG;
        [SerializeField] private GameObjectPool TOXIC_POOL;
        [SerializeField] private GameObject[] TOXIC_PREFABS;

        protected override void Awake()
        {
            base.Awake();
            mRuntimeStageData.ToxicGuage.Value = 0.0f;
            mRuntimeStageData.SludgeCount.Value = 0;
            mRuntimeStageData.MaxSludgeCount.Value = TOXIC_CONFIG.MaxSludgeAmount;
            mRuntimeStageData.SludgeCount.AddListener(onSludgeCountChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mRuntimeStageData.SludgeCount.RemoveListener(onSludgeCountChanged);
        }

        public void SpreadSludge(Vector3 spawnOriginalPoint, eSludgeType sldugeType, int spreadAmount, float spreadRange)
        {
            for (int i = 0; i < spreadAmount; i++)
            {
                Vector3 spawnPoint = getRandomSpreadPoint(spawnOriginalPoint, spreadRange);
                increaseSludge(spawnOriginalPoint, spawnPoint, sldugeType);
            }
        }

        public void DecreaseSludge()
        {
            mRuntimeStageData.SludgeCount.Value--;
        }

        private Vector3 getRandomSpreadPoint(Vector3 center, float range)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return Vector3.zero;
        }


        private void increaseSludge(Vector3 startPos, Vector3 destPos, eSludgeType type)
        {
            if (mRuntimeStageData.MaxSludgeCount == mRuntimeStageData.SludgeCount)
            {
                return;
            }

            GameObject sludge = TOXIC_POOL.GetDeactiveGameobject(TOXIC_PREFABS[(int)type]);
            sludge.GetComponent<SludgeElement>().PlaySludge(startPos, destPos);
            mRuntimeStageData.SludgeCount.Value++;
        }

        private void onSludgeCountChanged(int newSludge)
        {
            mRuntimeStageData.ToxicGuage.Value = newSludge / (float)mRuntimeStageData.MaxSludgeCount;
        }
    }
}