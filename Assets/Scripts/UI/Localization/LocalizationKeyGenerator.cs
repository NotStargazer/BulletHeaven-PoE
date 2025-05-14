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
        
        private static KeyLocker CURRENT_LOCKER;
        
        public static void OpenEntry(string key)
        {
            if (CURRENT_LOCKER != null)
            {
                throw new Exception("Cannot open a new entry while creating another.");
            }

            CURRENT_LOCKER = new KeyLocker(key);
        }

        public static void AddKey(string key) => CURRENT_LOCKER.AddKey(key);

        public static void CloseEntry()
        {
            if (CURRENT_LOCKER == null)
            {
                throw new Exception("Cannot close entry, no entry is currently open.");
            }
            
            CURRENT_LOCKER.Close();
            CURRENT_LOCKER = null;
        }
    }
    #endif
}
