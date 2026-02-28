using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoboClean.Data;

namespace RoboClean.UI
{
    public class UpdateUIAlpha : MonoBehaviour
    {
        private Image[] mAllImages;
        private TMP_Text[] mAllTextes;
        private float mColorA;

        private void Awake()
        {
            mAllImages = GetComponentsInChildren<Image>();
            mAllTextes = GetComponentsInChildren<TMP_Text>();
        }

        private void Start()
        {
            mColorA = 1;
            RuntimeDataLoader.RuntimeUIData.IsFadeAllUI.AddListener(updateFadeAllUI, true);
        }

        private void updateImgAlpha()
        {
            if (mAllImages != null)
            {
                for (int i = 0; i < mAllImages.Length; i++)
                {
                    mAllImages[i].color = setColorAlpha(mAllImages[i].color, mColorA);
                }
            }

            if (mAllTextes != null)
            {
                for (int i = 0; i < mAllTextes.Length; i++)
                {
                    mAllTextes[i].color = setColorAlpha(mAllTextes[i].color, mColorA);
                }
            }
        }

        private void updateFadeAllUI(bool newIsFadeAllUI)
        {
            if (newIsFadeAllUI)
            {
                StartCoroutine(updateFadeOut());
            }
            else if (!newIsFadeAllUI)
            {
                StartCoroutine(updateFadeIn());
            }
        }

        private Color setColorAlpha(Color color, float alpha)
        {
            Color tempColor = color;
            tempColor.a = alpha;
            color = tempColor;
            return color;
        }

        private IEnumerator updateFadeIn()
        {
            yield return null;

            while (mColorA <= 1)
            {
                mColorA += 0.01f;
                updateImgAlpha();
                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator updateFadeOut()
        {
            yield return null;

            while (mColorA >= 0)
            {
                mColorA -= 0.01f;
                updateImgAlpha();
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}