using RoboClean.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoboClean.Character
{
    public class SludgeElement : MonoBehaviour
    {
        private SludgeBody BODY;
        private SludgeGroundElement GROUND;

        private void Awake()
        {
            BODY = GetComponentInChildren<SludgeBody>(true);
            GROUND = GetComponentInChildren<SludgeGroundElement>(true);
        }

        private void OnEnable()
        {
            AudioManager.PlaySludgeSoundEffect();//오물 생성 사운드
        }

        public void PlaySludge(Vector3 startPos, Vector3 destPos)
        {
            gameObject.SetActive(true);
            BODY.IntializeBody(startPos, destPos, () => GROUND.InitializeSludge(destPos, onSludgeDisabled));
        }

        private void onSludgeDisabled()
        {
            gameObject.SetActive(false);
        }
    }
}
