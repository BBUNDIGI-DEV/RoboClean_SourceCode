using RoboClean.Character.Player;
using RoboClean.Data;
using RoboClean.Utils;
using UnityEditor;
using UnityEngine;

namespace RoboClean.UI
{
    public class DamageUI : MonoBehaviour
    {
        [SerializeField] private GameObject sfTextPrefab;
        [SerializeField] private float sfSpawnWidth;
        [SerializeField] private float sfSpawnHeight;
        private GameObjectPool mDamgeTextPool;
        private bool mIsUsedByPlayer;

        private void Awake()
        {
            mDamgeTextPool = GetComponent<GameObjectPool>();
        }

        private void Start()
        {
            MonsterBase mosnterBase = GetComponentInParent<MonsterBase>();
            if (mosnterBase != null)
            {
                mosnterBase.Status.LastDealingDamage.AddListener(showDamageUI);
            }


            PlayerCharacterController playerBase = GetComponentInParent<PlayerCharacterController>();

            if (playerBase != null)
            {
                RuntimeDataLoader.PlayerRuntimeInfo.LastDamage.AddListener(showDamageUI);
                mIsUsedByPlayer = true;
            }
        }

        private void OnDestroy()
        {
            if (mIsUsedByPlayer)
            {
                RuntimeDataLoader.PlayerRuntimeInfo.LastDamage.RemoveListener(showDamageUI);
            }
        }

        private void showDamageUI(float lastDamage)
        {
            GameObject damageUI = mDamgeTextPool.GetDeactiveGameobject(sfTextPrefab);
            damageUI.GetComponentInChildren<TMPro.TMP_Text>().text = ((int)lastDamage).ToString();
            damageUI.GetComponent<RectTransform>().anchoredPosition += new Vector2(Random.Range(-sfSpawnWidth / 2, sfSpawnWidth / 2),
                                                            Random.Range(-sfSpawnHeight / 2, sfSpawnHeight / 2));
            damageUI.transform.SetAsLastSibling();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.blue;
            Handles.DrawWireCube(transform.position, new Vector3(sfSpawnWidth, sfSpawnHeight));
        }
#endif
    }
}

