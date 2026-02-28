using UnityEngine;

public class BasicTimer : TimerBase
{
    private System.Action mOnTimerEndOrNull;

    public BasicTimer() : base()
    {
        mOnTimerEndOrNull = null;
    }

    public BasicTimer(eTimerUpdateMode updateMode, float duration, System.Action onTimeOutOrNull)
        : base(updateMode, duration)
    {
        mOnTimerEndOrNull = onTimeOutOrNull;
    }

    public override void CaculateTimePass(float deltaTime)
    {
        if (!IsActivate)
        {
            return;
        }

        mCurrentDuration -= deltaTime;
        if (mCurrentDuration < 0)
        {
            if (mOnTimerEndOrNull != null)
            {
                mOnTimerEndOrNull.Invoke();
            }
            IsActivate = false;
        }
    }

    public BasicTimer ChangeCallback(System.Action onTimerEnd)
    {
        mOnTimerEndOrNull = onTimerEnd;
        return this;
    }

    public void PauseTimerWithCallback()
    {
        PauseTimer();
        mOnTimerEndOrNull.Invoke();
    }
}