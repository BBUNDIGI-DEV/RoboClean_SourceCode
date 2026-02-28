using RoboClean.Common;
using RoboClean.UI;
using UnityEngine;

namespace RoboClean.Managers
{
    public class UIManager : SingletonClass<UIManager>
    {
        [field: SerializeField]
        public DialogueUI DialogueManager
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject GameOverUI
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject EndingCreditUI
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject TutorialIamge
        {
            get; private set;
        }


        protected override void Awake()
        {
            base.Awake();
        }
    }
}