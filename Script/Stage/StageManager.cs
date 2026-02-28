using RoboClean.Character.Player;
using RoboClean.Common;
using RoboClean.Input;
using UnityEngine;

namespace RoboClean.Stage
{
    public class StageManager : SingletonClass<StageManager>
    {
        [SerializeField] private Transform PLAYER_ENTRANCE;
        [SerializeField] private float FOV = 9.0f;
        [SerializeField] private Collider CAMERA_BOUND;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            PlayerCharacterController.Instance.InitializeOnNewStage(PLAYER_ENTRANCE.position, CAMERA_BOUND, FOV);
            InputManager.Instance.SwitchInputSection(eInputSections.BattleGamePlay);
        }
    }
}
