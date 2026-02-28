using RoboClean.Data;
using RoboClean.Sound;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace RoboClean.Character.AI
{
    public class RoboBossMonsterController : MonsterBase
    {
        protected override void Awake()
        {
            base.Awake();
            Status.IsSuperArmor.Value = true;
            Status.IsDead.AddListener((isDead) =>
            {
                if (isDead)
                {
                    SteamUserStats.GetAchievement("ACHIEVEMENT_BOSS_CLEAR", out bool achieved);

                    if (achieved == false)
                    {
                        SteamUserStats.SetAchievement("ACHIEVEMENT_BOSS_CLEAR");
                    }
                }
            }, true);
        }

        private void Start()
        {
            AudioManager.PlayBossIdleSound();
            Status.NormalizedHP.AddListener((hp) =>
            {
                if (hp < 0.5f)
                {
                    BGMManager.Instance.SetBGMLabel("BossHP50%");
                }
                else
                {
                    BGMManager.Instance.SetBGMLabel("Boss");
                }
            }, true);
        }

        private void OnDestroy()
        {
            AudioManager.ReleaseBossIdleSound();
        }

        public void GotStunedByRush(StunData stunData)
        {
            C_STATE_MACHINE.MonsterSM.GetActor<RoboBossAISkillActor>(eActorType.AIAttack).SetRushHitObastacleFlag();
            C_STATE_MACHINE.MonsterSM.GetActor<StunActor>(eActorType.Stun).SetStunData(stunData);
            C_STATE_MACHINE.MonsterSM.TrySwitchActor(eActorType.Stun);
        }

        public void DoRushComboAttack()
        {
            C_STATE_MACHINE.MonsterSM.GetActor<RoboBossAISkillActor>(eActorType.AIAttack).SetRushHitPlayerFlag();
        }
    }
}