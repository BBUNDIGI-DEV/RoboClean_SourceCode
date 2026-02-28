using System;
using UnityEngine;

public sealed class DelayTimer : TimerBase
{
    private Action mOnTimerStart;
    private Action mOnTimerEnd;
    private UpdateTimer.OnTimerUpdate mOnTimerUpdatedOrNull;
    private float mDelayTime;
    private float mCurrentDeleyTime;

    public DelayTimer() : base()
    {
        mOnTimerStart = null;
        mOnTimerEnd = null;
    }
    public DelayTimer(eTimerUpdateMode updateMode, float duration, System.Action onTimerStartOrNull, System.Action onTimerEndOrNull) : base(updateMode, duration)
    {
        mOnTimerStart = onTimerStartOrNull;
        mOnTimerEnd = onTimerEndOrNull;
    }

    public void PauseTimerWithCallback()
    {
        PauseTimer();
        mOnTimerEnd.Invoke();
    }

    public override void CaculateTimePass(float deltaTime)
    {
        if (!IsActivate)
        {
            return;
        }

        if (mCurrentDeleyTime >= 0.0f)
        {
            mCurrentDeleyTime -= deltaTime;
            if (mCurrentDeleyTime < 0.0f)
            {
                mCurrentDeleyTime = float.MinValue;
                mOnTimerStart?.Invoke();
            }
            return;
        }

        mCurrentDuration -= deltaTime;
        if(mOnTimerUpdatedOrNull != null)
        {
            mOnTimerUpdatedOrNull.Invoke(1 - mCurrentDuration / mDuration, mDuration - mCurrentDuration);
        }
        if (mCurrentDuration < 0)
        {
            mOnTimerEnd?.Invoke();
            IsActivate = false;
        }
    }

    public override void StartTimer()
    {
        base.StartTimer();
        mCurrentDeleyTime = mDelayTime;
    }

    public DelayTimer ChangeDelayTime(float newDelayTime)
    {
        mDelayTime = newDelayTime;
        return this;
    }

    public DelayTimer ChangeCallback(Action onTimerStart, Action onTimerEnd)
    {
        mOnTimerStart = onTimerStart;
        mOnTimerEnd = onTimerEnd;
        return this;
    }

    public DelayTimer ChangeUpdatedCallback(UpdateTimer.OnTimerUpdate onTimerUpdated)
    {
        mOnTimerUpdatedOrNull = onTimerUpdated;
        return this;
    }
}
