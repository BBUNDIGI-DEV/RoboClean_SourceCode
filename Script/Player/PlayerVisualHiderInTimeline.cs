using UnityEngine;
using UnityEngine.Playables;
using RoboClean.Data;

namespace RoboClean.Character.Player
{
    public class PlayerVisualHiderInTimeline : MonoBehaviour
    {
        private Renderer[] mPlayerRenderers;
        private PlayableDirector playableDirector;

        private void Awake()
        {
            mPlayerRenderers = RuntimeDataLoader.PlayerRuntimeInfo.Trans.GetComponentsInChildren<Renderer>();
            playableDirector = GetComponent<PlayableDirector>();
        }

        private void Start()
        {
            Invoke("OnTimelineFinished", (float)playableDirector.duration);
            for (int i = 0; i < mPlayerRenderers.Length; i++)
            {
                mPlayerRenderers[i].enabled = false;
            }
            RuntimeDataLoader.RuntimePlayData.IsScenePlaying.Value = true;
        }

        private void OnTimelineFinished()
        {
            RuntimeDataLoader.RuntimePlayData.IsScenePlaying.Value = false;
            for (int i = 0; i < mPlayerRenderers.Length; i++)
            {
                mPlayerRenderers[i].enabled = true;
            }
            gameObject.SetActive(false);
        }
    }
}