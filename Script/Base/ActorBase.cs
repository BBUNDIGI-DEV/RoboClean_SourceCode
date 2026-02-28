using UnityEngine;

namespace RoboClean.Character
{
    public abstract class ActorBase
    {
        public eActorType ActorType
        {
            get
            {
                return BaseConfig.ActorType;
            }
        }

        public ActorConfig BaseConfig
        {
            get; protected set;
        }

        public string NameID
        {
            get; protected set;
        }

        public bool NeedUpdate
        {
            get; private set;
        }

        protected readonly Rigidbody RB;
        protected readonly CharacterAnimator ANIM;
        protected readonly ActorStateMachine OWNER;
        protected readonly int INSTANCE_ID;

        public ActorBase(Rigidbody rb, CharacterAnimator anim, ActorStateMachine onwerStateMachine, ActorConfig config, string nameID, GameObject gameObject)
        {
            RB = rb;
            ANIM = anim;
            INSTANCE_ID = gameObject.GetInstanceID();
            OWNER = onwerStateMachine;
            BaseConfig = config;
            NameID = nameID;
            SetEnabledUpdating(BaseConfig.IsUpdatedActor);
        }

        public virtual void InovkeActing()
        {
            Debug.LogError($"You Cannot call placeholder inovke acting [{NameID}]");
            //place holder
        }

        public virtual void UpdateActing()
        {
            Debug.LogError($"You Cannot call placeholder update acting [{NameID}]");
            //place holder
        }

        public abstract void StopActing();
        public abstract void DestoryActor();

        public void SetEnabledUpdating(bool enabled)
        {
            NeedUpdate = enabled;
        }
    }
}