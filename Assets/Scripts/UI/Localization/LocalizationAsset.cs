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
                return;
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

        public LocalizationEntry Resize(int size)
        {
            Array.Resize(ref Strings, size);
            return this;
        }
    }
    
    public class LocalizationAsset : ScriptableObject
    {
        [SerializeField] private List<LocalizationEntry> _entries;

#if UNITY_EDITOR
        public void Validate(HashSet<string> keys)
        {
            _entries ??= new List<LocalizationEntry>();
            var numLangKeys = LocalizationManager.LanguageKeys.Length;
            
            foreach (var key in keys)
            {
                var index = _entries.IndexOf(new LocalizationEntry(key, 0));
                if (index == -1)
                {
                    _entries.Add(new LocalizationEntry(key, numLangKeys));
                }
                else
                {
                    if (_entries[index].Strings.Length != numLangKeys)
                    {
                        _entries[index] = _entries[index].Resize(numLangKeys);
                    }
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
            var entry = _entries.Find(p => p.Equals(key));
            return entry.Strings?[(int)LocalizationManager.LocalizationKey.en_US];
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
