using RoboClean.Common;
using UnityEngine;
namespace RoboClean.Utils
{
    public class GameObjectPool : MonoBehaviour
    {
        private ePoolingObjectType mPoolingType;

        public static GameObjectPool CreateGameObjectPool(Transform parent, string name, ePoolingObjectType poolingType = ePoolingObjectType.None)
        {
            GameObject newGameobject = new GameObject(name, typeof(GameObjectPool));
            newGameobject.transform.SetParent(parent, false);
            GameObjectPool pool = newGameobject.GetComponent<GameObjectPool>();
            pool.setPoolingType(poolingType);
            return pool;
        }

        public static GameObjectPool TryGetGameobjectPool(Transform parent, string name, ePoolingObjectType poolingType = ePoolingObjectType.None)
        {
            Transform transform = parent.FindRecursive(name);
            if (transform == null)
            {
                return CreateGameObjectPool(parent, name, poolingType);
            }
            GameObjectPool pool = transform.GetComponent<GameObjectPool>();
            if (pool == null)
            {
                pool = transform.gameObject.AddComponent<GameObjectPool>();
            }
            pool.setPoolingType(poolingType);
            return pool;
        }

        public GameObject GetGameobject(GameObject prefab)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject checkObject = transform.GetChild(i).gameObject;
                if (checkObject.name == prefab.name)
                {
                    return checkObject;
                }
            }
            GameObject clone = instantiateGameobject(prefab);
            return clone;
        }

        public GameObject GetDeactiveGameobject(GameObject prefab)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject checkObject = transform.GetChild(i).gameObject;
                if (checkObject.name == prefab.name && !checkObject.activeInHierarchy)
                {
                    checkObject.SetActive(true);
                    return checkObject;
                }
            }
            GameObject clone = instantiateGameobject(prefab);
            clone.SetActive(true);
            return clone;
        }

        private void setPoolingType(ePoolingObjectType poolingType)
        {
            mPoolingType = poolingType;
        }

        private GameObject instantiateGameobject(GameObject prefab)
        {
            GameObject clone = Instantiate(prefab, transform);
            clone.name = prefab.name;
            switch (mPoolingType)
            {
                case ePoolingObjectType.None:
                    break;
                case ePoolingObjectType.AttackBox:
                    break;
                case ePoolingObjectType.EffectPool:
                    if (clone.GetComponent<ParticleDeactivator>() == null)
                    {
                        clone.AddComponent<ParticleDeactivator>();
                    }
                    break;
                default:
                    break;
            }
            return clone;
        }
    }

    public enum ePoolingObjectType
    {
        None,
        AttackBox,
        EffectPool,
    }
}