using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utility
{
    [Serializable]
    public class InterfaceRef<T> : ISerializationCallbackReceiver where T : class
    {
        [SerializeField] private Object _target;
        public T I => _target as T;
        public static implicit operator bool(InterfaceRef<T> ir) => ir._target != null;

        private void OnValidate()
        {
            if (_target is not T) 
            {
                if (_target is GameObject go)
                {
                    _target = null;
                    foreach (var c in go.GetComponents<Component>())
                    { 
                        if (c is T){
                            _target = c;
                            break;
                        }
                    }
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() => OnValidate();
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}
