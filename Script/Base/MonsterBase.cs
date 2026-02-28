using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using RoboClean.Player;
using RoboClean.Data;
using RoboClean.Character.AI;
using RoboClean.Character;
using RoboClean.Sound;

public abstract class MonsterBase : MonoBehaviour
{
    public MonsterStatus Status
    {
        get; protected set;
    }

    [SerializeField, Required, AssetsOnly] protected MonsterConfig CONFIG;
    protected NavMeshAgent AGENT;
    protected MonsterStateMachine C_STATE_MACHINE;

    protected virtual void Awake()
    {
        Status = new MonsterStatus();
        Status.MaxHP.Value = CONFIG.InitialMaxHP;
        Status.CurHP.Value = CONFIG.InitialHP;
        Status.IsSuperArmor.Value = CONFIG.InitialSuperArmor;
        AGENT = GetComponentInChildren<NavMeshAgent>();

        C_STATE_MACHINE = GetComponentInChildren<MonsterStateMachine>();
        C_STATE_MACHINE.Initialize(CONFIG, Status);
        SludgeSpawner spawner = GetComponentInChildren<SludgeSpawner>();
        Debug.Assert(spawner != null, $"Cannot found sludge spawner {gameObject.name}");
        spawner.InitializeSpanwer(Status);
    }

    public void OnEnable()
    {
        RuntimeDataLoader.RuntimeStageData.ToxicGuage.AddListener(updateStatusByToxicGuage);
    }

    public void OnDisable()
    {
        RuntimeDataLoader.RuntimeStageData.ToxicGuage.RemoveListener(updateStatusByToxicGuage);
    }


    public void SetPositionAndRotation(Vector3 pos, Quaternion rot)
    {
        AGENT.Warp(pos);
        AGENT.transform.rotation = rot;
    }

    public void GotAttack(SkillConfig hitSkill)
    {
        bool isShieldDecreased;
        float damage = hitSkill.BaseDamage;
        if(RuntimeDataLoader.PlayerRuntimeInfo.CheatDamage)
        {
            damage += 1000;
        }
        Status.DecreaseHP(damage, out isShieldDecreased);


        if(CONFIG.MonsterType == eMonsterType.RoboBoss)
        {
            AudioManager.PlayBossHitSoundEffect();
        }
        else
        {
            AudioManager.PlayMonsterHitSoundEffect();//몬스터 피격 사운드 출력
        }

        if (Status.CurHP <= 0)
        {
            Status.IsDead.Value = true;
            C_STATE_MACHINE.DoDeadAction(DestoryGameobject);
            return;
        }
        else
        {
         C_STATE_MACHINE.DoGotDamagedAction(hitSkill, isShieldDecreased || Status.IsSuperArmor);
        }
    }

    public void DestoryGameobject()
    {
        Status.IsDestory.Value = true;
        Destroy(gameObject);
    }

    private void updateStatusByToxicGuage(float toxicGuage)
    {
        Status.Defense.Value = Mathf.Lerp(1.0f, CONFIG.DefenceIncreaseByToxic, toxicGuage) * CONFIG.InitialDefense;
        AGENT.speed = Mathf.Lerp(1.0f, CONFIG.SpeedIncreaseByToxic, toxicGuage) * CONFIG.InitialSpeed;
    }
}

public class MonsterStatus
{
    public const float DEALING_RANGE = 3;
    public ObservedData<float> NormalizedHP;
    public ObservedData<float> CurHP;
    public ObservedData<float> MaxHP;
    public ObservedData<float> ShieldAmount;
    public ObservedData<float> MaxShieldAmount;
    public ObservedData<float> LastDealingDamage;
    public ObservedData<float> Defense;

    public ObservedData<bool> IsSuperArmor;
    public ObservedData<bool> IsDead;
    public ObservedData<bool> IsDestory;
    public ObservedData<bool> IsEnemeyFoundPlayer;

    public MonsterStatus()
    {
        CurHP.AddListener((hp) => NormalizedHP.Value = hp / MaxHP);
    }

    public void IncreaseHP(float healAmount)
    {
        if (Mathf.Approximately(CurHP, MaxHP))
        {
            healAmount = Mathf.Clamp(healAmount, 0.0f, MaxShieldAmount - ShieldAmount);
            ShieldAmount.Value = healAmount;
        }
        else
        {
            healAmount = Mathf.Clamp(healAmount, 0.0f, MaxHP - CurHP);
            CurHP.Value -= healAmount;
        }
    }

    public void DecreaseHP(float dealAmount)
    {
        bool placeholder;
        DecreaseHP(dealAmount, out placeholder);
    }

    public void DecreaseHP(float dealAmount, out bool isShieldDecreased)
    {
        isShieldDecreased = false;
        dealAmount -= Defense;

        dealAmount = Random.Range(dealAmount - DEALING_RANGE, dealAmount + DEALING_RANGE);
        dealAmount = Mathf.Clamp(dealAmount, 2.0f, dealAmount);
        dealAmount = Mathf.Max(dealAmount, 2.0f);
        LastDealingDamage.Value = dealAmount;

        if (ShieldAmount.Value > 0.0f)
        {
            dealAmount = Mathf.Clamp(dealAmount, 0.0f, ShieldAmount);
            ShieldAmount.Value -= dealAmount;
            isShieldDecreased = true;
        }
        else
        {
            dealAmount = Mathf.Clamp(dealAmount, 0.0f, CurHP);
            CurHP.Value -= dealAmount;
        }
    }
}