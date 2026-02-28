using RoboClean.Character;
using UnityEngine;

public abstract class ActorConfigDataContainerBase : DataConfigBase
{
    public ActorConfig BaseConfig;

    protected virtual void OnEnable()
    {
        BaseConfig.ActorType = eActorType.None;
        BaseConfig.IsUpdatedActor = false;
        initializeConfig();
        Debug.Assert(BaseConfig.ActorType != eActorType.None,
            "Actortype does not set by initliazeConfig Function");
    }

    protected abstract void initializeConfig();
}


public struct ActorConfig
{
    public eActorType ActorType;
    public bool IsUpdatedActor;

    public ActorConfig(eActorType actorType, bool isUpdateActor)
    {
        ActorType = actorType;
        IsUpdatedActor = isUpdateActor;
    }
}
