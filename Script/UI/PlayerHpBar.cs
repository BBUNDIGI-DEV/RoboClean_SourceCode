using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoboClean.Player;
using RoboClean.Data;

namespace RoboClean.UI
{
    public class PlayerHpBar : MonoBehaviour
    {
        [SerializeField] private RuntimePlayerInfo PLAYER_INFO;
        [SerializeField] private RuntimePlayData PLAY_DATA;
        [SerializeField] private Image HP_FILL_UP;
        [SerializeField] private Image HP_DECREASE_IMG;
        [SerializeField] private TMP_Text HP_TEXT;

        private float mLastMaxHP;
        private float mLastCurrentHP;

        private void Start()
        {
            PLAYER_INFO.MaxHP.AddListener(updateMaxHP, true);
            PLAYER_INFO.CurrentHP.AddListener(updateCurrentHP, true);
        }

        private void OnDestroy()
        {
            PLAYER_INFO.MaxHP.RemoveListener(updateMaxHP);
            PLAYER_INFO.CurrentHP.RemoveListener(updateCurrentHP);
        }

        private void updateMaxHP(float newMaxHP)
        {
            mLastMaxHP = newMaxHP;
            updateHPUI();
        }

        private void updateCurrentHP(float newHP)
        {
            mLastCurrentHP = newHP;
            updateHPUI();
        }

        private void updateHPUI()
        {
            HP_FILL_UP.fillAmount = mLastCurrentHP / mLastMaxHP;
            HP_TEXT.text = mLastCurrentHP + " / " + mLastMaxHP;
            StartCoroutine(showDecreaseHpBar());
        }


        private IEnumerator showDecreaseHpBar()
        {
            yield return null;

            while (HP_DECREASE_IMG.fillAmount >= HP_FILL_UP.fillAmount)
            {
                HP_DECREASE_IMG.fillAmount -= 0.002f;
                yield return new WaitForSeconds(0.01f);
            }

            HP_DECREASE_IMG.fillAmount = HP_FILL_UP.fillAmount;
        }
    }
}