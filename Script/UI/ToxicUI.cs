using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoboClean.Data;

namespace RoboClean.UI
{
    public class ToxicUI : MonoBehaviour
    {
        [SerializeField] private RuntimePlayData PLAY_DATA;
        [SerializeField] private TMP_Text TEXT_UI;
        [SerializeField] private Image TOXIC_IMG;
        [SerializeField] private Image TOXIC_PULSE_IMG;
        [SerializeField] private Sprite POLLUTION_SAFE_IMG;
        [SerializeField] private Sprite POLLUTION_WARNING_IMG;
        [SerializeField] private Sprite POLLUTION_DANGER_IMG;

        private int mCurrentSludgeCount;
        private int mMaxSludgeCount;

        private Animator mAnimator;
        private Image mPollutionImg;

        private float mColorR;
        private float mColorG;
        private float mColorB;

        private void Awake()
        {
            mAnimator = GetComponent<Animator>();
            mPollutionImg = GetComponent<Image>();
        }

        private void Start()
        {
            mColorR = 0;
            mColorG = 1;
            mColorB = 0;

            RuntimeDataLoader.RuntimeStageData.MaxSludgeCount.AddListener(updateMaxSludgeCount, true);
            RuntimeDataLoader.RuntimeStageData.SludgeCount.AddListener(updateSludgeCount, true);
        }

        private void OnDestroy()
        {
            RuntimeDataLoader.RuntimeStageData.MaxSludgeCount.RemoveListener(updateMaxSludgeCount);
            RuntimeDataLoader.RuntimeStageData.SludgeCount.RemoveListener(updateSludgeCount);
        }

        private void updateImgColor()
        {
            TOXIC_IMG.color = new Color(mColorR, mColorG, mColorB);
        }

        private void updateMaxSludgeCount(int newMaxSludgeCount)
        {
            mMaxSludgeCount = newMaxSludgeCount;
            updateUI();
        }

        private void updateSludgeCount(int newSludgeCount)
        {
            mCurrentSludgeCount = newSludgeCount;
            updateUI();
        }

        private void updateUI()
        {
            TEXT_UI.text = Mathf.RoundToInt(((float)mCurrentSludgeCount / (float)mMaxSludgeCount) * 100) + "%";
            TOXIC_IMG.fillAmount = (float)mCurrentSludgeCount / (float)mMaxSludgeCount;

            if (((float)mCurrentSludgeCount / (float)mMaxSludgeCount) * 100 < 50f)
            {
                if (mAnimator.GetBool("IsPulsed"))
                {
                    mAnimator.SetBool("IsPulsed", false);
                }
                mPollutionImg.sprite = POLLUTION_SAFE_IMG;
                StartCoroutine(updateDeactiveRedColor());
                StartCoroutine(updateActiveGreenColor());
                TOXIC_PULSE_IMG.fillAmount = 0f;
            }
            else if (((float)mCurrentSludgeCount / (float)mMaxSludgeCount) * 100 >= 50f &&
                ((float)mCurrentSludgeCount / (float)mMaxSludgeCount) * 100 < 80f)
            {
                if (mAnimator.GetBool("IsPulsed"))
                {
                    mAnimator.SetBool("IsPulsed", false);
                }
                mPollutionImg.sprite = POLLUTION_WARNING_IMG;
                StartCoroutine(updateActiveRedColor());
                StartCoroutine(updateActiveGreenColor());
                TOXIC_PULSE_IMG.fillAmount = 0f;
            }
            else if (((float)mCurrentSludgeCount / (float)mMaxSludgeCount) * 100 >= 80f)
            {
                if (!mAnimator.GetBool("IsPulsed"))
                {
                    mAnimator.SetBool("IsPulsed", true);
                }
                mPollutionImg.sprite = POLLUTION_DANGER_IMG;
                StartCoroutine(updateActiveRedColor());
                StartCoroutine(updateDeactiveGreenColor());
                TOXIC_PULSE_IMG.fillAmount = TOXIC_IMG.fillAmount;
            }
        }

        private IEnumerator updateActiveRedColor()
        {
            yield return null;

            while (mColorR <= 1)
            {
                mColorR += 0.01f;
                updateImgColor();
                yield return new WaitForSeconds(0.01f);
            }
        }
        private IEnumerator updateDeactiveRedColor()
        {
            yield return null;

            while (mColorR >= 0)
            {
                mColorR -= 0.01f;
                updateImgColor();
                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator updateActiveGreenColor()
        {
            yield return null;

            while (mColorG <= 1)
            {
                mColorG += 0.01f;
                updateImgColor();
                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator updateDeactiveGreenColor()
        {
            yield return null;

            while (mColorG >= 0)
            {
                mColorG -= 0.01f;
                updateImgColor();
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
