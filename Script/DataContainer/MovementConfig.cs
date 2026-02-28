using RoboClean.Character;
using UnityEngine;

namespace RoboClean.Data
{
    [CreateAssetMenu(menuName = "DataContainer/MovementConfig")]
    public class MovementConfig : ActorConfigDataContainerBase
    {
        public float Speed;
        public float RotSpeed = 0.2f;
        public float ZAxisPusher = 1.2f;


        protected override void initializeConfig()
        {
            BaseConfig.ActorType = eActorType.InputMovement;
            BaseConfig.IsUpdatedActor = true;
        }
    }
}