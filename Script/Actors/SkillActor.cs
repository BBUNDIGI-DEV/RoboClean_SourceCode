using RoboClean.Character.Player;
using RoboClean.Common;
using RoboClean.Data;
using RoboClean.Input;
using RoboClean.Managers;
using RoboClean.Player;
using RoboClean.Utils;
using UnityEngine;

namespace RoboClean.Character
{
    public class SkillActor : ActorBase
    {
        public SkillConfig Config
        {
            get; protected set;
        }

        public eSkillProgressState ProgressState
        {
            get; private set;
        }

        public bool IsOnAttack
        {
            get
            {
                return ProgressState != eSkillProgressState.None;
            }
        }

        public bool CanCancelable
        {
            get
            {
                return ProgressState == eSkillProgressState.Preparing || ProgressState == eSkillProgressState.Cancelable;
            }
        }

        public bool CanAttack
        {
            get
            {
                return CanAttackWithoutAfterCastDelay && !mAfterCastDelayTimer.IsActivate;
            }
        }

        public bool CanAttackWithoutAfterCastDelay
        {
            get
            {
                return !IsOnAttack
                    && !mCooltimeTimer.IsActivate
                    && OWNER.CurrentActorType != eActorType.Stun
                    && OWNER.CurrentActorType != eActorType.Damaged;
            }
        }

        protected readonly GameObjectPool MELEE_ATTACKBOX_POOL;
        protected readonly GameObjectPool PROJECTILE_POOL;
        protected readonly GameObjectPool EFFECT_POOL;
        protected readonly System.Action ON_ATTACK_END;

        protected ChainedTimer mAttackDurationTimer
        {
            get
            {
                return GlobalTimer.Instance.TryGetTimerOrAdd<ChainedTimer>
                    (getTimerKeyString("AttackDuration"),
                    eTimerUpdateMode.FixedUpdate
                    , INSTANCE_ID);
            }
            //AttackPreparing -> AttackDurtation -> CancleableAttackTime(Now Attack End) ->
            //AfterCastDeay, Cooltime Setting -> If combo attack set combable time....;
        }

        protected BasicTimer mAfterCastDelayTimer
        {
            get
            {
                return GlobalTimer.Instance.TryGetTimerOrAdd<BasicTimer>("AfterCastDelay", eTimerUpdateMode.FixedUpdate, INSTANCE_ID);
            }
        }

        protected DelayTimer mTransitionDelayTimer
        {
            get
            {
                return GlobalTimer.Instance.TryGetTimerOrAdd<DelayTimer>(getTimerKeyString("Transition"), eTimerUpdateMode.FixedUpdate, INSTANCE_ID);

            }
        }

        protected UpdateTimer mTrackPlayerUpdateTimer
        {
            get
            {
                Debug.Assert(!mIsPlayerActor,
                    $"Track Player option is not for skillconfig for player [{NameID}]");
                return GlobalTimer.Instance.TryGetTimerOrAdd<UpdateTimer>(getTimerKeyString("TrackPlayer"), eTimerUpdateMode.FixedUpdate, INSTANCE_ID);
            }
        }

        protected DelayTimer mAttackBoxDelayTimer
        {
            get
            {
                return GlobalTimer.Instance.TryGetTimerOrAdd<DelayTimer>(getTimerKeyString("AttackBox"), eTimerUpdateMode.FixedUpdate, INSTANCE_ID);
            }
        }

        protected BasicTimer mEffectActivateTimer
        {
            get
            {
                return GlobalTimer.Instance.TryGetTimerOrAdd<BasicTimer>(getTimerKeyString("AttackEffect"), eTimerUpdateMode.FixedUpdate, INSTANCE_ID);
            }
        }

        protected BasicTimer mCooltimeTimer
        {
            get
            {
                return GlobalTimer.Instance.TryGetTimerOrAdd<BasicTimer>(getTimerKeyString("CoolTime"), eTimerUpdateMode.FixedUpdate, INSTANCE_ID);
            }
        }

        protected ChainedTimer mProjectileMultiShotTimer;

        protected UpdateTimer mSludgeSpreadTimer;

        protected SkillConfig mStackedConfig;

        private bool mIsPlayerActor;

        public SkillActor(Rigidbody rb, CharacterAnimator anim, ActorStateMachine onwerStateMachine, SkillConfig config, GameObject gameObject, System.Action onAttackEnd = null, bool isPlayer = false)
            : base(rb, anim, onwerStateMachine, config.BaseConfig, config.NameID, gameObject)
        {
            Debug.Assert(config.BaseConfig.ActorType.ToString().Contains("Attack"),
                $"Don't pass eActortype not contain \"Attack\" in SkillActor [{config.BaseConfig.ActorType}]," +
                $" Only attack actorType can be type of SkillActor");

            Config = config;
            Config.InitializeInRuntime();

            MELEE_ATTACKBOX_POOL = GameObjectPool.TryGetGameobjectPool(gameObject.transform, "AttackBoxPool");
            EFFECT_POOL = GameObjectPool.TryGetGameobjectPool(gameObject.transform, "EffectPool", ePoolingObjectType.EffectPool);
            PROJECTILE_POOL = GameObjectPool.TryGetGameobjectPool(gameObject.transform.parent, "ProjectilePool", ePoolingObjectType.AttackBox);
            ON_ATTACK_END = onAttackEnd;

            mAttackDurationTimer
                .AddCallback("Preparing", Config.AcceledPreparingDuration, () => setSkillProgressState(eSkillProgressState.OnAttack))
                .AddCallback("OnAttacking", Config.AcceledAttackDuration, () => setSkillProgressState(eSkillProgressState.Cancelable))
                .AddCallback("Cancleable", Config.AcceledCancelableDuration, () => setSkillProgressState(eSkillProgressState.None));

            if (Config.RangeType == eSkillRangeType.Rangend && Config.ProjectileData.IsMultiShot)
            {
                setMultishotTimer(Config.ProjectileData);
            }
            else if (Config.RangeType == eSkillRangeType.SpreadSludge
                && Config.SludgeSpawnData.SpawnType == eSludgeSpawnType.SpreadGradual)
            {
                mSludgeSpreadTimer = new UpdateTimer(eTimerUpdateMode.FixedUpdate, Config.SludgeSpawnData.SpreadTotalDuration,
                    null, onSpreadToxic, Config.SludgeSpawnData.SpreadTimeInterval);
                GlobalTimer.Instance.AddTimerWithInstacneID("SludgeSpread", INSTANCE_ID, mSludgeSpreadTimer);
            }
            mIsPlayerActor = isPlayer;
            ProgressState = eSkillProgressState.None;

            if (!mIsPlayerActor && Config.InitialCoolTime != 0.0f)
            {
                mCooltimeTimer.ChangeDuration(Config.InitialCoolTime
                    + Random.Range(-Config.CoolTimeRandomRange, Config.CoolTimeRandomRange));
            }
        }

        public void TryChangeConfigOrStackedIn(SkillConfig newSkillConfig)
        {
            if (IsOnAttack)
            {
                mStackedConfig = newSkillConfig;
            }
            else
            {
                changeConfig(newSkillConfig);
            }
        }

        public override void StopActing()
        {
            if (ProgressState == eSkillProgressState.None)
            {
                return;
            }
            mAttackDurationTimer.PauseTimer();
            setSkillProgressState(eSkillProgressState.None);
        }

        public override void DestoryActor()
        {
        }

        public override void InovkeActing()
        {
            if (mAttackDurationTimer.IsActivate)
            {
                mAttackDurationTimer.PauseTimer();
            }

            Vector3 assistedPos = Vector3.zero;
            Vector3 attackDir = getAttackDir(out assistedPos);

            RB.EnrollRotation(Quaternion.LookRotation(attackDir), ActorType);


            handleAttackTransition(Config.TransitionData, assistedPos);

            switch (Config.RangeType)
            {
                case eSkillRangeType.Melee:
                    handleMeleeAttackAction(Config.MeleeAttackData);
                    break;
                case eSkillRangeType.Rangend:
                    handleRangedAttackAction(Config.ProjectileData, attackDir, Config.TargetTag);
                    break;
                case eSkillRangeType.SpreadSludge:
                    handleSludgeSpreadAction(Config.SludgeSpawnData, Config.SludgeType);
                    break;
                default:
                    Debug.LogError($"Default switch detected [{Config.RangeType}]");
                    break;
            }

            switch (Config.NockbackData.NockBackType)
            {
                case eNockBackType.None:
                    Config.NockbackData.NockbackDir = Vector3.zero;
                    break;
                case eNockBackType.PushToAttackDir:
                    Config.NockbackData.NockbackDir = attackDir;
                    break;
                case eNockBackType.CircularToHitPoint:
                    Config.NockbackData.NockbackDir = Vector3.zero;
                    break;
                case eNockBackType.Deaccelerate:
                    Config.NockbackData.NockbackDir = Vector3.zero;
                    break;
                default:
                    Debug.LogError($"Default switch detected [{Config.NockbackData.NockBackType}]");
                    break;
            }

            if (Config.EffectSpawnData.EffectPrefab != null)
            {
                var fxInfo = Config.EffectSpawnData;
                ParticleSystem ps = fxInfo.EffectPrefab.GetComponent<ParticleSystem>();
                mEffectActivateTimer
                    .ChangeCallback(() => EFFECT_POOL.GetDeactiveGameobject(fxInfo.EffectPrefab))
                    .ChangeDuration(fxInfo.InvokeDelay * Config.PreparingTimeMultiplier)
                    .StartTimer();
            }

            setSkillProgressState(eSkillProgressState.Preparing);

            updateAttackTimerDuration();
            mAttackDurationTimer.StartTimer();
            CameraUtils.Instance.Actor.ProcessCameraActing(Config.CameraActingOnUsingSkill);
            ANIM.TriggerAttackAnim(Config);

            if (Config.PreparingEffectOrNull != null)
            {
                EFFECT_POOL.GetDeactiveGameobject(Config.PreparingEffectOrNull)
                    .transform.localPosition = Config.PreparingEffectPos;
            }
            return;
        }

        public virtual void SetAttackSpeedMulitplier(float speed)
        {
            Config.RuntimeAttackSpeedMultiplier = speed;
        }

        protected virtual void onAttackEnd()
        {
            if (Config.RangeType == eSkillRangeType.Melee && mAttackBoxDelayTimer.IsActivate)
            {
                mAttackBoxDelayTimer.PauseTimerWithCallback();
            }
            if (Config.TransitionData.ActionType == eAttackTransitionType.MoveToSpecificDest && mTransitionDelayTimer.IsActivate)
            {
                mTransitionDelayTimer.PauseTimerWithCallback();
            }
            else if (Config.TransitionData.ActionType == eAttackTransitionType.RushToPlayer && mTrackPlayerUpdateTimer.IsActivate)
            {
                mTrackPlayerUpdateTimer.PauseTimer();
            }

            if (mEffectActivateTimer.IsActivate)
            {
                mEffectActivateTimer.PauseTimer();
            }
            else if (Config.EffectSpawnData.NeedDeactiveOnSkillEnd)
            {
                EFFECT_POOL
                    .GetGameobject(Config.EffectSpawnData.EffectPrefab)
                    .GetComponent<ParticleLoopDeactivator>().LoopPause();
            }



            mAfterCastDelayTimer.ChangeDuration(Config.AfterCastDelay)
                        .StartTimer();
            float coolTime = Config.CoolTime;

            if (!mIsPlayerActor)
            {
                coolTime += Random.Range(-Config.CoolTimeRandomRange, Config.CoolTimeRandomRange);
            }

            mCooltimeTimer.ChangeDuration(coolTime).StartTimer();
            RB.DisEnrollSetVelocity(ActorType);
            RB.DisEnrollRotation(ActorType);

            OWNER.CheckAndClearActor(NameID);

            if (Config.RangeType == eSkillRangeType.Rangend && Config.ProjectileData.IsMultiShot)
            {
                mProjectileMultiShotTimer.PauseTimer();
            }
            else if (Config.RangeType == eSkillRangeType.SpreadSludge
                && Config.SludgeSpawnData.SpawnType == eSludgeSpawnType.SpreadGradual)
            {
                mSludgeSpreadTimer.PauseTimer();
            }
            if (ON_ATTACK_END != null)
            {
                ON_ATTACK_END.Invoke();
            }

            ANIM.EndAttackAnim();
            ANIM.SetAttackSpeed(1);

            if (mStackedConfig != null)
            {
                changeConfig(mStackedConfig);
                mStackedConfig = null;
            }
        }

        protected virtual string getTimerKeyString(string baseName)
        {
            return baseName + Config.NameID;
        }

        protected virtual void setSkillProgressState(eSkillProgressState state)
        {
            ProgressState = state;
            switch (ProgressState)
            {
                case eSkillProgressState.None:
                    onAttackEnd();
                    break;
                case eSkillProgressState.Preparing:
                    ANIM.SetAttackSpeed(1.0f / Config.GetSpeedMultiplier(Config.PreparingTimeAccel));
                    break;
                case eSkillProgressState.OnAttack:
                    ANIM.SetAttackSpeed(1.0f / Config.GetSpeedMultiplier(Config.AttackDurationAccel));
                    break;
                case eSkillProgressState.Cancelable:
                    ANIM.SetAttackSpeed(1.0f / Config.GetSpeedMultiplier(Config.CancelableDurationAccel));
                    OWNER.TryInvokeBufferedActor();
                    break;
                default:
                    Debug.LogError($"Default switch detected [{ProgressState}]");
                    break;
            }
        }

        protected void changeConfig(SkillConfig config)
        {
            Config = config;
            BaseConfig = config.BaseConfig;
            Config.InitializeInRuntime();
            updateAttackTimerDuration();

            if (Config.RangeType == eSkillRangeType.Rangend && Config.ProjectileData.IsMultiShot)
            {
                updateMultishotTimer(Config.ProjectileData);
            }

            ProgressState = eSkillProgressState.None;
        }



        private void handleMeleeAttackAction(MeleeAttackData attackData)
        {
            GameObject collider = MELEE_ATTACKBOX_POOL.GetGameobject(attackData.AttackCollider);
            collider.transform.localPosition = attackData.AttackCollider.transform.localPosition;

            mAttackBoxDelayTimer.ChangeCallback(
                () => collider.SetActive(true),
                () => collider.SetActive(false)).
                ChangeDelayTime(attackData.ColliderInvokeTime * Config.PreparingTimeMultiplier).
                ChangeDuration(attackData.ColliderRemainTime).
                StartTimer();
        }

        private void handleAttackTransition(AttackTransitionData data, Vector3 assistedDest)
        {
            if (data.ActionType == eAttackTransitionType.None)
            {
                return;
            }

            switch (data.ActionType)
            {
                case eAttackTransitionType.SetDecellationFromCurrentSpeed:
                    if (RB.velocity.sqrMagnitude < 0.05f)
                    {
                        return;
                    }
                    RB.EnrollSetVelocity(RB.velocity * data.DecellationAmount, ActorType);
                    break;
                case eAttackTransitionType.MoveToSpecificDest:
                    float destDistance = data.DestDistance;

                    if (assistedDest != Vector3.zero)
                    {
                        destDistance = (RB.position - assistedDest).magnitude;
                        destDistance -= data.AssistedDestSubtraction;
                        destDistance = Mathf.Clamp(destDistance, 0.0f, data.DestDistance + 1);
                    }

                    RB.EnrollSetVelocity(Vector3.zero, ActorType);
                    mTransitionDelayTimer.ChangeCallback(
                        () => setSpeedByDestAndDuration(destDistance, data.Duration),
                        () => RB.EnrollSetVelocity(Vector3.zero, ActorType)).
                        ChangeDelayTime(data.Delay * Config.PreparingTimeMultiplier).
                        ChangeDuration(data.Duration).
                        StartTimer();
                    break;
                case eAttackTransitionType.Pause:
                    RB.EnrollSetVelocity(Vector3.zero, ActorType);
                    break;
                case eAttackTransitionType.RushToPlayer:
                    RB.EnrollRotation(RB.rotation, eActorType.AIAttack);
                    RB.EnrollSetVelocity(Vector3.zero, ActorType);

                    mTrackPlayerUpdateTimer
                        .ChangeUpdateCallback(trackPlayer)
                        .ChangeDuration(1000.0f)
                        .StartTimer();
                    break;
                default:
                    break;
            }
            return;
        }

        private void setSpeedByDestAndDuration(float dest, float duration)
        {
            Vector3 moveDir = RB.rotation * Vector3.forward;
            float speed = dest / duration;
            Vector3 velocity = moveDir * speed;
            RB.EnrollSetVelocity(velocity, ActorType);
        }

        private void handleRangedAttackAction(ProjectileAttackData attackData, Vector3 initialDir, eTargetTag target)
        {
            if (attackData.IsMultiShot)
            {
                shootProjectile(attackData, initialDir, target);
                for (int i = 0; i < attackData.ShootingAmount - 1; i++)
                {
                    Vector3 shootDir = initialDir;
                    switch (attackData.MultiShotAiming)
                    {
                        case ProjectileAttackData.eMultiShotAimingType.NoneChangeOnFirstShoot:
                            break;
                        case ProjectileAttackData.eMultiShotAimingType.TrackingPerShoot:
                            break;
                        case ProjectileAttackData.eMultiShotAimingType.ConstantRotate:
                            Quaternion newAim = Quaternion.Euler(0.0f, attackData.RotateDegreePerShoot * i, 0.0f);
                            shootDir = newAim * initialDir;
                            break;
                        default:
                            Debug.LogError(attackData.MultiShotAiming);
                            break;
                    }
                    mProjectileMultiShotTimer.ChangeCallback(
                        getMultishotChainedTimerTag(i), () => shootProjectile(attackData, shootDir, target));
                }
                mProjectileMultiShotTimer.StartTimer();
            }
            else
            {
                shootProjectile(attackData, initialDir, target);
            }
        }

        private void shootProjectile(ProjectileAttackData attackData, Vector3 attackDir, eTargetTag target)
        {
            if (attackData.IsMultiShot && attackData.MultiShotAiming == ProjectileAttackData.eMultiShotAimingType.TrackingPerShoot)
            {
                attackDir = getAttackDir();
            }

            switch (attackData.ShootingType)
            {
                case ProjectileAttackData.eShootingType.OneShot:
                    instanceProjectileAndShoot(attackData, attackDir, target);
                    break;
                case ProjectileAttackData.eShootingType.ShotGun:
                    if (attackData.ShotgunAmount == 0)
                    {
                        Debug.LogWarning("ShotgunAmount is set as 0");
                        return;
                    }
                    float intialRot = attackData.ShotgunAmount == 1 ? 0.0f : -attackData.ShotgunAngle / 2;
                    float rotAmountPerShoot = attackData.ShotgunAmount == 1 ? 0.0f : attackData.ShotgunAngle / (attackData.ShotgunAmount - 1);

                    for (int i = 0; i < attackData.ShotgunAmount; i++)
                    {
                        Quaternion newRot = Quaternion.Euler(0.0f, intialRot + rotAmountPerShoot * i, 0.0f);
                        Vector3 newAttackDir = newRot * attackDir;
                        instanceProjectileAndShoot(attackData, newAttackDir, target);
                    }
                    break;
                case ProjectileAttackData.eShootingType.Circular:
                    float rotAmount = attackData.CircularAmount == 1 ? 0.0f : 360.0f / attackData.CircularAmount;

                    for (int i = 0; i < attackData.CircularAmount; i++)
                    {
                        Quaternion newRot = Quaternion.Euler(0.0f, rotAmount * i, 0.0f);
                        Vector3 newAttackDir = newRot * attackDir;
                        instanceProjectileAndShoot(attackData, newAttackDir, target);
                    }
                    break;
                default:
                    break;
            }
        }

        private void instanceProjectileAndShoot(ProjectileAttackData attackData, Vector3 attackDir, eTargetTag target)
        {
            GameObject projectileObject = PROJECTILE_POOL.GetDeactiveGameobject(attackData.Projectile);
            ProjectileHandler projectileHandler = projectileObject.GetComponent<ProjectileHandler>();
            Debug.Assert(projectileHandler != null, $"Projectile Gameobject without projectile element detected [{projectileObject.name}]");

            projectileHandler.InitializeProjectile(attackData.ThrowDelay * Config.PreparingTimeMultiplier, attackData.ThrowSpeed, RB.position + attackData.Projectile.transform.position, attackDir, target);
        }

        private void setMultishotTimer(ProjectileAttackData attackData)
        {
            Debug.Assert(attackData.IsMultiShot, $"You Don't need to set projectile timer when not using multishot [{NameID}]");
            ChainedTimer chainedTimer = new ChainedTimer(eTimerUpdateMode.FixedUpdate);
            mProjectileMultiShotTimer = GlobalTimer.Instance.AddTimer("ProjectileTimer" + INSTANCE_ID,
                chainedTimer);

            for (int i = 0; i < attackData.ShootingAmount - 1; i++)
            {
                mProjectileMultiShotTimer.AddCallback(getMultishotChainedTimerTag(i), attackData.DelayBetweenShooting * Config.PreparingTimeMultiplier, null);
            }
        }

        private void updateMultishotTimer(ProjectileAttackData attackData)
        {
            Debug.Assert(attackData.IsMultiShot, $"You Don't need to set projectile timer when not using multishot [{NameID}]");
            for (int i = 0; i < attackData.ShootingAmount - 1; i++)
            {
                if (!mProjectileMultiShotTimer.HasKey(getMultishotChainedTimerTag(i)))
                {
                    mProjectileMultiShotTimer.AddCallback(getMultishotChainedTimerTag(i), attackData.DelayBetweenShooting * Config.PreparingTimeMultiplier, null);
                }
                mProjectileMultiShotTimer.ChangeTiming(getMultishotChainedTimerTag(i), attackData.DelayBetweenShooting * Config.PreparingTimeMultiplier);
            }
        }

        private void handleSludgeSpreadAction(SludgeSpawnData spawnData, eSludgeType sludgeType)
        {
            switch (spawnData.SpawnType)
            {
                case eSludgeSpawnType.SpreadAtOnce:
                    StageToxicManager.Instance.SpreadSludge(RB.position, sludgeType, spawnData.SpreadAmount, spawnData.SpreadRange);
                    break;
                case eSludgeSpawnType.SpreadGradual:
                    mSludgeSpreadTimer
                       .ChangeUpdateInterval(spawnData.SpreadTimeInterval)
                       .ChangeUpdateCallback(onSpreadToxic)
                       .ChangeDuration(spawnData.SpreadTotalDuration)
                       .StartTimer();
                    break;
                default:
                    break;
            }
        }


        private string getMultishotChainedTimerTag(int shootCount)
        {
            return "Shoot" + shootCount;
        }


        private void updateAttackTimerDuration()
        {
            mAttackDurationTimer
                .ChangeTiming("Preparing", Config.AcceledPreparingDuration)
                .ChangeTiming("OnAttacking", Config.AcceledAttackDuration)
                .ChangeTiming("Cancleable", Config.AcceledCancelableDuration);
        }

        private void trackPlayer(float normalizedTime, float timePass)
        {
            Debug.Assert(!mIsPlayerActor, "this function is for monster not player");
            AttackTransitionData data = Config.TransitionData;
            RuntimePlayerInfo playerInfo = RuntimeDataLoader.PlayerRuntimeInfo;


            if (timePass < data.RushDelay * Config.PreparingTimeMultiplier)
            {
                Vector3 enemeyToPlayer = playerInfo.Trans.position - RB.position;
                RB.EnrollRotation(Quaternion.LookRotation(enemeyToPlayer), ActorType);
            }
            else
            {
                Vector3 towardFromVector = RB.velocity;
                if (RB.velocity == Vector3.zero)
                {
                    towardFromVector = RB.GetForward();
                }
                Vector3 towardVector = playerInfo.TowardVectorToPlayer(towardFromVector.normalized, RB.position, data.TrackRotatingSpeed);
                RB.EnrollRotation(Quaternion.LookRotation(towardVector), ActorType);
                RB.EnrollSetVelocity(towardVector * data.RushSpeed, ActorType);
            }
        }

        private void onSpreadToxic(float normalizedTime, float timePass)
        {
            StageToxicManager.Instance.SpreadSludge(RB.transform.position, Config.SludgeType, 1, Config.SludgeSpawnData.SpreadRange);
        }

        private Vector3 getAttackDir(out Vector3 assistedPos)
        {
            Vector3 attackDir = Vector3.zero;
            assistedPos = Vector3.zero;

            if (mIsPlayerActor)
            {
                attackDir = InputManager.Instance.GetAttackAim(RB);
                if (Config.AimAisstanceConfig != null)
                {
                    AimHelper.TryToGetAimAssist(ref attackDir, out assistedPos, RB.position, Config.AimAisstanceConfig);
                }
            }
            else
            {
                RuntimePlayerInfo playerInfo = RuntimeDataLoader.PlayerRuntimeInfo;
                attackDir = playerInfo.Trans.position - RB.position;
                attackDir.Set(attackDir.x, 0.0f, attackDir.z);
                attackDir.Normalize();
            }
            return attackDir;
        }

        private Vector3 getAttackDir()
        {
            Vector3 placeHolder;
            return getAttackDir(out placeHolder);
        }
    }
}