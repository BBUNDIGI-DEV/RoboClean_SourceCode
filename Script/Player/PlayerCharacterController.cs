using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using UnityEngine.InputSystem;
using RoboClean.Common;
using RoboClean.Data;
using RoboClean.Player;
using RoboClean.Input;
using RoboClean.Managers;
using RoboClean.Utils;
using RoboClean.Sound;

namespace RoboClean.Character.Player
{
    public class PlayerCharacterController : SingletonClass<PlayerCharacterController>
    {
        public Cinemachine.CinemachineVirtualCamera PlayerVCam
        {
            get
            {
                return PLAYER_VCAM;
            }
        }
        public bool IsInputBlocked
        {
            get
            {
                return mIsInputBlocked;
            }
            set
            {
                mIsInputBlocked = value;
            }
        }
        private PlayerConfig mConfig
        {
            get
            {
                return RuntimeDataLoader.PlayerConfig;
            }
        }

        private RuntimePlayerInfo PlayerInfo
        {
            get
            {
                return RuntimeDataLoader.PlayerRuntimeInfo;
            }
        }

        [SerializeField] private PlayableDirector DEATH_TIMELINE;
        [SerializeField] private Cinemachine.CinemachineVirtualCamera PLAYER_VCAM;

        private ActorStateMachine mPlayerSM;
        private Rigidbody PlayerRB;
        private int mLastToxicDebuffTear;
        private bool mIsInputBlocked;
        protected override void Awake()
        {
            base.Awake();
            mPlayerSM = new ActorStateMachine();
            Rigidbody rigidbody = GetComponentInChildren<Rigidbody>();
            PlayerRB = rigidbody;
            CharacterAnimator anim = GetComponentInChildren<CharacterAnimator>();

            ComboSkillActor normalSkillActor = new ComboSkillActor(rigidbody, anim, mPlayerSM, mConfig.NormalAttack, gameObject, null, true);
            mPlayerSM.AddActor(normalSkillActor);
            SkillActor specialSkillActor = new SkillActor(rigidbody, anim, mPlayerSM, mConfig.SpecialAttack, gameObject, null, true);
            mPlayerSM.AddActor(specialSkillActor);
            SkillActor dashSkillActor = new SkillActor(rigidbody, anim, mPlayerSM, mConfig.DashAttack, gameObject, null, true);
            mPlayerSM.AddActor(dashSkillActor);
            DashActor dashActor = new DashActor(rigidbody, anim, mPlayerSM, mConfig.DashConfig, gameObject, null);
            mPlayerSM.AddActor(dashActor);
            InputMovementActor inputActor = new InputMovementActor(rigidbody, anim, mPlayerSM, mConfig.MovementConfig, gameObject);
            mPlayerSM.AddActor(inputActor);
            DamagedActor damagedActor = new DamagedActor(rigidbody, anim, mPlayerSM, mConfig.DamagedActorConfig, gameObject, null);
            mPlayerSM.AddActor(damagedActor);
            StunActor stunActor = new StunActor(rigidbody, anim, mPlayerSM, gameObject, mConfig.StunConfig, null);
            mPlayerSM.AddActor(stunActor);

            GetComponent<Rigidbody>().GetLayeredRigidbody().SetZAxisPusher(mConfig.MovementConfig.ZAxisPusher);
            initializePlayerInfo();
        }

        private void Start()
        {
            int testIndex = ArrayHelper.FindCloseIndex(mConfig.ToxicDebuffTearChecker, 1.0f);

            DashActor dashActor = mPlayerSM.GetActor<DashActor>(eActorType.Dash);

            InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Dash.ToString(), tryDash);
            InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.NormalAttack.ToString(), tryNormalAttack);
            InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.SpeicalAttack.ToString(), trySpecialAttack);
            InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.CheatDamage.ToString(), toggleCheatDamage);

            RuntimeDataLoader.RuntimeStageData.ToxicGuage.AddListener(setDebuffByToxicGuage);
            if (mConfig.DebuffByToxicConfig.Length != 0)
            {
                updateToxicDebuff(mConfig.DebuffByToxicConfig[0]);
            }
        }

        private void FixedUpdate()
        {
            if (PlayerInfo.IsPaused)
            {
                return;
            }


            mPlayerSM.UpdateActor();
            if (!mIsInputBlocked)
            {
                PlayerRB.UpdateVelocity();
                PlayerRB.UpdateRotation();
            }
        }

        private void OnDisable()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (GlobalTimer.IsExist)
            {
                GlobalTimer.Instance.RemoveAllTimerByInstance(gameObject.GetInstanceID());
            }

            if (InputManager.IsExist)
            {
                InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Dash.ToString(), tryDash);
                InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.NormalAttack.ToString(), tryNormalAttack);
                InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.SpeicalAttack.ToString(), trySpecialAttack);
                InputManager.Instance.RemoveInputCallback(eInputSections.BattleGamePlay, eBattleInputName.CheatDamage.ToString(), toggleCheatDamage);
            }
        }

        public void InitializeOnNewStage(Vector3 initialPos, Collider cameraBound, float cameraFOV)
        {
            if (PlayerInfo.IsDead)
            {
                initializePlayerInfo();
            }
            IsInputBlocked = false;
            enabled = true;
            PlayerRB.position = initialPos;
            PlayerRB.velocity = Vector3.zero;
            PlayerVCam.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingVolume = cameraBound;
            PlayerVCam.m_Lens.FieldOfView = cameraFOV;
            CharacterAnimator anim = GetComponentInChildren<CharacterAnimator>();
            anim.UpdateMovementAnim(Vector3.zero, false);
            DEATH_TIMELINE.gameObject.SetActive(false);
        }

        public void MoveToAnotherScene(Vector3 approachPoint, Vector3 exitPoint, eSceneName sceneName)
        {
            StartCoroutine(moveAnotherSceneRoutine(approachPoint, exitPoint, sceneName));
        }

        public void GotAttack(SkillConfig config)
        {
            PlayerInfo.CurrentHP.Value -= config.BaseDamage;
            PlayerInfo.LastDamage.Value = config.BaseDamage;
            PlayerInfo.NormalizedHP.Value = PlayerInfo.CurrentHP / PlayerInfo.MaxHP;
            if (PlayerInfo.CurrentHP <= 0)
            {
                PlayerInfo.IsDead.Value = true;
                DEATH_TIMELINE.gameObject.SetActive(true);
                DEATH_TIMELINE.Play();
                GlobalTimer.Instance
                    .TryGetBasicTimerOrAdd("DeathTimer", eTimerUpdateMode.Update, () => UIManager.Instance.GameOverUI.SetActive(true))
                    .ChangeDuration((float)DEATH_TIMELINE.duration)
                    .StartTimer();

                IsInputBlocked = true;
                mPlayerSM.PauseAllActorWithoutDead();
                transform.FindRecursive("HitBox").GetComponent<Collider>().enabled = false;
                return;
            }


            if (mPlayerSM.CurrentActorType == eActorType.Dash || PlayerInfo.IsDead)
            {
                return;
            }

            if (config.UseStunInstaed)
            {
                StunActor stunACtor = mPlayerSM.GetActor<StunActor>(eActorType.Stun);
                stunACtor.SetStunData(config.StunData);

                mPlayerSM.TrySwitchActor(eActorType.Stun);
            }
            else
            {
                DamagedActor damagedActor = mPlayerSM.GetActor<DamagedActor>(eActorType.Damaged);
                damagedActor.HitSkillConfig = config;
                AudioManager.PlayerHitVoiceSound();
                mPlayerSM.TrySwitchActor(eActorType.Damaged);
            }


            CameraUtils.Instance.GlitchInvoker.InvokeGlitchEffect();
        }

        private IEnumerator moveAnotherSceneRoutine(Vector3 approachPoint, Vector3 exitPoint, eSceneName sceneName)
        {
            CharacterAnimator anim = GetComponentInChildren<CharacterAnimator>();
            IsInputBlocked = true;
            float speed = 5.0f;
            enabled = false;
            Vector3 deltaVector = approachPoint - PlayerRB.position;
            deltaVector.Set(deltaVector.x, 0.0f, deltaVector.z);
            Debug.DrawRay(PlayerRB.position, deltaVector, Color.red, 10.0f);
            deltaVector.Normalize();
            Tween moveTween = PlayerRB.DOMove(approachPoint, speed).SetEase(Ease.Linear).SetSpeedBased();
            PlayerRB.gameObject.transform
                .DORotateQuaternion(Quaternion.LookRotation(deltaVector), 0.5f).SetEase(Ease.Linear);

            while (moveTween.IsActive())
            {
                anim.UpdateMovementAnim(deltaVector, false);
                yield return null;
            }

            deltaVector = exitPoint - PlayerRB.position;
            deltaVector.Set(deltaVector.x, 0.0f, deltaVector.z);
            deltaVector.Normalize();
            moveTween = PlayerRB.DOMove(exitPoint, speed).SetEase(Ease.Linear).SetSpeedBased();
            PlayerRB.gameObject.transform
                .DORotateQuaternion(Quaternion.LookRotation(deltaVector), 0.5f).SetEase(Ease.Linear);
            while (moveTween.IsActive())
            {
                anim.UpdateMovementAnim(deltaVector, false);
                yield return null;
            }

            PlayerRB.velocity = deltaVector * 5.0f;

            SceneSwitchingManager.Instance.LoadOtherScene(sceneName, true);
        }

        private void tryDash(InputAction.CallbackContext context)
        {
            if (IsInputBlocked)
            {
                return;
            }

            DashActor dashActor = mPlayerSM.GetActor<DashActor>(eActorType.Dash);
            if (!dashActor.CanDash)
            {
                return;
            }

            mPlayerSM.TrySwitchActor(eActorType.Dash);
        }

        private void tryNormalAttack(InputAction.CallbackContext context)
        {
            if (IsInputBlocked)
            {
                return;
            }

            DashActor dashActor = mPlayerSM.GetActor<DashActor>(eActorType.Dash);
            if (dashActor.CanDashAttack)
            {
                SkillActor dashSkillActor = mPlayerSM.GetActor<SkillActor>(eActorType.DashAttack);
                if (dashSkillActor.CanAttack)
                {
                    mPlayerSM.TrySwitchActor(eActorType.DashAttack, false);
                    return;
                }
            }

            ComboSkillActor normalSkillActor = mPlayerSM.GetActor<ComboSkillActor>(eActorType.NormalAttack);
            if (normalSkillActor.CanComboAttack)
            {
                if (normalSkillActor.IsWaitComboAttackInput)
                {
                    mPlayerSM.TrySwitchActor(eActorType.NormalAttack);
                }
                else
                {
                    normalSkillActor.IsComboAttackPressed = true;
                }
                return;
            }

            if (!normalSkillActor.CanAttack)
            {
                return;
            }


            mPlayerSM.TrySwitchActor(eActorType.NormalAttack);
        }

        private void trySpecialAttack(InputAction.CallbackContext context)
        {
            if (IsInputBlocked)
            {
                return;
            }


            SkillActor specialSkillActor = mPlayerSM.GetActor<SkillActor>(eActorType.SpecialAttack);

            if (!specialSkillActor.CanAttack)
            {
                return;
            }


            mPlayerSM.TrySwitchActor(eActorType.SpecialAttack);
        }

        private void toggleCheatDamage(InputAction.CallbackContext context)
        {
            if (IsInputBlocked)
            {
                return;
            }


            RuntimeDataLoader.PlayerRuntimeInfo.CheatDamage = !RuntimeDataLoader.PlayerRuntimeInfo.CheatDamage;
        }

        private void setDebuffByToxicGuage(float normalizedToxic)
        {
            int newTear = mConfig.ToxicDebuffTearChecker.FindCloseIndex(normalizedToxic);
            if (newTear == mLastToxicDebuffTear)
            {
                return;
            }

            mLastToxicDebuffTear = newTear;
            updateToxicDebuff(mConfig.DebuffByToxicConfig[mLastToxicDebuffTear]);
        }

        private void updateToxicDebuff(PlayerDebuffByToxicConfig toxicConfig)
        {
            PlayerRB
                .GetLayeredRigidbody()
                .SetVelocityMultiplier(toxicConfig.DecreaseSpeedAmount, eSpeedMultiplierSource.Toxic);

            mPlayerSM.GetActor<SkillActor>(eActorType.DashAttack).SetAttackSpeedMulitplier(toxicConfig.DecreaseAttackSpeed);
            mPlayerSM.GetActor<SkillActor>(eActorType.NormalAttack).SetAttackSpeedMulitplier(toxicConfig.DecreaseAttackSpeed);
        }

        private void initializePlayerInfo()
        {
            PlayerInfo.SetPlayerTransform(transform);

            PlayerInfo.MaxHP.Value = mConfig.InitialMaxHP;
            PlayerInfo.CurrentHP.Value = PlayerInfo.MaxHP;

            PlayerInfo.DashCoolTime.Value = mConfig.DashConfig.DashCooltime;
            PlayerInfo.CurrentDashCoolTime.Value = 0f;
            PlayerInfo.IsDead.Value = false;
            PlayerInfo.IsPaused = false;
            transform.FindRecursive("HitBox").GetComponent<Collider>().enabled = true;
        }
    }
}

