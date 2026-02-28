using RoboClean.Data;
using UnityEngine;

namespace RoboClean.Character.Player
{
    public class CheckPlayerDeathTimeline : MonoBehaviour
    {
        [SerializeField] GameObject PLAYER_DEATH_TIMELINE;

        private RuntimePlayData mPlayData
        {
            get
            {
                return RuntimeDataLoader.RuntimePlayData;
            }
        }

        private void Start()
        {
            RuntimeDataLoader.PlayerRuntimeInfo.IsDead.AddListener(playPlayerDeathTimeline, true);
        }

        private void playPlayerDeathTimeline(bool isPlayerDead)
        {
            if (isPlayerDead)
            {
                mPlayData.IsScenePlaying.Value = true;
                PLAYER_DEATH_TIMELINE.SetActive(true);
            }
        }
    }
}