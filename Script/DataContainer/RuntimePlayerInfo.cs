using RoboClean.Stage;
using UnityEngine;

namespace RoboClean.Player
{
    [CreateAssetMenu(menuName = "RuntimeData/RuntimePlayerInfo")]
    public class RuntimePlayerInfo : ScriptableObject
    {
        public Transform Trans
        {
            get; private set;
        }

        [HideInInspector] public ObservedData<float> MaxHP;
        [HideInInspector] public ObservedData<float> CurrentHP;
        [HideInInspector] public ObservedData<float> NormalizedHP;
        [HideInInspector] public ObservedData<float> DashCoolTime;
        [HideInInspector] public ObservedData<float> CurrentDashCoolTime;
        [HideInInspector] public bool IsPaused;
        [HideInInspector] public ObservedData<bool> IsDead;
        [HideInInspector] public ObservedData<eInteractableType> DetectedInteractableObject;
        [HideInInspector] public ObservedData<float> LastDamage;
        [HideInInspector] public eInteractableType ProcessedInteractableObject;

        //temp code
        [System.NonSerialized] public bool CheatDamage;
        //temp code

        public void SetPlayerTransform(Transform playerTrans)
        {
            Trans = playerTrans;
        }

        public bool IsPlayerInRange(Vector3 checkPos, float range)
        {
            float playerToEnemey = (Trans.position - checkPos).sqrMagnitude;
            float rushRange = range * range;

            return playerToEnemey < rushRange;
        }

        public Vector3 TowardVectorToPlayer(Vector3 from, Vector3 pos, float trackSpeed)
        {
            Vector3 targetToPlayer = Trans.position - pos;
            targetToPlayer = targetToPlayer.normalized;
            return Vector3.RotateTowards(from, targetToPlayer, trackSpeed, 0.0f);
        }
    }


    public struct ObservedData<T>
    {
        public delegate void OnDataChange(T param);

        public T Value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
                Callback?.Invoke(mValue);
                VoidCallback?.Invoke();
            }
        }

        [field: System.NonSerialized]
        public OnDataChange Callback
        {
            get; private set;
        }

        [field: System.NonSerialized]
        public System.Action VoidCallback
        {
            get; private set;
        }

        [SerializeField] private T mValue;

        public static implicit operator T(ObservedData<T> data)
        {
            return data.Value;
        }

        public void AddListener(OnDataChange callback, bool invokeOnAdd = false)
        {
            if (Callback == null)
            {
                Callback = callback;
            }
            else
            {
                Callback += callback;
            }

            if (invokeOnAdd)
            {
                callback.Invoke(mValue);
            }
        }

        public void RemoveListener(OnDataChange callback)
        {
            Debug.Assert(Callback != null);
            Callback -= callback;
        }

        public void AddListener(System.Action onValueChange, bool invokeOnAdd = false)
        {
            if (VoidCallback == null)
            {
                VoidCallback = onValueChange;
            }
            else
            {
                VoidCallback += onValueChange;
            }

            if (invokeOnAdd)
            {
                onValueChange.Invoke();
            }
        }

        public void RemoveListener(System.Action onValueChange)
        {
            Debug.Assert(VoidCallback != null);
            VoidCallback -= onValueChange;

        }

        public void Clear()
        {
            mValue = default(T);
            Callback = null;
        }


        public override string ToString()
        {
            return Value.ToString();
        }
    }
}