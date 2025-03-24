using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public interface IPoolInstance<T>
    {
        public void OnRent();
        public Action<T> Return { set; }
    }
    
    public class InstancePool<T> where T: MonoBehaviour, IPoolInstance<T>
    {
        private Stack<T> _pool;
        private int _expandSize;
        private int _limit;
        private T _instance;
        
        public InstancePool(T instance, int initialPool, int expandSize, int limit)
        {
            _instance = instance;
            _limit = limit;
            _expandSize = expandSize;
            _pool = new Stack<T>();
            InitializePool(initialPool);
        }

        public int TotalActive { private set; get; }

        private void InitializePool(int size)
        {
            for (var i = 0; i < size; i++)
            {
                var instance = GameObject.Instantiate(_instance, LevelRoot.Instance.transform);
                _pool.Push(instance);
            }
        }
        
        private void Expand(int size)
        {
            var limit = Mathf.Abs(Mathf.Min(size - _limit, 0));
            limit = Mathf.Min(limit, size);
            
            var asyncOp = GameObject.InstantiateAsync(_instance, limit, LevelRoot.Instance.transform);
            asyncOp.GetAwaiter().OnCompleted(() =>
            {
                foreach (var instance in asyncOp.Result)
                {
                    _pool.Push(instance);
                }
            });
        } 

        public T Rent()
        {
            if (_pool.Count <= _expandSize)
            {
                Expand(_expandSize);
            }
            
            var instance = _pool.Pop();
            instance.OnRent();
            instance.Return = OnReturn;
            TotalActive++;
            return instance;
        }

        private void OnReturn(T element)
        {
            TotalActive--;
            _pool.Push(element);
        }
    }
}
