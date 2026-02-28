using UnityEngine;
using UnityEngine.EventSystems;

namespace RoboClean.Input
{
    public class MobileJoystick : MobileDraggingUI
    {
        public Vector2 DeltaStickPos
        {
            get
            {
                Vector2 deltaStickPos = (Vector2)SD_STICK_RECT.position - mOriginPos;
                if (deltaStickPos.sqrMagnitude < DEAD_ZONE * DEAD_ZONE)
                {
                    return Vector2.zero;
                }
                else
                {
                    return deltaStickPos;
                }
            }
        }

        private const float DEAD_ZONE = 0.1f;
        [SerializeField] private RectTransform SD_STICK_RECT;
        [SerializeField] private float MAX_RADIUS;
        private Vector2 mOriginPos;

        public void Awake()
        {
            Canvas canavs = GetComponentInParent<Canvas>();
            mOriginPos = SD_STICK_RECT.position * (Vector2)canavs.transform.localScale;
            Debug.Log(SD_STICK_RECT.lossyScale);
            Debug.Log(canavs.transform.localScale);
            Debug.Log(mOriginPos);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            SD_STICK_RECT.position = clampInRadius(MAX_RADIUS, mOriginPos, eventData.position);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            SD_STICK_RECT.position = clampInRadius(MAX_RADIUS, mOriginPos, eventData.position);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            SD_STICK_RECT.position = mOriginPos;
        }

        private Vector2 clampInRadius(float radius, Vector2 origin, Vector2 target)
        {
            Vector2 deltaVector = target - origin;
            deltaVector = Vector2.ClampMagnitude(deltaVector, MAX_RADIUS);
            return deltaVector + origin;
        }
    }
}