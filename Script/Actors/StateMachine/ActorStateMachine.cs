using RoboClean.Data;
using RoboClean.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.Character
{
    public class ActorStateMachine
    {
        public eActorType CurrentActorType
        {
            get
            {
                return CurrentActorOrNull == null ? eActorType.None : CurrentActorOrNull.ActorType;
            }
        }

        public ActorBase CurrentActorOrNull
        {
            get; private set;
        }

        private eActorType mBufferedActor;
        private List<ActorBase> mUpdatedActors;
        private Dictionary<eActorType, ActorBase> mActorDic;
        private bool mIsCancleOperation;

        public ActorStateMachine()
        {
            mActorDic = new Dictionary<eActorType, ActorBase>();
            mUpdatedActors = new List<ActorBase>();
            mBufferedActor = eActorType.None;
        }

        public void AddActor(ActorBase actor)
        {
            Debug.Assert(!mActorDic.ContainsKey(actor.ActorType),
                $"Duplicate Actor in ActorStateMachine [{actor.ActorType}]");
            mActorDic.Add(actor.ActorType, actor);

            if (actor.BaseConfig.IsUpdatedActor)
            {
                mUpdatedActors.Add(actor);
            }
        }

        public void RemoveUpdatedActor(eActorType actorType)
        {
            for (int i = 0; i < mUpdatedActors.Count; i++)
            {
                if (mUpdatedActors[i].ActorType == actorType)
                {
                    mUpdatedActors.RemoveAt(i);
                    return;
                }
            }
            Debug.LogError($"Cannot found actor in statemachine {actorType}");
        }

        public void SetEnabledUpdate(bool enabled, eActorType actor)
        {
            for (int i = 0; i < mUpdatedActors.Count; i++)
            {
                if (mUpdatedActors[i].ActorType == actor)
                {
                    mUpdatedActors[i].SetEnabledUpdating(enabled);
                    return;
                }
            }
            Debug.LogError($"Cannot found updated actor in updatedActors List {actor}");
        }

        public T GetActor<T>(eActorType actorType) where T : ActorBase
        {
            Debug.Assert(mActorDic.ContainsKey(actorType), $"[{actorType}] is not contained");
            return mActorDic[actorType] as T;
        }


        public void UpdateActor()
        {
            for (int i = 0; i < mUpdatedActors.Count; i++)
            {
                if (mUpdatedActors[i].NeedUpdate)
                {
                    mUpdatedActors[i].UpdateActing();
                }
            }
        }

        public void CheckAndClearActor(eActorType checkType)
        {
            if (CurrentActorType != checkType)
            {
                return;
            }
            clearActor();
        }

        public void CheckAndClearActor(string nameID)
        {
            if (CurrentActorOrNull == null || CurrentActorOrNull.NameID != nameID)
            {
                return;
            }
            clearActor();
        }

        public void TryInvokeBufferedActor()
        {
            if (mBufferedActor != eActorType.None)
            {
                eActorType cashedActor = mBufferedActor;
                mBufferedActor = eActorType.None;

                TrySwitchActor(cashedActor);
            }
        }

        public bool TrySwitchActor(eActorType actorType, bool needPausePrevActor = true)
        {
            Debug.Assert(mActorDic.ContainsKey(actorType), $"You Cannot switch to actor not added in ActorStateMachine [{actorType}]");

            //해당 플래그를 통해 캔슬도중 의도치 않은 Actor변경을 방지합니다.
            //예를 들어 공격이 끝나면 Idle 모션으로 자동으로 이동합니다.
            //하지만 공격을 캔슬하고 대쉬를 실행하는 상황을 가정한다면, 공격이 멈추는 로직상 Idle모션으로 변경을 시도합니다
            //다만 현재 대쉬로 캔슬되는 과정이기때문에(mIsCancleOperation == true) 해당 플래그를 통해 공격의 정지와 동시에 Idle 이동하려는 동작은
            //아무런 처리 없이 return합니다.
            if (mIsCancleOperation == true)
            {
                return false;
            }

            mIsCancleOperation = true;
            ActorBase switchTarget = mActorDic[actorType];
            string debugString = CurrentActorOrNull == null ? "Null" : CurrentActorOrNull.NameID;
            bool canSwitchable = checkNewActorIsBlockedToSwitch(actorType);

            if (!canSwitchable)
            {
                //Debug.Log($"Cannot switch Actor from {debugString} to {switchTarget.NameID}");
                if (actorType != eActorType.Damaged)
                {
                    mBufferedActor = actorType;
                    //Damaged는 버퍼에 넣지 않습니다 즉 캔슬불가능한 상황에서 들어오는 딜은 데미지만 들어가고 엑터는 생략됩니다.
                }
                mIsCancleOperation = false;
                return canSwitchable;
            }

            if (CurrentActorOrNull != null && needPausePrevActor)
            {
                CurrentActorOrNull.StopActing();
            }

            switchTarget.InovkeActing();
            CurrentActorOrNull = switchTarget;
            //Debug.Log($"Switch Actor from {debugString} to {switchTarget.NameID}");
            mIsCancleOperation = false;
            return true;
        }

        public void DestoryActors()
        {
            foreach (var item in mActorDic)
            {
                item.Value.DestoryActor();
            }
        }

        public void PauseAllActorWithoutDead()
        {
            foreach (var item in mActorDic)
            {
                if (item.Value.ActorType == eActorType.Dead)
                {
                    continue;
                }

                item.Value.StopActing();
            }
        }

        private bool checkNewActorIsBlockedToSwitch(eActorType newActor)
        {
            if (CurrentActorOrNull == null)
            {
                return true;
            }

            if (newActor == eActorType.Damaged || newActor == eActorType.Dead || newActor == eActorType.Stun)
            {
                return true;
            }

            if (CurrentActorType == eActorType.Damaged ||
                CurrentActorType == eActorType.Stun ||
                CurrentActorType == eActorType.Dead)
            {
                return false;
            }


            if (CurrentActorType.IsAttackType())
            {
                SkillActor skillActor = CurrentActorOrNull as SkillActor;

                if (skillActor.ProgressState == eSkillProgressState.OnAttack)
                {
                    return false;
                }
            }
            return true;
        }

        private void clearActor()
        {
            CurrentActorOrNull = null;
            TryInvokeBufferedActor();
        }
    }
}