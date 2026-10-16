using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Components
{
    public class ZoomableScrollRect : ScrollRect
    {
        private float _zoom;

        public void Zoom(float amount)
        {
            _zoom = Mathf.Clamp(_zoom + amount * scrollSensitivity, 1, 6);
            viewport.localScale = Vector3.one * _zoom;
        }
        
        public void Scroll(Vector2 ctrlScroll)
        {
            normalizedPosition += ctrlScroll / Mathf.Max(_zoom, 1);
        }

        public override void OnScroll(PointerEventData data)
        {
            Zoom(data.scrollDelta.y);
        }
        
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.button is PointerEventData.InputButton.Right or PointerEventData.InputButton.Left)
            {
                eventData.button = PointerEventData.InputButton.Left;
                base.OnInitializePotentialDrag(eventData);
            }
        }
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button is PointerEventData.InputButton.Right or PointerEventData.InputButton.Left)
            {
                eventData.button = PointerEventData.InputButton.Left;
                base.OnBeginDrag(eventData);
            }
        }
        
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button is PointerEventData.InputButton.Right or PointerEventData.InputButton.Left)
            {
                eventData.button = PointerEventData.InputButton.Left;
                base.OnEndDrag(eventData);
            }
        }
        
        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.button is PointerEventData.InputButton.Right or PointerEventData.InputButton.Left)
            {
                eventData.button = PointerEventData.InputButton.Left;
                base.OnDrag(eventData);
            }
        }
    }
}
