using System.Collections.Generic;
using UnityEngine;

namespace Global
{
    public class DataBehavioursManager : MonoBehaviour
    {
        private List<IDataBehaviour> _dataBehaviours;

        private void Awake()
        {
            _dataBehaviours = new List<IDataBehaviour>();
        }

        public void AddBehaviour(IDataBehaviour behaviour)
        {
            _dataBehaviours.Add(behaviour);
        }
        
        public void RemoveBehaviours(IDataBehaviour behaviour)
        {
            _dataBehaviours.Remove(behaviour);
        }

        private void Update()
        {
            for (var index = 0; index < _dataBehaviours.Count; index++)
            {
                var behaviour = _dataBehaviours[index];
                if (behaviour.Enabled)
                {
                    behaviour.Update(Time.deltaTime, index);
                }
            }
        }
    }
}
