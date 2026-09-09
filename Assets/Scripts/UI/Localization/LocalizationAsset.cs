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
            if (count != 0)
            {
                Strings = new string[count];
            }
            Strings = null;
        }
        
        public string Key;
        public string[] Strings;

        public bool Equals(LocalizationEntry other)
        {
            return Equals(other.Key);
        }
        
        public bool Equals(string other)
        {
            return Key == other;
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

#if UNITY_EDITOR
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
#endif

        public string Get(string key)
        {
            return _entries.Find(p => p.Equals(key)).Strings[(int)LocalizationManager.LocalizationKey.en_US];
        }

        public void AppendStrings(Dictionary<string, string[]> stringList)
        {
            foreach (var entry in _entries)
            {
                stringList.Add(entry.Key, entry.Strings);
            }
        }
    }
}
