using System;
using System.Collections.Generic;

namespace UI.Localization
{
    #if UNITY_EDITOR
    public static class LocalizationKeyGenerator
    {
        private class KeyLocker
        {
            private readonly string _locker;
            private readonly HashSet<string> _keys;
        
            public KeyLocker(string locker)
            {
                _keys = new HashSet<string>();
                _locker = locker;
            }

            public void AddKey(string key)
            {
                if (!_keys.Add(key))
                {
                    throw new Exception($"Attempted to add duplicate entry: {key}");
                }
            }

            public void Close()
            {
                LocalizationManager.ValidateLocalizationFile(_keys, _locker);
            }
        }
        
        private static KeyLocker _currentLocker;
        
        public static void OpenEntry(string key)
        {
            if (_currentLocker != null)
            {
                throw new Exception("Cannot open a new entry while creating another.");
            }

            _currentLocker = new KeyLocker(key);
        }

        public static void AddKey(string key) => _currentLocker.AddKey(key);

        public static void CloseEntry()
        {
            if (_currentLocker == null)
            {
                throw new Exception("Cannot close entry, no entry is currently open.");
            }
            
            _currentLocker.Close();
            _currentLocker = null;
        }
    }
    #endif
}
