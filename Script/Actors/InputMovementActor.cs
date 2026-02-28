using RoboClean.Data;
using RoboClean.Input;
using RoboClean.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.Character.Player
{
    public class InputMovementActor : ActorBase
    {
        private readonly MovementConfig CONFIG;

        public InputMovementActor(Rigidbody rb, CharacterAnimator anim, ActorStateMachine ownerStateMachine, MovementConfig config, GameObject gameobject)
            : base(rb, anim, ownerStateMachine, config.BaseConfig, config.NameID, gameobject)
        {
            CONFIG = config;
        }

        public override void UpdateActing()
        {
            if (!NeedUpdate)
            {
                RB.DisEnrollSetVelocity(eActorType.InputMovement);
                RB.DisEnrollRotation(eActorType.InputMovement);
            }
            else
            {
                Vector3 moveDir = InputManager.Instance.MoveDir;
                Vector3 newVelocity = InputManager.Instance.MoveDir * CONFIG.Speed;
                newVelocity = new Vector3(newVelocity.x, RB.velocity.y, newVelocity.z);
                RB.EnrollSetVelocity(newVelocity, eActorType.InputMovement);

                if (moveDir != Vector3.zero)
                {
                    Vector3 newDirection = Vector3.RotateTowards(RB.GetForward(), moveDir, CONFIG.RotSpeed * Time.fixedDeltaTime, 0.0f);

                    RB.EnrollRotation(Quaternion.LookRotation(newDirection), ActorType);
                }
            }

            ANIM.UpdateMovementAnim(RB.velocity);
        }

        public override void StopActing()
        {
            SetEnabledUpdating(false);
        }

        public override void DestoryActor()
        {
        }
    }
}
