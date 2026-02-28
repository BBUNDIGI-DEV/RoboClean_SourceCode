using UnityEngine;
using Sirenix.OdinInspector;
using System;
using RoboClean.Utils;
using RoboClean.Character;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/SkillConfig")]
    public class SkillConfig : ActorConfigDataContainerBase
    {
        public string GetCooltimeKey
        {
            get
            {
                return NameID + "CoolTime";
            }
        }

        [TabGroup("Timing", "Total"), ShowInInspector]
        public float TotalDuration
        {
            get
            {
                return PreparingDuration + AttackDuration + CancelableDuration;
            }
        }

        [TabGroup("Timing", "Total"), PropertyOrder(-1), ShowInInspector, ReadOnly]
        public float AcceledTotalDuration
        {
            get
            {
                return AcceledPreparingDuration + AcceledAttackDuration + AcceledCancelableDuration;
            }
        }

        [TabGroup("Timing", "Total"), PropertyOrder(-2), ShowInInspector, Range(-1, 1)]
        public float TotalDurationAccel;

        [TabGroup("Timing", "Preparing"), ShowIf("mNeedManagingTimingDirectly")]
        public float PreparingDuration;

        [TabGroup("Timing", "Preparing"), PropertyOrder(-1), ShowInInspector, ReadOnly]
        public float AcceledPreparingDuration
        {
            get
            {
                return caculateAccelation(PreparingDuration, PreparingTimeAccel);
            }
        }

        [TabGroup("Timing", "Preparing"), PropertyOrder(-2), ShowInInspector, Range(-1, 1)]
        public float PreparingTimeAccel;

        [HideInInspector]
        public float PreparingTimeMultiplier
        {
            get
            {
                return GetSpeedMultiplier(PreparingTimeAccel);
            }
        }


        [TabGroup("Timing", "Attack"), ShowIf("mNeedManagingTimingDirectly")]
        public float AttackDuration;

        [TabGroup("Timing", "Attack"), PropertyOrder(-1), ShowInInspector, ReadOnly]
        public float AcceledAttackDuration
        {
            get
            {
                return caculateAccelation(AttackDuration, AttackDurationAccel);
            }
        }

        [TabGroup("Timing", "Attack"), PropertyOrder(-2), ShowInInspector, Range(-1, 1)]
        public float AttackDurationAccel;

        [HideInInspector]
        public float AttackTimeMultiplier
        {
            get
            {
                return GetSpeedMultiplier(PreparingTimeAccel);
            }
        }

        [TabGroup("Timing", "Cancle"), ShowIf("mNeedManagingTimingDirectly")]
        public float CancelableDuration;

        [TabGroup("Timing", "Cancle"), PropertyOrder(-1), ShowInInspector, ReadOnly]
        public float AcceledCancelableDuration
        {
            get
            {
                return caculateAccelation(CancelableDuration, CancelableDurationAccel);
            }
        }

        [TabGroup("Timing", "Cancle"), PropertyOrder(-2), ShowInInspector, Range(-1, 1)]
        public float CancelableDurationAccel;

        public float BaseDamage;
        public float AfterCastDelay;
        public float CoolTime;
        [HideIf("mIsPlayerConfig")] public float CoolTimeRandomRange;
        [HideIf("mIsPlayerConfig")] public float InitialCoolTime;
        public eTargetTag TargetTag;

        //ComboInfo
        [ToggleGroup("IsComboAttack")] public bool IsComboAttack;
        [ToggleGroup("IsComboAttack")] public int TotalComboCount;
        [ToggleGroup("IsComboAttack")] public float CombableTime;
        [ToggleGroup("IsComboAttack"), ShowInInspector, ReadOnly]
        public string ComboSkillName
        {
            get; private set;
        }
        [ToggleGroup("IsComboAttack"), ShowInInspector, ReadOnly]
        public int ComboIndex
        {
            get; private set;
        }

        public eSkillRangeType RangeType;
        [ShowIf("RangeType", eSkillRangeType.Melee)] public MeleeAttackData MeleeAttackData;
        [ShowIf("RangeType", eSkillRangeType.Rangend)] public ProjectileAttackData ProjectileData;
        [ShowIf("RangeType", eSkillRangeType.SpreadSludge)] public eSludgeType SludgeType;
        [ShowIf("RangeType", eSkillRangeType.SpreadSludge)] public SludgeSpawnData SludgeSpawnData;

        public bool UseStunInstaed;
        [HideIf("UseStunInstaed")] public NockbackData NockbackData;
        [ShowIf("UseStunInstaed")] public StunData StunData;
        public EffectSpawnData EffectSpawnData;
        public AttackTransitionData TransitionData;
        [ShowIf("RangeType", eSkillRangeType.Melee), ToggleGroup("mIsPlayerConfig")] public AimAssistanceConfig AimAisstanceConfig;
        [ToggleGroup("mIsPlayerConfig")] public CameraActingEventData CameraActingOnUsingSkill;
        [ToggleGroup("mIsPlayerConfig")] public CameraActingEventData CameraActingOnHit;
        [ToggleGroup("mIsPlayerConfig")] public bool UseBulletTime;
        [ToggleGroup("mIsPlayerConfig"), ShowIf("UseBulletTime")] public BulletTimeData BulletTime;
        public GameObject HitEffect;
        public GameObject PreparingEffectOrNull;
        public Vector3 PreparingEffectPos;
        [NonSerialized] public float RuntimeAttackSpeedMultiplier;

        private const string M_STATE_CHANGE_EVENT_NAME = "ChangeProgressState";
        private const float M_MAX_ACCELARTION_FACTOR = 3.0f;

        [SerializeField, PropertyOrder(-1), ToggleGroup("mIsPlayerConfig", "Is For Player")] private bool mIsPlayerConfig;
        [InlineButton("presettingAttackMotion", "Parsing Attack Motion")]
        [TabGroup("Timing", "Total"), SerializeField, HideIf("mNeedManagingTimingDirectly"), AssetsOnly] private AnimationClip ATTACK_MOTION;
        [SerializeField] private eActorType SKILL_ACTOR_TYPE;
        [TabGroup("Timing", "Total"), LabelText("Manging timing directly"), PropertyOrder(-3), SerializeField]
        private bool mNeedManagingTimingDirectly;
        [NonSerialized] private bool mIsInitializedInRuntime;

        public void SetAttackDir(Vector3 dir)
        {
            NockbackData.NockbackDir = dir;
        }

        public void InitializeInRuntime()
        {
            Debug.Assert(Application.isPlaying, "Runtime Function called in editor");
            if (mIsInitializedInRuntime)
            {
                return;
            }

            mIsInitializedInRuntime = true;
            presettingAttackMotion();
            presettingComboInfo();
        }

        public float GetSpeedMultiplier(float inspectorValue)
        {
            Debug.Assert(inspectorValue >= -1 && inspectorValue <= 1, $"accel out of range [{inspectorValue} {NameID}]");
            inspectorValue = Mathf.Clamp((inspectorValue + TotalDurationAccel + RuntimeAttackSpeedMultiplier), -1, 1);
            return ActorSpeedUtil.ConvertInspectorAccelToMultiplier(inspectorValue, M_MAX_ACCELARTION_FACTOR);
        }

        protected override void initializeConfig()
        {
            BaseConfig.ActorType = SKILL_ACTOR_TYPE;
            BaseConfig.IsUpdatedActor = false;
        }

        private void presettingAttackMotion()
        {
            if (mNeedManagingTimingDirectly)
            {
                if (!Application.isPlaying)
                {
                    Debug.Log("You Cannot setting timing by animation when ManagingTimingDirectly set on");
                }
                return;
            }

            float preparingDuration;
            float attackDuration;
            float cancelableDuration;
            bool eventCheckResult = checkAttackMotionEventIsExist(out preparingDuration, out attackDuration, out cancelableDuration);

            if (!eventCheckResult)
            {
                return;
            }

            PreparingDuration = preparingDuration;
            AttackDuration = attackDuration;
            CancelableDuration = cancelableDuration;

            if (Application.isPlaying)
            {
                removeFlaggingOnlyEvent();
            }

        }

        [ToggleGroup("IsComboAttack"), PropertyOrder(-3), Button("Parsing Comboattack Data")]
        private void presettingComboInfo()
        {
            if (!IsComboAttack)
            {
                return;
            }
            int parsedComboInex;
            int.TryParse(NameID.Substring(NameID.IndexOf("@") + 1), out parsedComboInex);
            ComboIndex = parsedComboInex;
            ComboSkillName = NameID.Substring(0, NameID.IndexOf("@"));
        }

        private void removeFlaggingOnlyEvent()
        {
            AnimationEvent[] events = ATTACK_MOTION.events;
            AnimationEvent[] newEvents = new AnimationEvent[events.Length - 3];
            int newEventIndex = 0;
            for (int i = 0; i < events.Length; i++)
            {
                AnimationEvent curEvent = events[i];
                if (curEvent.functionName == M_STATE_CHANGE_EVENT_NAME)
                {
                    continue;
                }

                newEvents[newEventIndex] = curEvent;
                newEventIndex++;
            }
            ATTACK_MOTION.events = newEvents;
        }

        private bool checkAttackMotionEventIsExist(out float perparingDuration, out float attackDuration, out float cancelableDuration)
        {
            AnimationEvent[] events = ATTACK_MOTION.events;
            int removedEventCount = 0;
            perparingDuration = 0.0f;
            attackDuration = 0.0f;
            cancelableDuration = 0.0f;

            for (int i = 0; i < events.Length; i++)
            {
                AnimationEvent curEvent = events[i];

                if (curEvent.functionName != M_STATE_CHANGE_EVENT_NAME)
                {
                    continue;
                }

                switch ((eSkillProgressState)removedEventCount)
                {
                    case eSkillProgressState.Preparing:
                        perparingDuration = curEvent.time;
                        break;
                    case eSkillProgressState.OnAttack:
                        attackDuration = curEvent.time - perparingDuration;
                        break;
                    case eSkillProgressState.Cancelable:
                        cancelableDuration = curEvent.time - attackDuration - perparingDuration;
                        break;
                    default:
                        Debug.Assert(false, (eSkillProgressState)removedEventCount);
                        break;
                }
                removedEventCount++;
            }
            return removedEventCount == 3;
        }

        private float caculateAccelation(float original, float accel)
        {
            float multplier = GetSpeedMultiplier(accel);

            return original * multplier;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (IsComboAttack)
            {
                int placeHolder;
                bool result = int.TryParse(NameID.Substring(NameID.IndexOf("@") + 1), out placeHolder);
                if (!result)
                {
                    Debug.LogWarning("ComboAttack으로 설정된 SkillConfig의 이름은 항상 이름@콤보로 구성되어야 합니다.");
                }
            }
            Debug.Assert(SKILL_ACTOR_TYPE.IsAttackType(),
                $"SkillActorType은 Attack Type만 지정 가능합니다. [{name}]");

            if (Application.isPlaying && !mIsInitializedInRuntime)
            {
                if (ATTACK_MOTION != null)
                {
                    float preparingDuration;
                    float attackDuration;
                    float cancelableDuration;
                    bool eventCheckResult = checkAttackMotionEventIsExist(out preparingDuration, out attackDuration, out cancelableDuration);
                    Debug.Assert(eventCheckResult, $"{ATTACK_MOTION.name}은\n" +
                        " [ChangeProgressState]라는 Function Name을 가진 이벤트가 반드시 3개이상 있어야 합니다.\n" +
                        "해당 이벤트 플래그를 기준으로 스킬의 준비동작, 공격동작, 캔슬가능한 시점이 결정됩니다.\n");
                }
            }
        }
#endif
    }

    [Serializable]
    public struct ProjectileAttackData
    {
        [RequiredIn(PrefabKind.PrefabAsset)] public GameObject Projectile;
        public eShootingType ShootingType;
        public float ThrowDelay;
        public float ThrowSpeed;

        [ShowIf("ShootingType", eShootingType.ShotGun)] public int ShotgunAmount;
        [ShowIf("ShootingType", eShootingType.ShotGun)] public float ShotgunAngle;

        [ShowIf("ShootingType", eShootingType.Circular)] public int CircularAmount;

        [ToggleGroup("IsMultiShot", "Multi shot option")] public bool IsMultiShot;
        [ToggleGroup("IsMultiShot", "Multi shot option")] public int ShootingAmount;
        [ToggleGroup("IsMultiShot", "Multi shot option")] public float DelayBetweenShooting;
        [ToggleGroup("IsMultiShot", "Multi shot option")] public eMultiShotAimingType MultiShotAiming;
        [ToggleGroup("IsMultiShot", "Multi shot option")
            , ShowIf("MultiShotAiming", eMultiShotAimingType.ConstantRotate)]
        public float RotateDegreePerShoot;

        public enum eShootingType
        {
            OneShot,
            ShotGun,
            Circular,
        }

        public enum eMultiShotAimingType
        {
            NoneChangeOnFirstShoot,
            TrackingPerShoot,
            ConstantRotate

        }
    }

    [Serializable]
    public struct MeleeAttackData
    {
        [RequiredIn(PrefabKind.PrefabAsset)] public GameObject AttackCollider;
        public float ColliderInvokeTime;
        public float ColliderRemainTime;
    }

    [Serializable]
    public struct NockbackData
    {
        public eNockBackType NockBackType;
        [ShowIf("@NockBackType == eNockBackType.PushToAttackDir || NockBackType == eNockBackType.CircularToHitPoint")] public float NockbackPower;
        [ShowIf("@NockBackType == eNockBackType.PushToAttackDir || NockBackType == eNockBackType.CircularToHitPoint")] public float NockbackTime;
        [ShowIf("NockBackType", eNockBackType.Deaccelerate)] public float Deacceleration;
        [HideIf("NockBackType", eNockBackType.None)] public float HitFreezeTime;

        [NonSerialized] public Vector3 NockbackDir;
    }

    [Serializable]
    public struct StunData
    {
        public float StunDuration;
    }

    [Serializable]
    public struct AttackTransitionData
    {
        public eAttackTransitionType ActionType;
        [ShowIf("ActionType", eAttackTransitionType.SetDecellationFromCurrentSpeed), Range(0, 1)] public float DecellationAmount;

        [ShowIf("ActionType", eAttackTransitionType.MoveToSpecificDest)] public float DestDistance;
        [ShowIf("ActionType", eAttackTransitionType.MoveToSpecificDest)] public float Delay;
        [ShowIf("ActionType", eAttackTransitionType.MoveToSpecificDest)] public float Duration;
        [InfoBox("이동 데이터를 가진 공격이 보정되어 몬스터를 타게팅 할 경우, 몬스터의 위치 보다 해당 값만큼 더 뒤로 떨어진 위치를 공격지점으로 삼습니다.")
        , ShowIf("ActionType", eAttackTransitionType.MoveToSpecificDest)]
        public float AssistedDestSubtraction;

        [ShowIf("ActionType", eAttackTransitionType.RushToPlayer)] public float RushDelay;
        [ShowIf("ActionType", eAttackTransitionType.RushToPlayer)] public float RushSpeed;
        [ShowIf("ActionType", eAttackTransitionType.RushToPlayer), Range(0.015f, 0.1f)] public float TrackRotatingSpeed;
    }

    [Serializable]
    public struct CameraActingEventData
    {
        public eCameraActingType ActingType;

        [ShowIf("@ActingType.HasFlag(eCameraActingType.Shake)"), Range(0, 1)] public float ShakeDelay;
        [ShowIf("@ActingType.HasFlag(eCameraActingType.Shake)")] public float ShakeDuration;
        [ShowIf("@ActingType.HasFlag(eCameraActingType.Shake)")] public AnimationCurve ShakeCurve;

        [ShowIf("@ActingType.HasFlag(eCameraActingType.ZoomInAndOutCurve)")] public float ZoomInOutDelay;
        [ShowIf("@ActingType.HasFlag(eCameraActingType.ZoomInAndOutCurve)")] public float ZoomInAndOutDuration;
        [ShowIf("@ActingType.HasFlag(eCameraActingType.ZoomInAndOutCurve)")] public AnimationCurve ZoomInOutCurve;
    }

    [Serializable]
    public struct EffectSpawnData
    {
        [RequiredIn(PrefabKind.PrefabAsset)] public GameObject EffectPrefab;
        public float InvokeDelay;
        public bool NeedDeactiveOnSkillEnd;
    }


    [System.Serializable]
    public struct BulletTimeData
    {
        public AnimationCurve BulletTimeCurve;
        public float BulletDuration;

    }
    [Flags]
    public enum eCameraActingType
    {
        Shake = 1 << 1,
        ZoomInAndOutCurve = 1 << 2,
    }

    public enum eAttackTransitionType
    {
        None,
        SetDecellationFromCurrentSpeed,
        MoveToSpecificDest,
        Pause,
        RushToPlayer,
    }

    public enum eSkillRangeType
    {
        Melee,
        Rangend,
        SpreadSludge
    }

    public enum eNockBackType
    {
        None,
        PushToAttackDir,
        CircularToHitPoint,
        Deaccelerate,
    }

    public enum eSkillProgressState
    {
        None = -1,
        Preparing,
        OnAttack,
        Cancelable,
    }

    public enum eTargetTag
    {
        Player,
        Enemey,
        All,
    }

    public enum eStunType
    {
        DefaultStun,
    }
}