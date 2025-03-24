using System;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Instantiates Singletons from Prefabs only.
    /// </summary>
    /// <typeparam name="T">MonoBehaviour Class</typeparam>
    public abstract class SingletonPrefab<T> : Singleton where T : MonoBehaviour
    {
        private static T INSTANCE;
        public static bool HasInstance => INSTANCE;

        public static T Instance
        {
            get
            {
                if (INSTANCE != null)
                {
                    return INSTANCE;
                }

                throw new SingletonException(typeof(T).ToString(), false);
            }
        }

        public override void Instantiate()
        {
            if (INSTANCE)
            {
                Debug.LogWarning($"Singleton of type {typeof(T)} has already been created");
                return;
            }
        
            if (gameObject.activeInHierarchy)
            {
                throw new SingletonException(typeof(T).ToString(), true);
            }

            var single = Instantiate(gameObject);
            single.name = $"[{Name}]";
            DontDestroyOnLoad(single);
            INSTANCE = single.GetComponent<T>();
        }
    }

    /// <summary>
    /// Auto instantiated singleton when level object loads. Can be set to destroy on level change.
    /// </summary>
    /// <typeparam name="T">MonoBehaviour Class</typeparam>
    public abstract class SingletonBehaviour<T> : Singleton where T : MonoBehaviour
    {
        [SerializeField] private bool _destroyOnLevelChange;
        private static T INSTANCE;
        public static bool HasInstance => INSTANCE;

        public static T Instance
        {
            get
            {
                if (INSTANCE != null)
                {
                    return INSTANCE;
                }
                
                throw new SingletonException(typeof(T).ToString());
            }
        }

        public void Awake()
        {
            if (INSTANCE)
            {
                Debug.LogWarning($"Multiple singletons of type {typeof(T)} detected. Destroying duplicates.");
                Destroy(gameObject);
                return;
            }

            var o = gameObject;
            o.name = $"[{Name}]";
            if (!_destroyOnLevelChange)
            {
                DontDestroyOnLoad(o);
            }
            INSTANCE = o.GetComponent<T>();

            Instantiate();
        }
    }

    ///Singleton Wrapper Class
    public abstract class Singleton : MonoBehaviour
    {
        [SerializeField] private string _singletonName;
        protected string Name => _singletonName;

        public abstract void Instantiate();
    
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_singletonName))
            {
                _singletonName = name;
            }
        }
    }

    public class SingletonException : Exception
    {
        public SingletonException(string className, bool isActive = false)
            : base(isActive
                ? $"Singleton may not be created through active hierarchy object. \nType: {className}"
                : $"Singleton not initialized of type {className}") {}
    }
}
