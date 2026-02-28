using RoboClean.Managers;
using UnityEngine;


namespace RoboClean.UI
{
    public class TutorialImageInvoker : MonoBehaviour
    {
        public void ShowTutorialImage()
        {
            UIManager.Instance.TutorialIamge.SetActive(true);
        }

        public void HideTutorialImage()
        {
            UIManager.Instance.TutorialIamge.SetActive(false);
        }
    }
}