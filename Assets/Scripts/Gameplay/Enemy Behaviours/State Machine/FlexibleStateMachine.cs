using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Enemies
{
    [Serializable]
    public class FlexibleStateMachine
    {
        private struct ExitCondition
        {
            private IExitCondition _exitConditionType;
            private Type _targetState;
        }
        
        private struct FlexibleState
        {
            public IState State;
            public ExitCondition[] ExitConditions;
        }

        [Serializable]
        public struct StateType<T>
        {
            [SerializeField] private string _typeName;

            public Type Type => Type.GetType(_typeName);
            
            // ReSharper disable once StaticMemberInGenericType
            private static string[] _typeNames;
            public static IReadOnlyCollection<string> TypeNames => _typeNames;
            static StateType()
            {
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(T).IsAssignableFrom(p) && !p.IsInterface).ToArray();
                _typeNames = new string[types.Length];
                for (var i = 0; i < types.Length; i++)
                {
                    _typeNames[i] = types[i].Name;
                }
            }
        }
        
        [Serializable]
        private struct ExitConditionDefinition
        {
            [SerializeField] private Type _exitConditionType;
            [SerializeField] private Type _targetState;
        }
        
        [Serializable]
        public struct StateDefinition
        {
            [SerializeField] private StateType<IState> _stateType;
            [SerializeField] private object[] _stateParameters;
            [SerializeField] private ExitConditionDefinition[] _exitConditions;
            [SerializeField] private object[][] _exitConditionParameters;
        }

        [SerializeField] private StateDefinition[] _stateDefinitions;
    }
}