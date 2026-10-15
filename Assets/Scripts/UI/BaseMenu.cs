using System;
using UnityEngine;

namespace UI
{
    public abstract class BaseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _rootObject;
        [SerializeField] private bool _defaultVisibility;
        protected bool Visible;

        private void Awake()
        {
            Visible = _rootObject.activeSelf;
            SetVisible(_defaultVisibility);
            
            MenuAwake();
        }

        protected abstract void MenuAwake();

        public virtual void SetVisible(bool visible)
        {
            if (visible == Visible)
            {
                return;
            }

            Visible = visible;

            if (Visible)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }
        
        protected virtual void OpenMenu()
        {
            _rootObject.gameObject.SetActive(true);
        }

        protected virtual void CloseMenu()
        {
            _rootObject.gameObject.SetActive(false);
        }
    }
}
