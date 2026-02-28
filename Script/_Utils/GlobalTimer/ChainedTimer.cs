using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ChainedTimer : TimerBase
{
    private List<(string tag, float duration, Action callbackOrNull)> mCallbackList;

    private int mCurrentIndex;

    public ChainedTimer() : this(eTimerUpdateMode.FixedUpdate)
    {
    }

    public ChainedTimer(eTimerUpdateMode updateMode)
        : base(updateMode, 0.0f)
    {
        mCallbackList = new List<(string tag, float timing, Action callback)>();
    }

    public override void CaculateTimePass(float deltaTime)
    {
        if (!IsActivate)
        {
            return;
        }

        mCurrentDuration -= deltaTime;
        if (mCurrentDuration <= 0.0f)
        {
            moveToNextTimer();
        }
    }

    public override void StartTimer()
    {
        Debug.Assert(mCallbackList.Count != 0, "Empty Callback list checked in chainedTimer");
        ChangeDuration(mCallbackList[0].duration);
        mCurrentIndex = 0;
        base.StartTimer();
    }

    public bool IsTaggedTimerActivated(string callbackTag)
    {
        return IsActivate && mCallbackList[mCurrentIndex].tag == callbackTag;
    }

    public void SkipTaggedTimer(string tag, bool withCallback = true)
    {
        Debug.Assert(IsTaggedTimerActivated(tag), $"you cannot skip that tagged  timer[{tag}]");
        moveToNextTimer(withCallback);
    }


    public ChainedTimer AddCallback(string callbackTag, float timing, Action callback)
    {
        Debug.Assert(!containTag(callbackTag), $"Duplicate tagged callback data detected [{callbackTag}]");
        mCallbackList.Add((callbackTag, timing, callback));
        mDuration += timing;
        return this;
    }

    public void RemoveCallback(string callbackTag)
    {
        int index = findIndex(callbackTag);
        Debug.Assert(index != -1, $"Cannot found tagged callback data [{callbackTag}]");
        mDuration -= mCallbackList[index].duration;
        mCallbackList.RemoveAt(index);
    }

    public ChainedTimer ChangeCallback(string callbackTag, Action newCallback)
    {
        int index = findIndex(callbackTag);
        Debug.Assert(index != -1, $"Cannot found tagged callback data [{callbackTag}]");

        var callbackData = mCallbackList[index];
        mCallbackList[index] = (callbackData.tag, callbackData.duration, newCallback);
        return this;
    }

    public ChainedTimer ChangeTiming(string callbackTag, float newTiming)
    {
        int index = findIndex(callbackTag);
        Debug.Assert(index != -1, $"Cannot found tagged callback data [{callbackTag}]");

        var callbackData = mCallbackList[index];
        mCallbackList[index] = (callbackData.tag, newTiming, callbackData.callbackOrNull);
        return this;
    }

    public bool HasKey(string callbackTag)
    {
        return findIndex(callbackTag) != -1;
    }

    public void PauseTimerWithCallback()
    {
        for (int i = mCurrentIndex; i < mCallbackList.Count; i++)
        {
            if (mCallbackList[i].callbackOrNull != null)
            {
                mCallbackList[i].callbackOrNull.Invoke();
            }
        }
        PauseTimer();
    }

    private void moveToNextTimer(bool invokeCallback = true)
    {
        var callbackData = mCallbackList[mCurrentIndex];

        mCurrentIndex++;
        if (mCallbackList.Count == mCurrentIndex)
        {
            IsActivate = false;
        }
        else
        {
            mCurrentDuration = mCallbackList[mCurrentIndex].duration;
        }

        //콜백에서 다시 StartTimer를 호출할 수 있음으로 최후에 호출함(아닐시 버그발생)
        //Ex) 추가 대쉬 계산일경우
        if (callbackData.callbackOrNull != null)
        {
            callbackData.callbackOrNull.Invoke();
        }
    }

    private bool containTag(string tag)
    {
        return findIndex(tag) != -1;
    }

    private int findIndex(string tag)
    {
        for (int i = 0; i < mCallbackList.Count; i++)
        {
            if (mCallbackList[i].tag == tag)
            {
                return i;
            }
        }
        return -1;
    }
}
