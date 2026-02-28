using RoboClean.Character;
using RoboClean.Data;
using RoboClean.Utils;
using UnityEngine;



public class ComboSkillActor : SkillActor
{
    public bool IsComboAttackPressed;

    public bool IsWaitComboAttackInput
    {
        get; private set;
    }

    public bool CanComboAttack
    {
        get
        {
            if(IsOnAttack)
            {
                return mCurrentComboCount != 0 && mCurrentComboCount < Config.TotalComboCount;
            }
            else
            {
                return mCurrentComboCount != 0 && mCurrentComboCount < Config.TotalComboCount && mCombableTimer.IsActivate;
            }
        }
    }

    private BasicTimer mCombableTimer
    {
        get
        {
            return GlobalTimer.Instance.TryGetTimerOrAdd<BasicTimer>(getTimerKeyString("CombableTime"), 
                eTimerUpdateMode.FixedUpdate
                , INSTANCE_ID);
        }
    }

    private SkillConfig[] mComboSkillConfigs;
    private int mCurrentComboCount = 0;

    public ComboSkillActor(Rigidbody rb, CharacterAnimator anim, ActorStateMachine onwerStateMachine, SkillConfig config, 
        GameObject gameobject,System.Action onAttackEnd ,bool isPlayer = false) 
        : base(rb, anim, onwerStateMachine, config, gameobject, onAttackEnd, isPlayer)
    {
        Debug.Assert(Config.IsComboAttack,
            $"SkillConfig for ComboSkillAactor must be set as comboattack");

        mComboSkillConfigs = new SkillConfig[config.TotalComboCount];
        for (int i = 0; i < mComboSkillConfigs.Length; i++)
        {
            mComboSkillConfigs = RuntimeDataLoader.ComboSkillConfigs[config.ComboSkillName];
            if(i == 0) // 0 index of config will be initialized in base skillactor
            {
                continue;
            }
            mComboSkillConfigs[i].InitializeInRuntime();
        }
        mCombableTimer.ChangeCallback(resetComboInfo);
    }

    public override void InovkeActing()
    {
        Debug.Assert(mCurrentComboCount < Config.TotalComboCount,
            $"Combo Count Out Of Range[{NameID}]");
        changeConfig(mComboSkillConfigs[mCurrentComboCount]);
        if(mCombableTimer.IsActivate)
        {
            mCombableTimer.PauseTimer();
        }
        base.InovkeActing();
        if (mCombableTimer.IsActivate)
        {
            mCombableTimer.PauseTimer();
        }

        IsWaitComboAttackInput = false;
        mCurrentComboCount++;
    }

    public override void SetAttackSpeedMulitplier(float speed)
    {
        for (int i = 0; i < mComboSkillConfigs.Length; i++)
        {
            mComboSkillConfigs[i].RuntimeAttackSpeedMultiplier = speed;
        }
    }

    protected override void onAttackEnd()
    {
        base.onAttackEnd();
        if (mCurrentComboCount < Config.TotalComboCount)
        {
            mCombableTimer.ChangeDuration(Config.CombableTime).StartTimer();
        }
        else if(mCurrentComboCount == Config.TotalComboCount)
        {
            resetComboInfo();
        }
    }

    protected override string getTimerKeyString(string baseName)
    {
        return baseName + Config.ComboSkillName;
    }

    protected override void setSkillProgressState(eSkillProgressState state)
    {
        if (state != eSkillProgressState.Cancelable)
        {
            goto Return;
        }

        if (!OWNER.CurrentActorType.IsAttackType())
        {
            goto Return;
        }

        if (!CanComboAttack)
        {
            goto Return;
        }

        if (IsComboAttackPressed)
        {
            OWNER.TrySwitchActor(ActorType);
            IsComboAttackPressed = false;
        }
        else
        {
            IsWaitComboAttackInput = true;
        }

        Return:
        base.setSkillProgressState(state);
        return;
    }

    private void resetComboInfo()
    {
        IsWaitComboAttackInput = false;
        mCurrentComboCount = 0;
    }
}
