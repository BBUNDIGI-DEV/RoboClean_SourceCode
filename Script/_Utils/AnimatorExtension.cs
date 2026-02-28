using UnityEngine;
namespace RoboClean.Utils
{
    public static class AnimatorExtension
    {
        public static void PlayOrRewind(this Animator anim, string name)
        {
            Debug.Assert(anim.HasState(0, Animator.StringToHash(name)), $"Cannot Goto animation state [{name}]");
            anim.Play(name, 0, 0.0f);
        }

    }
}