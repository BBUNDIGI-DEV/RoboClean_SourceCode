using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoboClean.Data;
using RoboClean.Player;

public class PlayerDashUI : MonoBehaviour
{
    [SerializeField] private RuntimePlayData PLAY_DATA;
    [SerializeField] private RuntimePlayerInfo PLAYER_INFO;
    [SerializeField] private Image DASH_COOLDOWN;

    private float mDashCooltime;
    private float mCurrentDashcooltime;

    private void Start()
    {
        PLAYER_INFO.DashCoolTime.AddListener(updateDashCooltime, true);
        PLAYER_INFO.CurrentDashCoolTime.AddListener(updateCurrentDashCooltime, true);
    }

    private void OnDestroy()
    {
        PLAYER_INFO.DashCoolTime.RemoveListener(updateDashCooltime);
        PLAYER_INFO.CurrentDashCoolTime.RemoveListener(updateCurrentDashCooltime);
    }

    private void updateDashCooltime(float Cooltime)
    {
        mDashCooltime = Cooltime;
        updateDashUI();
    }

    private void updateCurrentDashCooltime(float newCurrentCooltime)
    {
        mCurrentDashcooltime = newCurrentCooltime;
        updateDashUI();
    }

    private void updateDashUI()
    {
        DASH_COOLDOWN.fillAmount = mCurrentDashcooltime / mDashCooltime;
    }
}
