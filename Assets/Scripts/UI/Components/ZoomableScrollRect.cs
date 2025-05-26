using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Components
{
    public class ZoomableScrollRect : ScrollRect
    {
        private float _zoom;
    
        public override void OnScroll(PointerEventData data)
        {
            _zoom += data.scrollDelta.y * Time.deltaTime * scrollSensitivity;
            _zoom = Mathf.Clamp(_zoom, 1, 6);
            viewport.localScale = Vector3.one * _zoom;
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
