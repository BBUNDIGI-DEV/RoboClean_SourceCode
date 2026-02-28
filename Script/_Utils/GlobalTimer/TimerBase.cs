    [System.Serializable]
    public abstract class TimerBase
    {
    public bool IsTimeOut
    {
        get
        {
            return mCurrentDuration <= 0.0f;
        }
    }

    public bool IsActivate
    {
        get; protected set;
    }

    public string NameID
    {
        get; private set;
    }

    public eTimerUpdateMode UpdateMode
    {
        get; private set;
    }

    public bool IsTimerAffectedByBulletTime
    {
        get; private set;
    }

    protected float mDuration;
    protected float mCurrentDuration;

    public TimerBase()
    {
        mDuration = 0.0f;
        UpdateMode = eTimerUpdateMode.Update;
    }

    public TimerBase(eTimerUpdateMode updateMode, float duration)
    {
        mDuration = duration;
        UpdateMode = updateMode;
    }

    public abstract void CaculateTimePass(float deltaTime);

    public virtual void StartTimer()
    {
        mCurrentDuration = mDuration;
        IsActivate = true;
    }

    public virtual void PauseTimer()
    {
        mCurrentDuration = 0;
        IsActivate = false;
    }

    public virtual TimerBase ChangeUpdateMode(eTimerUpdateMode updateMode)
    {
        UpdateMode = updateMode;
        return this;
    }

    public virtual TimerBase ChangeDuration(float newDuration)
    {
        mDuration = newDuration;
        return this;
    }

    public void SetNameID(string nameID)
    {
        NameID = nameID;
    }

    public void SetIsAffectedByBulletTime(bool isTimerAffectedByBulletTime)
    {
        IsTimerAffectedByBulletTime = isTimerAffectedByBulletTime;
    }
    }
    public enum eTimerUpdateMode
    {
    Update,
    FixedUpdate,
    LateUpdate
    }
