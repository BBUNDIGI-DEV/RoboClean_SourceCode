using RoboClean.Character.AI;
using UnityEngine;
using UnityEngine.UI;

namespace RoboClean.UI
{
    public class MonsterHpBar : MonoBehaviour
    {
        [SerializeField] private Image HP_FILL_UP;

        private CommonMonsterController mMonsterController;
        private float mLastUsedMaxHP;
        private float mLastUsedCurrentHP;
        private bool mIsEnabled;

        private void Start()
        {
            mMonsterController = GetComponentInParent<CommonMonsterController>();
            Debug.Assert(mMonsterController != null, "MonsterHPBar is initailzied without Parent Common monster Controller");

            mMonsterController.Status.MaxHP.AddListener(updateMaxHP, true);
            mMonsterController.Status.CurHP.AddListener(updateCurrentHP, true);
            mIsEnabled = false;
            gameObject.SetActive(false);
        }

        private void updateMaxHP(float newMaxHP)
        {
            mLastUsedMaxHP = newMaxHP;
            updateHPUI();
        }

        private void updateCurrentHP(float newHP)
        {
            if (!mIsEnabled)
            {
                gameObject.SetActive(true);
                mIsEnabled = true;
            }
            mLastUsedCurrentHP = newHP;
            updateHPUI();
        }

        private void updateHPUI()
        {
            HP_FILL_UP.fillAmount = mLastUsedCurrentHP / mLastUsedMaxHP;
        }
    }
}