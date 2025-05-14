using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Localization
{
    [Serializable]
    public struct LocalizationEntry : IEquatable<LocalizationEntry>
    {
        public LocalizationEntry(string key, int count)
        {
            Key = key;
            Strings = new string[count];
        }
        
        public string Key;
        public string[] Strings;

        public bool Equals(LocalizationEntry other)
        {
            return Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            return obj is LocalizationEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Key != null ? Key.GetHashCode() : 0);
        }
    }
    
    public class LocalizationAsset : ScriptableObject
    {
        [SerializeField] private List<LocalizationEntry> _entries;
        
        public void Validate(HashSet<string> keys)
        {
            _entries ??= new List<LocalizationEntry>();
            
            foreach (var key in keys)
            {
                if (!_entries.Contains(new LocalizationEntry(key, 0)))
                {
                    _entries.Add(new LocalizationEntry(key, LocalizationManager.LocalizationKeys.Length));
                }
            }

            for (var i = _entries.Count - 1; i >= 0; i--)
            {
                if (!keys.Contains(_entries[i].Key))
                {
                    var index = _entries.IndexOf(_entries[i]);
                    _entries.RemoveAt(index);
                }
            }
        }
    }
}
