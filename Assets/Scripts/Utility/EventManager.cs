using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class EventManager : SingletonBehaviour<EventManager>
    {
        public enum ExecutionOrder
        {
            BeforeDuration,
            AfterDuration,
        }
        
        private record Chain
        {
            private struct Link
            {
                public float Duration;
                public Action Action;
                public ExecutionOrder Order;
            }

            private float _next;
            private Link? _current;
            private readonly Queue<Link> _actions;

            public Chain()
            {
                _actions = new Queue<Link>();
            }

            public void Tick()
            {
                if (Time.time >= _next)
                {
                    if (_current is { Order: ExecutionOrder.AfterDuration })
                    {
                        var action = _current.Value.Action;
                        _current = null;
                        action();
                    }
                    if (_actions.TryDequeue(out var current))
                    {
                        if (current is { Order: ExecutionOrder.BeforeDuration })
                        {
                            current.Action();
                        }
                        _current = current;
                        _next = current.Duration + Time.time;
                    }
                }
            }

            public void Add(float duration, Action action, ExecutionOrder order)
            {
                _actions.Enqueue(new Link
                {
                    Duration = duration,
                    Action = action,
                    Order = order,
                });
                
                Tick();
            } 
        }
        
        public readonly struct EventChain
        {
            private readonly Guid _guid;

            public EventChain(Guid guid)
            {
                _guid = guid;
            }
            
            public void AddEvent(float duration, ExecutionOrder order, Action action)
            {
                Instance.AddEvent(duration, action, order, _guid);
            }            
            
            public void Complete()
            {
                var key = _guid;
                Instance.AddEvent(0, () =>
                {
                    Instance._markedForComplete.Add(key);
                }, ExecutionOrder.BeforeDuration, _guid);
            }
        }
        
        public static EventChain CreateChain()
        {
            var guid = Guid.NewGuid();
            Instance._eventChains.Add(guid, new Chain());
            return new EventChain(guid);
        }

        private Dictionary<Guid, Chain> _eventChains;
        private List<Guid> _markedForComplete;
        private bool _isExecuting;
        
        public override void Instantiate()
        {
            _eventChains = new Dictionary<Guid, Chain>();
            _markedForComplete = new List<Guid>();
        }

        private void AddEvent(float duration, Action action, ExecutionOrder order, Guid guid)
        {
            
            _eventChains[guid].Add(duration, action, order);
        }
        
        //JUST LET ME CONVERT COLLECTIONS TO SPANS C# PLEASE
        private unsafe void Update()
        {
            if (_eventChains.Count == 0)
            {
                return;
            }
            
            var guidCount = _eventChains.Count;
            var guids = stackalloc Guid[guidCount];
            var index = 0;
            foreach (var guid in _eventChains.Keys)
            {
                guids[index] = guid;
                index++;
            }

            for (var i = 0; i < guidCount; i++)
            {
                _eventChains[guids[i]].Tick();
            }
            
            foreach (var guid in _markedForComplete)
            {
                _eventChains.Remove(guid);
            }
            
            _markedForComplete.Clear();
        }
    }
}
