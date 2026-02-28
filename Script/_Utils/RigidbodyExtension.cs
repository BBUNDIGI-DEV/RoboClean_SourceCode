using RoboClean.Character;
using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.Utils
{
    public static class RigidbodyExtension
    {
        private static Dictionary<int, LayeredRigidbody> mLayeredRBDics;
        static RigidbodyExtension()
        {
            mLayeredRBDics = new Dictionary<int, LayeredRigidbody>();
        }

        public static LayeredRigidbody GetLayeredRigidbody(this Rigidbody rb)
        {
            return getRigidbodyLayerOrAdd(rb);
        }

        public static void DebugSetVelocity(this Rigidbody rb, Vector3 velocity, string user)
        {
            if (rb.velocity == velocity) // Only Cover Vector3.zero 
            {
                return;
            }
            rb.velocity = velocity;
        }

        public static void DebugSetRotation(this Rigidbody rb, Quaternion rot, string user)
        {
            if (rb.rotation == rot) // Only Cover Quaternion.Identity
            {
                return;
            }
            rb.rotation = rot;
        }

        public static Vector3 GetForward(this Rigidbody rb)
        {
            return rb.rotation * Vector3.forward;
        }

        public static void EnrollSetVelocity(this Rigidbody rb, Vector3 velocity, eActorType actorType)
        {
            rb.getRigidbodyLayerOrAdd().EnrollVelocity(velocity, actorType);
        }

        public static void DisEnrollSetVelocity(this Rigidbody rb, eActorType actorType)
        {
            if (!mLayeredRBDics.ContainsKey(rb.GetInstanceID()))
            {
                return;
            }
            LayeredRigidbody layeredRB = rb.getRigidbodyLayerOrAdd();
            layeredRB.DisEnrollVelocity(actorType);
            rb.UpdateVelocity();
        }

        public static void EnrollRotation(this Rigidbody rb, Quaternion quaternion, eActorType actorType)
        {
            rb.getRigidbodyLayerOrAdd().EnrollRotation(quaternion, actorType);
        }

        public static void DisEnrollRotation(this Rigidbody rb, eActorType actorType)
        {
            if (!mLayeredRBDics.ContainsKey(rb.GetInstanceID()))
            {
                return;
            }
            rb.getRigidbodyLayerOrAdd().DisEnrollRotation(actorType);
            rb.UpdateRotation();
        }

        public static void UpdateVelocity(this Rigidbody rb)
        {
            int instanceID = rb.GetInstanceID();
            if (!mLayeredRBDics.ContainsKey(instanceID))
            {
                return;
            }
            var velocitySetter = mLayeredRBDics[instanceID];
            Vector3 velocity;
            eActorType actor;
            bool result = velocitySetter.TryGetVelocity(out velocity, out actor);
            if (result)
            {
                rb.DebugSetVelocity(velocity, actor.ToString());
            }
            else
            {
                rb.DebugSetVelocity(Vector3.zero, actor.ToString());
            }
        }

        public static void UpdateRotation(this Rigidbody rb)
        {
            int instanceID = rb.GetInstanceID();
            if (!mLayeredRBDics.ContainsKey(instanceID))
            {
                return;
            }
            var ovelaySetter = mLayeredRBDics[instanceID];
            Quaternion quaternion;
            eActorType actor;
            bool result = ovelaySetter.TryGetRotation(rb.rotation, out quaternion, out actor);
            if (result)
            {
                rb.DebugSetRotation(quaternion, actor.ToString());
            }
        }

        private static LayeredRigidbody getRigidbodyLayerOrAdd(this Rigidbody rb)
        {
            int instanceID = rb.GetInstanceID();
            if (!mLayeredRBDics.ContainsKey(rb.GetInstanceID()))
            {
                LayeredRigidbody newLayer = new LayeredRigidbody();
                mLayeredRBDics.Add(instanceID, newLayer);
                return newLayer;
            }
            return mLayeredRBDics[instanceID];
        }
    }
}