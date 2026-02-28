using System;
using UnityEngine;

namespace RoboClean.Utils
{
    public static class TransformExtensions
    {
        public static Transform FindRecursive(this Transform self, string exactName)
        {
            return self.findRecursive(child => child.name == exactName);
        }

        private static Transform findRecursive(this Transform self, Func<Transform, bool> selector)
        {
            foreach (Transform child in self)
            {
                if (selector(child))
                {
                    return child;
                }

                var finding = child.findRecursive(selector);

                if (finding != null)
                {
                    return finding;
                }
            }

            return null;
        }
    }
}