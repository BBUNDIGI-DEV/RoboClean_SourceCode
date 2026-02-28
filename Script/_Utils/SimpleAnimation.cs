using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.Common
{
    [RequireComponent(typeof(Animation))]
    public class SimpleAnimation : MonoBehaviour
    {
        private Animation ANIM;
        [SerializeField] private string[] ANIM_KEYS;

        private void Awake()
        {
            ANIM = GetComponent<Animation>();
        }

        public void InovkeAnim()
        {

        }
    }
}