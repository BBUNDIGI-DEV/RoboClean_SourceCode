using UnityEngine;

public sealed class UpdateTimer : BasicTimer
{
    public delegate void OnTimerUpdate(float normalizedDuration, float timePass);
    private OnTimerUpdate mOnTimerUpdated;
    private float mUpdateInterval;
    private float mUpdateLastTime;
    public UpdateTimer() : base()
    {
        mOnTimerUpdated = null;
    }
    public UpdateTimer(eTimerUpdateMode updateMode, float duration,
        System.Action onTimeOutOrNull, 
        OnTimerUpdate onTimerUpdated,
        float updateInterval = 0.0f)
        : base(updateMode, duration, onTimeOutOrNull)
    {
        mOnTimerUpdated = onTimerUpdated;
    }

    public override void CaculateTimePass(float deltaTime)
    {
        if(!IsActivate)
        {
            return;
        }
        mUpdateLastTime -= deltaTime;
        if(mUpdateLastTime < 0.0f)
        {
            mUpdateLastTime = mUpdateInterval;
            mOnTimerUpdated.Invoke(1 - mCurrentDuration / mDuration, mDuration - mCurrentDuration);
        }
        base.CaculateTimePass(deltaTime);
    }

    public UpdateTimer ChangeUpdateInterval(float newInterval)
    {
        mUpdateInterval = newInterval;
        return this;
    }

    public UpdateTimer ChangeUpdateCallback(OnTimerUpdate onUpdate)
    {
        mOnTimerUpdated = onUpdate;
        return this;
    }
}