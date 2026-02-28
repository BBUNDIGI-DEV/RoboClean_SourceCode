using RoboClean.Common;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTimer : SingletonClass<GlobalTimer>
{
    private Dictionary<string, TimerBase> mTimerDic;
    private Dictionary<int, List<string>> mTimerKeyByInstanceID;
    private List<KeyValuePair<string, TimerBase>> mBufferedAddedTimer;
    private List<string> mBufferedRemovedTimer;
    private float mBulletTimeFactor;

    protected override void Awake()
    {
        base.Awake();
        mTimerDic = new Dictionary<string, TimerBase>();
        mBufferedAddedTimer = new List<KeyValuePair<string, TimerBase>>(8);
        mBufferedRemovedTimer = new List<string>(8);
        mTimerKeyByInstanceID = new Dictionary<int, List<string>>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void Update()
    {
        processTimepass(eTimerUpdateMode.Update, Time.deltaTime);
        addAndRemoveBufferedTimers();
    }

    private void FixedUpdate()
    {
        processTimepass(eTimerUpdateMode.FixedUpdate, Time.fixedDeltaTime);
        addAndRemoveBufferedTimers();
    }

    private void LateUpdate()
    {
        processTimepass(eTimerUpdateMode.LateUpdate, Time.deltaTime);
    }

    public void AddTimer(eCommonTimertag tag, TimerBase data)
    {
        AddTimer(tag.ToString(), data);
    }

    public void AddTimerWithInstacneID(eCommonTimertag tag, int instanceID, TimerBase data)
    {
        AddTimerWithInstacneID(tag.ToString(), instanceID, data);
    }

    public void AddTimerWithInstacneID(string tag, int instanceID, TimerBase data)
    {
        bindTimerKeyToInstanceID(tag, instanceID);
        AddTimer(tag + instanceID, data);
    }

    public void AddTimer(string key, TimerBase data)
    {
        Debug.Assert(!mTimerDic.ContainsKey(key), $"Duplicate timer insert detected [{key} aready in]");
        data.SetNameID(key);
        mBufferedAddedTimer.Add(new KeyValuePair<string, TimerBase>(key, data));
    }

    public T AddTimer<T>(string key, T data) where T : TimerBase
    {
        AddTimer(key, (TimerBase)data);
        return data;
    }

    public T AddTimer<T>(eCommonTimertag tag, T data) where T : TimerBase
    {
        return AddTimer(tag.ToString(), data);
    }

    public void RemoveAllTimerByInstance(int instanceID)
    {
        Debug.Assert(mTimerKeyByInstanceID.ContainsKey(instanceID), "You Cannot remove not conatined");

        List<string> timerKeyList = mTimerKeyByInstanceID[instanceID];

        for (int i = 0; i < timerKeyList.Count; i++)
        {
            RemoveTimer(timerKeyList[i]);
        }
        mTimerKeyByInstanceID.Remove(instanceID);
    }

    public void RemoveTimer(string key)
    {
        Debug.Assert(mTimerDic.ContainsKey(key) || checkContainInBufferedTimer(key) != -1, $"You cannot remove key not contain in mTimerDic [{key} not in]");
        mBufferedRemovedTimer.Add(key);
    }

    public void RemoveTimer(eCommonTimertag tag)
    {
        RemoveTimer(tag.ToString());
    }

    public void RemoveTimer(TimerBase timerBase)
    {
        Debug.Log(timerBase.NameID);
        RemoveTimer(timerBase.NameID);
    }

    public TimerBase GetTimerData(eCommonTimertag tag)
    {
        return GetTimerData(tag.ToString());
    }

    public TimerBase GetTimerData(eCommonTimertag tag, int instanceID)
    {
        return GetTimerData(tag.ToString() + instanceID);
    }

    public TimerBase GetTimerData(string key)
    {
        TimerBase timer;
        bool result = TryGetTimerDataOrNull(key, out timer);
        Debug.Assert(result,($"Cannot Found Timer [Key: {key}]"));
        return timer;
    }

    public T GetTimerData<T>(string key) where T : TimerBase
    {
        return GetTimerData(key) as T;
    }

    public bool TryGetTimerDataOrNull(string key, out TimerBase timer)
    {
        timer = null;
        if (mTimerDic.ContainsKey(key))
        {
            timer = mTimerDic[key];
            return true;
        }

        for (int i = 0; i < mBufferedAddedTimer.Count; i++)
        {
            if (mBufferedAddedTimer[i].Key == key)
            {
                timer = mBufferedAddedTimer[i].Value;
                return true;

            }
        }
        return false;
    }

    public bool TryGetTimerDataOrNull<T>(string key, out T timer) where T : TimerBase
    {
        TimerBase baseTimer;
        bool result = TryGetTimerDataOrNull(key, out baseTimer);
        timer = baseTimer as T;
        return result;
    }

    public T TryGetTimerOrAdd<T>(string key, eTimerUpdateMode updateMode, int instanceID = int.MinValue) where T : TimerBase, new()
    {
        T timer;
        string combinedKey = instanceID == int.MinValue ? key : key + instanceID;

        bool result = TryGetTimerDataOrNull(combinedKey, out timer);
        if (!result)
        {
            if (instanceID != int.MinValue)
            {
                bindTimerKeyToInstanceID(combinedKey, instanceID, false);
            }

            AddTimer(combinedKey, new T());

            timer = GetTimerData<T>(combinedKey);
            timer.ChangeUpdateMode(updateMode);
        }
        return timer;
    }

    public BasicTimer TryGetBasicTimerOrAdd(string key, eTimerUpdateMode updateMode, System.Action onTimerEnd, int instanceID = int.MinValue)
    {
        BasicTimer timer = TryGetTimerOrAdd<BasicTimer>(key, updateMode, instanceID);
        timer.ChangeCallback(onTimerEnd);
        return timer;
    }

    public void SetBulletPowerFactor(float bulletTimeFactor)
    {
        mBulletTimeFactor = bulletTimeFactor;
    }
    
    private void processTimepass(eTimerUpdateMode updateMode, float timePass)
    {
        foreach (var item in mTimerDic)
        {
            if (item.Value.UpdateMode == updateMode)
            {
                timePass = item.Value.IsTimerAffectedByBulletTime ? timePass * (1 / mBulletTimeFactor) : timePass;
                item.Value.CaculateTimePass(timePass);
            }
        }
    }

    private void addAndRemoveBufferedTimers()
    {
        for (int i = 0; i < mBufferedAddedTimer.Count; i++)
        {
            var item = mBufferedAddedTimer[i];
            Debug.Assert(!mTimerDic.ContainsKey(item.Key), $"Timer Dic Already Contain that key [{item.Key}]");


            mTimerDic.Add(item.Key, item.Value);
        }
       
        for (int i = 0; i < mBufferedRemovedTimer.Count; i++)
        {
            string key = mBufferedRemovedTimer[i];
            Debug.Assert(mTimerDic.ContainsKey(key) || checkContainInBufferedTimer(key) != -1);
            if(mTimerDic.ContainsKey(key))
            {
                mTimerDic.Remove(key);
            }
            else
            {
                mBufferedAddedTimer.RemoveAt(checkContainInBufferedTimer(key));
            }
        }
        mBufferedAddedTimer.Clear();
        mBufferedRemovedTimer.Clear();
    }
    
    private void bindTimerKeyToInstanceID(string timerKey, int instanceID, bool needCombineKeyAndID = true)
    {
        if (!mTimerKeyByInstanceID.ContainsKey(instanceID))
        {
            mTimerKeyByInstanceID.Add(instanceID, new List<string>());
        }

        if(needCombineKeyAndID)
        {
            timerKey += instanceID;
        }
        mTimerKeyByInstanceID[instanceID].Add(timerKey);
    }

    private int checkContainInBufferedTimer(string key)
    {
        int result = -1;
        for (int i = 0; i < mBufferedAddedTimer.Count; i++)
        {
            if (mBufferedAddedTimer[i].Key == key)
            {
                result = i;
                break;
            }
        }
        return result;
    } 
#if UNITY_EDITOR
    [Sirenix.OdinInspector.Button]
    [Sirenix.OdinInspector.HideInEditorMode]
    private void debugGlobalTimer()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        foreach (var item in mTimerDic)
        {
            builder.AppendLine(item.Key);
        }

        Debug.Log(builder.ToString());
    }
#endif
}


public enum eCommonTimertag
{
    DashCoolDown,
    ExtraDashableTime,
    AttackDelayTimer,
    AttackCoolTime,
}
