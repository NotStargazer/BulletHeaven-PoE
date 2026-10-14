using System;
using UnityEngine;

namespace Gameplay.Data
{
    [Serializable]
    public class RendererPropertyBlock : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        private MaterialPropertyBlock _propertyBlock;
        
        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void SetProperty<T>(int propertyId, T value)
        {
            switch (value)
            {
                case int val:
                    _propertyBlock.SetInt(propertyId, val);
                    break;
                case float val:
                    _propertyBlock.SetFloat(propertyId, val);
                    break;
                case Color val:
                    _propertyBlock.SetColor(propertyId, val);
                    break;
                case Vector4 val:
                    _propertyBlock.SetVector(propertyId, val);
                    break;
            }
            
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}