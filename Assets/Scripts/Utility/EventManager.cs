using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
            private readonly GUID _guid;

            public EventChain(GUID guid)
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
            var guid = GUID.Generate();
            Instance._eventChains.Add(guid, new Chain());
            return new EventChain(guid);
        }

        private Dictionary<GUID, Chain> _eventChains;
        private List<GUID> _markedForComplete;
        
        public override void Instantiate()
        {
            _eventChains = new Dictionary<GUID, Chain>();
            _markedForComplete = new List<GUID>();
        }

        private void AddEvent(float duration, Action action, ExecutionOrder order, GUID guid)
        {
            _eventChains[guid].Add(duration, action, order);
        }
        
        private void Update()
        {
            foreach (var eventChain in _eventChains.Values)
            {
                eventChain.Tick();
            }

            foreach (var guid in _markedForComplete)
            {
                _eventChains.Remove(guid);
            }
            
            _markedForComplete.Clear();
        }
    }
}
