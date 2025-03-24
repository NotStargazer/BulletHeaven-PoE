using System;
using UnityEngine;

namespace Global
{
    public interface IRegistrableData
    {
        public object GetInstance();
    }
    
    [Serializable]
    public class RegistryEntry<T> : RegistryEntry where T : IRegistrableData
    {
        [SerializeField] private T _entry;

        public RegistryEntry(T entry)
        {
            _entry = entry;
        }
        
        public T CreateInstance()
        {
            return (T)_entry.GetInstance();
        }

        protected override IRegistrableData Get()
        {
            return _entry;
        }
    }

    public abstract class RegistryEntry
    {
        public RegistryEntry<TR> Get<TR>() where TR : IRegistrableData
        {
            return new RegistryEntry<TR>((TR)Get());
        }
        protected abstract IRegistrableData Get();
    }
}
