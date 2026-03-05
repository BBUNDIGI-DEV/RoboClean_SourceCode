# SkillConfig Documentation

`SkillConfig`는 **스킬의 동작과 전투 정보를 정의하는
ScriptableObject**입니다.

이 설정을 통해 다음과 같은 요소를 정의할 수 있습니다.

-   스킬 타이밍
-   공격 방식
-   피해량
-   콤보 공격
-   상태 이상
-   이동 동작
-   이펙트
-   카메라 연출
-   플레이어 전용 기능

``` csharp
[CreateAssetMenu(menuName = "DataContainer/SkillConfig")]
public class SkillConfig : ActorConfigDataContainerBase
```

------------------------------------------------------------------------

# Table of Contents

-   Skill Lifecycle
-   Timing Configuration
-   Combat Configuration
-   Combo Attack
-   Range / Attack Type
-   Melee Settings
-   Projectile Settings
-   Crowd Control
-   Movement / Transition
-   Effects
-   Camera Effects
-   Bullet Time
-   Aim Assistance
-   Runtime Values
-   Animation Event Rules

------------------------------------------------------------------------

# Skill Lifecycle

모든 스킬은 다음 3단계를 통해 실행됩니다.

    Preparing → Attack → Cancelable

  단계         설명
  ------------ ---------------------------------
  Preparing    공격 준비 단계
  Attack       실제 공격이 발생하는 단계
  Cancelable   공격 후 행동 취소가 가능한 단계

------------------------------------------------------------------------

# Timing Configuration

스킬 각 단계의 **지속 시간**을 정의합니다.

  변수                   설명
  ---------------------- --------------------------
  `PreparingDuration`    공격 준비 시간
  `AttackDuration`       공격 진행 시간
  `CancelableDuration`   공격 이후 캔슬 가능 시간
  `TotalDuration`        전체 스킬 지속 시간

## Speed Acceleration

  변수                        설명
  --------------------------- ---------------------
  `TotalDurationAccel`        전체 스킬 속도 배율
  `PreparingTimeAccel`        준비 단계 속도 배율
  `AttackDurationAccel`       공격 단계 속도 배율
  `CancelableDurationAccel`   캔슬 단계 속도 배율

## Accelerated Durations

가속 값이 적용된 **최종 스킬 시간**입니다.

  변수
  -----------------------------
  `AcceledPreparingDuration`
  `AcceledAttackDuration`
  `AcceledCancelableDuration`
  `AcceledTotalDuration`

------------------------------------------------------------------------

# Combat Configuration

스킬의 기본 전투 정보를 정의합니다.

  변수                    설명
  ----------------------- --------------------------
  `BaseDamage`            기본 공격 피해량
  `CoolTime`              스킬 쿨타임
  `CoolTimeRandomRange`   AI 스킬 쿨타임 랜덤 범위
  `InitialCoolTime`       최초 쿨타임
  `AfterCastDelay`        스킬 사용 후 추가 딜레이
  `TargetTag`             공격 대상 필터

## TargetTag

    Player
    Enemy
    All

  타입     설명
  -------- -----------------
  Player   플레이어만 공격
  Enemy    적만 공격
  All      모든 대상 공격

------------------------------------------------------------------------

# Combo Attack

콤보 공격 스킬 설정입니다.

  변수                설명
  ------------------- --------------------------
  `IsComboAttack`     콤보 공격 여부
  `TotalComboCount`   전체 콤보 단계 수
  `CombableTime`      다음 콤보 입력 가능 시간
  `ComboSkillName`    콤보 스킬 이름
  `ComboIndex`        콤보 순서

## Combo Naming Rule

콤보 스킬은 다음 규칙을 따릅니다.

    SkillName@Index

예시

    Slash@1
    Slash@2
    Slash@3

------------------------------------------------------------------------

# Range / Attack Type

스킬의 공격 방식을 정의합니다.

  타입           설명
  -------------- ------------------
  Melee          근접 공격
  Ranged         투사체 공격
  SpreadSludge   슬러지 확산 공격

관련 변수

  변수
  -------------------
  `RangeType`
  `MeleeAttackData`
  `ProjectileData`
  `SludgeType`
  `SludgeSpawnData`

------------------------------------------------------------------------

# Melee Settings

`RangeType = Melee`일 때 사용됩니다.

  변수                   설명
  ---------------------- --------------------
  `AttackCollider`       공격 판정 콜라이더
  `ColliderInvokeTime`   콜라이더 생성 시점
  `ColliderRemainTime`   콜라이더 유지 시간

------------------------------------------------------------------------

# Projectile Settings

`RangeType = Ranged`일 때 사용됩니다.

  변수             설명
  ---------------- ----------------
  `Projectile`     투사체 프리팹
  `ThrowDelay`     발사 지연 시간
  `ThrowSpeed`     투사체 속도
  `ShootingType`   발사 방식

## Shooting Types

    OneShot
    ShotGun
    Circular

  타입       설명
  ---------- ------------------
  OneShot    단일 발사
  ShotGun    부채꼴 다중 발사
  Circular   원형 발사

## Shotgun 옵션

  변수
  -----------------
  `ShotgunAmount`
  `ShotgunAngle`

## Circular 옵션

  변수
  ------------------
  `CircularAmount`

## Multi Shot

연속 발사 설정

  변수
  ------------------------
  `IsMultiShot`
  `ShootingAmount`
  `DelayBetweenShooting`
  `MultiShotAiming`

------------------------------------------------------------------------

# Crowd Control

적에게 적용되는 상태 효과입니다.

  변수               설명
  ------------------ ---------------------
  `UseStunInstaed`   넉백 대신 스턴 사용
  `NockbackData`     넉백 설정
  `StunData`         스턴 설정

------------------------------------------------------------------------

# Knockback Data

  변수               설명
  ------------------ ----------------
  `NockBackType`     넉백 타입
  `NockbackPower`    넉백 힘
  `NockbackTime`     넉백 지속 시간
  `Deacceleration`   감속
  `HitFreezeTime`    히트 정지 시간

------------------------------------------------------------------------

# Movement / Transition

공격 중 이동 동작을 정의합니다.

  변수
  ------------------
  `TransitionData`

## AttackTransitionType

    None
    SetDecellationFromCurrentSpeed
    MoveToSpecificDest
    Pause
    RushToPlayer

------------------------------------------------------------------------

# Effects

스킬 실행 시 발생하는 이펙트입니다.

  변수                      설명
  ------------------------- ------------------
  `EffectSpawnData`         공격 이펙트
  `HitEffect`               히트 이펙트
  `PreparingEffectOrNull`   준비 단계 이펙트
  `PreparingEffectPos`      준비 이펙트 위치

------------------------------------------------------------------------

# Camera Effects

플레이어 스킬 전용 카메라 연출입니다.

  변수
  ----------------------------
  `CameraActingOnUsingSkill`
  `CameraActingOnHit`

## Camera Types

    Shake
    ZoomInAndOutCurve

------------------------------------------------------------------------

# Bullet Time

슬로우 모션 연출입니다.

  변수
  -----------------
  `UseBulletTime`
  `BulletTime`

------------------------------------------------------------------------

# Aim Assistance

플레이어 스킬의 **에임 보정 설정**입니다.

  변수
  ----------------------
  `AimAisstanceConfig`

------------------------------------------------------------------------

# Runtime Values

런타임에서만 사용되는 값입니다.

  변수                             설명
  -------------------------------- ----------------
  `RuntimeAttackSpeedMultiplier`   공격 속도 보정
  `GetCooltimeKey`                 쿨타임 저장 키

------------------------------------------------------------------------

# Animation Event Rules

스킬 타이밍은 **애니메이션 이벤트를 통해 자동 계산**될 수 있습니다.

필수 이벤트

    ChangeProgressState

필요한 상태 이벤트

    Preparing
    OnAttack
    Cancelable

이 이벤트를 기준으로 다음 값이 계산됩니다.

    PreparingDuration
    AttackDuration
    CancelableDuration
