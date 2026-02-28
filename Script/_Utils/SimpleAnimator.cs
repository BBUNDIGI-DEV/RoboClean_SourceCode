using System;
using UnityEngine;

namespace RoboClean.Common
{
    [RequireComponent(typeof(Animator))]
    public class SimpleAnimator : MonoBehaviour
    {
        public Animator Animator
        {
            get;
            private set;
        }
        private int[] mAnimHashKeys;
        private string[] SD_ANIMATION_STATE_KEY;
        public void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        public void InitAnimPlayer(Type enumType)
        {
            SD_ANIMATION_STATE_KEY = Enum.GetNames(enumType);
            mAnimHashKeys = new int[SD_ANIMATION_STATE_KEY.Length];
            for (int i = 0; i < mAnimHashKeys.Length; i++)
            {
                mAnimHashKeys[i] = Animator.StringToHash(SD_ANIMATION_STATE_KEY[i]);
            }
        }

        public void SetBool(int keyIndex, bool toggle)
        {
            Animator.SetBool(mAnimHashKeys[keyIndex], toggle);
        }

        public void SetTrigger(int keyIndex)
        {
            Animator.SetTrigger(mAnimHashKeys[keyIndex]);
        }

        public void SetFloat(int keyIndex, float value)
        {
            Animator.SetFloat(mAnimHashKeys[keyIndex], value);
        }

        public void Play(int keyIndex)
        {
            Animator.Play(SD_ANIMATION_STATE_KEY[keyIndex]);
        }

        public void PlayByString(string state)
        {
            Animator.Play(state);
        }

        public bool CheckAnimState(string stateName, int layerIndex)
        {
            return Animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        }
    }
}
