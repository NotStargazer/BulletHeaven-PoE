using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Enemies
{
    public partial class FlexibleStateMachine
    {
        public struct ExitCondition
        {
            public IExitCondition ExitCon;
            public string NextState;
        }
        
        public struct FlexibleState
        {
            public IState State;
            public ExitCondition[] ExitConditions;
        }
        
        [Serializable]
        public struct StateParameter
        {
            [SerializeField] private string _name;
            [SerializeField] private string _type;
            [SerializeField] private string _stringVal;
            [SerializeField] private bool _boolVal;
            [SerializeField] private int _intVal;
            [SerializeField] private float _floatVal;
            [SerializeField] private Vector2 _vector2Val;
            [SerializeField] private Vector3 _vector3Val;
            [SerializeField] private Vector2Int _vector2IntVal;
            [SerializeField] private Vector3Int _vector3IntVal;
            [SerializeField] private Quaternion _quaternionVal;

            public object Param
            {
                get
                {
                    var type = Type.GetType(_type);
                    if (type == typeof(string))
                        return _stringVal;
                    if (type == typeof(bool))
                        return _boolVal;
                    if (type == typeof(int))
                        return _intVal;
                    if (type == typeof(float))
                        return _floatVal;
                    if (type == typeof(Vector2))
                        return _vector2Val;
                    if (type == typeof(Vector3))
                        return _vector3Val;
                    if (type == typeof(Vector2Int))
                        return _vector2IntVal;
                    if (type == typeof(Vector3Int))
                        return _vector3IntVal;
                    if (type == typeof(Quaternion))
                        return _quaternionVal;

                    throw new Exception("Param value is not supported type");
                }
            }

            public static object ToObject(StateParameter param) => param.Param;
        }
        
        [Serializable]
        public struct StateType<T>
        {
            [SerializeField] private string _typeName;

            public Type Type => Type.GetType(_typeName);
            
            // ReSharper disable once StaticMemberInGenericType
            private static string[] _typeNames;
            public static IList<string> TypeNames => _typeNames;
            static StateType()
            {
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(T).IsAssignableFrom(p) && !p.IsInterface).ToArray();
                _typeNames = new string[types.Length];
                for (var i = 0; i < types.Length; i++)
                {
                    _typeNames[i] = types[i].FullName;
                }
            }
        }
        
        [Serializable]
        public struct ExitConditionDefinition
        {
            [SerializeField] private StateType<IExitCondition> _exitCondition;
            [SerializeField] private string _nextState;
            [SerializeField] private StateParameter[] _parameters;

            public ExitCondition Value
            {
                get
                {
                    var prams = new object[_parameters.Length];
                    for (var i = 0; i < prams.Length; i++)
                    {
                        prams[i] = _parameters[i].Param;
                    }
                    
                    return new ExitCondition
                    {
                        ExitCon = (IExitCondition)_exitCondition.Type.GetConstructors()[0].Invoke(prams),
                        NextState = _nextState
                    };
                }
            }
        }
        
        [Serializable]
        public struct StateDefinition
        {
            [SerializeField] private string _stateName;
            [SerializeField] private StateType<IState> _stateType;
            [SerializeField] private StateParameter[] _stateParameters;
            [SerializeField] private ExitConditionDefinition[] _exitConditions;

            public void AddTo(Dictionary<string, FlexibleState> stateDefinitions, Enemy subject)
            {
                var prams = new object[_stateParameters.Length];
                for (var i = 0; i < prams.Length; i++)
                {
                    prams[i] = _stateParameters[i].Param;
                }
                var exitConditions = new ExitCondition[_exitConditions.Length];
                for (var i = 0; i < exitConditions.Length; i++)
                {
                    var exitCon = _exitConditions[i].Value;
                    exitCon.ExitCon.Subject = subject;
                    exitConditions[i] = exitCon;
                }
                var state = new FlexibleState
                {
                    State = (IState)_stateType.Type.GetConstructors()[0].Invoke(prams),
                    ExitConditions = exitConditions,
                };
                state.State.Subject = subject;
                stateDefinitions.Add(_stateName, state);
            }
        }
    }
}