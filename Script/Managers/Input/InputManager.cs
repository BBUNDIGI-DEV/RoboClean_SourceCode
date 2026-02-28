using UnityEngine;
using UnityEngine.InputSystem;
using RoboClean.Common;

namespace RoboClean.Input
{
    [DefaultExecutionOrder(-55)]
    public class InputManager : SingletonClass<InputManager>
    {
        public Vector3 MoveDir
        { get; private set; }
        public eInputSections CurrentInputSection
        { get; private set; }
        public InputHandlerBase LoadedInputHandler
        {
            get; private set;
        }

        public bool IsInputEnabled
        {
            get
            {
                return mIsInputEnabled;
            }
            set
            {
                mIsInputEnabled = value;
                LoadedInputHandler.SetEnabled(mIsInputEnabled);
            }
        }
        private bool mIsInputEnabled;


        protected override void Awake()
        {
            base.Awake();
            InputHandlerBase[] inputHandlers = GetComponentsInChildren<InputHandlerBase>(true);

            for (int i = 0; i < inputHandlers.Length; i++)
            {
                if (LoadedInputHandler == null && inputHandlers[i].CheckPlatform(Application.platform))
                {
                    LoadedInputHandler = inputHandlers[i];
                }
                else
                {
                    Destroy(inputHandlers[i].gameObject);
                }
            }
            Debug.Assert(LoadedInputHandler != null, "Input Handler not founded");
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            IsInputEnabled = true;
        }

        public void Update()
        {
            if (CurrentInputSection == eInputSections.BattleGamePlay)
            {
                MoveDir = LoadedInputHandler.GetMoveDir();
            }
        }

        public void SwitchInputSection(eInputSections newMap)
        {
            if (CurrentInputSection == newMap)
            {
                return;
            }
            CurrentInputSection = newMap;
            LoadedInputHandler.SwitchActionMap(newMap);
        }

        public void AddInputCallback(eInputSections map, string action, System.Action<InputAction.CallbackContext> callback)
        {
            LoadedInputHandler.AddInputCallback(map, action, callback);
        }

        public void RemoveInputCallback(eInputSections map, string action, System.Action<InputAction.CallbackContext> callback)
        {
            LoadedInputHandler.RemoveInputCallback(map, action, callback);
        }

        public Vector3 GetAttackAim(Rigidbody playerRB)
        {
            return LoadedInputHandler.GetAttackAim(playerRB);
        }
    }

    public enum eBattleInputName
    {
        Move,
        Dash,
        NormalAttack,
        Interactive,
        PauseGame,
        SpeicalAttack,
        CheatDamage,
    }

    public enum eCutSceneInputName
    {
        CutSceneInteraction,
        Skip,
    }

    public enum eDialougeInputName
    {
        DialogueInteraction,
        Skip
    }

    public enum eMenuInputName
    {

    }

    public enum eUIInputName
    {
        PauseGame,
    }

    public enum eInputDeviceType
    {
        KeyboardAndMouse,
        GamePad,
        Mobile,
    }

    public enum eInputSections
    {
        None = -1,
        BattleGamePlay,
        CutScene,
        Dialouge,
        Menu,
        UI,
        Count,
    }
}